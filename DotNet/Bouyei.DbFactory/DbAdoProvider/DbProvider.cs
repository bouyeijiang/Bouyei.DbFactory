/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2016/4/26 9:19:46
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *    Ltd: Microsoft
 *   guid: 93caaa3a-b22b-4b82-8d92-62d598962222
---------------------------------------------------------------*/
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Bouyei.DbFactory.DbAdoProvider
{
    using DbUtils;

    public class DbProvider : DbCommonBuilder, IDbProvider
    {
        #region variable
        private LockParam lParam = new LockParam()
        {
            LockTimeout = 5,//s
            SleepInterval = 1000,
        };
        private bool disposed = false;

        public string DbConnectionString { get; set; }
        
        public DbType DbType { get; set; }

        public int LockTimeout
        {
            get { return lParam.LockTimeout; }
            set
            {
                lParam.LockTimeout = value < 5 ? 5 : value;
            }
        }

        #endregion

        #region  dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (this.dbConn != null) this.dbConn.Dispose();
                if (this.dbDataAdapter != null) this.dbDataAdapter.Dispose();
                if (this.dbCommand != null) this.dbCommand.Dispose();
                if (this.dbBulkCopy != null) this.dbBulkCopy.Dispose();
                if (this.dbCommandBuilder != null) dbCommandBuilder.Dispose();
                if (this.dbTransaction != null) dbTransaction.Dispose();
            }
            disposed = true;
        }

        #endregion

        #region  structure
        public DbProvider(
            string connectionString,
            DbType providerType = DbType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.DbType = providerType;
            this.DbConnectionString = connectionString;
        }

        public DbProvider(
            DbType providerType = DbType.SqlServer,
            bool isSingleton = false)
            : base(providerType, isSingleton)
        {
            this.DbType = providerType;
        }

        #endregion

        #region public
        public DbResult<bool, string> Connect(string ConnectionString)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                this.DbConnectionString = ConnectionString;
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    {
                        return new DbResult<bool, string>(true, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    return new DbResult<bool, string>(false, ex.ToString());
                }
            }
        }

        public DbResult<DataTable, string> Query(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = this.CreateCommand(conn, dbParameter,trans))
                    using (DbDataAdapter adapter = this.CreateAdapter())
                    {
                        DataTable dt = new DataTable();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);

                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        return new DbResult<DataTable, string>(dt, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    return new DbResult<DataTable, string>(null, ex.ToString());
                }
            }
        }

        public DbResult<DataSet, string> QueryToSet(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    using (DbDataAdapter adapter = CreateAdapter())
                    {
                        DataSet ds = new DataSet();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(ds);

                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        return DbResult<DataSet, string>.Create(ds, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    return new DbResult<DataSet, string>(null, ex.ToString());
                }
            }
        }

        public DbResult<int, string> QueryToReader(Parameter dbParameter, Func<IDataReader, bool> rowAction)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    int rows = 0;
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        if (reader.HasRows == false)
                            return DbResult<int, string>.Create(0, string.Empty);

                        bool isContinue = false;

                        while (reader.Read())
                        {
                            isContinue = rowAction(reader);
                            if (isContinue == false) break;
                            ++rows;
                        }
                    }
                    return DbResult<int, string>.Create(rows, string.Empty);
                }
                catch (Exception ex)
                {
                    return new DbResult<int, string>(-1, ex.ToString());
                }
            }
        }

        public DbResult<int,string> QueryTo<T>(Parameter dbParameter,Func<T,bool> rowAction)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    int rows = 0;
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        if (reader.HasRows == false)
                            return DbResult<int, string>.Create(0, string.Empty);

                        bool isContinue = false;
                        while (reader.Read())
                        {
                            T row = reader.DataReaderTo<T>();
                            isContinue = rowAction(row);
                            if (isContinue == false) break;
                            ++rows;
                        }
                    }

                    return DbResult<int, string>.Create(rows, string.Empty);
                }
                catch (Exception ex)
                {
                    return new DbResult<int, string>(-1, ex.ToString());
                }
            }
        }

        public DbResult<IDataReader, string> QueryToReader(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    DbConnection conn = CreateConnection(DbConnectionString);
                    DbCommand cmd = CreateCommand(conn, dbParameter);
                    IDataReader reader = cmd.ExecuteReader();
                    return DbResult<IDataReader, string>.Create(reader, string.Empty);
                }
                catch (Exception ex)
                {
                    return DbResult<IDataReader, string>.Create(null, ex.ToString());
                }
            }
        }

        public DbResult<int, string> ExecuteCmd(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    {
                        int rt = cmd.ExecuteNonQuery();

                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        var rValue = GetReturnParameter(cmd);

                        return DbResult<int, string>.Create(rt < 0 ? 0 : rt,
                            rValue == null ? string.Empty : rValue.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }

        public DbResult<int, string> QueryToTable(Parameter dbParameter, DataTable dstTable)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    using (DbDataReader dReader = cmd.ExecuteReader())
                    {
                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        int oCnt = dstTable.Rows.Count;

                        dstTable.Load(dReader);
 
                        return DbResult<int, string>.Create(dstTable.Rows.Count - oCnt, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }

        public DbResult<int, string> ExecuteTransaction(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction tran = BeginTransaction(conn))
                    {
                        try
                        {
                            using (DbCommand cmd = CreateCommand(conn, dbParameter, tran))
                            {
                                int rt = cmd.ExecuteNonQuery();
                                tran.Commit();

                                var rValue = GetReturnParameter(cmd);

                                return DbResult<int, string>.Create(rt < 0 ? 0 : rt,
                                    rValue != null ? rValue.ToString() : string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return DbResult<int, string>.Create(-1, ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }

        public DbResult<int, string> ExecuteTransaction(string[] CommandTexts,int timeout = 1800, Func<int, bool> action = null)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction tran = BeginTransaction(conn))
                    {
                        try
                        {
                            int rows = 0;
                            using (DbCommand cmd = CreateCommand(conn, null, tran))
                            {
                                cmd.CommandTimeout = timeout;
                                for (int i = 0; i < CommandTexts.Length; ++i)
                                {
                                    cmd.CommandText = CommandTexts[i];
                                    int erow = cmd.ExecuteNonQuery();

                                    if (action != null)
                                    {
                                        bool isContinue = action(erow);
                                        if (isContinue == false)
                                        {
                                            rows = 0;
                                            break;
                                        }
                                    }

                                    if (erow < 0)
                                    {
                                        rows = 0;
                                        break;
                                    }
                                    rows += erow;
                                }
                            }

                            if (rows > 0)
                            {
                                tran.Commit();
                            }
                            return new DbResult<int, string>(rows, string.Empty);
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            return new DbResult<int, string>(-1, ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new DbResult<int, string>(-1, ex.ToString());
                }
            }
        }

        public DbResult<T, string> ExecuteScalar<T>(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    {
                        object obj = cmd.ExecuteScalar();

                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        var rValue = GetReturnParameter(cmd);

                        return DbResult<T, string>.Create(obj == null ? default(T) : (T)Convert.ChangeType(obj, typeof(T)),
                          rValue == null ? string.Empty : rValue.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<T, string>.Create(default(T), ex.ToString());
                }
            }
        }

        [Obsolete("已过时,请使用泛型方法代替")]
        public DbResult<int, string> BulkCopy(BulkParameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    Exception temex = null;
                    int cnt = 0;
                    using (DbCommonBulkCopy bulkCopy = CreateBulkCopy(DbConnectionString, dbParameter))
                    {
                        bulkCopy.Open();
                        bulkCopy.BulkCopiedHandler = dbParameter.BulkCopiedHandler;

                        bulkCopy.BatchSize = dbParameter.BatchSize;
                        bulkCopy.BulkCopyTimeout = dbParameter.ExecuteTimeout;

                        try
                        {
                            if ((dbParameter.DataSource == null
                                || dbParameter.DataSource.Rows.Count == 0)
                                && dbParameter.IDataReader != null)
                            {
                                bulkCopy.WriteToServer(dbParameter.IDataReader, dbParameter.TableName);
                                cnt = 1;
                            }
                            else
                            {
                                if (dbParameter.BatchSize > dbParameter.DataSource.Rows.Count)
                                    dbParameter.BatchSize = dbParameter.DataSource.Rows.Count;

                                bulkCopy.WriteToServer(dbParameter.DataSource);
                                cnt = dbParameter.DataSource.Rows.Count;
                            }

                            //use transaction
                            if (dbParameter.IsTransaction)
                            {
                                //有事务回调则由外边控制事务提交,否则直接提交事务
                                if (dbParameter.TransactionCallback != null)
                                    dbParameter.TransactionCallback(bulkCopy.dbTrans, cnt);
                                else
                                    bulkCopy.dbTrans.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            temex = ex;
                            if (dbParameter.IsTransaction && bulkCopy.dbTrans != null)
                            {
                                bulkCopy.dbTrans.Rollback();
                            }
                        }
                    }

                    if (temex != null) throw temex;
                    return DbResult<int, string>.Create(cnt, string.Empty);
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }

        public DbResult<int,string> BulkCopy<T>(CopyParameter<T> dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    Exception temex = null;
                    int cnt = 0;
                    using (DbCommonBulkCopy bulkCopy = CreateBulkCopy(DbConnectionString, dbParameter))
                    {
                        bulkCopy.Open();
                        bulkCopy.BulkCopiedHandler = dbParameter.BulkCopiedHandler;

                        bulkCopy.BatchSize = dbParameter.BatchSize;
                        bulkCopy.BulkCopyTimeout = dbParameter.ExecuteTimeout;

                        try
                        {
                            if (dbParameter.dataSource is DataTable)
                            {
                                var data = dbParameter.dataSource as DataTable;
                                if (dbParameter.BatchSize > data.Rows.Count)
                                    dbParameter.BatchSize = data.Rows.Count;

                                bulkCopy.WriteToServer(data);
                                cnt = data.Rows.Count;
                            }
                            else if (dbParameter.dataSource is IDataReader)
                            {
                                bulkCopy.WriteToServer(dbParameter.dataSource as IDataReader, dbParameter.TableName);
                                cnt = 1;
                            }
                            else if (dbParameter.dataSource is Array)
                            {
                                var array = dbParameter.dataSource as Array;

                                bulkCopy.WriteToServer(array, dbParameter.TableName);
                                cnt = array.Length;
                            }
                            else
                                throw new Exception("not support data type");

                            //use transaction
                            if (dbParameter.IsTransaction)
                            {
                                //有事务回调则由外边控制事务提交,否则直接提交事务
                                if (dbParameter.TransactionCallback != null)
                                    dbParameter.TransactionCallback(bulkCopy.dbTrans, cnt);
                                else
                                    bulkCopy.dbTrans.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            temex = ex;
                            if (dbParameter.IsTransaction && bulkCopy.dbTrans != null)
                            {
                                bulkCopy.dbTrans.Rollback();
                            }
                        }
                    }

                    if (temex != null) throw temex;
                    return DbResult<int, string>.Create(cnt, string.Empty);
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }

        public DbResult<List<T>, string> Query<T>(Parameter dbParameter)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (dbParameter.IsTransaction)
                            trans.Commit();

                        if (reader.HasRows == false)
                            return DbResult<List<T>, string>.Create(new List<T>(1), string.Empty);

                        List<T> items = reader.DataReaderToList<T>();

                        return DbResult<List<T>, string>.Create(items, string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<List<T>, string>.Create(null, ex.ToString());
                }
            }
        }

        public DbResult<int, string> QueryChanged(Parameter dbParameter, Func<DataTable,bool> action)
        {
            using (LockWait lwait = new LockWait(ref lParam))
            {
                try
                {
                    using (DbConnection conn = CreateConnection(DbConnectionString))
                    using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                    using (DbCommand cmd = CreateCommand(conn, dbParameter,trans))
                    using (DbDataAdapter adapter = CreateAdapter())
                    {
                        DataTable dt = new DataTable();
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);

                        if (dt.Rows.Count == 0) return DbResult<int, string>.Create(0, "无可更新的数据行");

                        bool isContinue = action(dt);
                        if (isContinue == false) return DbResult<int, string>.Create(0, string.Empty);

                        DataTable changedt = dt.GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);

                        if (changedt == null || changedt.Rows.Count == 0)
                            return DbResult<int, string>.Create(0, string.Empty);

                        using (DbCommandBuilder dbBuilder = this.CreateCommandBuilder())
                        {
                            dbBuilder.DataAdapter = adapter;
                            int rt = adapter.Update(changedt);

                            if (dbParameter.IsTransaction)
                                trans.Commit();

                            return DbResult<int, string>.Create(rt, string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return DbResult<int, string>.Create(-1, ex.ToString());
                }
            }
        }
        #endregion

        #region private

        private object GetReturnParameter(DbCommand cmd)
        {
            if (cmd.Parameters != null && cmd.Parameters.Count > 0)
            {
                for(int i = 0; i < cmd.Parameters.Count; ++i)
                {
                    if (cmd.Parameters[i].Direction != ParameterDirection.Input)
                    {
                        return cmd.Parameters[i].Value;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
