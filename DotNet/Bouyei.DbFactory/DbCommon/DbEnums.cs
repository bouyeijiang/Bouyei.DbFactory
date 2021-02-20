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

namespace Bouyei.DbFactory
{
    [Flags]
    public enum FactoryType : byte
    {
        SqlServer = 0x00,
        DB2 = 0x01,
        //[Obsolete("请使用Oracle 第三方provider代替")]
        //MsOracle = 0x02,
        Oracle = 0x04,
        MySql = 0x08,
        SQLite = 0x10,
        OleDb = 0x20,
        Odbc = 0x40,
        PostgreSQL=0x80
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

    [Flags]
    public enum BulkCopyOptions
    {
        None = -1,
        // 摘要: 
        //     对所有选项使用默认值。
        Default = 0,
        //
        // 摘要: 
        //     保留源标识值。 如果未指定，则由目标分配标识值。
        KeepIdentity = 1,
        //
        // 摘要: 
        //     请在插入数据的同时检查约束。 默认情况下，不检查约束。
        CheckConstraints = 2,
        //
        // 摘要: 
        //     在批量复制操作期间获取批量更新锁。 如果未指定，则使用行锁。
        TableLock = 4,
        //
        // 摘要: 
        //     保留目标表中的空值，而不管默认值的设置如何。 如果未指定，则空值将由默认值替换（如果适用）。
        KeepNulls = 8,
        //
        // 摘要: 
        //     指定后，会导致服务器为插入到数据库中的行激发插入触发器。
        FireTriggers = 16,
        //
        // 摘要: 
        //     如果已指定，则每一批批量复制操作将在事务中发生。 如果指示了此选项，并且为构造函数提供了 System.Data.SqlClient.SqlTransaction
        //     对象，则发生 System.ArgumentException。
        UseInternalTransaction = 32,
    }
}
