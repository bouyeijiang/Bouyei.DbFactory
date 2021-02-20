using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
{
    using DbSqlProvider.SqlKeywords;
    using DbSqlProvider.SqlFunctions;

    public static class FunctionsExtensions
    {
        public static Count Count<T>(this Select<T> select, string columnName = "*")
        {
            Count count = new Count(columnName);
            count.SqlString = select.SqlString + "," + count.ToString();
            return count;
        }

        public static Count Count(this Select select, string columnName = "*")
        {
            Count count = new Count(columnName);
            count.SqlString = select.SqlString + "," + count.ToString();
            return count;
        }

        public static Max Max<T>(this Select<T> select,string columnName)
        {
            Max max = new Max(columnName);
            max.SqlString = select.SqlString + "," + max.ToString();
            return max;
        }

        public static Max Max(this Select select, string columnName)
        {
            Max max = new Max(columnName);
            max.SqlString = select.SqlString + "," + max.ToString();
            return max;
        }

        public static Min Min(this Select select, string columnName)
        {
            Min min = new Min(columnName);
            min.SqlString = select.SqlString + "," + min.ToString();
            return min;
        }

        public static Min Min<T>(this Select<T> select, string columnName)
        {
            Min min = new Min(columnName);
            min.SqlString = select.SqlString + "," + min.ToString();
            return min;
        }

        public static Sum Sum<T>(this Select<T> select, string columnName)
        {
            Sum sum = new Sum(columnName);
            sum.SqlString = select.SqlString + "," + sum.ToString();
            return sum;
        }

        public static Sum Sum(this Select select, string columnName)
        {
            Sum sum = new Sum(columnName);
            sum.SqlString = select.SqlString + "," + sum.ToString();
            return sum;
        }


        public static Avg Avg<T>(this Select<T> select, string columnName)
        {
            Avg avg = new Avg(columnName);
            avg.SqlString = select.SqlString +","+ avg.ToString();
            return avg;
        }

        public static Avg Avg(this Select select, string columnName)
        {
            Avg avg = new Avg(columnName);
            avg.SqlString = select.SqlString + "," + avg.ToString();
            return avg;
        }
    }
}
