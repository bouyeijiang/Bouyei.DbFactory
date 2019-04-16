using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbAdoProvider.Factories
{
    public interface IFactory:IDisposable
    {
        DbProviderFactory GetFactory();

        int WriteToServer(DataTable dataSource, int batchSize = 10240);
        int WriteToServer(Array dataSource, string tableName, int batchSize = 1024);
        void WriteToServer(IDataReader iDataReader, string tableName, int batchSize = 10240);

        void ReadFromServer<T>(string tableName, Func<T, bool> action);
    }
}
