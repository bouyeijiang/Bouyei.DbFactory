using System;
using System.Data;
using Bouyei.DbFactoryCore;
using System.Linq;

namespace DotNetCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=.;Initial Catalog=testdb;User ID=sa;Password=bouyei;";
            IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);
            //var rt = adoProvider.Query(new Parameter()
            //{
            //    CommandText = "select * from MemUser"
            //});

            //foreach (DataRow dr in rt.Result.Rows)
            //{
            //    Console.WriteLine(string.Join(",", dr.ItemArray));
            //}

            DataTable dt = new DataTable();
            dt.TableName = "std_user";
            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("name", typeof(string));

            for (int i = 0; i < 500000; ++i)
            {
                dt.Rows.Add(new object[] { i, "ad" + i });
            }

            adoProvider = AdoProvider.CreateProvider("PORT=5432;DATABASE=test;HOST=localhost;PASSWORD=bouyei;USER ID=postgres", ProviderType.PostgreSQL);
            var rt = adoProvider.Query(new Parameter()
            {
                CommandText = "select * from std_user"
            });

           var brt= adoProvider.BulkCopy(new BulkParameter(dt));

            IOrmProvider ormProvider = OrmProvider.CreateProvider(ProviderType.SqlServer, connectionString);
            var items = ormProvider.Query<DbEntity.User>("select * from MemUser").ToList();
            foreach (var item in items)
            {
                Console.WriteLine(item.uName);
            }
            Console.ReadKey();
        }

       public class Info
        {
            public int id { get; set; }

            public string name { get; set; }
        }
    }
}
