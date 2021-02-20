using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Mysql
{
   internal class MysqlFactory:BaseFactory
    {
        public MysqlFactory(int timeout=1800)
            : base( FactoryType.MySql, timeout) { }

        public MysqlFactory(string ConnectionString,int timeout = 1800)
        : base(FactoryType.MySql,timeout,ConnectionString) { }

        public static DbProviderFactory GetFactory()
        {
            return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        }
    }
}
