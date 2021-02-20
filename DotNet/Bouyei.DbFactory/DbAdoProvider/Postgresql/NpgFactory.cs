using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Postgresql
{
   internal class NpgFactory:BaseFactory
    {
       public NpgFactory(int timeout = 1800)
            : base(FactoryType.PostgreSQL, timeout) { }

        public NpgFactory(string ConnectionString,int timeout = 1800)
            : base(FactoryType.PostgreSQL, timeout,ConnectionString) { }

        public static DbProviderFactory GetFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }
    }
}
