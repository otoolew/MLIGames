using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altova.Types;
using XMLRules;

namespace AiRuleEngine
{
    public class Rule : IEquatable<Rule>
    {
		public RuleType m_Rule { get; set; }
		public string m_Name { get; set; }
        bool m_HasFired;
        float m_Priority;
        InferenceEngine m_Context;
        List<string> m_VariableReferences = new List<string>();
        List<BaseAction> m_ScriptReferences = new List<BaseAction>();

        public Rule()
        {
            m_Name = "";
            m_HasFired = false;
            m_Priority = 0;
            m_Context = null;
        }

        public Rule(Rule rule)
        {
            this.Assign(rule);
        }

        public Rule(RuleType rule)
        {
            m_Rule = rule;
            m_Name = rule.GetName().Value;
            m_HasFired = false;
            m_Priority = 0;
            m_Context = null;

            FindVariableReferences();
        }

        public void SetContext(InferenceEngine inferenceEngine)
        {
            m_Context = inferenceEngine;
            FindScriptReferences();
        }

        public InferenceEngine GetContext()
        {
            return m_Context;
        }

        public void Assign(Rule rule)
        {
            m_Rule = rule.m_Rule;
            m_Name = rule.m_Name;
            m_HasFired = rule.m_HasFired;
            m_Priority = rule.m_Priority;
            m_Context = rule.m_Context;
        }

        public bool Eval()
        {
            bool result = false;

            if (!m_HasFired && HasDirtyReferences() && m_Rule.HasCondition())
                result = EvaluateLogicalExpression(m_Rule.GetCondition());

            return result;
        }

        public bool Validate()
        {
            bool validCondition = true;
			bool validAction = true;

            if (m_Rule.HasCondition())
				ValidateExpression(m_Rule.GetCondition(), out validCondition);

			for (int i = 0; i < m_Rule.GetActionCount(); i++) 
			{
				XMLRules.ActionType action = m_Rule.GetActionAt(i);
				if (action.HasChangeState())
					ValidateAssignmentExpression(action.GetChangeState(), out validAction);
			}

            return validCondition && validAction;
        }

        public void Execute()
        {
            m_HasFired = true;

            for (int i = 0; i < m_Rule.GetActionCount(); i++)
            {
                XMLRules.ActionType action = m_Rule.GetActionAt(i);

                if (action.HasChangeState())
                    EvaluateAssignmentExpression(action.GetChangeState());
                else
                {
                    foreach (BaseAction script in m_ScriptReferences)
                        script.Execute();
                }
            }
        }

        public override string ToString()
        {
            string retVal = "Rule " + m_Name + ": ";

            retVal += "if (";
            retVal += GetLogicalExpressionString(m_Rule.GetCondition());
            retVal += ")\n";
            retVal += GetActionString();

            return (string)retVal;
        }

		public override int GetHashCode()
		{
			return m_Name.GetHashCode();
		}

        public bool Equals(Rule rhs)
        {
			if (rhs == null)
				return false;

            return (m_Name == rhs.m_Name);
        }

        public bool GetHasFired()
        {
            return m_HasFired;
        }

        public void SetPriority(float priority)
        {
            m_Priority = priority;
        }

        public float GetPriority()
        {
            return m_Priority;
        }

        public void Reset()
        {
            m_HasFired = false;
        }

		VariableType ValidateAssignmentExpression(AssignmentExpression assignment, out bool valid)
		{
			bool expressionValid = true;
			VariableType returnType = VariableType.INT;
			valid = true;

			VariableType varType = (VariableType)assignment.GetVariable().GetType2().Value;
			VariableType expressionType = ValidateArithmeticExpression(assignment.GetExpression(), out expressionValid);

			bool typesValid = IsTypeCompatible(varType, expressionType, out returnType);
			valid = typesValid && expressionValid;

			return returnType;
		}

        VariableType ValidateExpression(LogicalExpression exp, out bool valid)
        {
            VariableType retType = VariableType.INT;
            valid = true;

            if (exp.HasUnary())
            { // condition is of type Unary
                retType = ValidateExpression(exp.GetUnary().GetLogicalExpression(), out valid);
                if (retType != VariableType.BOOLEAN)
                    valid = false;
            }
            else if (exp.HasValue())
            {
                if (exp.GetValue().HasVariable()) //is a variable
				{
                    retType = (VariableType)exp.GetValue().GetVariable().GetType2().Value;
				}
                else // is a constant
                    retType = GetConstantType(exp.GetValue().GetConstant());
            }
            else if (exp.HasLogical())// condition is of type Logical
            {
				bool valid1;
				bool valid2;

				VariableType t1 = ValidateExpression(exp.GetLogical().GetLHSLogicalExpression(), out valid1);
				VariableType t2 = ValidateExpression(exp.GetLogical().GetRHSLogicalExpression(), out valid2);

				valid = valid1 && valid2;

                if ((t1 == VariableType.BOOLEAN) && (t2 == VariableType.BOOLEAN))
                {
                    retType = VariableType.BOOLEAN;
                }
                else
                    valid = false;
            }
            else // condition is of type relational
            {
                VariableType type = VariableType.INT;

                if (IsValidRelational(exp.GetRelational().GetLHSArithmeticExpression(), exp.GetRelational().GetRHSArithmeticExpression(),
                    exp.GetRelational().GetRelationalOperator(), type))
                    retType = type;

                else
                    valid = false;
            }

            return retType;
        }

        bool IsValidRelational(ArithmeticExpression lhs, ArithmeticExpression rhs, RelationalOperator op, VariableType type)
        {
            bool valid1;
			bool valid2;

            VariableType t1 = ValidateArithmeticExpression(lhs, out valid1);

            VariableType t2 = ValidateArithmeticExpression(rhs, out valid2);

            return IsTypeCompatible(t1, t2, out type) && valid1 && valid2;
        }

        VariableType ValidateArithmeticExpression(ArithmeticExpression exp, out bool valid)
        {
            VariableType varType = VariableType.INT;
            valid = true;

            if (exp.HasSubExpression())
            {
				bool valid1;
				bool valid2;

                //ArithemticOperator op = exp.GetSubExpression().GetArithmeticOperator();
                VariableType t1 = ValidateArithmeticExpression(exp.GetSubExpression().GetLHSArithmeticExpression(), out valid1);
                VariableType t2 = ValidateArithmeticExpression(exp.GetSubExpression().GetRHSArithmeticExpression(), out valid2);

                valid = IsTypeCompatible(t1, t2, out varType) && valid1 && valid2;
            }
            else //is a value
            {
                if (exp.GetValue().HasVariable()) //is a variable
                    varType = (VariableType)exp.GetValue().GetVariable().GetType2().Value;
                else // is a constant
                    varType = GetConstantType(exp.GetValue().GetConstant());
            }

            return varType;
        }

        bool IsTypeCompatible(VariableType t1, VariableType t2, out VariableType retType)
        {
            bool retVal = false;

            retType = t1;

            if (t1 == t2)
                retVal = true;
            else if ((t1 == VariableType.FLOAT && t2 == VariableType.INT) || (t1 == VariableType.INT && t2 == VariableType.FLOAT))
            {
                retType = VariableType.FLOAT;
                retVal = true;
            }

            return retVal;
        }

        bool EvaluateLogicalExpression(LogicalExpression exp)
        {
            bool retVal = false;

            if (exp.HasUnary())
            {
                retVal = EvaluateLogicalExpression(exp.GetUnary().GetLogicalExpression());
            }
            else if (exp.HasRelational())
            {
                RelationalOperator op = exp.GetRelational().GetRelationalOperator();
                ConstantType lhs = EvaluateArithmeticExpression(exp.GetRelational().GetLHSArithmeticExpression());
                ConstantType rhs = EvaluateArithmeticExpression(exp.GetRelational().GetRHSArithmeticExpression());

                if (op.HasEquals())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        retVal = ((bool)lhs.GetBoolean().Value == (bool)rhs.GetBoolean().Value);
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value == (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value == (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        string lhsString = (string)lhs.GetString2().Value;
                        string rhsString = (string)rhs.GetString2().Value;

                        retVal = (lhsString == rhsString);
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value == (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value == (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasNotEquals())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        retVal = ((bool)lhs.GetBoolean().Value != (bool)rhs.GetBoolean().Value);
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value != (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value != (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        retVal = (lhs.GetString2() != rhs.GetString2());
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value != (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value != (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasPartOf())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        retVal = ((bool)lhs.GetBoolean().Value == (bool)rhs.GetBoolean().Value);
                    }  
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value == (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value == (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        retVal = (lhs.GetString2() == rhs.GetString2());
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value == (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value == (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasNotPartOf())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        retVal = ((bool)lhs.GetBoolean().Value != (bool)rhs.GetBoolean().Value);
                    }     
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value != (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value != (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        retVal = (lhs.GetString2().Value != rhs.GetString2().Value);
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value != (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value != (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasLessThan())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        throw new InvalidRuleException("Incompatible types in expression");
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value < (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value < (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        retVal = String.Compare(lhs.GetString2().Value, rhs.GetString2().Value, true) == -1;
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value < (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value < (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasLessThanOrEquals())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        throw new InvalidRuleException("Incompatible types in expression");
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value <= (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value <= (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        int result = String.Compare(lhs.GetString2().Value, rhs.GetString2().Value);
                        retVal = (result == -1) || (result == 0);
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value <= (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value <= (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasGreaterThan())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        throw new InvalidRuleException("Incompatible types in expression");
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value > (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value > (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        retVal = String.Compare(lhs.GetString2().Value, rhs.GetString2().Value, true) == 1;
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value > (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value > (float)rhs.GetInteger().Value);
                    }
                }
                else if (op.HasGreaterThanOrEquals())
                {
                    if (lhs.HasBoolean() && rhs.HasBoolean())
                    {
                        throw new InvalidRuleException("Incompatible types in expression");
                    }
                    else if (lhs.HasInteger())
                    {
                        if (rhs.HasInteger())
                            retVal = ((int)lhs.GetInteger().Value >= (int)rhs.GetInteger().Value);
                        else if (rhs.HasFloat2())
                            retVal = ((int)lhs.GetInteger().Value >= (int)rhs.GetFloat2().Value);
                    }
                    else if (lhs.HasString2() && rhs.HasString2())
                    {
                        int result = String.Compare(lhs.GetString2().Value, rhs.GetString2().Value, true);
                        retVal = (result == 1) || (result == 0);
                    }
                    else if (lhs.HasFloat2())
                    {
                        if (rhs.HasFloat2())
                            retVal = ((float)lhs.GetFloat2().Value >= (float)rhs.GetFloat2().Value);
                        else if (rhs.HasInteger())
                            retVal = ((float)lhs.GetFloat2().Value >= (float)rhs.GetInteger().Value);
                    }
                }
            }
            else if (exp.HasLogical())
            {
                bool lhs = EvaluateLogicalExpression(exp.GetLogical().GetLHSLogicalExpression());
                bool rhs = EvaluateLogicalExpression(exp.GetLogical().GetRHSLogicalExpression());

                if (exp.GetLogical().GetLogicalOperator().HasAnd())
                {
                    retVal = lhs && rhs;
                }
                else//OR
                {
                    retVal = lhs || rhs;
                }
            }
            else if (exp.HasValue())
            {
                if (exp.GetValue().HasConstant())
                {
                    retVal = exp.GetValue().GetConstant().GetBoolean().Value;
                }
                else
                {
                    string currName = exp.GetValue().GetVariable().GetName().Value;
                    Variable variable;

                    if (m_Context.GetState().GetVariable(currName, out variable))
                    {
                        retVal = (bool)variable.GetValue();
                    }
                }
            }
            return retVal;
        }

        void EvaluateAssignmentExpression(AssignmentExpression exp)
        {
            ConstantType val = EvaluateArithmeticExpression(exp.GetExpression());

            Variable variable;

            if (m_Context.GetState().GetVariable(exp.GetVariable().GetName().Value, out variable))
            {
                variable.SetValue(Variable.GetValueFromConstant(variable.GetVariableType(), val));
            }
        }

        ConstantType EvaluateArithmeticExpression(ArithmeticExpression exp)
        {
            ConstantType retVal;

            if (exp.HasSubExpression())
            {
                retVal = EvalSubExpression(exp.GetSubExpression());
            }
            else
            {
                if (exp.GetValue().HasConstant())
                    retVal = exp.GetValue().GetConstant();
                else
                {
                    Variable currVar;
                    string currName = exp.GetValue().GetVariable().GetName().Value;
                    retVal = new ConstantType();

                    if (m_Context.GetState().GetVariable(currName, out currVar))
                    {
                        if (currVar.GetVariableType() == VariableType.BOOLEAN)
                        {
                            retVal.AddBoolean(new SchemaBoolean((bool)currVar.GetValue()));
                        }
						else if (currVar.GetVariableType() == VariableType.INT)
                        {
                            retVal.AddInteger(new SchemaLong((int)currVar.GetValue()));
                        }
						else if (currVar.GetVariableType() == VariableType.FLOAT)
                        {
                            retVal.AddFloat2(new SchemaDecimal((decimal)(float)currVar.GetValue()));
                        }
						else if (currVar.GetVariableType() == VariableType.STRING)
                        {
                            string value = (string)currVar.GetValue();
                            retVal.AddString2(new SchemaString(value));
                        }
                    }
                }
            }

            return retVal;
        }

        ConstantType EvalSubExpression(SubExpressionType exp)
        {
            ConstantType retVal = new ConstantType();

            ArithemticOperator op = exp.GetArithmeticOperator();
            decimal lhs = (decimal)GetFloatOrIntValue(EvaluateArithmeticExpression(exp.GetLHSArithmeticExpression()));
            decimal rhs = (decimal)GetFloatOrIntValue(EvaluateArithmeticExpression(exp.GetRHSArithmeticExpression()));

            //Add
            if (op.HasAdd())
            {
                retVal.AddFloat2(new SchemaDecimal(lhs + rhs));
            }
            //Subtract
            else if (op.HasSubtract())
            {
                retVal.AddFloat2(new SchemaDecimal(lhs - rhs));
            }
            //Divide
            else if (op.HasDivide())
            {
                retVal.AddFloat2(new SchemaDecimal(lhs / rhs));
            }
            //Multiply
            else if (op.HasMultiply())
            {
                retVal.AddFloat2(new SchemaDecimal(lhs * rhs));
            }
            //Power
            else if (op.HasPower())
            {
                retVal.AddFloat2(new SchemaDecimal((decimal)Math.Pow((double)lhs, (double)rhs)));
            }
            //Log : assumes that RHS is the base and LHS is the number. 
            else if (op.HasLog())
            {
                retVal.AddFloat2(new SchemaDecimal((decimal)(Math.Log((double)lhs) / Math.Log((double)rhs))));
            }

            return retVal;
        }

        VariableType GetConstantType(ConstantType constant)
        {
            if (constant.HasBoolean())
                return VariableType.BOOLEAN;
            else if (constant.HasFloat2())
                return VariableType.FLOAT;
            else if (constant.HasInteger())
                return VariableType.INT;
            else
                return VariableType.STRING;
        }

        float GetFloatOrIntValue(ConstantType constant)
        {
            if (GetConstantType(constant) == VariableType.FLOAT)
                return (float)constant.GetFloat2().Value;
            else if (GetConstantType(constant) == VariableType.INT)
                return constant.GetInteger().Value;

            return 0;
        }

        string GetLogicalExpressionString(LogicalExpression exp)
        {
            string retVal = "";

            if (exp.HasUnary())
            {
                retVal = GetLogicalExpressionString(exp.GetUnary().GetLogicalExpression());
            }
            else if (exp.HasRelational())
            {
                RelationalOperator op = exp.GetRelational().GetRelationalOperator();
                string lhs = GetArithmeticExpressionString(exp.GetRelational().GetLHSArithmeticExpression());
                string rhs = GetArithmeticExpressionString(exp.GetRelational().GetRHSArithmeticExpression());
                string opString = "";

                if (op.HasEquals())
                {
                    opString = "==";
                }
                else if (op.HasNotEquals())
                {
                    opString = "!=";
                }
                else if (op.HasPartOf())
                {
                    opString = "partof";
                }
                else if (op.HasNotPartOf())
                {
                    opString = "notpartof";
                }
                else if (op.HasLessThan())
                {
                    opString = "<";
                }
                else if (op.HasLessThanOrEquals())
                {
                    opString = "<=";
                }
                else if (op.HasGreaterThan())
                {
                    opString = ">";
                }
                else if (op.HasGreaterThanOrEquals())
                {
                    opString = ">=";
                }

                retVal = lhs + " " + opString + " " + rhs;
            }
            else if (exp.HasLogical())
            {
                string lhs = GetLogicalExpressionString(exp.GetLogical().GetLHSLogicalExpression());
                string rhs = GetLogicalExpressionString(exp.GetLogical().GetRHSLogicalExpression());
                string opString;

                if (exp.GetLogical().GetLogicalOperator().HasAnd())
                {
                    opString = "and";
                }
                else //OR
                {
                    opString = "or";
                }

                retVal = lhs + " " + opString + " " + rhs;
            }
            else if (exp.HasValue())
            {
                if (exp.GetValue().HasConstant())
                {
                    if ((bool)exp.GetValue().GetConstant().GetBoolean().Value)
                        retVal = "true";
                    else
                        retVal = "false";
                }
                else
                {
                    retVal = exp.GetValue().GetVariable().GetName().Value;
                }
            }

            return retVal;
        }

        string GetArithmeticExpressionString(ArithmeticExpression exp)
        {
            string retVal = "";

            if (exp.HasSubExpression())
            {
                retVal = GetSubExpressionString(exp.GetSubExpression());
            }
            else
            {
                if (exp.GetValue().HasConstant())
                    retVal = GetConstantString(exp.GetValue().GetConstant());
                else
                    retVal = exp.GetValue().GetVariable().GetName().Value;
            }

            return retVal;
        }

        string GetConstantString(ConstantType constant)
        {
            string retVal = "Unknown Value";

            if (constant.HasBoolean())
            {
                if ((bool)constant.GetBoolean().Value)
                    retVal = "true";
                else
                    retVal = "false";
            }
            else if (constant.HasFloat2())
            {
                retVal = constant.GetFloat2().ToString();
            }
            else if (constant.HasInteger())
            {
                retVal = constant.GetInteger().Value.ToString();
            }
            else if (constant.HasString2())
            {
                retVal = (string)constant.GetString2().Value;
            }

            return retVal;
        }

        string GetSubExpressionString(SubExpressionType exp)
        {
            string retVal;

            ArithemticOperator op = exp.GetArithmeticOperator();

            string lhs = GetArithmeticExpressionString(exp.GetLHSArithmeticExpression());
            string rhs = GetArithmeticExpressionString(exp.GetRHSArithmeticExpression());
            string opString = "";

            //Add
            if (op.HasAdd())
            {
                opString = "+";
            }
            //Subtract
            else if (op.HasSubtract())
            {
                opString = "-";
            }
            //Divide
            else if (op.HasDivide())
            {
                opString = "/";
            }
            //Multiply
            else if (op.HasMultiply())
            {
                opString = "*";
            }
            //Power
            else if (op.HasPower())
            {
                opString = "^";
            }
            //Log : assumes that RHS is the base and LHS is the number. 
            else if (op.HasLog())
            {
                opString = "log";
            }

            retVal = lhs + " " + opString + " " + rhs;

            return retVal;
        }

        string GetAssignmentExpressionString(AssignmentExpression exp)
        {
            string retVal;

            string rhs = GetArithmeticExpressionString(exp.GetExpression());
            string variableName = exp.GetVariable().GetName().Value;

            retVal = variableName + " = " + rhs;

            return retVal;
        }

        string GetActionString()
        {
            string retVal = "";

            for (int i = 0; i < m_Rule.GetActionCount(); i++)
            {
                XMLRules.ActionType action = m_Rule.GetActionAt(i);

                if (action.HasChangeState())
                {
                    retVal += GetAssignmentExpressionString(action.GetChangeState());
                    retVal += "\n";
                }
                else if (action.HasCallScript())
                {
                    retVal += "Call script: " + action.GetCallScript().GetScriptName();
                    retVal += "\n";
                }
            }

            return retVal;
        }

        void FindVariableReferences()
        {
            m_VariableReferences.Clear();

            if (m_Rule.HasCondition())
            {
                VisitLogicalExpression(m_Rule.GetCondition());

                // Get rid of duplicates
                m_VariableReferences = m_VariableReferences.Distinct().ToList();
            }
        }

        void FindScriptReferences()
        {
            if (m_Rule.HasAction())
            {
                for (int i = 0; i < m_Rule.GetActionCount(); i++)
                {
                    XMLRules.ActionType action = m_Rule.GetActionAt(i);

                    if (action.HasCallScript())
                    {
                        BaseScript actionScript;

                        if (BaseScript.Load(action.GetCallScript(), m_Context.gameObject, out actionScript))
                        {
                            m_ScriptReferences.Add((BaseAction)actionScript);
                        }
                    }
                }
            }
        }

        bool HasDirtyReferences()
        {
            bool result = false;

	        foreach (string variableName in m_VariableReferences)
	        {
		        Variable variable = new Variable();

		        if (m_Context.GetState().GetVariable(variableName, out variable))
		        {
			        if (variable.m_IsDirty)
			        {
				        result = true;
				        break;
			        }
		        }
	        }

	        return result;
        }

        void VisitLogicalExpression(LogicalExpression exp)
        {
            if (exp.HasUnary())
            {
                VisitLogicalExpression(exp.GetUnary().GetLogicalExpression());
            }
            else if (exp.HasRelational())
            {
                VisitArithmeticExpression(exp.GetRelational().GetLHSArithmeticExpression());
                VisitArithmeticExpression(exp.GetRelational().GetRHSArithmeticExpression());
            }
            else if (exp.HasLogical())
            {
                VisitLogicalExpression(exp.GetLogical().GetLHSLogicalExpression());
                VisitLogicalExpression(exp.GetLogical().GetRHSLogicalExpression());
            }
            else if (exp.HasValue())
            {
                if (exp.GetValue().HasVariable())
                {
                    m_VariableReferences.Add(exp.GetValue().GetVariable().GetName().Value);
                }
            }
        }

        void VisitArithmeticExpression(ArithmeticExpression exp)
        {
            if (exp.HasSubExpression())
            {
                VisitSubExpression(exp.GetSubExpression());
            }
            else
            {
                if (exp.GetValue().HasVariable())
                {
                    m_VariableReferences.Add(exp.GetValue().GetVariable().GetName().Value);
                }
            }
        }

        void VisitSubExpression(SubExpressionType exp)
        {
            VisitArithmeticExpression(exp.GetLHSArithmeticExpression());
            VisitArithmeticExpression(exp.GetRHSArithmeticExpression());
        }
    }
}
