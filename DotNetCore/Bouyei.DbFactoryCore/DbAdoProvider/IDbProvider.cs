/*-------------------------------------------------------------
 *auth:bouyei
 *date:2016/4/26 9:19:56
 *contact:qq453840293
 *machinename:BOUYEI-PC
 *company/organization:Microsoft
 *profile:www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    public interface IDbProvider: IDisposable
    {
        string DbConnectionString { get; set; }
        ProviderType DbType { get; set; }
        ResultInfo<bool, string> Connect(string connString);
        ResultInfo<DataTable, string> Query(Parameter dbParameter);
        ResultInfo<List<T>, string> Query<T>(Parameter dbParameter);
        ResultInfo<DataSet, string> QueryToSet(Parameter dbParameter);
        ResultInfo<int, string> QueryTo<T>(Parameter dbParameter, Func<T, bool> rowAction);
        ResultInfo<int, string> QueryToReader(Parameter dbParameter, Func<IDataReader,bool> rowAction);
        ResultInfo<IDataReader, string> QueryToReader(Parameter dbParameter);
        ResultInfo<int, string> QueryChanged(Parameter dbParameter, Func<DataTable,bool> action);
        ResultInfo<int, string> QueryToTable(Parameter dbParameter, DataTable dstTable);
        ResultInfo<int, string> ExecuteCmd(Parameter dbParameter);
        ResultInfo<int, string> ExecuteTransaction(Parameter dbParameter);
        ResultInfo<int, string> ExecuteTransaction(string[] CommandTexts, int timeout = 1800, Func<int, bool> rowAction=null);
        ResultInfo<T, string> ExecuteScalar<T>(Parameter dbParameter);
        ResultInfo<int, string> BulkCopy(BulkParameter dbParameter);
    }
}
