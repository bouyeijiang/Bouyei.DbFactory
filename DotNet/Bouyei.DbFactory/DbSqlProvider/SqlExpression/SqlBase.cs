using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bouyei.DbFactory.DbSqlProvider.SqlExpression
{
    public class SqlBase
    {
        private const string numericReg = @"^[+-]?\d*[.]?\d*$";

        public SqlBase()
        {

        }

        protected virtual string ToString(string[] columnNames)
        {
            return string.Join(",", columnNames);
        }

        protected virtual string[] ToColumns<T>()
        {
            var columnType = typeof(T);

            var items = columnType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return items.Select(x => x.Name).ToArray();
        }

        protected string ParameterFormat(params object[] param)
        {
            if (param == null || param.Length == 0) return string.Empty;

            string[] values = new string[param.Length];
            for (int i = 0; i < param.Length; ++i)
            {
                string val = param[i] == null ? string.Empty : param[i].ToString();
                bool isTrue = Regex.IsMatch(val, numericReg);

                if (isTrue == false || val == string.Empty) values[i] = "'" + val + "'";
                else values[i] = val;
            }
            return string.Join(",", values);
        }

        protected string ParameterFormat<T>(T value)
        {
            var pInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<string> values = new List<string>(pInfos.Length);

            foreach (var pi in pInfos)
            {
                object val = pi.GetValue(value, null);
                if (val == null || val == DBNull.Value)
                    continue;

                string _val = val.ToString();
                bool isTrue = Regex.IsMatch(_val, numericReg);

                if (isTrue == false) values.Add("'" + _val + "'");
                else values.Add(_val);
            }
            return string.Join(",", values);
        }

        protected bool IsDigital(object value)
        {
            return Regex.IsMatch(value.ToString(), numericReg);
        }
    }
}
