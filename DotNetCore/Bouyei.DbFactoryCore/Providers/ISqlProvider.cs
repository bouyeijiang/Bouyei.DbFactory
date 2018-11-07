using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    using DbSqlProvider.SqlKeywords;

    public interface ISqlProvider
    {
        DbType ProviderType { get; set; }

        Select Select(params string[] columns);

        Select Select<T>() where T : class;

        Insert Insert(string tableName, string[] columnNames);

        Insert Insert<T>() where T : class;

        Update Update(string tableName);

        Update Update<T>() where T : class;

        Delete Delete();
    }
}
