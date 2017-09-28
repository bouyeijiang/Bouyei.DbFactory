/*-------------------------------------------------------------
 *project:Bouyei.DbFactory
 *   auth: bouyei
 *   date: 2017/9/28 22:20:30
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

    internal class LockWait : IDisposable
    {
        LockParam lParam = null;
        static int ExecuteTimeout = 1800;

        public LockWait(ref LockParam lParam)
        {
            this.lParam = lParam;

            while (Interlocked.CompareExchange(ref lParam.Signal, 1, 0) == 1)
            {
                Thread.Sleep(lParam.SleepInterval);

                //waiting for lock to do;
                int maxTimeout = (lParam.LockTimeout + ExecuteTimeout);
                if (lParam.time >= maxTimeout)
                {
                    lParam.time = 0;
                    throw new Exception("lock timeout..." + maxTimeout);
                }
                lParam.time += (lParam.SleepInterval / 1000);
            }
            lParam.time = 0;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref lParam.Signal, 0);
        }
    }

    internal class LockParam
    {
        internal int Signal = 0;

        internal int SleepInterval = 1000;

        internal int LockTimeout=30;

        internal int time = 0;
    }
}
