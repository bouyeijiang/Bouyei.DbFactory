using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bouyei.DbFactoryCore.DbEntityProvider
{
    public class EntityProvider:IDisposable,IEntityProvider
    {
        private EntityContext eContext=null;

        public string DbConnectionString { get; set; }

        public FactoryType DbType { get; set; }

        private object lobject = new object();

        public EntityProvider(FactoryType dbType=FactoryType.SqlServer,string DbConnectionString=null)
        {
            lock (lobject)
            {
                if (DbConnectionString != this.DbConnectionString)
                    this.DbConnectionString = DbConnectionString;

                Dispose(true);

                this.DbType = dbType;
                eContext = new EntityContext(dbType, DbConnectionString);
            }
        }

        public void DatabaseCreateOrMigrate()
        {
            lock (lobject)
            {
                eContext.DbMigrate();
            }
        }

		public void Refresh<TEntity>(TEntity entity) where TEntity : class
		{
            lock (lobject)
            {
                eContext.Reload(entity);
            }
		}

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)where TEntity:class
        {
            lock (lobject)
            {
                return eContext.Count(predicate);
            }
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class
        {
            lock (lobject)
            {
                return eContext.Any(predicate);
            }
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            lock (lobject)
            {
                return eContext.Table<TEntity>();
            }
        }
        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity,bool>> predicate) where TEntity : class
        {
            lock (lobject)
            {
                return eContext.Query(predicate);
            }
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity,bool>>predicate) where TEntity : class
        {
            lock (lobject)
            {
                return eContext.QueryNoTracking(predicate);
            }
        }

        public TEntity GetById<TEntity>(params object[] keys) where TEntity : class
        {
            lock (lobject)
            {
                return eContext.GetById<TEntity>(keys);
            }
        }

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobject)
            {
                return eContext.Insert<TEntity>(entity, isSaveChange);
            }
		}

        public IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities, bool isSaveChange = false) where TEntity:class
        {
            lock (lobject)
            {
                return eContext.InsertRange<TEntity>(entities, isSaveChange);
            }
        }

        public long BulkCopy<TEntity>(IList<TEntity> buffer,int batchSize=10240) where TEntity : class
        {
            lock (lobject)
            {
                return eContext.BulkCopy<TEntity>(buffer, batchSize);
            }
        }

        public void Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobject)
            {
                eContext.Update(entity, isSaveChange);
            }
        }

        public void Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobject)
            {
                eContext.Delete(entity, isSaveChange);
            }
        }

        public IEnumerable<TEntity> Delete<TEntity>(Func<TEntity, bool> predicate, bool isSaveChange = false) where TEntity : class
        {
            lock (lobject)
            {
                var items = this.eContext.Set<TEntity>().Where(predicate);
                foreach (var item in items)
                {
                    eContext.Delete(item, isSaveChange);
                }

                return items;
            }
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            lock (lobject)
            {
                return eContext.ExecuteCommand(command, parameters);
            }
        }

        public int ExecuteTransaction(string command,
            System.Data.IsolationLevel IsolationLevel=System.Data.IsolationLevel.Serializable, params object[] parameters)
        {
            lock (lobject)
            {
                return eContext.ExecuteTransaction(command, IsolationLevel, parameters);
            }
        }

        public int ExecuteTransaction(string[] commands,params object[] parameters)
        {
            lock (lobject)
            {
                return eContext.ExecuteTransaction(commands, parameters);
            }
        }

        public IQueryable<T> Query<T>(string command, params object[] parameters) where T : class
        {
            lock (lobject)
            {
                return eContext.Query<T>(command, parameters);
            }
        }

        public int SaveChanges()
        {
            lock (lobject)
            {
                return eContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            lock (lobject)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        ~EntityProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.eContext != null)
                {
                    this.eContext.Dispose();
                    this.eContext = null;
                }
            }
        }
    }
}
