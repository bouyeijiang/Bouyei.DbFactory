#Bouyei.DbFactory 基于.net framework 4.6.1+

#Bouyei.DbFactoryCore基于.net core2.2+

#Bystd.DbFactory 基于.net standard 2.0+

# Package
---

Package  | NuGet 
-------- | :------------ 
Bouyei.DbFactory		| [![NuGet](https://img.shields.io/nuget/v/Bouyei.DbFactory.svg)](https://www.nuget.org/packages/Bouyei.DbFactory)
Bouyei.DbFactoryCore		| [![NuGet](https://img.shields.io/nuget/v/Bouyei.DbFactoryCore.svg)](https://www.nuget.org/packages/Bouyei.DbFactoryCore)
Bystd.DbFactory	| [![NuGet](https://img.shields.io/nuget/v/Bystd.DbFactory.svg)](https://www.nuget.org/packages/Bystd.DbFactory)

#1、Ado使用例子
 
	string connectionString = "Data Source=.;Initial Catalog=testdb;User ID=sa;Password=bouyei;";

	IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

	var rt = adoProvider.Query(new Parameter()
	{
		CommandText = "select * from MemUser"
	});

	//删除
	var del= adoProvider.Delete<user>(x => x.uname =="hkj" && (x.sex==Sex.Female ||x.uage==30));

	//插入
	  var insert = adoProvider.Insert<user>(new user() {
                 name="bouyei",
                 age=30
            });

	//查询
	var users = adoProvider.Query<user>(x =>x.name.Container);

    //分页查询
	var users = adoProvider.QueryPage<user>(x => 1 == 1,0,10);

	foreach (DataRow dr in rt.Result.Rows)
	{
		Console.WriteLine(string.Join(",", dr.ItemArray));
	}

#2、表实体映射例子

    //使用例子1
    private void execute()
    {
        UserDto user = new UserDto()
        {
            UserName = "bouyei"
        };
        user.Insert(user);
    }

    //使用例子2
    public class DbMapperService:BaseEntity<UserDto>
    {
        public int Insert(UserDto user)
        {
           return Insert(user);
        }

        public int UpdateDo(UserDto dto,Expression<Func<UserDto,bool>> whereClause)
        {
            return Update(dto, whereClause);
        }
    }

    [MappedName("db_user")]
    public class UserDto:BaseEntity<UserDto>
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
            Initilize(provider);
        }
    }

#基于EF的ORM需要再配置文件加相应实体映射dll路径，详细看demo代码例子

	IOrmProvider ormProvider = OrmProvider.CreateProvider(ProviderType.SqlServer, connectionString);

	var items= ormProvider.Query<DbEntity.User>("select * from MemUser").ToList();

	foreach(var item in items)
	{
		Console.WriteLine(item.uName);
	}
	Console.ReadKey();

#sql表达式生成例子

	//生成简单查询脚本
        //group by 
            string sqlgroupby = sqlProvider.Select<User>().Count().From<User>()
                .Where(x => x.uage == 1).GroupBy<User>().SqlString;

            //in 语法
            string[] values = new string[] { "a","b"};
            var inSql = sqlProvider.Select<User>().From().Where(x => values.Contains(x.uname)).SqlString;

            //like 语法 '%bouyei%'
            var likeSql = sqlProvider.Select<User>().From().Where(x => x.uname.Contains("bouyei")).SqlString;

            //like 语法'bouyei%'
            var beginSql = sqlProvider.Select<User>().From().Where(x => x.uname.StartsWith("bouyei") || x.uname.StartsWith("bb")).SqlString;

            //like 语法'%bouyei'
            var endSql = sqlProvider.Select<User>().From().Where(x => x.uname.EndsWith("bouyei")).SqlString;

            //select count(*) from user where id=1
            string commandText = sqlProvider.Select<User>(new Count("*")).From<User>().Where(x=>x.id==1).SqlString;

            //function 
            string sqlfun = sqlProvider.Select<User>(new Max("age")).From<User>().Where(x=>x.uage>20).SqlString;

            //order by
            var osql = sqlProvider.Select<User>().From<User>().OrderBy(SortType.Asc, "name").SqlString;