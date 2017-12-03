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
                if (syncParameter.IsPreprovision)
                    SqlInitSync(serverConn, clientConn);

                var rt = SqlExecuteSyncTask(clientConn, serverConn, syncParameter.Direction);

                if (syncParameter.IsDeprovision)
                    SqlDeprovisionSync(clientConn, serverConn);

                return rt;
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

        private SyncResultInfo SqlExecuteSyncTask(SqlConnection sourceConn, SqlConnection targetConn, SyncDirectionType direction)
        {
            //DbSyncProvider d = new DbSyncProvider();
            //SqlConnection conn = (SqlConnection)sourceConn;

            //d.SelectScopeInfoCommand = new SqlCommand("", conn);
            //d.UpdateScopeInfoCommand = new SqlCommand("", conn);
            //d.SelectNewTimestampCommand = new SqlCommand("", conn);

            SyncOrchestrator syncSession = new SyncOrchestrator
            {
                //LocalProvider = new DbSyncProvider()
                //{
                //    Connection = sourceConn,
                //    ScopeName = ScopeName
                //},
                //RemoteProvider = new DbSyncProvider()
                //{
                //    Connection = targetConn,
                //    ScopeName = ScopeName
                //},

                LocalProvider = new SqlSyncProvider(ScopeName, sourceConn),
                RemoteProvider = new SqlSyncProvider(ScopeName, targetConn),
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

        private void SqlInitSync(SqlConnection targetConn, SqlConnection sourceConn)
        {
            var scopeDesc = PreProvisionTarget();
            SqlSetScopeProvisioning(targetConn, scopeDesc);

            SqlPreProvisionSourceFromTarget(sourceConn, targetConn);
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
            DbSyncScopeDescription scopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(ScopeName, dbConnection);
            SqlSyncScopeProvisioning scopeProvisioning = new SqlSyncScopeProvisioning(dbConnection, scopeDesc);
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

            // SqlSetScopeProvisioning(targetConn, targetScopeDesc);
        }

        private void SqlPreProvisionSourceFromTarget(SqlConnection sourceConn, SqlConnection targetConn)
        {
            DbSyncScopeDescription targetScopeDesc = SqlSyncDescriptionBuilder.GetDescriptionForScope(ScopeName, targetConn);
            SqlSyncScopeProvisioning sourceScopeProvisioning = new SqlSyncScopeProvisioning(sourceConn, targetScopeDesc);
            bool exist = sourceScopeProvisioning.ScopeExists(ScopeName);
            if (exist == false)
            {
                sourceScopeProvisioning.Apply();
            }
        }

        private bool SqlSetScopeProvisioning(SqlConnection sqlConn, DbSyncScopeDescription scopeDesc)
        {
            SqlSyncScopeProvisioning targetScopeProvisioning = new SqlSyncScopeProvisioning(sqlConn, scopeDesc);
            bool exist = targetScopeProvisioning.ScopeExists(ScopeName);
            if (exist == false)
            {
                targetScopeProvisioning.SetCreateTableDefault(DbSyncCreationOption.CreateOrUseExisting);
                targetScopeProvisioning.Apply();
            }

            return exist == false;
        }
    }
}
