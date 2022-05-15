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

        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new System.Data.SQLite.SQLiteParameter(param.ParameterName, param.Value);
            p.DbType = param.DbType;
            p.Size = param.Size;
            p.Direction = param.Direction;
            p.SourceColumn = param.SourceColumn;
            p.SourceVersion = param.SourceVersion;
            p.SourceColumnNullMapping = param.SourceColumnNullMapping;
            return p;
        }
    }
}
