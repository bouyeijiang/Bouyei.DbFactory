####1、使用例子
 
	string connectionString = "Data Source=.;Initial Catalog=testdb;User ID=sa;Password=bouyei;";

	IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

	var rt = adoProvider.Query(new Parameter()
	{
		CommandText = "select * from MemUser"
	});

	foreach (DataRow dr in rt.Result.Rows)
	{
		Console.WriteLine(string.Join(",", dr.ItemArray));
	}

#基于EF的ORM需要再配置文件加相应实体映射dlll路径，详细看demo代码例子

	IOrmProvider ormProvider = OrmProvider.CreateProvider(ProviderType.SqlServer, connectionString);

	var items= ormProvider.Query<DbEntity.User>("select * from MemUser").ToList();

	foreach(var item in items)
	{
		Console.WriteLine(item.uName);
	}
	Console.ReadKey();