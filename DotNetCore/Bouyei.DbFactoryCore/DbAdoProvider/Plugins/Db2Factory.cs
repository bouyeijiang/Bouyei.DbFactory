using System;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Plugins
{
   internal class Db2Factory
    {
        public DbProviderFactory GetFactory()
        {
            return IBM.Data.DB2.Core.DB2Factory.Instance;
        }
    }
}
