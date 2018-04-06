using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.Providers
{
    public class SqlProvider : ISqlProvider
    {
       public ProviderType ProviderType { get; private set; }

        public SqlProvider(ProviderType providerType=ProviderType.SqlServer)
        {
            this.ProviderType = providerType;
        }

        public static ISqlProvider CreateProvider(ProviderType providerType = ProviderType.SqlServer)
        {
            return new SqlProvider(providerType);
        }
    }
}
