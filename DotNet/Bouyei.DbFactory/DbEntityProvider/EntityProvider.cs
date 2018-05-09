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

        public EntityProvider(string DbConnectionString=null)
        {
            this.DbConnectionString = DbConnectionString;
            eContext = new EntityContext(DbConnectionString);
        }

        public void DatabaseCreateOrMigrate()
        {
            eContext.CreateOrMigrateDb();
        }

        //public DbSet<TEntity> DbSet<TEntity>() where TEntity : class
        //{
        //    return eContext.DSet<TEntity>();
        //}

		public void Refresh<TEntity>(TEntity entity) where TEntity : class
		{
            eContext.Reload(entity);
		}

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)where TEntity:class
        {
           return eContext.Count(predicate);
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity:class
        {
            return eContext.Any(predicate);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {
            return eContext.Query<TEntity>();
        }
        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity,bool>> predicate) where TEntity : class
        {
            return eContext.Query(predicate);
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity,bool>>predicate) where TEntity : class
        {
            return eContext.QueryNoTracking(predicate);
        }

        public TEntity GetById<TEntity>(object id) where TEntity : class
        {
            return eContext.Set<TEntity>().Find(id);
        }

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
			return eContext.Insert<TEntity>(entity, isSaveChange);
		}

        public IEnumerable<TEntity> InsertRange<TEntity>(TEntity[] entities, bool isSaveChange = false) where TEntity:class
        {
           return eContext.InsertRange<TEntity>(entities, isSaveChange);
        }

        public long BulkCopy<TEntity>(IList<TEntity> buffer,int batchSize=10240) where TEntity : class
        {
            return eContext.BulkCopy<TEntity>(buffer,batchSize);
        }

        public void Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            eContext.Update(entity, isSaveChange);
        }

        public void Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            eContext.Delete(entity, isSaveChange);
        }

        public IEnumerable<TEntity> Delete<TEntity>(Func<TEntity, bool> predicate, bool isSaveChange = false) where TEntity : class
        {
            var items = this.eContext.Set<TEntity>().Where(predicate);
            foreach (var item in items)
            {
                eContext.Delete(item,isSaveChange);
            }

            return items;
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
           return eContext.ExecuteCommand(command, parameters);
        }

        public int ExecuteTransaction(string command,
            System.Data.IsolationLevel IsolationLevel=System.Data.IsolationLevel.Serializable, params object[] parameters)
        {
            return eContext.ExecuteTransaction(command, IsolationLevel,parameters);
        }

        public int ExecuteTransaction(string[] commands,params object[] parameters)
        {
            return eContext.ExecuteTransaction(commands, parameters);
        }

        public  List<T> Query<T>(string command, params object[] parameters)
        {
            return eContext.Query<T>(command, parameters);
        }

        public int SaveChanges()
        {
            return eContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
