using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bouyei.DbFactoryCore.DbSqlProvider.SqlKeywords
{
    public class WordsBase
    {
        private const string numericReg = @"^[+-]?\d*[.]?\d*$";

        public string SqlString { get; set; }

        protected Type type = null;

        public WordsBase()
        { }

        public WordsBase(Type type)
        {
            this.type = type;
        }

        public virtual string ToString(string[] columnNames)
        {
            return string.Join(",", columnNames);
        }

        public virtual string ToString(IEnumerable<string> columnNames)
        {
            return string.Join(",", columnNames);
        }

        public virtual string GetTableName()
        {
            string tabName = GetMappedAttributeName();
            if (tabName == string.Empty) tabName = type.Name;

            return tabName;
        }

        protected virtual IEnumerable<string> GetColumns()
        {
            return GetProperties().Select(x => x.Name);
        }

        protected virtual IEnumerable<PropertyInfo> GetProperties()
        {
            var items = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            return items.Where(x => ExistIgnoreAttribute(x) == false);
        }

        protected string ParameterFormat(object[] param)
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

        protected string ParameterFormat(PropertyInfo[] pInfo,object value)
        {
            var pInfos = pInfo;// typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<string> values = new List<string>(pInfos.Length);

            foreach (var pi in pInfos)
            {
                if (ExistIgnoreAttribute(pi))
                {
                    continue;
                }

                object val = pi.GetValue(value, null);
                if (val == null || val == DBNull.Value)
                {
                    values.Add("''");
                }
                else
                {
                    string _val = val.ToString();
                    bool isTrue = Regex.IsMatch(_val, numericReg);

                    if (isTrue == false) values.Add("'" + _val + "'");
                    else values.Add(_val);
                }
            }
            return string.Join(",", values);
        }

        protected bool IsDigital(object value)
        {
            return Regex.IsMatch(value.ToString(), numericReg);
        }
        protected string GetMappedAttributeName()
        {
            var attr = type.GetCustomAttributes<MappedNameAttribute>(false).FirstOrDefault();

            if (attr == null) return string.Empty;

            return attr.Name;
        }

        protected string GetColumnAttributeName(PropertyInfo pInfo)
        {
            var bAttr = pInfo.GetCustomAttribute<BaseAttribute>(false);
            if (bAttr == null) return string.Empty;
            return bAttr.Name;
        }

        protected AttributeType GetColumnAttributeType(PropertyInfo pInfo)
        {
            var bAttr = pInfo.GetCustomAttribute<BaseAttribute>(false);
            if (bAttr == null) return AttributeType.None;
            return bAttr.AttrType;
        }

        protected bool ExistIgnoreAttribute(PropertyInfo pInfo)
        {
            var attrs = pInfo.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                if (attr is IgnoreAttribute
                    || attr is IgnoreWriteAttribute
                    || attr is IgnoreReadAttribute) return true;
            }
            return false;
        }

        protected bool IsDefaultAttribute(PropertyInfo pInfo)
        {
            var bAttr = pInfo.GetCustomAttribute<BaseAttribute>(false);
            if (bAttr == null) return false;
            return bAttr.IsDefaultAttribute();
        }
    }
}
