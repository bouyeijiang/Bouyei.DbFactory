using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using Bouyei.DbFactory;
using System.Configuration;
using System.Threading;
using System.Data.Common;

namespace Bouyei.DbFactoryDemo
{
    using Bouyei.DbFactory.DbAdoProvider;
    using Bouyei.DbFactory.DbMapper;
    using Bouyei.DbEntities;

    class Program
    {
        static void Main(string[] args)
        {
            //生成简单查询脚本
            ISqlProvider sqlProvider = SqlProvider.CreateProvider();

            //查询
           var sql= sqlProvider.Select<User>()
                .From<User>().Where<User>(x => x.id == 1).SqlString;

            //修改
            sql = sqlProvider.Update<User>()
                .Set<User>(new User() { name = "bouyei"})
                .Where<User>(x => x.id == 1).SqlString;

            //删除
            sql = sqlProvider.Delete()
                .From<User>().Where<User>(x => x.name == "bouyei").SqlString;

            //插入
            sql = sqlProvider.Insert<User>()
                .Values<User>(new User[] {
                new User() { name ="hello", age=12 }
                ,new User() { name="bouyei",age=23} }).SqlString;

            //////ado.net 使用例子
             string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            //AdoDemo(connectionString);

            ////orm
            OrmDemo(connectionString);

            //Data Sync Provider
            //SyncProviderDemo syncProvider = new SyncProviderDemo();
            //syncProvider.Execute();

            //AdoDemo("");
            OrmDemo(connectionString);
        }

        private static void AdoDemo(string connectionString)
        {
           DataTable inverant=  DbProviderFactories.GetFactoryClasses();

            IAdoProvider dbProvider = AdoProvider.CreateProvider(connectionString, ProviderType.MySql);
            var ext = dbProvider.Connect(connectionString);
            var adort = dbProvider.Query(new Parameter()
            {
                CommandText = "select * from [user]"
            });

            DataTable dt = new DataTable();
            dt.Columns.Add("uname");
            dt.Columns.Add("age");

            dt.Rows.Add(new object[] { "bouyei", 27 });
            dt.Rows.Add(new object[] { "aileenyin", 25 });
            dt.Rows.Add(new object[] { "hhhh", 13 });
            dt.TableName = "user";

            var brt = dbProvider.BulkCopy(new BulkParameter()
            {
                DataSource = dt
            });
        }

        private static void OrmDemo(string connectionString)
        {
            //entity framework 使用例子
            IOrmProvider ormProvider = OrmProvider.CreateProvider();
            try
            {
                User item = new User() {
                     id=4,
                      name="bouyei"
                };// ormProvider.GetById<User>(1);
                UserDto ud = new UserDto()
                {
                    UserName = "http://aileenyin.com/"
                };

                //int c= ormProvider.Delete<User>(x => x.id == 3,true);

                //var query = ormProvider.QueryNoTracking<User>(x => true).FirstOrDefault();

                ////使用mapper修改对象
                User u = new User() {
                     name="b",
                      id=1
                };

                User b = new User() {
                     name="a",
                      id=2
                };
                EntityMapper.MapTo(u, b, FilterType.Include, "name");

                ormProvider.Update(item,true);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        class UserDto
        {
            public string UserName { get; set; }

            public string Pwd { get; set; }
        }
    }
}
