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

            //MappedName 测试
           var sqls= sqlProvider.Insert<UserDto>().Values(new UserDto[] { new UserDto() {
                 Pwd="ds",
                  UserName="d"
            } }).SqlString;

            //查询
           var sql= sqlProvider.Select<User>()
                .From().Where(x => x.id == 1).SqlString;

            //修改
            sql = sqlProvider.Update<User>()
                .Set(new User() { name = "bouyei"})
                .Where<User>(x => x.id == 1).SqlString;

            //删除
            sql = sqlProvider.Delete()
                .From<User>().Where(x => x.name == "bouyei").SqlString;

            //插入
            sql = sqlProvider.Insert<User>()
                .Values(new User[] {
                new User() { name ="hello", age=12 }
                ,new User() { name="bouyei",age=23} }).SqlString;

            //////ado.net 使用例子
             //string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            //AdoDemo(connectionString);

            ////orm
            //OrmDemo(connectionString);

            //Data Sync Provider
            //SyncProviderDemo syncProvider = new SyncProviderDemo();
            //syncProvider.Execute();

            //AdoDemo("");
           // OrmDemo(connectionString);

            Bulkcopy();
        }

        private static void Bulkcopy()
        {
            //DataTable dt = new DataTable();
            //dt.TableName = "gz_gwqhzs";

            //dt.Columns.Add("sprovince",typeof(string));
            //dt.Columns.Add("sbiaotuzhidi",typeof(double));

            //dt.Rows.Add("贵州", 111.2);
            //dt.Rows.Add("贵州", 111.4);
            //dt.Rows.Add("贵州", 114.3);
            //dt.Rows.Add("贵州", 111.2);
            //dt.Rows.Add("贵州", 131.2);
            //dt.Rows.Add("贵州", 131.2);
            //dt.Rows.Add("贵州", 115.2);
            //dt.Rows.Add("贵州", 1141.2);
            //rt.Result.TableName = "gz_gwqhzs";
            //var brt = pgProvider.BulkCopy(new BulkParameter(rt.Result));
            //if (string.IsNullOrEmpty(brt.Info) == false)
            //    throw new Exception(brt.Info);

            // rt.Result.TableName = "gz_weight";        

            // IAdoProvider dbprovider = AdoProvider.CreateProvider("PORT=5432;DATABASE=GDZL;HOST=localhost;PASSWORD=123456;USER ID=postgres",DbFactory.DbType.PostgreSQL);
            //var brt= dbprovider.BulkCopy(new BulkParameter(rt.Result));


            DataTable dt = new DataTable();
            dt.Columns.Add("uname",typeof(string));
            dt.Columns.Add("upwd", typeof(string));
            dt.Columns.Add("age",typeof(int));
            dt.Columns.Add("score", typeof(float));

            dt.Rows.Add("bouyei", "232a", 12, 239.4);
            dt.Rows.Add("hell哦", "232a", 12, 239.4);
            dt.TableName = "luser";

            string str = "server=127.0.0.1;port=3306;user=root;password=123456; database=gdzl;";
            IAdoProvider dbProvider = AdoProvider.CreateProvider(str, DbFactory.DbType.MySql);
            var c = dbProvider.Connect(str);

            var brt= dbProvider.BulkCopy(new BulkParameter(dt));
        }

        private static void AdoDemo(string connectionString)
        {
            IAdoProvider dbProvider = AdoProvider.CreateProvider(connectionString);
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


            var rt= dbProvider.Query<User>(x => x.age >= 20);

            dbProvider.Delete<User>(x => x.name == "bouyei");

        }

        private static void OrmDemo(string connectionString)
        {
            //entity framework 使用例子
            IOrmProvider ormProvider = OrmProvider.CreateProvider();
            try
            {
                User item =
                //    new User() {
                //     id=4,
                //      name="bouyei"
                //};//
                ormProvider.GetById<User>(1);
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

        [MappedName("db_user")]
        class UserDto
        {
            public string UserName { get; set; }

            [Ignore]
            public string Pwd { get; set; }
        }
    }

    public class TdWeight
    {
        public string kfq_type { get; set; }
        public string kfq_name { get; set; }

        public decimal kfq_weight { get; set; }

        public string kfq_target { get; set; }

        public decimal kfq_target_weight { get; set; }

        public string kfq_subtarget { get; set; }

        public decimal kfq_subtarget_weight { get; set; }

        public string kfq_index { get; set; }

        public decimal kfq_index_weight { get; set; }
    }
}
