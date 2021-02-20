using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Configuration;
using System.Data.SqlClient;
//using System.Data.Common;
//using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity;

namespace Bouyei.DbFactory.DbEntityProvider
{
    using DbUtils;
    using System.Data.Entity.ModelConfiguration;

    //using System.Data.Entity.ModelConfiguration;

    internal class EntityContext : DbContext, IDisposable
    { 
        public string ConnectionString { get { return base.Database.Connection.ConnectionString; } }

        public EntityContext(string NameOrConnectionString = null)
            : base(string.IsNullOrEmpty(NameOrConnectionString) ? "Name=DbConnection" : NameOrConnectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.Initialize(false);
        }

        public void CreateOrMigrateDb()
        {
            DbInitialize init = new DbInitialize(this.Database.Connection.ConnectionString);
            init.InitializeDatabase(this);

            Database.SetInitializer(init);
        }

        public void Reload<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).Reload();
        }

        #region public

        //public DbSet<TEntity> Sets<TEntity>() where TEntity : class
        //{
        //    return Set<TEntity>();
        //}

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)where TEntity:class
        {
            return Set<TEntity>().Count(predicate);
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Any(predicate);
        }

        public IQueryable<TEntity> Table<TEntity>() where TEntity : class
        {
            return Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Where(predicate);
        }

        public TEntity GetById<TEntity>(params object[] keys) where TEntity:class
        {
            return Set<TEntity>().Find(keys);
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Where(predicate).AsNoTracking();
        }

        public TEntity Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            //if (EnsureChange<TEntity>(entity) > 0)
            //{
            Set<TEntity>().Attach(entity);
            this.Entry(entity).State = EntityState.Modified;

            if (isSaveChange)
            {
                int rt = SaveChanges();
                if (rt > 0) return entity;
                else return default(TEntity);
            }
            //}
            return default(TEntity);
        }

        public TEntity Delete<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            this.Set<TEntity>().Attach(entity);
            Set<TEntity>().Remove(entity);
            this.Entry<TEntity>(entity).State = EntityState.Deleted;
            if (isSaveChange)
            {
                int rt = SaveChanges();
                if (rt > 0) return entity;
                else return default(TEntity);
            }
            return entity;
        }

        public int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isSaveChange = false) where TEntity : class
        {
            var items = Set<TEntity>().Where(predicate);
            int c = 0;
            foreach (var item in items)
            {
                Delete(item);
                ++c;
            }

            if (isSaveChange)
            {
                if (c > 0) return SaveChanges();
                else return c;
            }
            else return c;
        }

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange=false) where TEntity : class
        {
            TEntity rentity = Set<TEntity>().Add(entity);
            this.Entry<TEntity>(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                int rt = SaveChanges();
                if (rt > 0) return entity;
                else return default(TEntity);
            }
            return rentity;
        }

        public IEnumerable<TEntity> InsertRange<TEntity>(IEnumerable<TEntity> entities, bool isSaveChange = false) where TEntity : class
        {
            var items = Set<TEntity>().AddRange(entities);
            if (isSaveChange)
            {
                int rt = SaveChanges();
                return rt > 0 ? items : null;
            }
            return items;
        }

        public long BulkCopy<TEntity>(IList<TEntity> collection, int batchSize = 10240) where TEntity : class
        {
            System.Data.DataTable dt = collection.ConvertTo();
            if (dt.Rows.Count == 0) return 0;

            if (this.Database.Connection.State != System.Data.ConnectionState.Open)
                this.Database.Connection.Open();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)this.Database.Connection))
            {
                foreach (System.Data.DataColumn col in dt.Columns)
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                if (batchSize > dt.Rows.Count)
                    bulkCopy.BatchSize = dt.Rows.Count;
                else bulkCopy.BatchSize = batchSize;

                long copiedRows = 0;
                bulkCopy.SqlRowsCopied += (object sender, SqlRowsCopiedEventArgs e) =>
                {
                    copiedRows = e.RowsCopied;
                };
                bulkCopy.WriteToServer(dt);

                return copiedRows;
            }
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(command, parameters);
        }

        public int ExecuteTransaction(string command,
            System.Data.IsolationLevel IsolationLevel, params object[] parameters)
        {
            if (this.Database.Connection.State != System.Data.ConnectionState.Open)
                this.Database.Connection.Open();

            using (System.Data.Common.DbTransaction dbTrans = this.Database.Connection.BeginTransaction(IsolationLevel))
            {
                this.Database.UseTransaction(dbTrans);
                try
                {
                    int rt = this.Database.ExecuteSqlCommand(command, parameters);
                    if (rt > 0) dbTrans.Commit();
                    else dbTrans.Rollback();

                    return rt;
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    throw ex;
                }
            }
        }

        public int ExecuteTransaction(string[] commands, params object[] parameters)
        {
            using (System.Data.Common.DbTransaction dbTrans = this.Database.Connection.BeginTransaction())
            {
                this.Database.UseTransaction(dbTrans);
                try
                {
                    int rows = 0;

                    for (int i = 0; i < commands.Length; ++i)
                    {
                        rows += this.Database.ExecuteSqlCommand(commands[i], parameters);
                    }
                    dbTrans.Commit();

                    return rows;
                }
                catch (Exception ex)
                {
                    dbTrans.Rollback();
                    throw ex;
                }
            }
        }

        public List<T> Query<T>(string command, params object[] parameters)
        {
            return this.Database.SqlQuery<T>(command, parameters).ToList();
        }
        #endregion

        #region private
        private int EnsureChange<TEntity>(TEntity entity) where TEntity : class
        {
            var dbEntityEntry = Entry(entity);
            int changedCnt = 0;

            foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
            {
                var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                if (original != null)
                {
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (!original.Equals(current))
                    {
                        changedCnt += 1;
                        dbEntityEntry.Property(property).IsModified = true;
                    }
                }
            }
            return changedCnt;
        }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //自定义映射程序集所在位置
            string mappingDLL = ConfigurationManager.AppSettings.Get("assemblyFile");
            if (string.IsNullOrEmpty(mappingDLL))
                throw new Exception("找不到数据库实体配置assemblyFile节点,如<add key=\"assemblyFile\" value=\"DbMapping.dll\"/>" + mappingDLL);

            string path = AppDomain.CurrentDomain.BaseDirectory + mappingDLL;

            if (string.IsNullOrEmpty(path) || System.IO.File.Exists(path) == false)
                throw new Exception("找不到数据库表实体映射配置路径:" + path);

            modelBuilder.Configurations.AddFromAssembly(Assembly.LoadFrom(path));

            //var regTypes = Assembly.LoadFrom(path).GetTypes()
            //    .Where(type => type.BaseType != null && type.BaseType.GetGenericTypeDefinition() == typeof(DbEntity<>));

            //if (regTypes.Count() == 0)
            //    throw new Exception("无实体映射,请添加实体映射" + path);

            //foreach (var type in regTypes)
            //{
            //    dynamic typeInstance = Activator.CreateInstance(type);
            //    modelBuilder.Configurations.Add(typeInstance);
            //}

            base.OnModelCreating(modelBuilder);
        }

    }

    public interface IDbEntity
    {

    }

    public class DbEntity<T> : EntityTypeConfiguration<T>, IDbEntity where T : class
    {

    }
}
