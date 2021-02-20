using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bouyei.DbFactoryCore.DbUtils
{
   public static class DataReaderAdapter
    {
        static IDbReaderToGeneric dbReaderToGeneric = new DbReaderDelegateToGeneric();
        //static IDbReaderToGeneric dbReaderToGeneric = new DbReaderExpressionToGeneric();
      //static IDbReaderToGeneric dbReaderToGeneric = new DbReaderExpressionExToGeneric();

        public static T DataReaderTo<T>(this IDataReader dataReader)
        {
            if (dbReaderToGeneric.IsPrimitType<T>())
            {
                return dbReaderToGeneric.FromDbReaderToPrimit<T>(dataReader);
            }
            else
            {
                return dbReaderToGeneric.FromDbDataReader<T>((DbDataReader)dataReader);
            }
        }

        public static List<T> DataReaderToList<T>(this IDataReader dataReader)
        {
            if (dbReaderToGeneric.IsPrimitType<T>())
            {
                return dbReaderToGeneric.FromDbReaderToPrimitList<T>(dataReader);
            }
            else
            {
                return dbReaderToGeneric.FromDbDataReaderToList<T>((DbDataReader)dataReader);
            }
        }
    }
}
