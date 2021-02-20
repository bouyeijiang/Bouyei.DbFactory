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

namespace Bouyei.DbFactoryCore
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

    internal class DbReaderExpressionToGeneric:DbParseBase, IDbReaderToGeneric
    {
        public T FromDbDataReader<T>(DbDataReader reader)
        {
            ExpressionProperty<T> expPro = new ExpressionProperty<T>();
            Type toType = expPro.classType;
            T value = Activator.CreateInstance<T>();

            var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var schemas = GetSchemasFromDbReader(reader);
            var pros = ToMappingPropertyEx(pinfos, schemas);

            foreach (var pi in pros)
            {
                if (pi.DbIndex == -1) continue;
                if (reader.IsDBNull(pi.DbIndex)) continue;

                object dbValue = reader.GetValue(pi.DbIndex);

                expPro.SetValue(value, pi.Name, dbValue);
            }
            return value;
        }

        public List<T> FromDbDataReaderToList<T>(DbDataReader reader)
        {          
            ExpressionProperty<T> expPro = new ExpressionProperty<T>();
            Type toType = expPro.classType;

            var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var schemas = GetSchemasFromDbReader(reader);
            var pros = ToMappingPropertyEx(pinfos, schemas);

            List<T> items = new List<T>(32);

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();

                foreach (var pi in pros)
                {
                    if (pi.DbIndex == -1) continue;
                    if (reader.IsDBNull(pi.DbIndex)) continue;

                    object dbValue = reader.GetValue(pi.DbIndex);
 
                    expPro.SetValue(value, pi.Name, dbValue);
 
                }
                items.Add(value);
            }
            return items;
        }
    }

    internal class DbReaderDelegateToGeneric:DbParseBase, IDbReaderToGeneric
    {
        public DbReaderDelegateToGeneric()
        {  }

        public T FromDbDataReader<T>(DbDataReader reader)
        {
            ExpressionProperty<T> expressPro = new ExpressionProperty<T>();

            Type toType = expressPro.classType;
            var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var schemas = GetSchemasFromDbReader(reader);
            var pros = PropertyInfoToEx(pinfos, schemas);

            T value = Activator.CreateInstance<T>();

            foreach (var pi in pros)
            {
                if (pi.DbIndex == -1) continue;
                if (reader.IsDBNull(pi.DbIndex)) continue;

                DataReaderDelegateToGeneric<T>(reader, pi.DbIndex, value, pi, expressPro);
            }
            return value;
        }

        public List<T> FromDbDataReaderToList<T>(DbDataReader reader)
        {
            ExpressionProperty<T> expressPro = new ExpressionProperty<T>();
            Type toType = expressPro.classType;

            var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            var schemas = GetSchemasFromDbReader(reader);
            var pros=PropertyInfoToEx(pinfos,schemas);

            List<T> items = new List<T>(32);

            while (reader.Read())
            {
                T value = Activator.CreateInstance<T>();

                foreach (var pi in pros)
                {
                    //filter not mapping column
                    if (pi.DbIndex == -1) continue;
                    if (reader.IsDBNull(pi.DbIndex)) continue;

                    DataReaderDelegateToGeneric<T>(reader, pi.DbIndex, value, pi, expressPro);
                }
                items.Add(value);
            }
            return items;
        }

        internal void DataReaderDelegateToGeneric<T>(DbDataReader reader, int i,
         T value, PropertyInfoEx pi, IExpProperty<T> exp)
        {
            try
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

                            //if (dbValue == null || dbValue == DBNull.Value)
                            //    return;

                            exp.SetValue(value, pi.Name, dbValue);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(pi.Name + "," + pi.ProType.ToString() + " error message:" + ex.Message);
            }
        }
    }

    //internal class DbReaderExpressionExToGeneric : DbParseBase, IDbReaderToGeneric
    //{
    //    public T FromDbDataReader<T>(DbDataReader reader)
    //    {
    //        var schema = reader.GetColumnSchema();

    //        ExpressionProperty<T> expressPro = new ExpressionProperty<T>();

    //        Type toType = expressPro.classType;
    //        var pinfos = toType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
    //            .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

    //        T value = Activator.CreateInstance<T>();

    //        var pros = PropertyInfoToEx(pinfos, schema);

    //        foreach (var pi in pros)
    //        {
    //            if (pi.DbIndex == -1) continue;
    //            if (reader.IsDBNull(pi.DbIndex)) continue;

    //            DataReaderExpToGeneric<T>(reader, pi.DbIndex, value, pi, expressPro);
    //            //for (int i = 0; i < reader.FieldCount; ++i)
    //            //{
    //            //    if (NameEquals(pi.Name, reader.GetName(i)))
    //            //    {
    //            //        DataReaderExpToGeneric<T>(reader, i, value, pi, expressPro);
    //            //        break;
    //            //    }
    //            //}
    //        }
    //        return value;
    //    }

    //    public List<T> FromDbDataReaderToList<T>(DbDataReader reader)
    //    {
    //        var schema = reader.GetColumnSchema();

    //        IExpProperty<T> expressPro = new ExpressionProperty<T>();
    //        Type toType = expressPro.classType;

    //        var pinfos = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //            .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

    //        var pros = PropertyInfoToEx(pinfos, schema);
    //        List<T> items = new List<T>(32);

    //        while (reader.Read())
    //        {
    //            T value = Activator.CreateInstance<T>();

    //            foreach (var pi in pros)
    //            {
    //                if (pi.DbIndex == -1) continue;
    //                if (reader.IsDBNull(pi.DbIndex)) continue;

    //                //优化性能
    //                DataReaderExpToGeneric<T>(reader, pi.DbIndex, value, pi, expressPro);
    //                //for (int i = 0; i < reader.FieldCount; ++i)
    //                //{
    //                //    if (NameEquals(pi.Name, reader.GetName(i)))
    //                //    {
    //                //        DataReaderExpToGeneric<T>(reader, i, value, pi, expressPro);
    //                //        break;
    //                //    }
    //                //}
    //            }
    //            items.Add(value);
    //        }
    //        return items;
    //    }

    //    internal void DataReaderExpToGeneric<T>(DbDataReader reader, int i,
    //    T value, PropertyInfoEx pi, IExpProperty<T> exp)
    //    {
    //        switch (pi.ProType)
    //        {
    //            case ProType.Int:
    //                {
    //                    var val = reader.GetInt32(i);
    //                    ExpressionPropertyEx<T, int>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Decimal:
    //                {
    //                    var val = reader.GetDecimal(i);
    //                    ExpressionPropertyEx<T, decimal>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.String:
    //                {
    //                    var val = reader.GetString(i);
    //                    ExpressionPropertyEx<T, string>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Bool:
    //                {
    //                    var val = reader.GetBoolean(i);
    //                    ExpressionPropertyEx<T, bool>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Double:
    //                {
    //                    var val = reader.GetDouble(i);
    //                    ExpressionPropertyEx<T, double>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Float:
    //                {
    //                    var val = reader.GetFloat(i);
    //                    ExpressionPropertyEx<T, float>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Byte:
    //                {
    //                    var val = reader.GetByte(i);
    //                    ExpressionPropertyEx<T, byte>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Char:
    //                {
    //                    var val = reader.GetChar(i);
    //                    ExpressionPropertyEx<T, char>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.DateTime:
    //                {
    //                    var val = reader.GetDateTime(i);
    //                    ExpressionPropertyEx<T, DateTime>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Guid:
    //                {
    //                    var val = reader.GetGuid(i);
    //                    ExpressionPropertyEx<T, Guid>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Long:
    //                {
    //                    var val = reader.GetInt64(i);
    //                    ExpressionPropertyEx<T, long>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.Short:
    //                {
    //                    var val = reader.GetInt16(i);
    //                    ExpressionPropertyEx<T, short>.expProperty.SetValue(value, pi.Name, val);
    //                }
    //                break;
    //            case ProType.None:
    //            default:
    //                {
    //                    object dbValue = reader.GetValue(i);

    //                    //if (dbValue == null || dbValue == DBNull.Value)
    //                    //    return;

    //                    exp.SetValue(value, pi.Name, dbValue);
    //                }
    //                break;
    //        }
    //    }
    //}

    public class ExpressionProperty<T>:IExpProperty<T>
    {
        private Func<T, string, object> getValue = null;
        private Dictionary<string, Action<T, object>> setterExpressionCaching = null;
    
        public Type classType { get; set; }

        public ExpressionProperty()
        {
            classType = typeof(T);
            setterExpressionCaching = new Dictionary<string, Action<T, object>>();
        }

        public PropertyInfo[] GetFieldNames()
        {
            var pros = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
               .Where(x => x.SetMethod != null && x.SetMethod.IsPublic);

            return pros.ToArray();
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

    internal class DelegateProperty<Entity, Value>
    {
        public delegate Value DelegateGetValue(Entity entity);
        public delegate void DelegateSetValue(Entity entity, Value v);

        internal Type getClassType = null;
        internal Type setClassType = null;

        internal Type tClassType = null;

        #region pre instance
        public static DelegateProperty<Entity, Value> delegateProperty = new DelegateProperty<Entity, Value>();
        private Dictionary<string, DelegateGetValue> getCaching = new Dictionary<string, DelegateGetValue>();
        private Dictionary<string, DelegateSetValue> setCaching = new Dictionary<string, DelegateSetValue>();
        private object lobject = new object();
        #endregion

        public DelegateProperty()
        {
            getClassType = typeof(DelegateGetValue);
            setClassType = typeof(DelegateSetValue);
            tClassType = typeof(Entity);
        }


        public Value GetValue(Entity value, string proName)
        {
            lock (lobject)
            {
                DelegateGetValue exp = null;
                string key = getClassType.FullName + proName;

                if (getCaching.TryGetValue(key, out exp) == false)
                {
                    exp = (DelegateGetValue)Delegate.CreateDelegate(getClassType, tClassType.GetProperty(proName).GetGetMethod());
                    if (exp == null)
                        return default(Value);

                    getCaching.Add(key, exp);
                }

                return exp(value);
            }
        }

        public void SetValue(Entity value, string proName, Value proValue)
        {
            lock (lobject)
            {
                DelegateSetValue exp = null;
                string key = getClassType.FullName + proName;

                if (setCaching.TryGetValue(key, out exp) == false)
                {
                    exp = (DelegateSetValue)Delegate.CreateDelegate(setClassType, tClassType.GetProperty(proName).GetSetMethod());
                    if (exp == null) return;

                    setCaching.Add(key, exp);
                }
                exp(value, proValue);
            }
        }
    }

    //internal class ExpressionPropertyEx<Entity,Value>
    //{
    //    internal Type tClassType = null;
    //    internal Type vClassType = null;

    //    private Dictionary<string, Func<Entity, Value>> getCaching = null;
    //    private Dictionary<string, Action<Entity, Value>> setCaching = null;

    //    public static ExpressionPropertyEx<Entity, Value> expProperty = new ExpressionPropertyEx<Entity, Value>();

    //    public ExpressionPropertyEx()
    //    {
    //        tClassType = typeof(Entity);
    //        vClassType = typeof(Value);

    //        getCaching = new Dictionary<string, Func<Entity, Value>>();
    //        setCaching = new Dictionary<string, Action<Entity, Value>>();
    //    }

    //    public Value GetValue<V>(Entity value, string proName)
    //    {
    //        string key = tClassType.FullName + proName;
    //        Func<Entity, Value> act = null;

    //        if (getCaching.TryGetValue(key, out act) == false)
    //        {
    //            act = GenericGetExpression(proName);
    //            getCaching.Add(key, act);
    //        }
    //        return act(value);
    //    }

    //    public Entity SetValue(Entity value, string proName, Value proValue)
    //    {
    //        string key = tClassType.FullName + proName;
    //        Action<Entity, Value> act = null;

    //        if (setCaching.TryGetValue(key, out act) == false)
    //        {
    //            act = GenericSetExpression(proName);
    //            setCaching.Add(key, act);
    //        }

    //        act(value, proValue);
    //        return value;
    //    }

    //    public Entity SetValue(string proName, Value proValue)
    //    {
    //        Entity obj = Activator.CreateInstance<Entity>();
    //        string key = tClassType.FullName + proName;
    //        Action<Entity, Value> act = null;

    //        if(setCaching.TryGetValue(key,out act) == false)
    //        {
    //            act = GenericSetExpression(proName);
    //            setCaching.Add(key, act);
    //        }
    //        act(obj, proValue);

    //        return obj;
    //    }

    //    private Func<Entity, Value> GenericGetExpression(string proName)
    //    {
    //        var property = tClassType.GetProperty(proName);
    //        var target = Expression.Parameter(property.DeclaringType);
    //        var getPropertyValue = Expression.Property(target, property);

    //        var exp = Expression.Lambda<Func<Entity, Value>>(getPropertyValue, target).Compile();

    //        return exp;
    //    }

    //    private Action<Entity, Value> GenericSetExpression(string proName)
    //    {
    //        var property = tClassType.GetProperty(proName);
    //        var target = Expression.Parameter(property.DeclaringType);
    //        var propertyValue = Expression.Parameter(vClassType);
    //        var setPropertyValue = Expression.Call(target, property.GetSetMethod(), propertyValue);
    //        return Expression.Lambda<Action<Entity, Value>>(setPropertyValue, target, propertyValue).Compile();
    //    }
    //}   
}
