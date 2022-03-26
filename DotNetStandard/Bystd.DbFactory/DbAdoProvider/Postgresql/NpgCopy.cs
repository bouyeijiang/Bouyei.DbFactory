using System;
using System.Linq;
using System.Data;
using System.Reflection;

using Npgsql;
namespace Bystd.DbFactory.DbAdoProvider.Postgresql
{
    using System.Collections.Generic;

    internal class NpgCopy : BaseFactory
    {
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public NpgCopy(string ConnectionString, int timeout = 1800)
            :base(FactoryType.PostgreSQL,timeout)
        {
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = timeout;
        }

        public int WriteToServer(DataTable dataSource,int batchSize=10240)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                
                List<string> ls = new List<string>(dataSource.Columns.Count);
                foreach (DataColumn col in dataSource.Columns)
                    ls.Add(col.ColumnName);

                using (var import = conn.BeginBinaryImport(ToImportFormat(dataSource.TableName,ls)))
                {
                    foreach (DataRow dr in dataSource.Rows)
                    {
                        import.WriteRow(dr.ItemArray);
                    }
                    import.Complete();
                }
            }
            return dataSource.Rows.Count;
        }

        public int WriteToServer(Array dataSource,string tableName,int batchSize=-1)
        {
            var type = dataSource.GetValue(0).GetType();
            var pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int rows = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var import = conn.BeginBinaryImport(ToImportFormat(tableName, getColumnNames(pros))))
                {
                    foreach (var item in dataSource)
                    {
                        object[] cols = new object[pros.Length];
                        for (int i = 0; i < cols.Length; ++i)
                        {
                            cols[i] = pros[i].GetValue(item, null);
                        }
                        import.WriteRow(cols);
                        ++rows;
                    }
                    import.Complete();
                }
            }
            return rows;
        }

        public void WriteToServer(IDataReader dataSource,string tableName,int batchSize = 10240)
        {
            throw new Exception("no support");
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            ExpressionProperty<T> exp = new ExpressionProperty<T>();
            var pros = exp.GetFieldNames();

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var export = conn.BeginBinaryExport(string.Format("COPY {0}({1}) TO STDOUT (FORMAT BINARY)",
                    tableName,string.Join(",", getColumnNames(pros)))))
                {
                    BinaryToList<T>(exp, pros, export, action);
                }
            }
        }

        private string ToImportFormat(string tabName,IEnumerable<string> columns)
        {
            string str= string.Format("COPY {0}({1}) FROM STDIN (FORMAT BINARY)", tabName,string.Join(",", columns));
            return str;
        }

        private List<string> getColumnNames(PropertyInfo[] pros)
        {
            List<string> ls = new List<string>(pros.Length);
            foreach (var p in pros)
            {
                ls.Add(p.Name);
            }
            return ls;
        }
 
        private void BinaryToList<T>(ExpressionProperty<T> exp,PropertyInfo[] pros,
            NpgsqlBinaryExporter exporter, Func<T, bool> rowAction)
        {
            while (exporter.StartRow() != -1)
            {
                T value = Activator.CreateInstance<T>();
                foreach (var item in pros)
                {
                    if (exporter.IsNull) continue;

                    var proType = item.PropertyType;

                    var rValue = ReadValue(proType.Name, exporter);
                    if (rValue == null) continue;

                    exp.SetValue(proType.Name, rValue);

                    //pro.SetValue(value, rValue);
                }
                bool isContinue = rowAction(value);

                if (isContinue == false)
                {
                    exporter.Cancel();
                    break;
                }
            }
        }

        private object ReadValue(string typeName, NpgsqlBinaryExporter exporter)
        {
            switch (typeName)
            {
                case "String": return exporter.Read<string>();
                case "Int":
                case "Int32": return exporter.Read<Int32>();
                case "Long":
                case "Int64": return exporter.Read<Int64>();
                case "Decimal":return exporter.Read<decimal>();
                case "Double": return exporter.Read<double>();
                case "Float": return exporter.Read<float>();
                case "Byte": return exporter.Read<byte>();
                case "Char":return exporter.Read<char>();
                case "Boolean": return exporter.Read<bool>();
                case "DateTime":return exporter.Read<DateTime>();
                default:return exporter.Read<string>();
            }
        }

        //private void WriteValue(object value,string typeName,NpgsqlBinaryImporter importer)
        //{
        //    switch (typeName)
        //    {
        //        case "String":
        //            importer.Write(value.ToString());
        //            break; 
        //        case "Int":
        //        case "Int32":
        //            importer.Write((int)value);
        //            break;
        //        case "Long":
        //        case "Int64":
        //            importer.Write((long)value);
        //            break;
        //        case "Decimal":
        //            importer.Write((decimal)value);
        //            break;
        //        case "Double":
        //            importer.Write((double)value);
        //            break;
        //        case "Float":
        //            importer.Write((float)value);
        //            break;
        //        case "Byte":
        //            importer.Write((byte)value);
        //            break;
        //        case "Char":
        //            importer.Write((char)value);
        //            break;
        //        case "Boolean":
        //            importer.Write((bool)value);
        //            break;
        //        case "DateTime":
        //            importer.Write((DateTime)value);
        //            break;
        //        default:
        //            importer.Write(value.ToString());
        //            break;
        //    }
        //}
    }
}
