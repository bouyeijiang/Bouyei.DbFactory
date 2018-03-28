/*-------------------------------------------------------------
 *project:Bouyei.DbFactory.Providers
 *   auth: bouyei
 *   date: 2017/12/3 11:39:36
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
    public interface IDbSyncProvider
    {
        string SourceConnectionString { get; set; }

        string TargetConnectionString { get; set; }

        SyncProgressArgs SyncProgressHandler { get; set; }

        SyncStateArgs SyncStateHandler { get; set; }

        SyncResultInfo ExecuteSync(SyncParameter syncParameter);

        void DeprovisionScope();

        void ProvisionScope(List<SyncFilterSchema> filterSchemaes);
    }
}
