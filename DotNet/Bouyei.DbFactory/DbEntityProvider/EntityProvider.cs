using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

//using System.Collections.Generic;
//using System.Text;

namespace Bouyei.DbFactory.DbEntityProvider
{
    public class EntityProvider:IDisposable,IEntityProvider
    {
        private EntityContext eContext=null;

        public string DbConnectionString { get; set; }

        private object lobjcct = new object();

        public EntityProvider(string NameOrDbConnectionString = null)
        {
            lock (lobjcct)
            {
                //if (DbConnectionString != this.DbConnectionString)
                //    this.DbConnectionString = DbConnectionString;

                Dispose(true);

                eContext = new EntityContext(NameOrDbConnectionString);

                DbConnectionString = eContext.ConnectionString;
            }
        }

        public void DatabaseCreateOrMigrate()
        {
            lock (lobjcct)
            {
                eContext.CreateOrMigrateDb();
            }
        }

        //public DbSet<TEntity> DbSet<TEntity>() where TEntity : class
        //{
        //    return eContext.DSet<TEntity>();
        //}

		public void Refresh<TEntity>(TEntity entity) where TEntity : class
		{
            lock (lobjcct)
            {
                eContext.Reload(entity);
            }
		}

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)where TEntity:class
        {
            lock (lobjcct)
            {
                return eContext.Count(predicate);
            }
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class
        {
            lock (lobjcct)
            {
                return eContext.Any(predicate);
            }
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.Table<TEntity>();
            }
        }
        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity,bool>> predicate) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.Query(predicate);
            }
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity,bool>>predicate) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.QueryNoTracking(predicate);
            }
        }

        public TEntity GetById<TEntity>(params object[] keys) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.GetById<TEntity>(keys);
            }
        }

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.Insert<TEntity>(entity, isSaveChange);
            }
		}

        public IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities, bool isSaveChange = false) where TEntity:class
        {
            lock (lobjcct)
            {
                return eContext.InsertRange<TEntity>(entities, isSaveChange);
            }
        }

        public long BulkCopy<TEntity>(IList<TEntity> buffer,int batchSize=10240) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.BulkCopy<TEntity>(buffer, batchSize);
            }
        }

        public void Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobjcct)
            {
                eContext.Update(entity, isSaveChange);
            }
        }

        public void Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            lock (lobjcct)
            {
                eContext.Delete(entity, isSaveChange);
            }
        }

        public int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isSaveChange = false) where TEntity : class
        {
            lock (lobjcct)
            {
                return eContext.Delete<TEntity>(predicate, isSaveChange);
            }
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            lock (lobjcct)
            {
                return eContext.ExecuteCommand(command, parameters);
            }
        }

        public int ExecuteTransaction(string command,
            System.Data.IsolationLevel IsolationLevel=System.Data.IsolationLevel.Serializable, params object[] parameters)
        {
            lock (lobjcct)
            {
                return eContext.ExecuteTransaction(command, IsolationLevel, parameters);
            }
        }

        public int ExecuteTransaction(string[] commands,params object[] parameters)
        {
            lock (lobjcct)
            {
                return eContext.ExecuteTransaction(commands, parameters);
            }
        }

        public  List<T> Query<T>(string command, params object[] parameters)
        {
            lock (lobjcct)
            {
                return eContext.Query<T>(command, parameters);
            }
        }

        public int SaveChanges()
        {
            lock (lobjcct)
            {
                return eContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            lock (lobjcct)
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
