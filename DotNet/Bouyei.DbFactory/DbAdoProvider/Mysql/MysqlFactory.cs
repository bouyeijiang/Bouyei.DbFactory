using System;
using System.Data.Common;

namespace Bouyei.DbFactory.DbAdoProvider.Mysql
{
   internal class MysqlFactory:BaseFactory
    {
        public MysqlFactory(int timeout=1800)
            : base( FactoryType.MySql, timeout) { }

        public MysqlFactory(string ConnectionString,int timeout = 1800)
        : base(FactoryType.MySql,timeout,ConnectionString) { }


        public static DbProviderFactory GetFactory()
        {
            return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
        }

        public static DbParameter GetParameter(CmdParameter param)
        {
            var p = new MySql.Data.MySqlClient.MySqlParameter(param.ParameterName, param.Value);
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
