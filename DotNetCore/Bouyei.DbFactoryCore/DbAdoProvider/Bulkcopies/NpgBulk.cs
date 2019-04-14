using System;
using System.Linq;
using System.Data;

using Npgsql;
namespace Bouyei.DbFactoryCore.DbAdoProvider.Bulkcopies
{
    using Factories;
    using System.Collections.Generic;
    using System.Reflection;

    internal class NpgBulk : BaseFactory, IFactory
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public NpgBulk() { }

        public NpgBulk(string ConnectionString, int timeout = 1800)
        {
            this.ConnectionString = ConnectionString;
            this.ExecuteTimeout = timeout;
        }

        public int WriteToServer(DataTable dataSource,int batchSize=10240)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var import = conn.BeginBinaryImport(ToImportFormat(dataSource.TableName,
                    dataSource.Columns.Cast<DataColumn>().Select(x => x.ColumnName))))
                {
                    foreach (DataRow dr in dataSource.Rows)
                    {
                        import.WriteRow(dr.ItemArray);
                    }
                }
            }
            return dataSource.Rows.Count;
        }

        public int WriteToServer<T>(List<T> dataList, int batchSize = -1)
        {
            ExpressionProperty<T> exp = new ExpressionProperty<T>();
            var types = exp.GetFieldNames();
            int rows = 0;
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var import = conn.BeginBinaryImport(ToImportFormat(exp.classType.Name, types.Select(x => x.Name))))
                {
                    foreach (var item in dataList)
                    {
                        foreach (var col in types)
                        {
                            object val = exp.GetValue(item, col.Name);
                            if (val == null) continue;

                            WriteValue(val, col.DeclaringType.Name, import);
                            ++rows;
                        }
                    }
                    import.Complete();
                }
            }
            return rows;
        }


        public void WriteToServer(IDataReader reader,string tableName,int batchSize = 10240)
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
                using (var export = conn.BeginBinaryExport(string.Format("COPY {0}({1}) TO STDOUT (FORMAT BINARY)", tableName,
                     string.Join(",", pros.Select(x => x.Name)))))
                {
                    BinaryToList<T>(exp, pros, export, action);
                }
            }
        }

        private string ToImportFormat(string tabName, IEnumerable<string> columns)
        {
            string str = string.Format("COPY {0}({1}) FROM STDIN (FORMAT BINARY)", tabName, string.Join(",", columns));
            return str;
        }

        private void BinaryToList<T>(ExpressionProperty<T> exp, PropertyInfo[] pros,
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
                case "Decimal": return exporter.Read<decimal>();
                case "Double": return exporter.Read<double>();
                case "Float": return exporter.Read<float>();
                case "Byte": return exporter.Read<byte>();
                case "Char": return exporter.Read<char>();
                case "Boolean": return exporter.Read<bool>();
                case "DateTime": return exporter.Read<DateTime>();
                default: return exporter.Read<string>();
            }
        }

        private void WriteValue(object value, string typeName, NpgsqlBinaryImporter importer)
        {
            switch (typeName)
            {
                case "String":
                    importer.Write(value.ToString());
                    break;
                case "Int":
                case "Int32":
                    importer.Write((int)value);
                    break;
                case "Long":
                case "Int64":
                    importer.Write((long)value);
                    break;
                case "Decimal":
                    importer.Write((decimal)value);
                    break;
                case "Double":
                    importer.Write((double)value);
                    break;
                case "Float":
                    importer.Write((float)value);
                    break;
                case "Byte":
                    importer.Write((byte)value);
                    break;
                case "Char":
                    importer.Write((char)value);
                    break;
                case "Boolean":
                    importer.Write((bool)value);
                    break;
                case "DateTime":
                    importer.Write((DateTime)value);
                    break;
                default:
                    importer.Write(value.ToString());
                    break;
            }
        }
    }
}
