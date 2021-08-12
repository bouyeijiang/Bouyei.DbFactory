using Bouyei.DbFactoryCore.DbSqlProvider.SqlFunctions;
using Bouyei.DbFactoryCore.DbSqlProvider.SqlKeywords;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Bouyei.DbFactoryCore
{
    public static class AdoProviderExtensions
    {
        public static DbResult<List<T>, string> Query<T>(this IAdoProvider dbProvider,
            Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Select<T>().From<T>().Where<T>(predicate).SqlString;

            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<List<T>, string> Query<T>(this IAdoProvider dbProvider,
       string[] selectColumns, Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);

            var commandText = sql.Select(selectColumns).From<T>().Where(predicate).SqlString;
            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\r\n" + commandText;

            return rt;
        }

        public static DbResult<List<T>, string> Query<T>(this IAdoProvider dbProvider,
        string[] selectColumns, string tableName, Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);

            var commandText = sql.Select(selectColumns).From(tableName).Where(predicate).SqlString;
            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\r\n" + commandText;

            return rt;
        }

        public static DbResult<List<T>, string> QueryPage<T>(this IAdoProvider dbProvider,
           Expression<Func<T, bool>> predicate, int page = 0, int size = 1) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            int offset = page * size;
            string commandText = sql.Select<T>().From<T>().Where<T>(predicate)
                .Top<T>(dbProvider.FactoryType, offset, size).SqlString;

            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<List<T>, string> QueryOrderBy<T>(this IAdoProvider dbProvider,
        Expression<Func<T, bool>> predicate,string[] orderColumnNames,
        SortType sType = SortType.Desc, int page = 0, int size = 1) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            int offset = page * size;

            string commandText = sql.Select<T>().From<T>()
                .Where<T>(predicate).OrderBy<T>(sType, orderColumnNames).Top<T>(dbProvider.FactoryType, offset, size).SqlString;

            var rt = dbProvider.Query<T>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<int, string> QueryCount<T>(this IAdoProvider dbProvider,
        Expression<Func<T, bool>> predicate, string countColumn = "*") where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            string commandText = sql.Select<T>(new Count(countColumn)).From<T>().Where(predicate).SqlString;

            var rt = dbProvider.ExecuteScalar<int>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<R, string> QueryMax<T,R>(this IAdoProvider dbProvider,
        Expression<Func<T, bool>> predicate, string maxColumn) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            string commandText = sql.Select<T>(new Max(maxColumn)).From<T>().Where(predicate).SqlString;

            var rt = dbProvider.ExecuteScalar<R>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<R, string> QuerySum<T,R>(this IAdoProvider dbProvider,
        Expression<Func<T, bool>> predicate, string sumColumn) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            string commandText = sql.Select<T>(new Sum(sumColumn)).From<T>().Where(predicate).SqlString;

            var rt = dbProvider.ExecuteScalar<R>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }


        public static DbResult<int, string> Delete<T>(this IAdoProvider dbProvider
            , Expression<Func<T, bool>> predicate)
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Delete().From<T>().Where<T>(predicate).SqlString;

            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<int, string> Update<T>(this IAdoProvider dbProvider,
            T value, Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Update<T>().Set<T>(value).Where<T>(predicate).SqlString;

            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<int, string> Update<T>(this IAdoProvider dbProvider,
       Dictionary<string, object> setKeyValues, Expression<Func<T, bool>> predicate) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Update<T>().Set(setKeyValues).Where(predicate).SqlString;

            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\r\n" + commandText;

            return rt;
        }

        public static DbResult<int, string> Insert<T>(this IAdoProvider dbProvider, params T[] value) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Insert<T>().Values<T>(value).SqlString;
            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }

        public static DbResult<int, string> InsertParameter<T>(this IAdoProvider dbProvider, T value) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Insert<T>().SqlString;

            var wb = new WordsBase(typeof(T), AttributeType.IgnoreWrite);
            var pros = wb.GetProperties().ToArray();

            string valueStr = string.Empty;
            string flag = "@";

            if (dbProvider.FactoryType == FactoryType.Oracle)
            {
                flag = ":";
            }
            else if (dbProvider.FactoryType == FactoryType.MySql)
            {
                flag = "?";
            }
            else if (dbProvider.FactoryType == FactoryType.DB2)
            {
                flag = "";
            }

            valueStr = string.Join(",", pros.Select(x => flag + x.Name));

            commandText = commandText + " Values(" + valueStr + ")";

            int cnt = pros.Length;
            var param = new Parameter(commandText);
            param.cmdParameters = new CmdParameter[cnt];
            for (int i = 0; i < cnt; ++i)
            {
                var pro = pros[i];
                var v = pro.GetValue(value, null);

                param.cmdParameters[i] = new CmdParameter()
                {
                    DbType = (DbType)Enum.Parse(typeof(DbType), pro.PropertyType.Name),
                    Value =v??DBNull.Value,
                    ParameterName = flag + pro.Name
                };
            }

            var rt = dbProvider.ExecuteCmd(param);

            if (rt.IsSuccess() == false)
                rt.Info = rt.Info + "\n\r" + commandText;

            return rt;
        }


        public static DbResult<int, string> Insert<T>(this IAdoProvider dbProvider,
           Dictionary<string, object> insertKeyValues) where T : class
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.FactoryType);
            var commandText = sql.Insert<T>(insertKeyValues).Values(insertKeyValues).SqlString;

            var rt = dbProvider.ExecuteCmd(new Parameter(commandText));

            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\r\n" + commandText;

            return rt;
        }
    }
}
