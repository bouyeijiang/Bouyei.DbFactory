using System;
using System.Collections;
using System.Text;
using System.Linq;
using SysExp= System.Linq.Expressions;
using System.Collections.Generic;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Where:WhereBase
    {
        public Where()
        {

        }

        public string ToString<T>(SysExp.Expression<Func<T, bool>> fun)
        {
            type = typeof(T);

            return string.Format("Where {0} ", ExpressionParse<T>(fun));
        }

        protected string ExpressionParse<T>(SysExp.Expression<Func<T, bool>> exp)
        {
            StringBuilder builder = new StringBuilder();
            PredicateParser(exp.Body, builder,ExpDirection.Left);

            int left = FindChar(builder, '(');
            int right = FindChar(builder, ')');
            if (left > right) builder.Append(')');
            else if (left < right) builder.Insert(0, '(');
            return builder.ToString();
        }
    }

    public class Where<T>:WhereBase
    {
        public Where() :base(typeof(T))
        {

        }

        public string ToString(SysExp.Expression<Func<T, bool>> fun)
        {
            return string.Format("Where {0} ", ExpressionParse(fun));
        }

        private string ExpressionParse(SysExp.Expression<Func<T, bool>> exp)
        {
            StringBuilder builder = new StringBuilder();
            PredicateParser(exp.Body, builder, ExpDirection.Left);

            int left = FindChar(builder, '(');
            int right = FindChar(builder, ')');
            if (left > right) builder.Append(')');
            else if (left < right) builder.Insert(0,'(');

            return builder.ToString();
        }
    }
}
