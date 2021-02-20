.net 异构数据库通用访问库，支持db2、sql server、oracle、mysql、acess等多种类型的数据库操作，使用统一的简洁接口调用，并提供db2、sqlserver、oracle的百万级高性能批量入库方法bulkcopy，并提供给entity framework的封装使用模块，接口简洁简单；

Install-Package Bouyei.DbFactory -Version 1.1.6601.28462

#1、Ado使用例子
 
	string connectionString = "Data Source=.;Initial Catalog=testdb;User ID=sa;Password=bouyei;";

	IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

	var rt = adoProvider.Query(new Parameter()
	{
		CommandText = "select * from MemUser"
	});

	//删除
	var del= adoProvider.Delete<user>(x => x.name == "hello");

	//插入
	  var insert = adoProvider.Insert<user>(new user() {
                 name="bouyei",
                 age=30
            });

	//查询
	var users = adoProvider.Query<user>(x => 1 == 1);

	foreach (DataRow dr in rt.Result.Rows)
	{
		Console.WriteLine(string.Join(",", dr.ItemArray));
	}


#2、EF使用例子


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


#3、数据库表同步

            List<SyncTableSchema> tableSchema = new List<SyncTableSchema>();
            tableSchema.Add(new SyncTableSchema()
            {
                TableName = "user",
                Columns = new List<SyncColumnName>() {
                    new SyncColumnName("name"){ DataType="nvarchar",Size=50},
                    new SyncColumnName("id"){ DataType="int", IsPrimaryKey= true, IncrementStart=1, IncrementStep=1,Size=4},
                    new  SyncColumnName("no"){ DataType="int",Size=4},
                    new SyncColumnName("age"){DataType="int",Size=4 }
                }
            });

            string sourceConnString = "Server=127.0.0.1;Database=A;User Id=sa;Password=bouyei;";
            string targetConnString = "Server=127.0.0.1;Database=B;User Id=sa;Password=bouyei;";
            dbSyncProvider = DbSyncProvider.CreateProvider(sourceConnString, targetConnString,
                "ScopeName", tableSchema);

            //清空同步记录设置 需要重新初始化设置
            dbSyncProvider.DeprovisionScope();

            //重设同步记录设置 初次使用需要初始化
            // dbSyncProvider.ProvisionScope(null);

            dbSyncProvider.ProvisionScope(new List<SyncFilterSchema>() {
                 new SyncFilterSchema(){
                      FilterColumns=new List<string>(){"[age]"},
                      FilterClause="[side].[age]>20"
                 }
            });

            var rt = dbSyncProvider.ExecuteSync(new SyncParameter()
            {
                Direction = SyncDirectionType.Upload,
            });

