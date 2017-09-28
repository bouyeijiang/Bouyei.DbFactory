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

namespace DbFactoryDemo
{
    using Bouyei.DbFactory.DbAdoProvider;
    using Bouyei.DbFactory.DbSqlProvider;
    using Bouyei.DbFactory.DbSqlProvider.Extensions;
    using Bouyei.DbFactory.DbMapper;
    using Bouyei.DbEntities; 

    class Program
    {
        static void Main(string[] args) 
        {
            //生成简单查询脚本
            var sqlProvider = SqlProvider.CreateProvider();

            var sql = sqlProvider.Select("username", "realname", "age")
                .From("sys_user").Where(new KeyValue()
                {
                    Name = "username",
                    Value = "bouyei"
                }).SqlString;

            //结果:Select username,realname,age From sys_user Where username='bouyei' 

            ////ado.net 使用例子
            string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            IAdoProvider dbProvider = AdoProvider.CreateProvider(connectionString,ProviderType.SqlServer);
            var ext = dbProvider.Connect(connectionString);
            var adort = dbProvider.Query(new DbExecuteParameter()
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

            var brt = dbProvider.BulkCopy(new DbExecuteBulkParameter()
            {
                DstDataTable = dt
            });
 
            //entity framework 使用例子
            IOrmProvider ormProvider = OrmProvider.CreateProvider("DbConnection");
            try
            {
                User item = ormProvider.GetById<User>(1);
                UserDto ud = new UserDto()
                {
                    UserName = "http://aileenyin.com/"
                };

                var query = ormProvider.Query<User>().FirstOrDefault();

                //使用mapper修改对象
                EntityMapper.MapTo<UserDto, User>(ud, item);
                ormProvider.Update(item);
                //保存修改
                int rt = ormProvider.SaveChanges();
            }
            catch(Exception ex)
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
