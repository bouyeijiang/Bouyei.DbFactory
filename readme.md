.net 异构数据库通用访问库，支持db2、sql server、oracle、mysql、acess等多种类型的数据库操作，使用统一的简洁接口调用，并提供db2、sqlserver、oracle的百万级高性能批量入库方法bulkcopy，并提供给entity framework的封装使用模块，接口简洁简单；

ado.net访问数据库例子：
 
	string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

	IAdoProvider dbProvider = AdoProvider.CreateProvider(connectionString,ProviderType.SqlServer);
  	var adort = dbProvider.Query(new DbExecuteParameter()
 	{
  	   CommandText = "select * from [user]"
 	});

	//批量入库
 	    DataTable dt = new DataTable();
            dt.Columns.Add("uname");
            dt.Columns.Add("age");

            dt.Rows.Add(new object[] { "bouyei", 27 });
            dt.Rows.Add(new object[] { "aileenyin", 25 });
            dt.Rows.Add(new object[] { "hhhh", 13 });
            dt.TableName = "user";

            var rt = dbProvider.BulkCopy(new DbExecuteBulkParameter()
            {
                DstDataTable = dt
            });


//entity framework 使用例子：
//  <connectionStrings>
//  <add name="DbConnection" connectionString="Data Source=127.0.0.1;Initial //Catalog=dbprovider;uid=sa;pwd=123456;MultipleActiveResultSets=True" //providerName="System.Data.SqlClient"/>
//</connectionStrings>
  <appSettings>

//使用entity framework 模块必需配置：
//<appSettings>
//  <add key="mappingDLL" value="Bouyei.DbEntities.dll"/>
//</appSettings>

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
                ormProvider.Update(item,true);//true直接保存更改
                //单独保存修改
                //int rt = ormProvider.SaveChanges();