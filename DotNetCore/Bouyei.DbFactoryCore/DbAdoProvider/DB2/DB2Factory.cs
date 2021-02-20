using System;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbAdoProvider.DB2
{

    internal class Db2Factory:BaseFactory,IBaseFactory
    {
        public Db2Factory(int timeout = 1800)
            : base( FactoryType.DB2, timeout)
        {

        }

        public Db2Factory(string ConnectionString,int timeout):
            base(FactoryType.DB2,timeout,ConnectionString)
        { }

        public Db2Factory(IDbConnection dbConnection,IDbTransaction dbTransaction, int timeout = 1800)
            : base(FactoryType.DB2, timeout,dbConnection,dbTransaction)
        {

        }

        public static  DbProviderFactory GetFactory()
        {
            return IBM.Data.DB2.Core.DB2Factory.Instance;
        }
    }
}
