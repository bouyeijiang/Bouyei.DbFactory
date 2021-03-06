using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SysExp = System.Linq.Expressions;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class WhereBase:WordsBase
    {
        public WhereBase():base()
        { }

        public WhereBase(Type type):base(type)
        { }

        internal void PredicateParser(SysExp.Expression exp, StringBuilder builder, ExpDirection direct)
        {
            SysExp.BinaryExpression binExp = exp as SysExp.BinaryExpression;
            if (binExp != null)
            { 
                builder.Append("(");//每次递归需要一个括号
                PredicateParser(binExp.Left, builder, ExpDirection.Left);
            }
            string where = string.Empty;
            switch (exp.NodeType)
            {
                case SysExp.ExpressionType.Parameter:
                    where = VisitParameterExp(exp as SysExp.ParameterExpression);
                    break;
                case SysExp.ExpressionType.Constant:
                    where = VisitConstantExp(exp as SysExp.ConstantExpression);
                    break;
                case SysExp.ExpressionType.Call:
                    where = VisitMethodCallExp(exp as SysExp.MethodCallExpression,direct);
                    break;
                case SysExp.ExpressionType.MemberAccess:
                    where = VisitMemberExp(exp as SysExp.MemberExpression,direct);
                    break;
                case SysExp.ExpressionType.Convert:
                    {
                        var mem = (exp as SysExp.UnaryExpression).Operand;
                        where = VisitMemberExp(mem as SysExp.MemberExpression,direct);
                    }
                    break;
                default:
                    where = VisitOperatorExp(exp as SysExp.BinaryExpression);
                    break;
            }

            builder.Append(where);

            if (binExp != null)
            {
                PredicateParser(binExp.Right, builder, ExpDirection.Right);
                builder.Append(")");
            }
        }

        internal string VisitMemberExp(SysExp.MemberExpression memberExp,ExpDirection direct)
        {
            string value = string.Empty;
            if(memberExp.Expression==null)
            {
                if (memberExp.Type.Name == "Guid"
                    && memberExp.Member.Name == "Empty")
                {
                    return Guid.Empty.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            bool isValue = memberExp.Expression.ToString().StartsWith("value(");

            if (isValue==false)
            {
                value = GetTableName() + "." + memberExp.Member.Name;
            }
            else
            {
                value = VisitMemberExpValue(memberExp);
            }

            return value;
        }

        protected string VisitParameterExp(SysExp.ParameterExpression parameterExp)
        {
            return GetTableName() + "." + parameterExp.Name;
        }

        protected string VisitConstantExp(SysExp.ConstantExpression constantExp)
        {
            string value = string.Empty;
            object val = constantExp.Value;

            switch (constantExp.Type.Name)
            {
                case "Boolean":
                    {
                        if (val is true) value = "1=1";
                        else if (val is false) value = "1=0";
                        else value = val.ToString();
                    }
                    break;
                case "Int32":
                case "Int16":
                case "Double":
                case "Single":
                    value = val.ToString();
                    break;
                case "DateTime":
                case "String":
                    {
                        if (val == null) value = "NULL";
                        else value = $"'{val}'";
                    }
                    break;
                case "Int32[]":
                case "Int64[]":
                case "Int16[]":
                case "Single[]":
                case "Double[]":
                    {
                        var values = val as IEnumerable;
                        var param = (from object v in values select v).ToArray();
                        value = string.Join(",", param);
                    }
                    break;
                case "String[]":
                case "DateTime[]":
                case "Array":
                    {
                        var values = val as IEnumerable;
                        var param = (from object v in values select v).ToArray();
                        value = $"'{ string.Join("','", param)}'";
                    }
                    break;
                default:
                    {
                        if (val == null) value = "''";
                        else value = $"'{val}'";
                    }
                    break;
            }
            return value;
        }

        internal string VisitMemberExpValue(SysExp.MemberExpression memberExp)
        {
            string value = string.Empty;
            switch (memberExp.Type.Name)
            {
                case "Boolean":
                    {
                        var val = SysExp.Expression.Lambda<Func<bool>>(memberExp).Compile()();
                        if (val is true) value = "1=1";
                        else if (val is false) value = "1=0";
                        else value = val.ToString();
                    }
                    break;
                case "Int16":
                case "Int32":
                    {
                        var val = SysExp.Expression.Lambda<Func<int>>(memberExp).Compile()();
                        value = val.ToString();
                    }
                    break;
                case "Double":
                case "Single":
                    {
                        var val = SysExp.Expression.Lambda<Func<double>>(memberExp).Compile()();
                        value = val.ToString();
                    }
                    break;
                case "DateTime":
                    {
                        var val = SysExp.Expression.Lambda<Func<DateTime>>(memberExp).Compile()();
                        value = $"'{val}'";
                    }
                    break;
                case "String":
                    {
                        var val = SysExp.Expression.Lambda<Func<string>>(memberExp).Compile()();
                        if (val == null) val = "NULL";
                        else value = $"'{val}'";
                    }
                    break;
                case "Guid":
                    {
                        var val = SysExp.Expression.Lambda<Func<Guid>>(memberExp).Compile()();
                        value = $"'{val}'";
                    }
                    break;
                case "Single[]":
                case "Int16[]":
                case "Int32[]":
                case "Int64[]":
                case "Double[]":
                    {
                        var values = SysExp.Expression.Lambda<Func<IEnumerable>>(memberExp).Compile()();
                        var param = (from object val in values select val).ToArray();
                        value = string.Join(",", param);
                    }
                    break;
                case "String[]":
                case "Array":
                case "DateTime[]":
                    {
                        var values = SysExp.Expression.Lambda<Func<IEnumerable>>(memberExp).Compile()();
                        var param = (from object val in values select val).ToArray();
                        value = $"'{string.Join("','", param)}'";
                    }
                    break;
                default:
                    {
                        var val = SysExp.Expression.Lambda<Func<object>>(memberExp).Compile()();
                        if (val == null) val = "''";
                        else value = $"'{val}'";
                    }
                    break;
            }
            return value;
        }

        internal string VisitMethodCallExp(SysExp.MethodCallExpression methodCallExp, ExpDirection direct)
        {
            string field = string.Empty, values = string.Empty;

            if (methodCallExp.Method.Name == "Contains")
            {
                if (methodCallExp.Arguments.Count == 1)
                {
                    var left = methodCallExp.Object as SysExp.MemberExpression;
                    var right = methodCallExp.Arguments[0] as SysExp.ConstantExpression;

                    field = left.Member.Name;
                    values = right.Value.ToString();

                    return field + " Like '%" + values + "%'";
                }

                if (methodCallExp.Arguments[1] is SysExp.MemberExpression caller)
                    field = VisitMemberExp(caller, direct);
                if (methodCallExp.Arguments[0] is SysExp.MemberExpression param)
                    values = VisitMemberExp(param, direct);

                return field + " In(" + values + ")";
            }
            else if (methodCallExp.Method.Name == "StartsWith")
            {
                var left = methodCallExp.Object as SysExp.MemberExpression;
                var right = methodCallExp.Arguments[0] as SysExp.ConstantExpression;

                field = left.Member.Name;
                values = right.Value.ToString();
                return field + " Like '%" + values + "'";
            }
            else if (methodCallExp.Method.Name == "EndsWith")
            {
                var left = methodCallExp.Object as SysExp.MemberExpression;
                var right = methodCallExp.Arguments[0] as SysExp.ConstantExpression;

                field = left.Member.Name;
                values = right.Value.ToString();

                return field + " Like '" + values + "%'";
            }
            else if (methodCallExp.Method.Name == "Equals")
            {
                if (methodCallExp.Object is SysExp.MemberExpression caller)
                    field = VisitMemberExp(caller, direct);
                if (methodCallExp.Arguments[0] is SysExp.MemberExpression param)
                    values = VisitMemberExp(param, direct);

                return field + "=" + values;
            }
            else
            {
                List<object> array = new List<object>(methodCallExp.Arguments.Count);

                foreach (var exp in methodCallExp.Arguments)
                {
                    if (exp is SysExp.MemberExpression mem)
                        array.Add(VisitMemberExp(mem, direct));
                    else if (exp is SysExp.ConstantExpression constant)
                        array.Add(VisitConstantExp(constant));
                }

                return methodCallExp.Method.Name + "(" + string.Join(",", array) + ")";
            }
        }

        protected string VisitOperatorExp(SysExp.BinaryExpression exp)
        {
            string op = string.Empty;
            switch (exp.NodeType)
            {
                case SysExp.ExpressionType.Equal:
                    op = "=";
                    break;
                case SysExp.ExpressionType.NotEqual:
                    op = "<>";
                    break;
                case SysExp.ExpressionType.LessThan:
                    op = "<";
                    break;
                case SysExp.ExpressionType.LessThanOrEqual:
                    op = "<=";
                    break;
                case SysExp.ExpressionType.GreaterThan:
                    op = ">";
                    break;
                case SysExp.ExpressionType.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case SysExp.ExpressionType.And:
                    op = "&";
                    break;
                case SysExp.ExpressionType.AndAlso:
                    op = " And ";
                    break;
                case SysExp.ExpressionType.Or:
                    op = "|";
                    break;
                case SysExp.ExpressionType.OrElse:
                    op = " Or ";
                    break;
                case SysExp.ExpressionType.Add:
                    op = "+";
                    break;
                case SysExp.ExpressionType.Subtract:
                    op = "-";
                    break;
                case SysExp.ExpressionType.Multiply:
                    op = "*";
                    break;
                case SysExp.ExpressionType.Divide:
                    op = "/";
                    break;
                case SysExp.ExpressionType.Modulo:
                    op = "%";
                    break;
            }
            return op;
        }

        protected int FindChar(StringBuilder builder, char c)
        {
            int cnt = 0;
            for (int i = 0; i < builder.Length; ++i)
            {
                if (builder[i] == c) ++cnt;
            }
            return cnt;
        }
    }

    internal enum ExpDirection
    {
        Left,
        Operate,
        Right
    }
}
