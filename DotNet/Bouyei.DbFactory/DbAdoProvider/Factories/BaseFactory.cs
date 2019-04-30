using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Bouyei.DbFactory.DbAdoProvider.Factories
{
    public class BaseFactory
    {
        public string ConnectionString { get; protected set; }

        public int ExecuteTimeout { get; protected set; }

       public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public virtual DbProviderFactory GetFactory()
        {
            return null;
        }

        public DataTable ArrayToDataTable(Array dataSource, string tableName)
        {
            var firstRow = dataSource.GetValue(0).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            DataTable dt = new DataTable();
            dt.TableName = tableName;

            foreach (var p in firstRow)
            {
                dt.Columns.Add(p.Name, p.PropertyType);
            }

            foreach (var row in dataSource)
            {
                var types = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                var vals = types.Select(x => x.GetValue(row, null)).Where(x => x != null).ToArray();
                dt.Rows.Add(vals);
            }

            return dt;
        }
    }
}
