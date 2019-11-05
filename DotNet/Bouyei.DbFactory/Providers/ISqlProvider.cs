using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
{
    using DbSqlProvider.SqlKeywords;

    public interface ISqlProvider
    {
        FactoryType ProviderType { get; set; }

        Select Select(params string[] columns);

        Select<T> Select<T>() where T : class;

        Insert Insert(string tableName, string[] columnNames);

        Insert<T> Insert<T>() where T : class;

        Update Update(string tableName);

        Update<T> Update<T>() where T : class;

        Delete Delete();
    }
}
