using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    internal class SqliteFactory : BaseFactory, IFactory
    {
        public void Dispose()
        {

        }

        public override DbProviderFactory GetFactory()
        {
            return Microsoft.Data.Sqlite.SqliteFactory.Instance;
        }

        public int WriteToServer(DataTable dataSource, int batchSize = 10240)
        {
            throw new Exception("not support");
        }
        public int WriteToServer(Array dataSource, string tableName, int batchSize = 10240)
        {
            throw new Exception("not support");
        }
        public void WriteToServer(IDataReader reaader,string tableName,int batchSize=10240)
        {
            throw new Exception("not support");
        }

        public void ReadFromServer<T>(string tableName, Func<T, bool> action)
        {
            throw new Exception("not support");
        }
    }
}
