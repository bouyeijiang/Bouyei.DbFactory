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
    //internal class DbReflection:DbParseBase
    //{
    //    public T FromDbDataReader<T>(DbDataReader reader) 
    //    {
    //        T value = Activator.CreateInstance<T>();
    //        Type toType = typeof(T);
    //        PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        
    //        foreach (var pi in pinfos)
    //        {
    //            for (int i = 0; i < reader.FieldCount; ++i)
    //            {
    //                if (NameEquals(pi.Name, reader.GetName(i)))
    //                {
    //                    object dbValue = reader.GetValue(i);

    //                    if (dbValue == null || dbValue == DBNull.Value)
    //                        continue;

    //                    //转换类型
    //                    var dstPro = toType.GetProperty(pi.Name);
    //                    var dstType = dstPro.PropertyType;

    //                    object dstValue = null;

    //                    if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //                    {
    //                        dstValue = Convert.ChangeType(dbValue, dstType.GetGenericArguments()[0]);
    //                    }
    //                    else if (dstType.IsEnum)
    //                    {
    //                        dstValue = Enum.ToObject(dstType, dbValue);
    //                    }
    //                    else
    //                    {
    //                        dstValue = Convert.ChangeType(dbValue, dstType);
    //                    }
    //                    pi.SetValue(value, dstValue);
    //                    break;
    //                }
    //            }
    //        }
    //        return value;
    //    }

    //    /// <summary>
    //    /// 根据DbDataReader映射到结构体集合
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="reader"></param>
    //    /// <returns></returns>
    //    public List<T> FromDbDataReaderToList<T>(DbDataReader reader) 
    //    {
    //        List<T> items = new List<T>(64);
    //        Type toType = typeof(T);

    //        PropertyInfo[] pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    //        ExpressProperty<T> expPros = new ExpressProperty<T>();

    //        while (reader.Read())
    //        {
    //            T value = Activator.CreateInstance<T>();
                
    //            foreach (var pi in pinfos)
    //            {
    //                for (int i = 0; i < reader.FieldCount; ++i)
    //                {
    //                    if (NameEquals(pi.Name, reader.GetName(i)))
    //                    {
    //                        object dbValue = reader.GetValue(i);

    //                        if (dbValue == null || dbValue == DBNull.Value)
    //                            continue;

    //                        //转换为指定类型
    //                        var dstPro = toType.GetProperty(pi.Name);
    //                        var dstType = dstPro.PropertyType;

    //                        object dstValue = null;

    //                        if (dstType.IsGenericType && dstType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //                        {
    //                            dstValue = Convert.ChangeType(dbValue, dstType.GetGenericArguments()[0]);
    //                        }
    //                        else if (dstType.IsEnum)
    //                        {
    //                            dstValue = Enum.ToObject(dstType, dbValue);
    //                        }
    //                        else
    //                        {
    //                            dstValue = Convert.ChangeType(dbValue, dstType);
    //                        }
    //                        pi.SetValue(value, dbValue);

    //                        break;
    //                    }
    //                }
    //            }
    //            items.Add(value);
    //        }
    //        return items;
    //    }
    //}

    internal class DbExpression:DbParseBase
    {
        public T FromDbDataReader<T>(DbDataReader reader)
        {
            ExpressProperty<T> expressPro = new ExpressProperty<T>();
            Type toType = expressPro.classType;
            T value = Activator.CreateInstance<T>();

            var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

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
            ExpressProperty<T> expPro = new ExpressProperty<T>();
            Type toType = expPro.classType;

            var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

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

                            expPro.SetValue(value, pi.Name, dbValue);
                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class DbReaderDelegateToGeneric:DbParseBase
    {
        public DbReaderDelegateToGeneric()
        {

        }

        public T FromDbDataReader<T>(DbDataReader reader)
        {
            ExpressProperty<T> expressPro = new ExpressProperty<T>();

            Type toType = expressPro.classType;
            var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var pros = PropertyInfoToEx(pinfos);

            T value = Activator.CreateInstance<T>();

            foreach (var pi in pros)
            {
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (NameEquals(pi.Name, reader.GetName(i)))
                    {
                        DataReaderDelegateToGeneric<T>(reader, i, value, pi, expressPro);
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

            var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var pros=PropertyInfoToEx(pinfos);

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();

                foreach (var pi in pros)
                {
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (NameEquals(pi.Name, reader.GetName(i)))
                        {
                            DataReaderDelegateToGeneric<T>(reader, i, value, pi, expressPro);
                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class DbReaderExpressionToGeneric : DbParseBase
    {
        public T FromDbDataReader<T>(DbDataReader reader)
        {
            ExpressProperty<T> expressPro = new ExpressProperty<T>();

            Type toType = expressPro.classType;
            var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            T value = Activator.CreateInstance<T>();

            var pros = PropertyInfoToEx(pinfos);

            foreach (var pi in pros)
            {
                for (int i = 0; i < reader.FieldCount; ++i)
                {
                    if (NameEquals(pi.Name, reader.GetName(i)))
                    {
                        DataReaderExpToGeneric<T>(reader, i, value, pi, expressPro);
                        break;
                    }
                }
            }
            return value;
        }

        public List<T> FromDbDataReaderToList<T>(DbDataReader reader)
        {
            List<T> items = new List<T>(64);
            IExpProperty<T> expressPro = new ExpressProperty<T>();
            Type toType = expressPro.classType;

            var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var pros = PropertyInfoToEx(pinfos);

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();

                foreach (var pi in pros)
                {
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        if (NameEquals(pi.Name, reader.GetName(i)))
                        {
                            DataReaderExpToGeneric<T>(reader, i, value, pi, expressPro);
                            break;
                        }
                    }
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class PropertyInfoEx
    {
        public string Name { get; set; }

        public ProType ProType { get; set; }
    }

    internal enum ProType:short
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
        String=13
    }

    internal class ExpressProperty<T>:IExpProperty<T>
    {
        private Func<T, string, object> getValue = null;
        private Dictionary<string, Action<T, object>> setterExpressionCaching = null;
    
        public Type classType { get; set; }

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

            var method = pro.GetSetMethod(true);

            var instance = Expression.Parameter(classType, "instance");

            var value = Expression.Parameter(typeof(object), "value");

            UnaryExpression instanceCast = (!pro.DeclaringType.IsValueType)
                ? Expression.TypeAs(instance, pro.DeclaringType)
                : Expression.Convert(instance, pro.DeclaringType);

            UnaryExpression valueCast = (!pro.PropertyType.IsValueType)
                ? Expression.TypeAs(value, pro.PropertyType)
                : Expression.Convert(value, pro.PropertyType);

            var exp = Expression.Lambda<Action<T, object>>(Expression.Call(instanceCast,method, valueCast),
                  new ParameterExpression[] { instance, value }).Compile();

            return exp;
        }

    }

    internal interface IExpProperty<T>
    {
        Type classType { get; set; }
        object GetValue(T value, string proName);
        V GetValue<V>(T value, string proName);
        void SetValue<V>(T value, string proName, V proValue);
        void SetValue(T value, string proName, object proValue);
    }

    internal class ExpressionProperty<Entity,Value>
    {
        internal Type tClassType = null;
        internal Type vClassType = null;
        public static ExpressionProperty<Entity, Value> expProperty = new ExpressionProperty<Entity, Value>();

        public ExpressionProperty()
        {
            tClassType = typeof(Entity);
            vClassType = typeof(Value);
        }

        public Value GetValue<V>(Entity value, string proName)
        {
            var act = GenericGetExpression(proName);
            return act(value);
        }

        public Entity SetValue(Entity value, string proName, Value proValue)
        {
            var act = GenericSetExpression(proName);
            act(value, proValue);
            return value;
        }

        public Entity SetValue(string proName, Value proValue)
        {
            Entity obj = Activator.CreateInstance<Entity>();
            var act = GenericSetExpression(proName);
            act(obj, proValue);

            return obj;
        }

        private Func<Entity, Value> GenericGetExpression(string proName)
        {
            var property = tClassType.GetProperty(proName);
            var target = Expression.Parameter(property.DeclaringType);
            var getPropertyValue = Expression.Property(target, property);

            var exp = Expression.Lambda<Func<Entity, Value>>(getPropertyValue, target).Compile();

            return exp;
        }

        private Action<Entity, Value> GenericSetExpression(string proName)
        {
            var property = tClassType.GetProperty(proName);
            var target = Expression.Parameter(property.DeclaringType);
            var propertyValue = Expression.Parameter(vClassType);
            var setPropertyValue = Expression.Call(target, property.GetSetMethod(), propertyValue);
            return Expression.Lambda<Action<Entity, Value>>(setPropertyValue, target, propertyValue).Compile();
        }
    }

    internal class DelegateProperty<Entity,Value>
    {
        public delegate Value DelegateGetValue();
        public delegate void DelegateSetValue(Value v);

        internal Type getClassType = null;
        internal Type setClassType = null;

        internal Type tClassType = null;


        #region pre instance
        public static DelegateProperty<Entity, Value> delegateProperty = new DelegateProperty<Entity, Value>();
        #endregion

        public DelegateProperty()
        {
            getClassType = typeof(DelegateGetValue);
            setClassType = typeof(DelegateSetValue);
            tClassType = typeof(Entity);
        }

        public Value GetValue(Entity value,string proName)
        {
           var exp= (DelegateGetValue)Delegate.CreateDelegate(getClassType, value,
              tClassType.GetProperty(proName).GetGetMethod());

            return exp();
        }

        public void SetValue(Entity value, string proName, Value proValue)
        {
            var exp = (DelegateSetValue)Delegate.CreateDelegate(setClassType,value,
                tClassType.GetProperty(proName).GetSetMethod());

            exp(proValue);
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

        /// <summary>
        /// 预处理类型
        /// </summary>
        /// <param name="infos"></param>
        internal List<PropertyInfoEx> PropertyInfoToEx(IEnumerable<PropertyInfo> infos)
        {
            List<PropertyInfoEx> items = new List<PropertyInfoEx>();
            foreach (var item in infos)
            {
                var _it = new PropertyInfoEx()
                {
                    Name = item.Name
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
                else if(item.PropertyType==typeof(string))
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
                else if(item.PropertyType==typeof(byte))
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
                else if(item.PropertyType==typeof(bool))
                {
                    _it.ProType = ProType.Bool;
                }
                items.Add(_it);
            }

            return items;
        }

        internal void DataReaderExpToGeneric<T>(DbDataReader reader, int i,
         T value, PropertyInfoEx pi, IExpProperty<T> exp)
        {
            switch (pi.ProType)
            {
                case ProType.Int:
                    {
                        var val = reader.GetInt32(i);
                        ExpressionProperty<T, int>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Decimal:
                    {
                        var val = reader.GetDecimal(i);
                        ExpressionProperty<T, decimal>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.String:
                    {
                        var val = reader.GetString(i);
                        ExpressionProperty<T, string>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Bool:
                    {
                        var val = reader.GetBoolean(i);
                        ExpressionProperty<T, bool>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Double:
                    {
                        var val = reader.GetDouble(i);
                        ExpressionProperty<T, double>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Float:
                    {
                        var val = reader.GetFloat(i);
                        ExpressionProperty<T, float>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Byte:
                    {
                        var val = reader.GetByte(i);
                        ExpressionProperty<T, byte>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Char:
                    {
                        var val = reader.GetChar(i);
                        ExpressionProperty<T, char>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.DateTime:
                    {
                        var val = reader.GetDateTime(i);
                        ExpressionProperty<T, DateTime>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Guid:
                    {
                        var val = reader.GetGuid(i);
                        ExpressionProperty<T, Guid>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Long:
                    {
                        var val = reader.GetInt64(i);
                        ExpressionProperty<T, long>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Short:
                    {
                        var val = reader.GetInt16(i);
                        ExpressionProperty<T, short>.expProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.None:
                default:
                    {
                        object dbValue = reader.GetValue(i);

                        if (dbValue == null || dbValue == DBNull.Value)
                            return;

                        exp.SetValue(value, pi.Name, dbValue);
                    }
                    break;
            }
        }

        internal void DataReaderDelegateToGeneric<T>(DbDataReader reader,int i,
            T value,PropertyInfoEx pi,IExpProperty<T> exp)
        {
            switch (pi.ProType)
            {
                case ProType.Int:
                    {
                        var val = reader.GetInt32(i);
                        DelegateProperty<T, int>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Decimal:
                    {
                        var val = reader.GetDecimal(i);
                        DelegateProperty<T, decimal>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.String:
                    {
                        var val = reader.GetString(i);
                        DelegateProperty<T, string>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Bool:
                    {
                        var val = reader.GetBoolean(i);
                        DelegateProperty<T, bool>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Double:
                    {
                        var val = reader.GetDouble(i);
                        DelegateProperty<T, double>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Float:
                    {
                        var val = reader.GetFloat(i);
                        DelegateProperty<T, float>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Byte:
                    {
                        var val = reader.GetByte(i);
                        DelegateProperty<T, byte>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Char:
                    {
                        var val = reader.GetChar(i);
                        DelegateProperty<T, char>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.DateTime:
                    {
                        var val = reader.GetDateTime(i);
                        DelegateProperty<T, DateTime>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Guid:
                    {
                        var val = reader.GetGuid(i);
                        DelegateProperty<T, Guid>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Long:
                    {
                        var val = reader.GetInt64(i);
                        DelegateProperty<T, long>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.Short:
                    {
                        var val = reader.GetInt16(i);
                        DelegateProperty<T, short>.delegateProperty.SetValue(value, pi.Name, val);
                    }
                    break;
                case ProType.None:
                default:
                    {
                        object dbValue = reader.GetValue(i);

                        if (dbValue == null || dbValue == DBNull.Value)
                            return;

                        exp.SetValue(value, pi.Name, dbValue);
                    }
                    break;
            }
        }
    }
}
