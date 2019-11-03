using System;
using System.Data;
using Bouyei.DbFactoryCore;
using System.Linq;
using Bouyei.DbFactoryCore.DbAdoProvider;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DotNetCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string connstr = "server=127.0.0.1;Port=5432;Database=postgres;uid=postgres;pwd=bouyei;";
            DbProvider dbProvider = new DbProvider(connstr, Bouyei.DbFactoryCore.FactoryType.PostgreSQL);

           var rt= dbProvider.Query<luser>(x => x.uname == "bouyei");

            //string str = "server=127.0.0.1;port=3306;user=root;password=123456; database=gdzl;";
            //IAdoProvider dbProvider = AdoProvider.CreateProvider(str,Bouyei.DbFactoryCore.DbType.MySql);
            //List<luser> ls = new List<luser>();
            //ls.Add(new luser()
            //{
            //    uname = "bouyei",
            //    uage = 28,
            //    score = 34.4f,
            //    upwd = "dfs"
            //});
            //ls.Add(new luser()
            //{
            //    uname = "hkj",
            //    uage = 30,
            //    score = 34.56f,
            //    upwd = "地方"
            //});

            //var param = new CopyParameter<Array>(ls.ToArray());
            //var arraybrt = dbProvider.BulkCopy(param);
            //Console.WriteLine(arraybrt.Info);
            // string connectionString = "Data Source=127.0.0.1;Initial Catalog=B;User ID=sa;Password=123456;";
            //IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

            // int c = 100,b=3;
            // double sum = 0;
            // while ((b--) >= 0)
            // {
            //     System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //     watch.Start();
            //     while ((c--) > 0)
            //     {
            //         var users = adoProvider.Query<DbEntity.User>(x => 1 == 1);
            //     }
            //     watch.Stop();
            //     sum += watch.Elapsed.TotalMilliseconds;
            // }
            // Console.Write(sum / 3);

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

            //IOrmProvider ormProvider = OrmProvider.CreateProvider(Bouyei.DbFactoryCore.DbType.SqlServer, connectionString);
            //var items = ormProvider.Table<DbEntity.User>();
            //foreach (var item in items)
            //{
            //    Console.WriteLine(item.no);
            //}

            Console.ReadKey();
        }

       public class user
        {
            public int age { get; set; }
            public string name { get; set; }
        }

    }

    public class fc3d
    {
        public string fname { get; set; }

        public long fcode { get; set; }
    }

    //[MappedName("db_user")]
    public class luser
    {
        public string uname { get; set; }

        public string upwd { get; set; }

        public int uage { get; set; }
        [Ignore]
        public float score { get; set; }
    }

    public class dbuser
    {
        public string umobile { get; set; }
    }
}
