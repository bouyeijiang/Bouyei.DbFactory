using System;
using System.Data;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    public interface ICopyFactory
    {
        int WriteToServer(DataTable dataSource, int batchSize = 10240);
        int WriteToServer(Array dataSource, string tableName, int batchSize = 1024);
        void WriteToServer(IDataReader iDataReader, string tableName, int batchSize = 10240);
        void ReadFromServer<T>(string tableName, Func<T, bool> action);
    }
}
