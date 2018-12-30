using System;
using System.Data;
using Bouyei.DbFactoryCore;
using System.Linq;
using Bouyei.DbFactoryCore.DbEntityProvider;
using Bouyei.DbFactoryCore.DbAdoProvider;

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



            //DataTable dt = new DataTable();
            //dt.TableName = "std_user";
            //dt.Columns.Add("id", typeof(int));
            //dt.Columns.Add("name", typeof(string));

            //for (int i = 0; i < 500000; ++i)
            //{
            //    dt.Rows.Add(new object[] { i, "ad" + i });
            //}

            //adoProvider = AdoProvider.CreateProvider("PORT=5432;DATABASE=test;HOST=localhost;PASSWORD=bouyei;USER ID=postgres", ProviderType.PostgreSQL);
            //var rt = adoProvider.Query(new Parameter()
            //{
            //    CommandText = "select * from std_user"
            //});

            //var brt= adoProvider.BulkCopy(new BulkParameter(dt));

            IOrmProvider ormProvider = OrmProvider.CreateProvider(Bouyei.DbFactoryCore.DbType.SqlServer, connectionString);
            var items = ormProvider.Table<DbEntity.User>();
            foreach (var item in items)
            {
                Console.WriteLine(item.no);
            }
            Console.ReadKey();
        }

        //  static string ToString<T>(Predicate<T> predicate)
        //{
        //    return predicate.ToString();
        //}

        static string ToString<T>(System.Linq.Expressions.Expression<T> expression)
        {
            return expression.ToString();
        }

       public class user
        {
            public int age { get; set; }
            public string name { get; set; }
        }

        public class ReportBid
        {
            public int ID { get; set; }
            public string LoginID { get; set; }
            public string RealName { get; set; }
            public string Mobile { get; set; }
            public DateTime CreateTime { get; set; }
            public decimal? ReMoney { get; set; }
            public decimal? InMoney { get; set; }
            public decimal? WithMoney { get; set; }
        }
    }
}
