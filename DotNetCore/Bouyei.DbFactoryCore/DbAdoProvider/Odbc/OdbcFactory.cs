using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore.DbAdoProvider.Odbc
{
    public class OdbcFactory : BaseFactory
    {
        public OdbcFactory(int timeout = 1800)
            : base(FactoryType.MySql, timeout) { }

        public OdbcFactory(string ConnectionString, int timeout = 1800)
        : base(FactoryType.MySql, timeout, ConnectionString) { }

        public OdbcFactory(IDbConnection dbConnection, IDbTransaction dbTransaction, int timeout)
            : base(FactoryType.MySql, timeout, dbConnection, dbTransaction)
        { }

        public static DbProviderFactory GetFactory()
        {
            return System.Data.Odbc.OdbcFactory.Instance;
        }
    }
}
