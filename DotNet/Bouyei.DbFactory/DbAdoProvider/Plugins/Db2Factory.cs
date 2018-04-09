using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Plugins
{
    internal class Db2Factory
    {
        public Db2Factory() { }

        public DbProviderFactory GetFactory()
        {
            return IBM.Data.DB2.DB2Factory.Instance;
        }
    }
}
