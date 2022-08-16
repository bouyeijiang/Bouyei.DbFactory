using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory
{
    using DbSqlProvider.SqlKeywords;
    using DbSqlProvider.SqlFunctions;

    public class SqlProvider : ISqlProvider
    {
       public FactoryType DbType { get;  set; }

        public SqlProvider(FactoryType dbType=FactoryType.PostgreSQL)
        {
            this.DbType = dbType;
        }

        public static ISqlProvider CreateProvider(FactoryType dbType = FactoryType.PostgreSQL)
        {
            return new SqlProvider(dbType);
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
        public Select<T> Select<T, R>(Func<T, R> selector) where T : class
        {
            Select<T> _select = new Select<T>();
            _select.SqlString = _select.ToString(selector);
            return _select;
        }
        public Select<T> Select<T>(Max input) where T : class
        {
            Select<T> _select = new Select<T>(input);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>(Min input) where T : class
        {
            Select<T> _select = new Select<T>(input);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>(Avg input) where T : class
        {
            Select<T> _select = new Select<T>(input);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>(Count input) where T : class
        {
            Select<T> _select = new Select<T>(input);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Select<T> Select<T>(Sum input) where T : class
        {
            Select<T> _select = new Select<T>(input);
            _select.SqlString = _select.ToString();
            return _select;
        }

        public Insert<T> Insert<T>() where T : class
        {
            Insert<T> insert = new Insert<T>();
            insert.SqlString = insert.ToString();

            return insert;
        }
        public Insert<T> Insert<T, R>(Func<T, R> selector) where T : class
        {
            Insert<T> insert = new Insert<T>();
            insert.SqlString = insert.ToString(selector);

            return insert;
        }
        public Insert Insert(string tableName,string[] columnNames)
        {
            Insert insert = new Insert(tableName);
            insert.SqlString = insert.ToString(columnNames);

            return insert;
        }

        public Insert Insert(string tableName, Dictionary<string, object> insertKeyValues)
        {
            Insert insert = new Insert(tableName);
            insert.SqlString = insert.ToString(insertKeyValues);

            return insert;
        }

        public Insert<T> Insert<T>(Dictionary<string, object> insertKeyValues) where T : class
        {
            Insert<T> insert = new Insert<T>();
            insert.SqlString = insert.ToString(insertKeyValues);

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
