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
        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new Npgsql.NpgsqlParameter(param.ParameterName, param.Value);
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
