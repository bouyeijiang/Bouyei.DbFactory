﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory
{
    using DbSqlProvider.SqlKeywords;
    using DbSqlProvider.SqlFunctions;

    public interface ISqlProvider
    {
        FactoryType DbType { get; set; }

        Select Select(params string[] columns);

        Select<T> Select<T>() where T : class;
        Select<T> Select<T>(Max input) where T : class;

        Select<T> Select<T>(Min input) where T : class;

        Select<T> Select<T>(Avg input) where T : class;

        Select<T> Select<T>(Count input) where T : class;

        Select<T> Select<T>(Sum input) where T : class;

        Insert Insert(string tableName, string[] columnNames);

        Insert<T> Insert<T>() where T : class;
        Insert<T> Insert<T>(Dictionary<string, object> columns) where T : class;

        Update Update(string tableName);

        Update<T> Update<T>() where T : class;

        Delete Delete();
    }
}