using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    using Bulkcopies;

   internal class MysqlFactory:MysqlBulk,IFactory
    {
        public MysqlFactory() { }

        public MysqlFactory(string ConnectionString,int timeout=1800)
            : base(ConnectionString, timeout) { }

        public override DbProviderFactory GetFactory()
        {
            return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        }
    }
}
