/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/4/22 1:49:15
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: caeafeba-afbf-4f82-92f3-5738bd6a902f
---------------------------------------------------------------*/
using System;
using System.Linq;
using System.Reflection;

namespace Bouyei.DbFactoryCore.DbMapper
{
    public static class EntityMapper
    {
        public static ToEntity MapToCreated<FromEntity, ToEntity>(this FromEntity from)
        {
            ToEntity to = Activator.CreateInstance<ToEntity>();
            var tTo = typeof(ToEntity);
            var psFrom = typeof(FromEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var pFrom in psFrom)
            {
                if (ExistIgnoreAttribute(pFrom))
                    continue;

                var pTo = tTo.GetProperty(pFrom.Name);
                if (pTo == null) continue;

                var vFrom = pFrom.GetValue(from);
                if (vFrom == null) continue;

                var pToType = pTo.PropertyType;
                object vTo = null;

                if (pToType.IsGenericType && pToType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    vTo = Convert.ChangeType(vFrom, pToType.GetGenericArguments()[0]);
                }
                else if (pToType.IsEnum)
                {
                    vTo = Enum.ToObject(pToType, vFrom);
                }
                else
                {
                    vTo = Convert.ChangeType(vFrom, pTo.PropertyType);
                }
                pTo.SetValue(to, vTo);
            }
            return to;
        }

        public static ToEntity MapTo<FromEntity, ToEntity>(this FromEntity from, ToEntity to)
        {
            var tTo = typeof(ToEntity);
            PropertyInfo[] psFrom = typeof(FromEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var pFrom in psFrom)
            {
                if (ExistIgnoreAttribute(pFrom))
                    continue;

                var vFrom = pFrom.GetValue(from);
                if (vFrom == null) continue;

                var pTo = tTo.GetProperty(pFrom.Name);
                if (pTo == null) continue;

                var pToType = pTo.PropertyType;
                object vTo = null;

                if (pToType.IsGenericType && pToType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    vTo = Convert.ChangeType(vFrom, pToType.GetGenericArguments()[0]);
                }
                else if (pToType.IsEnum)
                {
                    vTo = Enum.ToObject(pToType, vFrom);
                }
                else
                {
                    vTo = Convert.ChangeType(vFrom, pTo.PropertyType);
                }
                pTo.SetValue(to, vTo);
            }
            return to;
        }

        public static ToEntity MapTo<FromEntity, ToEntity>(this FromEntity from, ToEntity to,
            FilterType filterType,
            params string[] FilterColumns)
        {
            var tTo = typeof(ToEntity);
            var psFrom = typeof(FromEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var pFrom in psFrom)
            {
                if (ExistIgnoreAttribute(pFrom))
                    continue;

                if (filterType != FilterType.Default
                    && FilterColumns.Length > 0)
                {
                    switch (filterType)
                    {
                        case FilterType.Ignore:
                            if (FilterColumns.Contains(pFrom.Name)) continue;
                            break;
                        case FilterType.Include:
                            if (!FilterColumns.Contains(pFrom.Name)) continue;
                            break;
                    }
                }

                object vFrom = pFrom.GetValue(from);
                if (vFrom == null) continue;

                PropertyInfo pTo = tTo.GetProperty(pFrom.Name);
                if (pTo == null) continue;

                Type pToType = pTo.PropertyType;
                object vTo = null;

                if (pToType.IsGenericType && pToType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    vTo = Convert.ChangeType(vFrom, pToType.GetGenericArguments()[0]);
                }
                else if (pToType.IsEnum)
                {
                    vTo = Enum.ToObject(pToType, vFrom);
                }
                else
                {
                    vTo = Convert.ChangeType(vFrom, pTo.PropertyType);
                }
                if (filterType == FilterType.NullIgnore
                    && vTo == null) continue;

                pTo.SetValue(to, vTo);
            }
            return to;
        }

        private static bool ExistIgnoreAttribute(PropertyInfo pInfo)
        {
            var attrs = pInfo.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                if (attr is IgnoreAttribute ignore)
                {
                    if (ignore.AttrType == AttributeType.Ignore
                        || ignore.AttrType == AttributeType.IgnoreRead
                        || ignore.AttrType == AttributeType.IgnoreWrite)
                        return true;
                }
            }
            return false;
        }
    }

    public enum FilterType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 不包含忽略
        /// </summary>
        Ignore = 1,
        /// <summary>
        /// 包含忽略
        /// </summary>
        Include = 2,
        /// <summary>
        /// 空值忽略
        /// </summary>
        NullIgnore = 4
    }
}
