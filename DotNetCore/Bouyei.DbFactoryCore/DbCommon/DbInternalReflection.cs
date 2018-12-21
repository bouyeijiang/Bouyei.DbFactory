/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/7/11 9:17:06
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 6ceef553-44aa-427b-8bbb-b592657843da
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace Bouyei.DbFactoryCore.DbUtils
{
    internal class DbReflection:DbParseBase
    {
        public T FromDbDataReader<T>(DbDataReader reader) 
        {
            T value = Activator.CreateInstance<T>();
            Type toType = typeof(T);
            PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        
            foreach (var pi in pinfos)
            {
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (NameEquals(pi.Name, reader.GetName(i)))
                    {
                        object dbValue = reader.GetValue(i);

                        if (dbValue == null || dbValue == DBNull.Value)
                            continue;

                        //转换类型
                        var dstPro = toType.GetProperty(pi.Name);
                        var dstType = dstPro.PropertyType;

                        object dstValue = null;

                        if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            dstValue = Convert.ChangeType(dbValue, dstType.GetGenericArguments()[0]);
                        }
                        else if (dstType.IsEnum)
                        {
                            dstValue = Enum.ToObject(dstType, dbValue);
                        }
                        else
                        {
                            dstValue = Convert.ChangeType(dbValue, dstType);
                        }
                        pi.SetValue(value, dstValue);
                        break;
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 根据DbDataReader映射到结构体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public List<T> FromDbDataReaderToList<T>(DbDataReader reader) 
        {
            List<T> items = new List<T>(64);
            Type toType = typeof(T);

            PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            ExpressProperty<T> expPros = new ExpressProperty<T>();

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();
                
                foreach (var pi in pinfos)
                {
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (NameEquals(pi.Name, reader.GetName(i)))
                        {
                            object dbValue = reader.GetValue(i);

                            if (dbValue == null || dbValue == DBNull.Value)
                                continue;

                            //转换为指定类型
                            var dstPro = toType.GetProperty(pi.Name);
                            var dstType = dstPro.PropertyType;

                            object dstValue = null;

                            if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                dstValue = Convert.ChangeType(dbValue, dstType.GetGenericArguments()[0]);
                            }
                            else if (dstType.IsEnum)
                            {
                                dstValue = Enum.ToObject(dstType, dbValue);
                            }
                            else
                            {
                                dstValue = Convert.ChangeType(dbValue, dstType);
                            }
                            pi.SetValue(value, dbValue);

                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class DbExpression:DbParseBase
    {
        public T FromDbDataReader<T>(DbDataReader reader)
        {
            T value = Activator.CreateInstance<T>();
            ExpressProperty<T> expressPro = new ExpressProperty<T>();
            Type toType = expressPro.classType;

            PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var pi in pinfos)
            {
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (NameEquals(pi.Name, reader.GetName(i)))
                    {
                        object dbValue = reader.GetValue(i);

                        if (dbValue == null || dbValue == DBNull.Value)
                            continue;

                        expressPro.SetValue(value, pi.Name, dbValue);
                        break;
                    }
                }
            }
            return value;
        }

        public List<T> FromDbDataReaderToList<T>(DbDataReader reader)
        {
            List<T> items = new List<T>(64);
            ExpressProperty<T> expressPro = new ExpressProperty<T>();
            Type toType = expressPro.classType;

            PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            ExpressProperty<T> expPros = new ExpressProperty<T>();

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();

                foreach (var pi in pinfos)
                {
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (NameEquals(pi.Name, reader.GetName(i)))
                        {
                            object dbValue = reader.GetValue(i);

                            if (dbValue == null || dbValue == DBNull.Value)
                                continue;

                            expressPro.SetValue(value, pi.Name, dbValue);
                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class ExpressProperty<T>
    {
        private Func<T, string, object> getValue = null;
        private Dictionary<string, Action<T, object>> setterExpressionCaching = null;

        internal Type classType = null;

        public ExpressProperty()
        {
            classType = typeof(T);
            setterExpressionCaching = new Dictionary<string, Action<T, object>>();
        }

        public V GetValue<V>(T value, string proName)
        {
            if (getValue == null) getValue = GenerateGetExpress();

            return (V)getValue(value, proName);
        }
        public object GetValue(T value, string proName)
        {
            if (getValue == null) getValue = GenerateGetExpress();

            return getValue(value, proName);
        }

        public T SetValue<V>(string proName, V proValue)
        {
            T obj = Activator.CreateInstance<T>();
            GenerateSetExpress(proName)(obj, proValue);

            return obj;
        }
        public T SetValue(string proName, object proValue)
        {
            string key = classType.FullName + proName;
            Action<T, object> act = null;

            if (setterExpressionCaching.TryGetValue(key, out act) == false)
            {
                act = GenerateSetExpress(proName);
                setterExpressionCaching.Add(key, act);
            }

            T val = Activator.CreateInstance<T>();
            act(val, proValue);

            return val;
        }

        public void SetValue<V>(T value, string proName, V proValue)
        {
            string key = classType.FullName + proName;
            Action<T, object> act = null;
            if (setterExpressionCaching.TryGetValue(key, out act) == false)
            {
                act = GenerateSetExpress(proName);
                setterExpressionCaching.Add(key, act);
            }
            act(value, proValue);
        }

        public void SetValue(T value, string proName, object proValue)
        {
            string key = classType.FullName + proName;
            Action<T, object> act = null;

            if (setterExpressionCaching.TryGetValue(key, out act) == false)
            {
                act = GenerateSetExpress(proName);
                setterExpressionCaching.Add(key, act);
            }
            act(value, proValue);
        }

        private Func<T, string, object> GenerateGetExpress()
        {
            var objType = typeof(object);
            var intType = typeof(int);
            var stringType = typeof(string);

            var instance = Expression.Parameter(objType, "instance");
            var memberName = Expression.Parameter(stringType, "memberName");
            var nameHash = Expression.Variable(intType, "nameHash");
            var getHashCode = Expression.Assign(nameHash, Expression.Call(memberName, objType.GetMethod("GetHashCode")));
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null),
            (from propertyInfo in classType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
             let property = Expression.Property(Expression.Convert(instance, classType), propertyInfo.Name)
             let propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), intType)
             select Expression.SwitchCase(Expression.Convert(property, objType), propertyHash)).ToArray());

            var methodBody = Expression.Block(objType, new[] { nameHash }, getHashCode, switchEx);
            return Expression.Lambda<Func<T, string, object>>(methodBody, instance, memberName).Compile();
        }

        private Action<T, object> GenerateSetExpress(string proName)
        {
            var pro = classType.GetProperty(proName);

            var instance = Expression.Parameter(classType, "instance");

            var value = Expression.Parameter(typeof(object), "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that

            UnaryExpression instanceCast = (!pro.DeclaringType.IsValueType)
                ? Expression.TypeAs(instance, pro.DeclaringType)
                : Expression.Convert(instance, pro.DeclaringType);

            UnaryExpression valueCast = (!pro.PropertyType.IsValueType)
                ? Expression.TypeAs(value, pro.PropertyType)
                : Expression.Convert(value, pro.PropertyType);

            var exp = Expression.Lambda<Action<T, object>>(Expression.Call(instanceCast, pro.GetSetMethod(), valueCast),
                  new ParameterExpression[] { instance, value }).Compile();

            return exp;
        }
    }

    internal class DbParseBase
    {
        internal bool IsPrimitType<T>()
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

        internal T FromDataReader<T>(IDataReader reader, int index = 0)
        {
            object value = reader.GetValue(index);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal List<T> FromDataReaderToList<T>(IDataReader reader, int index = 0)
        {
            var vtype = typeof(T);
            List<T> values = new List<T>(64);

            while (reader.Read())
            {
                object value = reader.GetValue(index);
                if (vtype.IsEnum)
                {
                    values.Add((T)Enum.ToObject(vtype, value));
                }
                else
                {
                    values.Add((T)Convert.ChangeType(value, vtype));
                }
            }

            return values;
        }
    }
}
