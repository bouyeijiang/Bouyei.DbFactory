#Bouyei.DbFactory 基于.net framework 4.6.1+

#Bouyei.DbFactoryCore基于.net core2.2+

#Bystd.DbFactory 基于.net standard 2.0+

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
	var users = adoProvider.Query<user>(x => 1 == 1);

    //分页查询
	var users = adoProvider.PageQuery<user>(x => 1 == 1,0,10);

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
        ISqlProvider sqlProvider = SqlProvider.CreateProvider();

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
            .Set(new User() { name = "bouyei" })
            .Where<User>(x => x.id == 1 || (x.name == "b" && x.no == 2)).SqlString;

        //删除
        sql = sqlProvider.Delete()
            .From<User>().Where(x => x.name == "bouyei").SqlString;

        //插入
        sql = sqlProvider.Insert<User>()
            .Values(new User[] {
            new User() { name ="hello", age=12 }
            ,new User() { name="bouyei",age=23} }).SqlString;