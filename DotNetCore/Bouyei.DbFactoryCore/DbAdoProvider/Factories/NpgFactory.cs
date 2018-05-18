using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    using Bulkcopies;

   internal class NpgFactory:NpgBulk,IFactory
    {
        public NpgFactory() { }

       public NpgFactory(string ConnectionString, int timeout = 1800)
            : base(ConnectionString, timeout) { }

        public override DbProviderFactory GetFactory()
        {
            return Npgsql.NpgsqlFactory.Instance;
        }
    }
}
