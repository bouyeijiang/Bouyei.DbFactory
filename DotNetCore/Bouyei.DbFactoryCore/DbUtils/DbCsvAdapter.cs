/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/5/20 1:15:02
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 0f121e28-82ff-4415-a974-e622411a14ce
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;

namespace Bouyei.DbFactoryCore.DbUtils
{
    public class DbCsvAdapter
    {
        public Encoding encoding { get; set; } = Encoding.Default;

        public MemoryStream ExportCsv(DataTable dt)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter write = new StreamWriter(ms,encoding);
            {
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    write.Write(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : ""));
                }
                write.WriteLine();

                foreach (DataRow dr in dt.Rows)
                {
                    write.WriteLine(string.Join(",", FilterSpecialSymbol(dr.ItemArray)));
                }
            }

            return ms;
        }

        public byte[] ExportCsvTo(DataTable dt)
        {
            byte[] buffer = null;
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter write = new StreamWriter(ms, encoding))
            {
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    write.Write(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : ""));
                }
                write.WriteLine();

                foreach (DataRow dr in dt.Rows)
                {
                    string item = string.Join(",", FilterSpecialSymbol(dr.ItemArray));
                    write.WriteLine(item);
                }
                write.Flush();
                buffer = ms.ToArray();
            }

            return buffer;
        }

        public bool ExportCsvToFile(DataTable dt, string saveFileName)
        {
            using (StreamWriter write = new StreamWriter(saveFileName, false, encoding))
            {
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    write.Write(dt.Columns[i].ColumnName + (i < dt.Columns.Count - 1 ? "," : ""));
                }
                write.WriteLine();

                foreach (DataRow dr in dt.Rows)
                {
                    string item = string.Join(",", FilterSpecialSymbol(dr.ItemArray));
                    write.WriteLine(item);
                }
                write.Flush();
                return true;
            }
        }
        public bool ExportCsvToFile(Array array, string saveFileName)
        {
            var first = array.GetValue(0).GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (StreamWriter write = new StreamWriter(saveFileName, false, encoding))
            {
                //列名
                write.Write(string.Join(",", first.Select(x => x.Name)));

                write.WriteLine();
                //数据行

                foreach (var row in array)
                {
                    var pros = row.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var cols = pros.Select(x => x.GetValue(row, null)).ToArray();

                    string item = string.Join(",", FilterSpecialSymbol(cols));
                    write.WriteLine(item);
                }
                write.Flush();
                return true;
            }
        }
        private string[] FilterSpecialSymbol(object[] array)
        {
            string[] rarray = new string[array.Length];
            for (int i = 0; i < rarray.Length; ++i)
            {
                string itm = array[i].ToString().Replace("\"", "\"\"");
                if (itm.Contains(",") || itm.Contains("\"")
                || itm.Contains("\r") || itm.Contains("\n"))
                {
                    itm = string.Format("\"{0}\"", itm);
                }

                rarray[i] = itm;
            }
            return rarray;
        }
    }
}
