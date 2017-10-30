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
    public enum ProviderType : byte
    {
        SqlServer = 0x00,
        DB2 = 0x01,
        [Obsolete("请使用Oracle 第三方provider代替")]
        MsOracle = 0x02,
        Oracle = 0x04,
        MySql = 0x08,
        SQLite = 0x10,
        OleDb = 0x20,
        Odbc = 0x40
    }
}
