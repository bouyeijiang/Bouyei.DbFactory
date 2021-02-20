using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
//using System.Text;

namespace Bouyei.DbFactoryCore.DbEntityProvider
{
    public interface IEntityProvider:IDisposable
    {
        void DatabaseCreateOrMigrate();
        string DbConnectionString { get; set; }

        FactoryType DbType { get; set; }

        IQueryable<TEntity> Table<TEntity>() where TEntity : class;

        IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        //DbSet<TEntity> DbSet<TEntity>() where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class;

        void Refresh<TEntity>(TEntity entity) where TEntity : class;

        TEntity GetById<TEntity>(params object[] keys) where TEntity : class;

        TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class;

        IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities,bool isSaveChange=false) where TEntity : class;

        long BulkCopy<TEntity>(IList<TEntity> collection, int batchSize = 10240) where TEntity : class;

        void Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class;

        void Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class;

        IEnumerable<TEntity> Delete<TEntity>(Func<TEntity, bool> predicate, bool isSaveChange = false) where TEntity : class;

        int ExecuteCommand(string command, params object[] parameters);

        int ExecuteTransaction(string command,
            System.Data.IsolationLevel IsolationLevel=System.Data.IsolationLevel.Serializable, params object[] parameters);

        int ExecuteTransaction(string[] commands, params object[] parameters);

        IQueryable<T> Query<T>(string command, params object[] parameters) where T : class;

        int SaveChanges();

    }
}
