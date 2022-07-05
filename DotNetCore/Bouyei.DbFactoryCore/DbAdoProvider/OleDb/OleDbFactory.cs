using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore.DbAdoProvider.OleDb
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
        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new System.Data.OleDb.OleDbParameter(param.ParameterName, param.Value);
            p.DbType = param.DbType;
            p.Size = param.Size;
            p.Direction = param.Direction;
            p.SourceColumn = param.SourceColumn;
            p.SourceVersion = param.SourceVersion;
            p.SourceColumnNullMapping = param.SourceColumnNullMapping;
            return p;
        }
    }
}
