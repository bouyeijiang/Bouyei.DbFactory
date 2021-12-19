using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SysExp = System.Linq.Expressions;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class On<L, R>:WhereBase
    {
        private SysExp.Expression<Func<L, R, bool>> fun;

        public On()
            :base(typeof(L))
        {

        }

        public On(SysExp.Expression<Func<L, R, bool>> onWhere)
        {
            this.fun = onWhere;
        }

        public override string ToString()
        {
            return string.Format("On {0} ", ExpressionParse(fun));
        }

        protected string ExpressionParse(SysExp.Expression<Func<L,R, bool>> exp)
        {
            StringBuilder builder = new StringBuilder();
            //WhereBase l = new WhereBase(typeof(L));
            WhereBase r = new WhereBase(typeof(R));

            //l.PredicateParser(exp.Body, builder, ExpDirection.Left);
            r.PredicateParser(exp.Body, builder, ExpDirection.Right);

            int left = FindChar(builder, '(');
            int right = FindChar(builder, ')');
            if (left > right) builder.Append(')');
            else if (left < right) builder.Insert(0, '(');

            return builder.ToString();
        }
    }
}
