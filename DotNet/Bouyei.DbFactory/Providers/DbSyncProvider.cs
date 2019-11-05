/*-------------------------------------------------------------
 *project:Bouyei.DbFactory.Providers
 *   auth: bouyei
 *   date: 2017/12/3 11:40:36
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactory
{
    using DbBaseSyncProvider;

    /// <summary>
    /// dependency Microsoft Sync Framework sdk
    /// </summary>
    public class DbSyncProvider : IDbSyncProvider
    {
        SqlBaseSyncProvider sqlSyncProvider = null;

        public FactoryType SyncProviderType { get; set; }

        public string SourceConnectionString { get;  set; }

        public string TargetConnectionString { get;  set; }

        public SyncProgressArgs SyncProgressHandler { get; set; }

        public SyncStateArgs SyncStateHandler { get; set; }
     
        public DbSyncProvider(string SourceConnectionString, string TargetConnectionString, 
            string ScopeName, List<SyncTableSchema> TableSchemaes,
            FactoryType SyncProviderType=FactoryType.SqlServer)
        {
            this.SourceConnectionString = SourceConnectionString;
            this.TargetConnectionString = TargetConnectionString;
            this.SyncProviderType = SyncProviderType;

            if (SyncProviderType == FactoryType.SqlServer)
            {
                sqlSyncProvider = new SqlBaseSyncProvider(SourceConnectionString,
                    TargetConnectionString,
                    ScopeName, TableSchemaes);
            }
            else
            {
                throw new Exception("no support this provider type instance" + SyncProviderType.ToString());
            }
        }

        public static DbSyncProvider CreateProvider(string SourceConnectionString, string TargetConnectionString,
            string ScopeName, List<SyncTableSchema> TableSchemaes,
            FactoryType SyncProviderType = FactoryType.SqlServer)
        {
            return new DbSyncProvider(SourceConnectionString,
                TargetConnectionString,
                ScopeName, TableSchemaes, SyncProviderType);
        }

        /// <summary>
        /// executing task
        /// </summary>
        /// <param name="syncParameter"></param>
        /// <returns></returns>
        public SyncResultInfo ExecuteSync(SyncParameter syncParameter)
        {
            if (SyncProviderType == FactoryType.SqlServer)
            {
                sqlSyncProvider.SyncProgressHanlder = SyncProgressHandler;
                sqlSyncProvider.SyncStateHandler = SyncStateHandler;

               return sqlSyncProvider.ExecuteSync(syncParameter);
            }
            else
            {
                throw new Exception("no support this provider type instance" + SyncProviderType.ToString());
            }
        }

        /// <summary>
        ///remove setting
        /// </summary>
        public void DeprovisionScope()
        {
            if (SyncProviderType == FactoryType.SqlServer)
            {
                sqlSyncProvider.DeprovisionScope();
            }
            else
            {
                throw new Exception("no support this provider type instance" + SyncProviderType.ToString());
            }
        }

        /// <summary>
        /// initilize setting
        /// </summary>
        /// <param name="filterSchemaes"></param>
        public void ProvisionScope(List<SyncFilterSchema> filterSchemaes)
        {
            if (SyncProviderType == FactoryType.SqlServer)
            {
                sqlSyncProvider.ProvisionScope(filterSchemaes);
            }
            else
            {
                throw new Exception("no support this provider type instance" + SyncProviderType.ToString());
            }
        }
    }
}
