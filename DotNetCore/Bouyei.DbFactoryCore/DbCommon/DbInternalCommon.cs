/*-------------------------------------------------------------
 *project:Bouyei.DbFactoryCore
 *   auth: bouyei
 *   date: 2017/9/28 22:20:30
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Threading;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Bouyei.DbFactoryCore
{
    internal enum ProType : short
    {
        None = 0,
        Bool = 1,
        Byte = 2,
        Char = 3,
        Float = 5,
        Short = 6,
        Int = 7,
        Long = 8,
        Double = 9,
        Decimal = 10,
        DateTime = 11,
        Guid = 12,
        String = 13
    }

    internal class PropertyInfoEx
    {
        public string Name { get; set; }

        public ProType ProType { get; set; }

        public int DbIndex { get; set; }
    }

    internal interface IExpProperty<T>
    {
        Type classType { get; set; }
        object GetValue(T value, string proName);
        V GetValue<V>(T value, string proName);
        void SetValue<V>(T value, string proName, V proValue);
        void SetValue(T value, string proName, object proValue);
    }

    internal interface IDbReaderToGeneric
    {
        bool IsPrimitType<T>();
        T FromDbDataReader<T>(DbDataReader reader);
        List<T> FromDbDataReaderToList<T>(DbDataReader reader);
        T FromDbReaderToPrimit<T>(IDataReader reader, int index = 0);
        List<T> FromDbReaderToPrimitList<T>(IDataReader reader, int index = 0);
    }

    internal class DbParseBase
    {
        public bool IsPrimitType<T>()
        {
            var type = typeof(T);

            return (type.IsValueType
                || type.IsClass == false
                || type.Name == "String"
                || type.Name == "Object");
        }

        internal bool NameEquals(string srcName, string dstName)
        {
            return srcName == dstName;
        }

        public T FromDbReaderToPrimit<T>(IDataReader reader, int index = 0)
        {
            object value = reader.GetValue(index);
            var tType = typeof(T);

            if (value != null && value.GetType().Name != tType.Name)
            {
                throw new Exception("type mismatch:" + value.GetType().Name + "," + tType.Name);
            }

            return (T)Convert.ChangeType(value, tType);
        }

        public List<T> FromDbReaderToPrimitList<T>(IDataReader reader, int index = 0)
        {
            var vtype = typeof(T);
            List<T> values = new List<T>(32);

            while (reader.Read())
            {
                object value = reader.GetValue(index);
                if (vtype.IsEnum)
                {
                    values.Add((T)Enum.ToObject(vtype, value));
                }
                else
                {
                    if (value != null && value.GetType().Name != vtype.Name)
                    {
                        throw new Exception("type mismatch:" + value.GetType().Name + "," + vtype.Name);
                    }

                    values.Add((T)Convert.ChangeType(value, vtype));
                }
            }

            return values;
        }

        /// <summary>
        /// 预处理类型
        /// </summary>
        /// <param name="infos"></param>
        ///  <param name="schema"></param>
        internal List<PropertyInfoEx> PropertyInfoToEx(IEnumerable<PropertyInfo> infos,string[] schema)
        {
            List<PropertyInfoEx> items = new List<PropertyInfoEx>(infos.Count());
            foreach (var item in infos)
            {
                var _it = new PropertyInfoEx()
                {
                    Name = item.Name,
                    DbIndex = FindSchemaIndex(item.Name, schema)
                };

                if (item.PropertyType == typeof(int))
                {
                    _it.ProType = ProType.Int;
                }
                else if (item.PropertyType == typeof(short))
                {
                    _it.ProType = ProType.Short;
                }
                else if (item.PropertyType == typeof(long))
                {
                    _it.ProType = ProType.Long;
                }
                else if (item.PropertyType == typeof(string))
                {
                    _it.ProType = ProType.String;
                }
                else if (item.PropertyType == typeof(double))
                {
                    _it.ProType = ProType.Double;
                }
                else if (item.PropertyType == typeof(decimal))
                {
                    _it.ProType = ProType.Decimal;
                }
                else if (item.PropertyType == typeof(float))
                {
                    _it.ProType = ProType.Float;
                }
                else if (item.PropertyType == typeof(byte))
                {
                    _it.ProType = ProType.Byte;
                }
                else if (item.PropertyType == typeof(char))
                {
                    _it.ProType = ProType.Char;
                }
                else if (item.PropertyType == typeof(DateTime))
                {
                    _it.ProType = ProType.DateTime;
                }
                else if (item.PropertyType == typeof(Guid))
                {
                    _it.ProType = ProType.Guid;
                }
                else if (item.PropertyType == typeof(bool))
                {
                    _it.ProType = ProType.Bool;
                }
                items.Add(_it);
            }

            return items;
        }

        internal List<PropertyInfoEx> ToMappingPropertyEx(IEnumerable<PropertyInfo> infos, 
            string[] schema)
        {
            List<PropertyInfoEx> items = new List<PropertyInfoEx>(infos.Count());
            foreach (var item in infos)
            {
                var _it = new PropertyInfoEx()
                {
                    Name = item.Name,
                    DbIndex = FindSchemaIndex(item.Name, schema)
                };
                items.Add(_it);
            }
            return items;
        }

        private int FindSchemaIndex(string proName, string[] schema)
        {
            for (int i = 0; i < schema.Length; ++i)
            {
                if (NameEquals(proName, schema[i]))
                    return i;
            }
            return -1;
        }

        protected string[] GetSchemasFromDbReader(DbDataReader reader)
        {
            string[] schemas = null;

            if (reader.CanGetColumnSchema())
            {
                var rows = reader.GetSchemaTable().Rows;
                if (rows.Count > 0)
                {
                    schemas = rows.Cast<DataRow>().Select(x => x[0].ToString()).ToArray();
                    return schemas;
                }
            }

            schemas = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                schemas[i] = reader.GetName(i);
            }

            return schemas;
        }
    }
}
