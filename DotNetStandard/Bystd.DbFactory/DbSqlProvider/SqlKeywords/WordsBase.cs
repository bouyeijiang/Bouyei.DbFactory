using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bystd.DbFactory.DbSqlProvider.SqlKeywords
{
    public class WordsBase
    {
        private const string numericReg = @"^[+-]?\d*[.]?\d*$";

        public string SqlString { get; set; }

        protected Type type = null;

        protected AttributeType attrType = AttributeType.None;

        public WordsBase()
        { }

        public WordsBase(AttributeType attrType)
        {
            this.attrType = attrType;
        }

        public WordsBase(Type type)
        {
            this.type = type;
        }
        public WordsBase(Type type, AttributeType attrType)
            : this(attrType)
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

        protected string ParameterFormat(PropertyInfo[] pInfo, object value)
        {
            var pInfos = pInfo;// typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<string> values = new List<string>(pInfos.Length);

            foreach (var pi in pInfos)
            {
                object val = pi.GetValue(value, null);

                if (val == null || val == DBNull.Value)
                {
                    values.Add("NULL");
                    continue;
                }
                else if (pi.PropertyType.IsEnum)
                {
                    values.Add(Convert.ToInt32(val).ToString());
                    continue;
                }
                else
                {
                    string _val = val.ToString();
                    if (_val == string.Empty) values.Add("''");
                    else
                    {
                        bool isTrue = Regex.IsMatch(_val, numericReg);
                        if (isTrue == false) values.Add("'" + _val + "'");
                        else values.Add(_val);
                    }
                }
            }
            return string.Join(",", values);
        }

        protected bool IsDigital(object value)
        {
            if (value is int
                || value is long
                || value is float
                || value is byte
                || value is double) return true;

            string val = value.ToString();
            if (val == string.Empty) return false;

            return Regex.IsMatch(val, numericReg);
        }

        protected string GetMappedAttributeName()
        {
            var attr = type.GetCustomAttributes<MappedNameAttribute>(false).FirstOrDefault();

            if (attr == null) return string.Empty;

            return attr.Name;
        }

        protected bool ExistIgnoreAttribute(PropertyInfo pInfo)
        {
            if (attrType == AttributeType.None)
                return false;

            var attrs = pInfo.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                if (attr is IgnoreAttribute ignore)
                {
                    if (ignore.AttrType == AttributeType.Ignore)
                    {
                        if (ignore.AttrType == AttributeType.IgnoreRead
                       || ignore.AttrType == AttributeType.IgnoreWrite)
                            return true;
                    }

                    if (attrType == ignore.AttrType &&
                        (ignore.AttrType == AttributeType.IgnoreRead
                        || ignore.AttrType == AttributeType.IgnoreWrite))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
