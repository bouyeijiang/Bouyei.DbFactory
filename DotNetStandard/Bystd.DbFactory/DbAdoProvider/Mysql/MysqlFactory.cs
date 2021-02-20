using System;
using System.Data;
using System.Data.Common;

namespace Bystd.DbFactory.DbAdoProvider.Mysql
{
   internal class MysqlFactory:BaseFactory
    {
        public MysqlFactory(int timeout=1800)
            : base( FactoryType.MySql, timeout) { }

        public MysqlFactory(string ConnectionString,int timeout = 1800)
        : base(FactoryType.MySql,timeout,ConnectionString) { }

        public MysqlFactory(IDbConnection dbConnection,IDbTransaction dbTransaction, int timeout)
            : base(FactoryType.MySql,timeout, dbConnection, dbTransaction)
        { }

        public static DbProviderFactory GetFactory()
        {
            return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        }
    }
}
