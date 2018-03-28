/*-------------------------------------------------------------
 *project:Bouyei.DbFactory.DbCommon
 *   auth: bouyei
 *   date: 2017/10/30 11:22:05
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bouyei.DbFactoryCore
{
    [Flags]
    public enum ProviderType : byte
    {
        SqlServer = 0x00,
        DB2 = 0x01,
        Oracle = 0x04,
        MySql = 0x08,
        SQLite = 0x10
    }

    public enum SyncDirectionType
    {
        //
        // 摘要:
        //     先上载，再下载。
        UploadAndDownload = 0,
        //
        // 摘要:
        //     先下载，再上载。
        DownloadAndUpload = 1,
        //
        // 摘要:
        //     仅上载。
        Upload = 2,
        //
        // 摘要:
        //     仅下载。
        Download = 3
    }
}
