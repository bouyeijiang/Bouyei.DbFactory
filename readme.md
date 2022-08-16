#### 模块说明
---

#Bouyei.DbFactory 基于.net framework 4.6.1+

#Bouyei.DbFactoryCore基于.net core2.2+

#Bystd.DbFactory 基于.net standard 2.0+

#### Package
---

Package  | NuGet 
-------- | :------------ 
Bouyei.DbFactory | [![NuGet](https://img.shields.io/nuget/v/Bouyei.DbFactory.svg)](https://www.nuget.org/packages/Bouyei.DbFactory)
Bouyei.DbFactoryCore | [![NuGet](https://img.shields.io/nuget/v/Bouyei.DbFactoryCore.svg)](https://www.nuget.org/packages/Bouyei.DbFactoryCore)
Bystd.DbFactory	| [![NuGet](https://img.shields.io/nuget/v/Bystd.DbFactory.svg)](https://www.nuget.org/packages/Bystd.DbFactory)

#### Ado基本例子
---
 
	string connectionString = "Data Source=.;Initial Catalog=testdb;User ID=sa;Password=bouyei;";

	IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

	var rt = adoProvider.Query(new Parameter()
	{
		CommandText = "select * from MemUser"
	});

	//删除
	var del= adoProvider.Delete<user>(x => x.uname =="hkj" && (x.sex==Sex.Female ||x.uage==30));

	       //动态对象插入
           var irt= dbProvider.Insert<User>(x => new
             {
             uname="hello",
             id=11
           });

            //动态对象修改
         var urt = dbProvider.Update<User>(x => new
         {
            uname = "bouyei_hello"
         }, w => w.id == 11);

    //DbParameter 方式插入,字段为二进制blob类型等或其他情况使用
     var p= dbProvider.InsertParameter<User>(new User()
        {
            uname = "bouyei",
            uage = 33,
            score = 23.44f
        });

	//查询 like '%bouyei%'
	var users = adoProvider.Query<user>(x =>x.name.Contains("bouyei"));

    //分页查询,动态对象返回
	string[] orderbyColumn = new string[] { "uname" };
        var qrt = dbProvider.QueryOrderBy<User>(x => true,
        c => new { c.uname, c.id },/*动态列名返回*/
        orderbyColumn, SortType.Desc, 0, 10);

	foreach (DataRow dr in rt.Result.Rows)
	{
		Console.WriteLine(string.Join(",", dr.ItemArray));
	}

    //大批量数据入库
    fc3d[] fc = new fc3d[] {
        new fc3d(){ fname="dd", fcode=121 },
        new fc3d(){fname="sd",fcode=23 },
        new fc3d(){ fname="个",fcode=2323}
    };

    var param = new CopyParameter<Array>(fc);
    param.TableName = "fc3d";
    var rt = adoProvider.BulkCopy(param);

#### 表实体映射继承例子
---

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

#### 表达式生成SQL例子
---

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