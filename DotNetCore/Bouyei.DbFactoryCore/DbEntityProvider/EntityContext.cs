using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Data.SqlClient;

namespace Bouyei.DbFactoryCore.DbEntityProvider
{
    using DbUtils;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using System.Data.Common;

    internal class EntityContext : DbContext, IDisposable
    { 
        public string ConnectionString { get; set; }

        public FactoryType ProviderType { get; set; }

        public EntityContext(FactoryType dbType=FactoryType.SqlServer, string NameOrConnectionString = null)
        {
            this.ConnectionString = NameOrConnectionString;
            this.ProviderType = dbType;
        }

        public void DbMigrate()
        {
            base.Database.Migrate();
        }

        public EntityContext(DbContextOptions<EntityContext> option) :
            base(option)
        {
        }

        public void Reload<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).Reload();
        }

        #region public
        //public DbSet<TEntity> DSet<TEntity>() where TEntity : class
        //{
        //    return this.Set<TEntity>();
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

        public TEntity GetById<TEntity>(params object[] keys) where TEntity : class
        {
            return Set<TEntity>().Find(keys);
        }

        public IQueryable<TEntity> QueryNoTracking<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Where(predicate).AsNoTracking();
        }

        public TEntity Update<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            Set<TEntity>().Attach(entity);
            Set<TEntity>().Update(entity);
            this.Entry<TEntity>(entity).State = EntityState.Modified;

            if (isSaveChange)
            {
                int rt = SaveChanges();
                if (rt > 0) return entity;
                else return default(TEntity);
            }
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

        public TEntity Insert<TEntity>(TEntity entity, bool isSaveChange = false) where TEntity : class
        {
            Set<TEntity>().Add(entity);
            this.Entry<TEntity>(entity).State = EntityState.Added;
            if (isSaveChange)
            {
                int rt = SaveChanges();
                if (rt > 0) return entity;
                else return default(TEntity);
            }
            return entity;
        }

        public IEnumerable<TEntity> InsertRange<TEntity>(IEnumerable<TEntity> entities, bool isSaveChange = false) where TEntity : class
        {
             Set<TEntity>().AddRange(entities);
            if (isSaveChange)
            {
                int rt = SaveChanges();
                return rt > 0 ? entities : null;
            }
            return entities;
        }

        public long BulkCopy<TEntity>(IList<TEntity> collection, int batchSize = 10240) where TEntity : class
        {
            System.Data.DataTable dt = collection.ConvertTo();
            if (dt.Rows.Count == 0) return 0;

            this.Database.OpenConnection();

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)this.Database.GetDbConnection()))
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
            this.Database.OpenConnection();

            using (DbTransaction dbTrans = this.Database.GetDbConnection().BeginTransaction(IsolationLevel))
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
            using (DbTransaction dbTrans = this.Database.GetDbConnection().BeginTransaction())
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

        public IQueryable<T> Query<T>(string command, params object[] parameters) where T : class
        {
            return Set<T>().FromSql(command, parameters);
        }
        #endregion

        #region private
        private int EnsureChange<TEntity>(TEntity entity) where TEntity : class
        {
            var dbEntityEntry = Entry(entity);
            int changedCnt = 0;

            foreach (var property in dbEntityEntry.OriginalValues.Properties)
            {
                var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                if (original != null)
                {
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (!original.Equals(current))
                    {
                        changedCnt += 1;
                        dbEntityEntry.Property(property.Name).IsModified = true;
                    }
                }
            }
            return changedCnt;
        }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (ProviderType == FactoryType.SqlServer)
            {
                optionsBuilder.UseSqlServer(ConnectionString, (SqlServerDbContextOptionsBuilder option) =>
                {
                });
            }
            else if (ProviderType == FactoryType.SQLite)
            {
                optionsBuilder.UseSqlite(ConnectionString, (SqliteDbContextOptionsBuilder option) =>
                {

                });
            }
            //else if (ProviderType == FactoryType.MySql)
            //{
            //    optionsBuilder.UseMySQL(ConnectionString,(MySQLDbContextOptionsBuilder option) => {

            //    });
            //}
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string path = AppContext.BaseDirectory + JsonConfiguration.Configuration["AppSettings:EntityMapping"];

            var assem = Assembly.LoadFrom(path);
            //need inherit IEntityTypeConfiguration<>
            //modelBuilder.ApplyConfigurationsFromAssembly(assem, type => type.GetTypeInfo().BaseType != null
            //&& typeof(DbEntity).IsAssignableFrom(type));

            var eItems = assem.GetTypes()
            .Where(type => type.GetTypeInfo().BaseType != null&& typeof(DbEntity).IsAssignableFrom(type));

            foreach (var item in eItems)
            {
                if (modelBuilder.Model.FindEntityType(item) != null)
                    continue;

                modelBuilder.Model.AddEntityType(item);
            }

            base.OnModelCreating(modelBuilder);
        }
    }

    public class JsonConfiguration
    {
        public static IConfiguration Configuration { get; set; }

        static JsonConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", true, true).Build();
        }

        public static T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }
    }

    public interface IDbEntity
    {

    }

    public class DbEntity:IDbEntity
    {

    }
}
