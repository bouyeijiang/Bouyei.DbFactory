using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    using DbSqlProvider.SqlKeywords;

    public class SqlProvider : ISqlProvider
    {
       public FactoryType ProviderType { get;  set; }

        public SqlProvider(FactoryType providerType=FactoryType.SqlServer)
        {
            this.ProviderType = providerType;
        }

        public static ISqlProvider CreateProvider(FactoryType providerType = FactoryType.SqlServer)
        {
            return new SqlProvider(providerType);
        }

        public Select Select(params string[] columns)
        {
            Select _select = new Select(columns);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>() where T : class
        {
            Select<T> _select = new Select<T>();
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Insert<T> Insert<T>() where T : class
        {
            Insert<T> insert = new Insert<T>();
            insert.SqlString = insert.ToString();

            return insert;
        }

        public Insert Insert(string tableName,string[] columnNames)
        {
            Insert insert = new Insert(tableName);
            insert.SqlString = insert.ToString(columnNames);

            return insert;
        }

        public Update Update(string tableName)
        {
            Update up = new Update();
            up.SqlString = up.ToString(tableName);
            return up;
        }

        public Update<T> Update<T>() where T:class
        {
            Update<T> up = new Update<T>();
            up.SqlString = up.ToString();
            return up;
        }

        public Delete Delete()
        {
            Delete delete = new Delete();
            delete.SqlString = delete.ToString();

            return delete;
        }
    }
}
