using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Bystd.DbFactory.DbMapper
{
    public class TableMapper<T> : IDisposable where T : class
    {
        private IAdoProvider dbProvider;

        public IAdoProvider getProvider()
        {
            return this.dbProvider;
        }

        public TableMapper() { }

        public TableMapper(IAdoProvider adoProvider)
        {
            this.dbProvider = adoProvider;
        }

        protected virtual void Initialized(IAdoProvider adoProvider)
        {
            this.dbProvider = adoProvider;
        }

        public virtual int Insert(params T[] values)
        {
            var rt = dbProvider.Insert(values);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Insert(Dictionary<string, object> values)
        {
            var rt = dbProvider.Insert<T>(values);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Delete(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Delete(whereclause);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Update(T value, Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Update(value, whereclause);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Update(Dictionary<string, object> values, Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Update<T>(values, whereclause);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual List<T> Select(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Query(whereclause);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual List<T> Select(int page, int size, Expression<Func<T, bool>> whereclause)
        {
            if (page < 0) page = 0;
            if (size <= 0) size = 10;

            var rt = dbProvider.QueryPage(whereclause, page, size);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual List<T> SelectOrderBy(int page, int size, Expression<Func<T, bool>> whereclause,
            string[] orderColumNames,SortType sType = SortType.Desc)
        {
            if (page < 0) page = 0;
            if (size <= 0) size = 10;

            var rt = dbProvider.QueryOrderBy(whereclause, orderColumNames,sType,page, size);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int SelectCount<R>(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.QueryCount<T>(whereclause);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual R SelectSum<R>(string sumColumn, Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.QuerySum<T,R>(whereclause, sumColumn);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual T SelectFirst(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.QueryPage<T>(whereclause, 0, 1);
            if (rt.IsSuccess()==false)
                throw new Exception(rt.Info);

            if (rt.Result == null) return default(T);

            return rt.Result.FirstOrDefault();
        }

        /// <summary>
        /// 返回第一行第一列
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="whereclause"></param>
        /// <returns></returns>
        public virtual R SelectScalar<R>(string column,Expression<Func<T, bool>> whereclause)
        {
            ISqlProvider sql = SqlProvider.CreateProvider(dbProvider.DbType);
            var commandText = sql.Select(column).From<T>().Where(whereclause).SqlString;

            var rt = dbProvider.ExecuteScalar<R>(new Parameter(commandText));
            if (rt.IsSuccess()==false)
                rt.Info = rt.Info + "\r\n" + commandText;

            return rt.Result;
        }

        public void Dispose()
        {
            if (dbProvider != null)
            {
                dbProvider.Dispose();
            }
        }
    }
}
