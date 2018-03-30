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

            IOrmProvider ormProvider = OrmProvider.CreateProvider(ProviderType.SqlServer, connectionString);
            var items= ormProvider.Query<DbEntity.User>("select * from MemUser").ToList();
            foreach(var item in items)
            {
                Console.WriteLine(item.uName);
            }
            Console.ReadKey();
        }
    }
}
