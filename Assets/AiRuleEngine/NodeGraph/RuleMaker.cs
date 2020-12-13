using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AiRuleEngine;
using Altova;
using XMLRules;
 
namespace AiRuleEngine
{
    public class RuleMaker
    {
        public Value MakeVariable(string name, AiRuleEngine.VariableType type)
        {
            Value newValue = new Value();
            XMLRules.VariableType newVariable = new XMLRules.VariableType();
            newVariable.AddName(new Altova.Types.SchemaString(name));
            newVariable.AddType2(new Altova.Types.SchemaLong((long)type));
            newValue.AddVariable(newVariable);

            return newValue;
        }

        public Value MakeConstant(AiRuleEngine.VariableType type, object value)
        {
            Value newValue = new Value();
            ConstantType newConstant = new ConstantType();
            switch (type)
            {
                case AiRuleEngine.VariableType.BOOLEAN:
                    newConstant.AddBoolean(new Altova.Types.SchemaBoolean((bool)value));
                    break;

                case AiRuleEngine.VariableType.INT:
                    newConstant.AddInteger(new Altova.Types.SchemaLong((long)(int)value));
                    break;

                case AiRuleEngine.VariableType.FLOAT:
                    newConstant.AddFloat2(new Altova.Types.SchemaDecimal((decimal)(float)value));
                    break;

                case AiRuleEngine.VariableType.STRING:
                    newConstant.AddString2(new Altova.Types.SchemaString((string)value));
                    break;
            }

            newValue.AddConstant(newConstant);

            return newValue;
        }

        public ArithmeticExpression MakeAssignmentRHS(Value value)
        {
            ArithmeticExpression arithmeticExpression = new ArithmeticExpression();
            arithmeticExpression.AddValue(value);

            return arithmeticExpression;
        }
		
		public RelationalType MakeRelational(RelationalOperator op, ArithmeticExpression lhs, ArithmeticExpression rhs)
        {
            RelationalType newRelational = new RelationalType();

            newRelational.AddLHSArithmeticExpression(lhs);
            newRelational.AddRelationalOperator(op);
            newRelational.AddRHSArithmeticExpression(rhs);

            return newRelational;

        }
		
		public LogicalExpression MakeRelationalExperssion(string op, ArithmeticExpression lhs, ArithmeticExpression rhs)
        {
            RelationalOperator relationalOp = new RelationalOperator();
            if (op == "==")
                relationalOp.AddEquals(new Altova.Types.SchemaString("Equals"));
            else if (op == "!=")
                relationalOp.AddNotEquals(new Altova.Types.SchemaString("NotEquals"));
            else if (op == "<")
                relationalOp.AddLessThan(new Altova.Types.SchemaString("LessThan"));
            else if (op == "<=")
                relationalOp.AddLessThanOrEquals(new Altova.Types.SchemaString("LessThanOrEquals"));
            else if (op == ">")
                relationalOp.AddGreaterThan(new Altova.Types.SchemaString("GreaterThan"));
            else if (op == "<=")
                relationalOp.AddGreaterThanOrEquals(new Altova.Types.SchemaString("GreaterThanOrEquals"));

			RelationalType relationalExpression = MakeRelational(relationalOp, lhs, rhs);

            LogicalExpression newLogical = new LogicalExpression();
			newLogical.AddRelational(relationalExpression);

            return newLogical;
        }
		
		public ArithmeticExpression MakeArithmeticSubExperssion(string op, ArithmeticExpression lhs, ArithmeticExpression rhs)
        {
			ArithemticOperator arithmeticOP = new ArithemticOperator();
			SubExpressionType newArithmetic = new SubExpressionType();
			
		    if (op == "+")
                arithmeticOP.AddAdd(new Altova.Types.SchemaString("Addition"));
            else if (op == "-")
                arithmeticOP.AddSubtract(new Altova.Types.SchemaString("Subtract"));
            else if (op == "*")
                arithmeticOP.AddMultiply(new Altova.Types.SchemaString("Multiply"));
            else if (op == "/")
                arithmeticOP.AddDivide(new Altova.Types.SchemaString("Divide"));
            else if (op == "^")
                arithmeticOP.AddPower(new Altova.Types.SchemaString("Mod"));
			else if (op == "Log")
				arithmeticOP.AddLog(new Altova.Types.SchemaString("Log"));
			
            newArithmetic.AddLHSArithmeticExpression(lhs);
            newArithmetic.AddArithmeticOperator(arithmeticOP);
            newArithmetic.AddRHSArithmeticExpression(rhs);
			
			return MakeArithmeticExperssion(newArithmetic);
        }

		public ArithmeticExpression MakeArithmeticExperssion(SubExpressionType expression)
		{
			ArithmeticExpression arithmeticExpression = new ArithmeticExpression();
			
            arithmeticExpression.AddSubExpression(expression);	   					   

            return arithmeticExpression;
        }
		
		public ArithmeticExpression MakeArithmeticExperssion(Value value)
        {
			ArithmeticExpression arithmeticExpression = new ArithmeticExpression();
			
            arithmeticExpression.AddValue(value);  					   

            return arithmeticExpression;
        }
		

        public LogicalExpression MakeLogical(string op, LogicalExpression lhs, LogicalExpression rhs)
        {
            LogicalOperator logicalOp = new LogicalOperator();

            if (op == "And")
                logicalOp.AddAnd(new Altova.Types.SchemaString("And"));
            else
                logicalOp.AddOr(new Altova.Types.SchemaString("Or"));

            LogicalType newLogicalExpression = new LogicalType();
            newLogicalExpression.AddLHSLogicalExpression(lhs);
            newLogicalExpression.AddLogicalOperator(logicalOp);
            newLogicalExpression.AddRHSLogicalExpression(rhs);

            LogicalExpression newLogical = new LogicalExpression();
            newLogical.AddLogical(newLogicalExpression);

            return newLogical;
        }

		public LogicalExpression MakeLogical(LogicalExpression rhs)
		{
			UnaryOperator logicalOp = new UnaryOperator();

			logicalOp.AddNot(new Altova.Types.SchemaString("Not"));

			UnaryType newUnaryExpression = new UnaryType();
			newUnaryExpression.AddUnaryOperator(logicalOp);
			newUnaryExpression.AddLogicalExpression(rhs);
			
			LogicalExpression newLogical = new LogicalExpression();
			newLogical.AddUnary(newUnaryExpression);
			
			return newLogical;
		}

		public LogicalExpression MakeLogical(Value value)
		{
			LogicalExpression logicalExpression = new LogicalExpression();
			
			logicalExpression.AddValue(value);  					   
			
			return logicalExpression;
		}

        public XMLRules.ActionType MakeStateChangeAction(string name, AiRuleEngine.VariableType type, Value value)
        {
			XMLRules.ActionType action = new XMLRules.ActionType();
            AssignmentExpression assignment = new AssignmentExpression();
            assignment.AddVariable(MakeVariable(name, type).GetVariable());
            assignment.AddExpression(MakeAssignmentRHS(value));

            action.AddCertainty(new Altova.Types.SchemaDecimal(100));
            action.AddChangeState(assignment);

            return action;
        }

		public XMLRules.ActionType MakeStateChangeAction(string name, AiRuleEngine.VariableType type, ArithmeticExpression value)
		{
			XMLRules.ActionType action = new XMLRules.ActionType();
			AssignmentExpression assignment = new AssignmentExpression();
			assignment.AddVariable(MakeVariable(name, type).GetVariable());
			assignment.AddExpression(value);
			
			action.AddCertainty(new Altova.Types.SchemaDecimal(100));
			action.AddChangeState(assignment);
			
			return action;
		}

		public XMLRules.ActionType MakeCallScriptAction(CallNode callNode)
        {
			XMLRules.ActionType action = new XMLRules.ActionType();
            
            ScriptReference scriptReference;

            BaseScript.Save(callNode.m_Script, out scriptReference);
            
            action.AddCallScript(scriptReference);

            return action;
        }
    }
}
