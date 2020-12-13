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
	public class GraphToRule
	{
		RuleMaker m_RuleMaker = new RuleMaker();
		State m_State = null;

		public GraphToRule(State state)
		{
			m_State = state; 
		}

		public RuleType Convert(string name, NodeBase rootCondition, List<NodeBase> actionList)
		{
			RuleType rule = new RuleType();

			rule.AddName(new Altova.Types.SchemaString(name));

			//this first node is the condition. Called traverse twice to create the two expressions
			LogicalExpression expression = CreateLogical(rootCondition);

			rule.AddCondition(expression);

			foreach (NodeBase node in actionList)
			{
				if ((node.m_NodeType == NodeType.SETVARIABLE) || (node.m_NodeType == NodeType.CALLSCRIPT))
				{
					XMLRules.ActionType action = MakeAction(node);
					if (action != null)
						rule.AddAction(action);
					else
						Debug.LogError("Error saving an action node");
				}
			}

			rule.AddPriority(new Altova.Types.SchemaLong(100L));

			return rule;
		}

		XMLRules.ActionType MakeAction(NodeBase node)
		{
			if (node.m_NodeType == NodeType.SETVARIABLE)
			{
				SetNode setNode = (SetNode)node;

				if (m_State != null) 
				{
					Variable variable;

					if  (setNode.m_Children.Count >= 1)
					{
						if (m_State.GetVariable(setNode.m_VariableName, out variable))
						{
							ArithmeticExpression value = CreateArithmeticExpression(setNode.m_Children[0]);
							return m_RuleMaker.MakeStateChangeAction(setNode.m_VariableName, variable.m_Type, value);
						}
						else
						{
							Debug.LogError("Invalid assignment: variable " + setNode.m_VariableName + " not found");
						}
					}
					else
					{
						Debug.LogError("Invalid assignment: no value was specified");
					}
				}
			}
			else if (node.m_NodeType == NodeType.CALLSCRIPT)
			{
				CallNode callNode = (CallNode)node;
				return m_RuleMaker.MakeCallScriptAction(callNode);
			}

			return null;
		}

		LogicalExpression CreateLogical(NodeBase node)
		{
			LogicalExpression expression = null;

			switch (node.m_NodeType) 
			{
				case NodeType.RELATIONAL:
					expression = CreateRelationalExpression(node);
				break;

				case NodeType.LOGICAL:
					expression = CreateLogicalExpression(node);
				break;
			
				case NodeType.CONSTANT:
				{
					Value value = m_RuleMaker.MakeConstant(((ConstantNode)node).m_ConstantType, ((ConstantNode)node).m_ConstantValue);
					expression = m_RuleMaker.MakeLogical(value);
				}
				break;

				case NodeType.VARIABLE:
				{
					Value value = m_RuleMaker.MakeVariable(((VariableNode)node).m_VariableName, ((VariableNode)node).m_VariableType);
					expression = m_RuleMaker.MakeLogical(value);
				}
				break;
			}

			return expression;
		}

		LogicalExpression CreateLogicalExpression(NodeBase node)
		{
			LogicalExpression expression = null;

			LogicalNode logicalNode = (LogicalNode)node;

			if (logicalNode.m_LogicalType == LogicalOperatorType.NOT) 
			{
				LogicalExpression rhs = CreateLogical(node.m_Children[0]);
				expression = m_RuleMaker.MakeLogical(rhs);
			}
			else 
			{
				switch (node.m_NodeType) 
				{
					case NodeType.RELATIONAL:
					{
						ArithmeticExpression lhs = CreateArithmeticExpression(node.m_Children[0]);
						ArithmeticExpression rhs = CreateArithmeticExpression(node.m_Children[1]);
						expression = m_RuleMaker.MakeRelationalExperssion(((RelationalNode)node).GetOperator(), lhs, rhs);
					}
					break;

					case NodeType.LOGICAL:
					{
						LogicalExpression lhs = CreateLogical(node.m_Children[0]);
						LogicalExpression rhs = CreateLogical(node.m_Children[1]);
						expression = m_RuleMaker.MakeLogical(((LogicalNode)node).GetOperator(), lhs, rhs);
					}
					break;
				}
			}
	
			return expression;
		}

		LogicalExpression CreateRelationalExpression(NodeBase node)
		{
			ArithmeticExpression lhs = CreateArithmeticExpression(node.m_Children[0]);
			ArithmeticExpression rhs = CreateArithmeticExpression(node.m_Children[1]);
			
			return m_RuleMaker.MakeRelationalExperssion(((RelationalNode)node).GetOperator(), lhs, rhs);
		}

		ArithmeticExpression CreateArithmetic(NodeBase node)
		{
			ArithmeticExpression lhs = CreateArithmeticExpression(node.m_Children[0]);
			ArithmeticExpression rhs = CreateArithmeticExpression(node.m_Children[1]);
			
			return m_RuleMaker.MakeArithmeticSubExperssion(((ArithmeticNode)node).GetOperator(), lhs, rhs);
		}

		ArithmeticExpression CreateArithmeticExpression(NodeBase node)
		{
			ArithmeticExpression expression = null;

			switch (node.m_NodeType) 
			{
				case NodeType.ARITHMETIC:
					expression = CreateArithmetic(node);
				break;

				case NodeType.CONSTANT:
				{
					Value value = m_RuleMaker.MakeConstant(((ConstantNode)node).m_ConstantType, ((ConstantNode)node).m_ConstantValue);
					expression = m_RuleMaker.MakeArithmeticExperssion(value);
				}
				break;

				case NodeType.VARIABLE:
				{
					Value value = m_RuleMaker.MakeVariable(((VariableNode)node).m_VariableName, ((VariableNode)node).m_VariableType);
					expression = m_RuleMaker.MakeArithmeticExperssion(value);
				}
				break;
			}

			return expression;
		}
	}
}
#endif