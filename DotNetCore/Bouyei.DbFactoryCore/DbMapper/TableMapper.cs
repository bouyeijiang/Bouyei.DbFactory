using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Bouyei.DbFactoryCore.DbMapper
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
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Insert(Dictionary<string, object> values)
        {
            var rt = dbProvider.Insert<T>(values);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Delete(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Delete(whereclause);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Update(T value, Expression<Func<T, bool>> whereclause)
        {
            Dictionary<string, object> kv = new Dictionary<string, object>();
            var t = typeof(T);
            var pros = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in pros)
            {
                object v = p.GetValue(value);
                if (v == null || string.IsNullOrEmpty(v.ToString()))
                    continue;

                kv.Add(p.Name, v);
            }

            var rt = dbProvider.Update(kv, whereclause);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int Update(Dictionary<string, object> values, Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Update<T>(values, whereclause);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual List<T> Select(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.Query(whereclause);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual List<T> Select(int page, int size, Expression<Func<T, bool>> whereclause)
        {
            if (page < 0) page = 0;
            if (size <= 0) size = 10;

            var rt = dbProvider.PageQuery(whereclause, page, size);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int SelectCount(Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.QueryCount<T>(whereclause);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

            return rt.Result;
        }

        public virtual int SelectSum(string sumColumn, Expression<Func<T, bool>> whereclause)
        {
            var rt = dbProvider.QuerySum<T>(whereclause, sumColumn);
            if (string.IsNullOrEmpty(rt.Info) == false)
                throw new Exception(rt.Info);

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
