using System;
using System.Data;
using Bouyei.DbFactoryCore;
using System.Linq;
//using Bouyei.DbFactoryCore.DbAdoProvider;
using System.Collections.Generic;
using DbEntity;
using Bouyei.DbFactoryCore.DbMapper;

namespace DotNetCoreDemo
{
    using Bouyei.DbFactoryCore.DbSqlProvider.SqlFunctions;

    class Program
    {
        static void Main(string[] args)
        {
            SqlDemo();
            //string str = "Server=127.0.0.1;Port=5432;Userid=postgres;password=bouyei;database=postgres;";
            //IAdoProvider provider = AdoProvider.CreateProvider("", FactoryType.PostgreSQL);
            //User usr = new User() { 
            // age=1,
            //  no=2
            //};
            //var b= provider.Update<User>(usr, x => x.id == 1);

            //Bulkcopy(str);
            //AdoDemo(str);
            //OrmDemo(str);
        }

        private static void SqlDemo()
        {
            //生成简单查询脚本
            ISqlProvider sqlProvider = SqlProvider.CreateProvider();

            //group by 
            string sqlgroupby = sqlProvider.Select<User>().Count().From<User>()
                .Where(x => x.age == 1).GroupBy<User>().SqlString;

            //like 语法'bouyei%'
            var beginSql = sqlProvider.Select<User>().From().Where(x => x.name.StartsWith("18212") || x.name.StartsWith("18212")).SqlString;

            //function 
            string sqlfun = sqlProvider.Select<User>(new Max("age")).From<User>().Where(x => x.age > 20).SqlString;

            //order by
            var osql = sqlProvider.Select<User>().From<User>().OrderBy(SortType.Asc, "name").SqlString;

            var dic = new Dictionary<string, object>();
            dic.Add("name", "hellow");
            dic.Add("age", 0);
            dic.Add("score", 1.0);

            //MappedName 测试
            var sqls = sqlProvider.Insert<UserDto>().Values(new UserDto[] { new UserDto() {
                 Pwd="ds",
                  UserName="d"
            } }).SqlString;

            //查询
            var sql = sqlProvider.Select<User>()
                 .From().Where(x => x.id == 1).Top(FactoryType.PostgreSQL, 10).SqlString;

            //修改
            sql = sqlProvider.Update<User>()
                .Set(new User() {  no=1})
                .Where<User>(x => x.id == 1 || (x.name == "b" && x.no == 2)).SqlString;

            //删除
            sql = sqlProvider.Delete()
                .From<User>().Where(x => x.name == "bouyei").SqlString;

            //插入
            sql = sqlProvider.Insert<User>()
                .Values(new User[] {
                new User() { name ="hello", age=12 }
                ,new User() { name="bouyei",age=23} }).SqlString;
        }

        private static void Bulkcopy(string str)
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


            //DataTable dt = new DataTable();
            //dt.Columns.Add("uname",typeof(string));
            //dt.Columns.Add("upwd", typeof(string));
            //dt.Columns.Add("age",typeof(int));
            //dt.Columns.Add("score", typeof(float));

            //dt.Rows.Add("bouyei", "232a", 12, 239.4);
            //dt.Rows.Add("hell哦", "232a", 12, 239.4);
            //dt.TableName = "luser";

            IAdoProvider dbProvider = AdoProvider.CreateProvider(str, FactoryType.PostgreSQL);

            //var brt= dbProvider.BulkCopy(new BulkParameter(dt));

            fc3d[] fc = new fc3d[] {
                new fc3d(){ fname="dd", fcode=121 },
                new fc3d(){fname="sd",fcode=23 },
                new fc3d(){ fname="个",fcode=2323}
            };

            var param = new CopyParameter<Array>(fc);
            param.TableName = "fc3d";

            var rt = dbProvider.BulkCopy(param);

        }

        private static void AdoDemo(string connectionString)
        {
            IAdoProvider dbProvider = AdoProvider.CreateProvider(connectionString);
          
            var ext = dbProvider.Connect(connectionString);

            var adort = dbProvider.Query(new Parameter()
            {
                CommandText = "select * from [user]"
            });

            //
            var rt = dbProvider.PageQuery<User>(x => x.age >= 20, 0, 10);

            //定义更新
            var dic = new Dictionary<string, object>();
            dic.Add("name", "hellow");
            dic.Add("age", 0);
            dic.Add("score", 1.0);
            dbProvider.Update<User>(dic, x => x.id == 1);

            dbProvider.Delete<User>(x => x.name == "bouyei");

        }

        private static void OrmDemo(string connectionString)
        {
            //entity framework 使用例子
            IOrmProvider ormProvider = OrmProvider.CreateProvider(FactoryType.PostgreSQL);
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
                User u = new User()
                {
                    name = "b",
                    id = 1
                };

                User b = new User()
                {
                    name = "a",
                    id = 2
                };
                EntityMapper.MapTo(u, b, FilterType.Include, "name");

                ormProvider.Update(item, true);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
    [Flags]
        public enum Sex
        {
            Male,
            Female
        }

    [MappedName("db_user")]
    class UserDto:BaseEntity<UserDto>
    {
        public string UserName { get; set; }

        [Ignore]
        public string Pwd { get; set; }
    }
    public class BaseEntity<T> : TableMapper<T> where T : class
    {
        public BaseEntity()
        {
            string connstr = "Host=127.0.0.1;Port=5432;User id=postgres;Password=bouyei;Database=postgres;";
            var provider = AdoProvider.CreateProvider(connstr, FactoryType.PostgreSQL);
            Initialized(provider);
        }
    }

    public class fc3d
    {
        public string fname { get; set; }

        public long fcode { get; set; }
    }
}
