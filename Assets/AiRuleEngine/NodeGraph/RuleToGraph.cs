#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiRuleEngine;
using Altova;
using XMLRules;
using UnityEngine;

namespace AiRuleEngine
{
	public class RuleToGraph
	{
		private NodeContainer m_Container;

		public RuleToGraph(NodeContainer container)
		{
			m_Container = container;
		}

		public List<NodeBase> Convert(Rule rule)
		{
			List<NodeBase> nodeList;

			if (m_Container.GetType () == typeof(ConditionContainer))
				nodeList = ConvertCondition(rule);
			else 
			{
				nodeList = ConvertAction(rule);
			}

			foreach (NodeBase node in nodeList) 
			{
				foreach (NodeBase child in node.m_Children)
					m_Container.m_NodeEdges.Add(new NodeEdge(node, child));
			}

			return nodeList;
		}

		// Use the number of leaf nodes of a tree as an estimate of its width at the base. 
		int CountLeafNodes(NodeBase node)
		{
			int count = 0;

			if (node.m_Children.Count == 0)
				count++;
			else
				foreach (NodeBase child in node.m_Children) 
				{
					count += CountLeafNodes(child);
				}

			return count;
		}

		void LayoutTree(NodeBase node, float posx, float posy)
		{	
			node.m_Window.x = posx;
			node.m_Window.y = posy;

			float dx = 0;

			int leafNodes = CountLeafNodes(node);

			dx = leafNodes * 75 / 2;

			if (node.m_Children.Count >= 1)
				LayoutTree(node.m_Children[0], posx - dx, posy + 200);

			if (node.m_Children.Count == 2)
				LayoutTree(node.m_Children[1], posx + dx, posy + 200);
		}

		List<NodeBase> ConvertCondition(Rule rule)
		{
			List<NodeBase> nodeList = new List<NodeBase>();

			if (rule.m_Rule.HasCondition ()) 
			{
				NodeBase rootNode = CreateLogicalNode(rule.m_Rule.GetCondition());
				BuildNodeList(rootNode, ref nodeList);
		
				int leafNodes = 1;

				if (rootNode.m_Children.Count > 0)
					leafNodes = CountLeafNodes(rootNode.m_Children[0]) + 1;

				LayoutTree(rootNode, leafNodes * 150, 100);
			}

			return nodeList;
		}

		List<NodeBase> ConvertAction(Rule rule)
		{
			int actionCount = rule.m_Rule.GetActionCount();
			List<NodeBase> nodeList = new List<NodeBase>();
			List<NodeBase> rootNodes = new List<NodeBase>();

			for (int i = 0; i < actionCount; i++)
			{
				XMLRules.ActionType xmlAction = rule.m_Rule.GetActionAt(i);
				NodeBase action = CreateActionNode(xmlAction);
				rootNodes.Add(action);
				BuildNodeList(action, ref nodeList);
			}

			int totalLeafNodes = 0;

			foreach (NodeBase node in rootNodes)
				totalLeafNodes += CountLeafNodes(node);

			float startx = (totalLeafNodes / 2) * 200;

			foreach (NodeBase node in rootNodes)
			{
				int leafNodes = CountLeafNodes(node);
				node.m_Window.y = 100;
				LayoutTree(node, startx - (leafNodes / 2) * 100, 100);
				startx += (leafNodes + 1) * 100;
			}

			return nodeList;
		}

		void BuildNodeList(NodeBase rootNode, ref List<NodeBase> nodeList)
		{
			nodeList.Add(rootNode);

			foreach (NodeBase node in rootNode.m_Children) 
			{
				BuildNodeList(node, ref nodeList);
			}
		}

		NodeBase CreateActionNode(XMLRules.ActionType action)
		{
			NodeBase result = null;

			if (action.HasChangeState()) 
			{
				NodeBase rhsNode = CreateArithmeticNode(action.GetChangeState().GetExpression());

				result = new SetNode(m_Container);
				((SetNode)result).m_VariableName = action.GetChangeState().GetVariable().GetName().Value;
				result.AddChild(rhsNode);
			} 
			else if (action.HasCallScript()) 
			{
				result = new CallNode(m_Container);

				string scriptName = action.GetCallScript().GetScriptName().Value;

				List<ScriptDescriptor> actionList = ScriptManager.GetScriptsByType(typeof(BaseAction));

                foreach (ScriptDescriptor script in actionList)
				{
					if (scriptName == script.m_Type.ToString())
					{
                        BaseScript newScript = null;
                        BaseScript.Load(action.GetCallScript(), m_Container.m_Editor.m_TargetObject, out newScript);

                        ((CallNode)result).m_Script = (BaseAction)newScript;
						break;
					}
				}
			}

			return result;
		}

		NodeBase CreateLogicalNode(LogicalExpression exp)
		{
			NodeBase node = null;

			if (exp.HasUnary())
			{
				node = new LogicalNode(m_Container);		
				((LogicalNode)node).m_LogicalType = LogicalOperatorType.NOT;			
				node.AddChild(CreateLogicalNode(exp.GetUnary().GetLogicalExpression()));
			}
			else if (exp.HasRelational())
			{
				node = new RelationalNode(m_Container);
				RelationalOperator op = exp.GetRelational().GetRelationalOperator();
				if (op.HasEquals())
					((RelationalNode)node).m_RelationType = RelationType.EQUAL;
				else if (op.HasGreaterThan())
					((RelationalNode)node).m_RelationType = RelationType.GREATERTHAN;
				else if (op.HasGreaterThanOrEquals())
					((RelationalNode)node).m_RelationType = RelationType.GREATERTHANOREQUAL;
				else if (op.HasLessThan())
					((RelationalNode)node).m_RelationType = RelationType.LESSTHAN;
				else if (op.HasLessThanOrEquals())
					((RelationalNode)node).m_RelationType = RelationType.LESSTHANOREQUAL;
				else if (op.HasNotEquals())
					((RelationalNode)node).m_RelationType = RelationType.NOTEQUAL;
				
				node.AddChild(CreateArithmeticNode(exp.GetRelational().GetLHSArithmeticExpression()));
				node.AddChild(CreateArithmeticNode(exp.GetRelational().GetRHSArithmeticExpression()));
			}
			else if (exp.HasLogical())
			{
				node = new LogicalNode(m_Container);
				
				if (exp.GetLogical().GetLogicalOperator().HasAnd())
					((LogicalNode)node).m_LogicalType = LogicalOperatorType.AND;
				else
					((LogicalNode)node).m_LogicalType = LogicalOperatorType.OR;
				
				node.AddChild(CreateLogicalNode(exp.GetLogical().GetLHSLogicalExpression()));
				node.AddChild(CreateLogicalNode(exp.GetLogical().GetRHSLogicalExpression()));
			}
			else if (exp.HasValue())
			{
				if (exp.GetValue().HasConstant())
				{
					node = new ConstantNode(m_Container);
					((ConstantNode)node).m_ConstantValue = exp.GetValue().GetConstant().GetBoolean().Value;
					((ConstantNode)node).m_ConstantType = VariableType.BOOLEAN;
				}
				else
				{
					node = new VariableNode(m_Container);
					
					((VariableNode)node).m_VariableName = exp.GetValue().GetVariable().GetName().Value;
					((VariableNode)node).m_VariableType = VariableType.BOOLEAN;
				}
			}
			
			return node;
		}
		
		NodeBase CreateArithmeticNode(ArithmeticExpression exp)
		{
			NodeBase node = null;
			
			if (exp.HasSubExpression())
			{
				node = CreateMathNode(exp.GetSubExpression());
			}
			else
			{
				if (exp.GetValue().HasConstant())
				{
					node = new ConstantNode(m_Container);
					if (exp.GetValue().GetConstant().HasFloat2())
					{
						((ConstantNode)node).m_ConstantValue = (float)exp.GetValue().GetConstant().GetFloat2().Value;
						((ConstantNode)node).m_ConstantType = VariableType.FLOAT;
					}
					else if (exp.GetValue().GetConstant().HasInteger())
					{
						((ConstantNode)node).m_ConstantValue = (int)exp.GetValue().GetConstant().GetInteger().Value;
						((ConstantNode)node).m_ConstantType = VariableType.INT;
					}
					else if (exp.GetValue().GetConstant().HasBoolean())
					{
						((ConstantNode)node).m_ConstantValue = (bool)exp.GetValue().GetConstant().GetBoolean().Value;
						((ConstantNode)node).m_ConstantType = VariableType.BOOLEAN;
					}
					else if (exp.GetValue().GetConstant().HasString2())
					{
						((ConstantNode)node).m_ConstantValue = (string)exp.GetValue().GetConstant().GetString2().Value;
						((ConstantNode)node).m_ConstantType = VariableType.STRING;
					}
				}
				else
				{
					node = new VariableNode(m_Container);
					((VariableNode)node).m_VariableName = exp.GetValue().GetVariable().GetName().Value;
					((VariableNode)node).m_VariableType = (VariableType)exp.GetValue().GetVariable().GetType2().Value;
				}
			}
			
			return node;
		}
		
		NodeBase CreateMathNode(SubExpressionType exp)
		{
			ArithemticOperator op = exp.GetArithmeticOperator();
			OperationType operationType = OperationType.ADD;
			
			//Add
			if (op.HasAdd())
			{
				operationType = OperationType.ADD;
			}
			//Subtract
			else if (op.HasSubtract())
			{
				operationType = OperationType.SUBTRACT;
			}
			//Divide
			else if (op.HasDivide())
			{
				operationType = OperationType.DIVIDE;
			}
			//Multiply
			else if (op.HasMultiply())
			{
				operationType = OperationType.MULTIPLY;
			}
			//Power
			else if (op.HasPower())
			{
				operationType = OperationType.POWER;
			}
			//Log : assumes that RHS is the base and LHS is the number. 
			else if (op.HasLog())
			{
				operationType = OperationType.LOG;
			}
			
			ArithmeticNode node = new ArithmeticNode(m_Container);
			node.m_OperationType = operationType;
			node.AddChild(CreateArithmeticNode(exp.GetLHSArithmeticExpression()));
			node.AddChild(CreateArithmeticNode(exp.GetRHSArithmeticExpression()));
			
			return node;
		}
	}
}
#endif