using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbSqlProvider.SqlKeywords
{
    public class Join<L,R>:WordsBase
    {
        private Where<L> leftSelect = null;
        private Where<R> rightSelect = null;
        private Dictionary<string, string> onWhere = null;

        private JoinType joinType = JoinType.Left;

        public Join(Where<L> leftSelect, Where<R> rightSelect,
            Dictionary<string,string> onWhere,JoinType joinType=JoinType.Left)
        {
            this.joinType = joinType;
            this.leftSelect = leftSelect;
            this.rightSelect = rightSelect;
            this.onWhere = onWhere;

            //leftTable = new WordsBase(typeof(L), AttributeType.IgnoreRead | AttributeType.Ignore);
            //rightTable = new WordsBase(typeof(R), AttributeType.IgnoreRead | AttributeType.Ignore);
        }

        public override string ToString()
        {
            string sql = string.Empty;
            List<string> onBuilder = new List<string>(onWhere.Count);

            foreach(KeyValuePair<string,string> kv in onWhere)
            {
                onBuilder.Add($"a1.{kv.Key}=a2.{kv.Value}");
            }

            if (joinType == JoinType.Left)
            {
                sql = string.Format("({0}) a1 Left Join ({1}) a2 On {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onBuilder));
            }
            else if (joinType == JoinType.Right)
            {
                sql = string.Format("({0}) a1 Right Join ({1}) a2 On {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onBuilder));
            }
            else if (joinType == JoinType.Inner)
            {
                sql = string.Format("({0}) a1 Inner Join ({1}) a2 On {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onBuilder));
            }
            else
            {
                sql = string.Format("({0}) a1 Join ({1}) a2 On {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onBuilder));
            }

            return sql;
        }
    }

    public class JoinOn<L, R> : WordsBase
    {
        private Where<L> leftSelect = null;
        private Where<R> rightSelect = null;
        private On<L, R> onWhere = null;

        private JoinType joinType = JoinType.Left;

        public JoinOn(Where<L> leftSelect, Where<R> rightSelect,
         On<L, R> onWhere, JoinType joinType = JoinType.Left)
        {
            this.joinType = joinType;
            this.leftSelect = leftSelect;
            this.rightSelect = rightSelect;
            this.onWhere = onWhere;
        }

        public override string ToString()
        {
            string sql = string.Empty;
            this.onWhere.SqlString = onWhere.ToString();
        
            if (joinType == JoinType.Left)
            {
                sql = string.Format("({0}) Left Join ({1}) {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onWhere.SqlString));
            }
            else if (joinType == JoinType.Right)
            {
                sql = string.Format("({0}) Right Join ({1}) {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onWhere.SqlString));
            }
            else if (joinType == JoinType.Inner)
            {
                sql = string.Format("({0}) Inner Join ({1}) {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onWhere.SqlString));
            }
            else
            {
                sql = string.Format("({0}) Join ({1}) {2}",
                    leftSelect.SqlString, rightSelect.SqlString, string.Join(" and ", onWhere.SqlString));
            }

            return sql;
        }
    }
}

