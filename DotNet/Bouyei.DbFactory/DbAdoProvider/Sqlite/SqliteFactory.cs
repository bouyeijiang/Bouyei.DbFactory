using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Sqlite
{
    internal class SqliteFactory : BaseFactory
    {
        public SqliteFactory(int timeout):
            base(FactoryType.SQLite,timeout)
        { }

        public SqliteFactory(string ConnectionString,int timeout) :
           base(FactoryType.SQLite, timeout,ConnectionString)
        { }

        public SqliteFactory(IDbConnection dbConnection, IDbTransaction dbTransaction, int timeout)
            :base(FactoryType.SQLite, timeout, dbConnection, dbTransaction)
        { }

        public static DbProviderFactory GetFactory()
        {
            return System.Data.SQLite.SQLiteFactory.Instance;
        }
    }
}
