﻿using System;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Bystd.DbFactory.DbAdoProvider.Oracle
{
    internal class OracleFactory : BaseFactory
    {
        public OracleFactory(string ConnectionString, int timeout = 1800)
            : base(FactoryType.Oracle, timeout, ConnectionString) { }

        public OracleFactory(IDbConnection dbConnection,IDbTransaction dbTransaction, int timeout = 1800)
             : base(FactoryType.Oracle, timeout, dbConnection, dbTransaction) { }

        public static  DbProviderFactory GetFactory()
        {
            return OracleClientFactory.Instance;
        }
        public static DbParameter GetParameter(CmdParameter param)
        {
            OracleParameter p = new OracleParameter(param.ParameterName, param.Value);
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
