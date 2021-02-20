/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/11/22 16:35:29
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: 
 *   guid: 85661d58-c751-4319-b34b-90c396695f4b
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace Bouyei.DbFactory.DbUtils
{
    public static class DataTableAdapter
    {
        #region public
        /// <summary>
        /// 根据结构体创建DataTable数据集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataTable ConvertTo<T>()
        {
            PropertyInfo[] properties = null;
            DataTable table = CreateTable<T>(out properties);

            return table;
        }

        /// <summary>
        /// 只复制List集合到DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int CopyTo<T>(ref DataTable table, IList<T> list)
        {
            ExpressionProperty<T> expressPros = new ExpressionProperty<T>();
            Type type = expressPros.classType;

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //添加数据行
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyInfo p in properties)
                {
                    object v = expressPros.GetValue(item, p.Name);// p.GetValue(item);
                    if (v == null) continue;
                    row[p.Name] = v;
                }

                table.Rows.Add(row);
            }
            return list.Count;
        }

        /// <summary>
        /// 创建并复制表到DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ConvertTo<T>(this IList<T> list)
        {
            ExpressionProperty<T> expressPros = new ExpressionProperty<T>();
            PropertyInfo[] properties = null;
            DataTable table = CreateTable<T>(out properties);

            //添加数据行
            foreach (T item in list)
            {
                DataRow row = table.NewRow();
                foreach (PropertyInfo p in properties)
                {
                    object v = expressPros.GetValue(item, p.Name);// p.GetValue(item);
                    if (v == null) continue;
                    row[p.Name] = v;
                }

                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// DataTable数据转换到List集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ConvertTo<T>(this DataTable table)
        {
            ExpressionProperty<T> expressPros = new ExpressionProperty<T>();
            List<T> list = new List<T>(table.Rows.Count);
            Type type = expressPros.classType;

            //换行列
            List<PropertyInfo> properties = new List<PropertyInfo>(table.Columns.Count);
            foreach (DataColumn column in table.Columns)
            {
                PropertyInfo pinfo = type.GetProperty(column.ColumnName);
                if (pinfo == null) continue;

                properties.Add(pinfo);
            }

            var pros = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //转换行
            foreach (DataRow dr in table.Rows)
            {
                T obj = Activator.CreateInstance<T>();
                foreach (var p in pros)
                {
                    object value = dr[p.Name];
                    if (value == null) continue;

                    expressPros.SetValue(obj, p.Name, value);
                }
                list.Add(obj);
            }

            return list;
        }

        #endregion

        #region private

        private static DataTable CreateTable<T>(out PropertyInfo[] properties)
        {
            Type type = typeof(T);
            DataTable table = new DataTable(type.Name);
            properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo propertie in properties)
                table.Columns.Add(propertie.Name, propertie.PropertyType);

            return table;
        }

        #endregion
    }
}
