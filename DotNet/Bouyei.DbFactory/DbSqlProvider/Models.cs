/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/7/12 22:26:54
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: 6b4dd4de-0617-45f5-8972-80289aa7adf3
---------------------------------------------------------------*/
using System;
using System.Runtime.InteropServices;

namespace Bouyei.DbFactory.DbSqlProvider
{
    /// <summary>
    /// 排序方式
    /// </summary>
    [Flags]
    public enum Ordering:int
    {
        /// <summary>
        /// 降序
        /// </summary>
        Desc = 0,
        /// <summary>
        /// 升序
        /// </summary>
        Asc = 2
    }

    [Flags]
    public enum AndOr:int
    {
        /// <summary>
        /// 与
        /// </summary>
        And = 0,
        /// <summary>
        /// 或
        /// </summary>
        Or
    }

    [Flags]
    public enum SqlType:int
    {
        Normal = 0,
        SqlServer = 1,
        Oracle = 2,
        Mysql = 4,
        Db2 = 8
    }

    [Flags]
    public enum JoinType:int
    {
        Left,
        Right,
        Inner
    }

    [StructLayout(LayoutKind.Sequential)]
    public class KeyValue
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
