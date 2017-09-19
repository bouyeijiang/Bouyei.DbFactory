using System;
using System.Text;

namespace Bouyei.DbFactory.DbSqlProvider.Expression
{
    public class In : ExpTree
    {
        string[] ins = null;
        public In(params string[] ins)
        { this.ins = ins; }

        public override string ToString()
        {
            return "In (" + string.Join(",", ins) + ") ";
        }
    }
}
