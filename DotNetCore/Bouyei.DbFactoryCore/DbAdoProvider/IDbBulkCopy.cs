/*-------------------------------------------------------------
 *auth:bouyei
 *date:2016/7/12 12:00:15
 *contact:qq453840293
 *machinename:BOUYEI-PC
 *company/organization:Microsoft
 *profile:www.openthinking.cn
---------------------------------------------------------------*/
using System;
using System.Text;
using System.Data;

namespace Bouyei.DbFactoryCore.DbAdoProvider
{
    interface IDbBulkCopy: IDisposable
    {
        void WriteToServer(DataTable dataTable);


        void WriteToServer(IDataReader dataReader,string tableName);

        //void WriteToServer(DataRow[] dataRow);
    }
}
