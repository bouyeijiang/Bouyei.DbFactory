using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Postgresql
{
   internal class NpgFactory:BaseFactory
    {
       public NpgFactory(int timeout = 1800)
            : base(FactoryType.PostgreSQL, timeout) { }

        public NpgFactory(string ConnectionString,int timeout = 1800)
            : base(FactoryType.PostgreSQL, timeout,ConnectionString) { }

        public NpgFactory(IDbConnection dbConnection,IDbTransaction dbTransaction, int timeout = 1800)
            : base(FactoryType.PostgreSQL, timeout,dbConnection,dbTransaction) { }

        public static DbProviderFactory GetFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }
    }
}
