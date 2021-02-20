/*-------------------------------------------------------------
 *project:Bouyei.DbFactory.SyncProvider
 *   auth: bouyei
 *   date: 2017/12/3 11:41:24
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.Server;
using Microsoft.Synchronization.Data.SqlServer;

namespace Bouyei.DbFactory.DbBaseSyncProvider
{
    internal class SqlBaseSyncProvider
    {
        private string ScopeName = "Bouyei.ScopeName";
        private List<SyncTableSchema> SyncTableSchemaes = null;

        public string SourceConnectionString { get; private set; }

        public string TargetConnectionString { get; private set; }

        public SyncProgressArgs SyncProgressHanlder { get; set; }

        public SyncStateArgs SyncStateHandler { get; set; }

        public SqlBaseSyncProvider(string sourceConnectionString, string targetConnectionString,
            string scopeName, List<SyncTableSchema> syncTableSchames)
        {
            this.SourceConnectionString = sourceConnectionString;
            this.TargetConnectionString = targetConnectionString;
            this.SyncTableSchemaes = syncTableSchames;

            if (string.IsNullOrEmpty(scopeName) == false)
                this.ScopeName = scopeName;
        }

        public SyncResultInfo ExecuteSync(SyncParameter syncParameter)
        {
            using (SqlConnection serverConn = new SqlConnection(TargetConnectionString))
            using (SqlConnection clientConn = new SqlConnection(SourceConnectionString))
            {
               return SqlExecuteSyncTask(clientConn, serverConn, syncParameter.Direction);
            }
        }

        public void DeprovisionScope()
        {
            using (SqlConnection serverConn = new SqlConnection(TargetConnectionString))
            using (SqlConnection clientConn = new SqlConnection(SourceConnectionString))
            {
                SqlDeprovisionSync(clientConn, serverConn);
            }
        }

        public void ProvisionScope(List<SyncFilterSchema> filterSchemaes)
        {
            using (SqlConnection serverConn = new SqlConnection(TargetConnectionString))
            using (SqlConnection clientConn = new SqlConnection(SourceConnectionString))
            {
                SqlInitSync(clientConn, serverConn, filterSchemaes);
            }
        }

        private SyncResultInfo SqlExecuteSyncTask(SqlConnection sourceConn, SqlConnection targetConn, SyncDirectionType direction)
        {
            using (SqlSyncProvider localProvider = new SqlSyncProvider(ScopeName, sourceConn))
            using (SqlSyncProvider remoteProvider = new SqlSyncProvider(ScopeName, targetConn))
            {
                SyncOrchestrator syncSession = new SyncOrchestrator
                {
                    LocalProvider = localProvider,
                    RemoteProvider = remoteProvider,
                    Direction = (SyncDirectionOrder)direction,
                };

                if (SyncProgressHanlder != null)
                {
                    syncSession.SessionProgress += (object sender, SyncStagedProgressEventArgs e) =>
                    {
                        SyncProgressHanlder(new SyncProgressInfo()
                        {
                            CompletedValue = (int)e.CompletedWork,
                            SessionStage = e.Stage.ToString(),
                            SynPosition = e.ReportingProvider.ToString(),
                            TotalValue = (int)e.TotalWork
                        });
                    };
                }
                if (SyncStateHandler != null)
                {
                    syncSession.StateChanged += (object sender, SyncOrchestratorStateChangedEventArgs e) =>
                    {
                        SyncStateHandler(new SyncStateInfo()
                        {
                            NewState = e.NewState.ToString(),
                            OldState = e.OldState.ToString()
                        });
                    };
                }

                SyncOperationStatistics syncResult = syncSession.Synchronize();
                return new SyncResultInfo
                {
                    SyncStartTime = syncResult.SyncStartTime,
                    SyncEndTime = syncResult.SyncEndTime,
                    DownloadChangesApplied = syncResult.DownloadChangesApplied,
                    DownloadChangesFailed = syncResult.DownloadChangesFailed,
                    DownloadChangesTotal = syncResult.DownloadChangesTotal,
                    UploadChangesApplied = syncResult.UploadChangesApplied,
                    UploadChangesFailed = syncResult.UploadChangesFailed,
                    UploadChangesTotal = syncResult.UploadChangesTotal
                };
            }
        }

        private void SqlInitSync(SqlConnection targetConn, SqlConnection sourceConn,List<SyncFilterSchema> filterSchemaes)
        {
            var scopeDesc = PreProvisionTarget();

            SqlSetScopeProvisioning(targetConn, scopeDesc,filterSchemaes);
            SqlSetScopeProvisioning(sourceConn, scopeDesc,filterSchemaes);
        }

        private void SqlDeprovisionSync(SqlConnection sourceConn, SqlConnection targetConn)
        {
            bool exist = SqlScopeExists(sourceConn);
            if (exist)
            {
                SqlSyncScopeDeprovisioning sourceDeprovisioning = new SqlSyncScopeDeprovisioning(sourceConn);
                sourceDeprovisioning.DeprovisionScope(ScopeName);
            }

            exist = SqlScopeExists(targetConn);
            if (exist)
            {
                SqlSyncScopeDeprovisioning targetDeprovisioning = new SqlSyncScopeDeprovisioning(targetConn);
                targetDeprovisioning.DeprovisionScope(ScopeName);
            }
        }

        private bool SqlScopeExists(SqlConnection dbConnection)
        {
            SqlSyncScopeProvisioning scopeProvisioning = new SqlSyncScopeProvisioning(dbConnection);
            return scopeProvisioning.ScopeExists(ScopeName);
        }

        private DbSyncScopeDescription PreProvisionTarget()
        {
            DbSyncScopeDescription targetScopeDesc = new DbSyncScopeDescription(ScopeName);
            foreach (var item in SyncTableSchemaes)
            {
                //IList<string> columns = item.Columns.Select(x => x.ColumnName).ToList();
                //var desc = SqlSyncDescriptionBuilder.GetDescriptionForTable(item.TableName,
                //    new System.Collections.ObjectModel.Collection<string>(columns), (SqlConnection)targetConn);

                DbSyncTableDescription desc = new DbSyncTableDescription(item.TableName);
                foreach (var col in item.Columns)
                {
                    desc.Columns.Add(new DbSyncColumnDescription(col.ColumnName, col.DataType)
                    {
                        IsPrimaryKey = col.IsPrimaryKey,
                        AutoIncrementSeed = col.IncrementStart,
                        AutoIncrementStep = col.IncrementStep,
                        Size = col.Size.ToString()
                    });
                }

                targetScopeDesc.Tables.Add(desc);
            }

            return targetScopeDesc;
        }

        private bool SqlSetScopeProvisioning(SqlConnection sqlConn, DbSyncScopeDescription scopeDesc,
            List<SyncFilterSchema> filterSchemaes)
        {
            SqlSyncScopeProvisioning scopeProvisioning = new SqlSyncScopeProvisioning(sqlConn, scopeDesc);
            bool exist = scopeProvisioning.ScopeExists(ScopeName);
            if (exist == false)
            {
                scopeProvisioning.SetCreateTableDefault(DbSyncCreationOption.CreateOrUseExisting);
                scopeProvisioning.SetUseBulkProceduresDefault(true);
                if (filterSchemaes != null)
                    foreach (var table in filterSchemaes)
                    {
                        foreach (var col in table.FilterColumns)
                        {
                            scopeProvisioning.Tables[table.TableName].AddFilterColumn(col);
                        }
                        scopeProvisioning.Tables[table.TableName].FilterClause = table.FilterClause;
                    }

                scopeProvisioning.Apply();
            }

            return exist == false;
        }
    }
}
