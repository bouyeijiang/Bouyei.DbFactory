using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory.DbAdoProvider.OleDb
{
    public class OleDbFactory : BaseFactory
    {
        public OleDbFactory(int timeout = 1800)
            : base(FactoryType.MySql, timeout) { }

        public OleDbFactory(string ConnectionString, int timeout = 1800)
        : base(FactoryType.MySql, timeout, ConnectionString) { }

        public OleDbFactory(IDbConnection dbConnection, IDbTransaction dbTransaction, int timeout)
            : base(FactoryType.MySql, timeout, dbConnection, dbTransaction)
        { }

        public static DbProviderFactory GetFactory()
        {
            return System.Data.OleDb.OleDbFactory.Instance;
        }
    }
}
