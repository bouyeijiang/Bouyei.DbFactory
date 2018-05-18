using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Factories
{
    public class BaseFactory
    {
        public string ConnectionString { get; protected set; }

        public int ExecuteTimeout { get; protected set; }

       public BulkCopiedArgs BulkCopiedHandler { get; set; }

        public virtual DbProviderFactory GetFactory()
        {
            return null;
        }
    }
}
