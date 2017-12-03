/*-------------------------------------------------------------
 *project:Bouyei.DbFactoryDemo
 *   auth: bouyei
 *   date: 2017/12/3 16:17:20
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryDemo
{
    using Bouyei.DbFactory;

    internal class SyncProviderDemo
    {
        IDbSyncProvider dbSyncProvider = null;
        public void Execute()
        {
            List<SyncTableSchema> tableSchema = new List<SyncTableSchema>();
            tableSchema.Add(new SyncTableSchema()
            {
                TableName = "user",
                Columns = new List<SyncColumnName>() {
                    new SyncColumnName("name"){ DataType="nvarchar",Size=50},
                    new SyncColumnName("id"){ DataType="int", IsPrimaryKey= true, IncrementStart=1, IncrementStep=1,Size=4},
                    new  SyncColumnName("no"){ DataType="int",Size=4}
                }
            });

            string sourceConnString = "Server=127.0.0.1;Database=A;User Id=sa;Password=bouyei;";
            string targetConnString = "Server=127.0.0.1;Database=B;User Id=sa;Password=bouyei;";
            dbSyncProvider = DbSyncProvider.CreateProvider(sourceConnString, targetConnString,
                "ScopeName", tableSchema);

            //dbSyncProvider.DeprovisionScope();

            var rt = dbSyncProvider.ExecuteSync(new SyncParameter()
            {
                Direction = SyncDirectionType.Upload
            });

            Console.WriteLine("beginTime" + rt.SyncStartTime);
            Console.WriteLine("endTime" + rt.SyncEndTime);
            Console.WriteLine("UploadChangesApplied" + rt.UploadChangesApplied);
            Console.WriteLine("UploadChangesTotal" + rt.UploadChangesTotal);

            return;

        }
    }
}
