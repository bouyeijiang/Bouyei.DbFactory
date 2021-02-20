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

    public class DbProvider : DbBaseProvider, IDbProvider
    {
        #region variable

        private bool disposed = false;

        #endregion

        #region  dispose

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                base.Dispose();
            }
            disposed = true;
        }

        #endregion

        #region  structure
        public DbProvider(
            string ConnectionString,
            FactoryType dbType = FactoryType.PostgreSQL)
            : base(dbType, ConnectionString)
        {
           
        }

        public DbProvider(string ConnectionString,
            int ExecuteTimeout,
            FactoryType dbType = FactoryType.PostgreSQL)
            : base(dbType,ExecuteTimeout,ConnectionString)
        { }

        public DbProvider(
            FactoryType dbType = FactoryType.PostgreSQL)
            : base(dbType)
        {
            
        }

        #endregion

        #region public
        public DbResult<bool, string> Connect(string ConnectionString)
        {
            base.ConnectionString = ConnectionString;
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                {
                    return new DbResult<bool, string>(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                return new DbResult<bool, string>(false, ex.ToString());
            }
        }

        public DbResult<DataTable, string> Query(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = this.CreateCommand(conn, dbParameter, trans))
                using (DbDataAdapter adapter = this.CreateDataAdapter())
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

        public DbResult<DataSet, string> Querys(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
                using (DbDataAdapter adapter = CreateDataAdapter())
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

        public DbResult<int, string> QueryToReader(Parameter dbParameter, Func<IDataReader, bool> rowAction)
        {
            try
            {
                int rows = 0;
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
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

        public DbResult<int, string> Query<T>(Parameter dbParameter, Func<T, bool> rowAction)
        {
            try
            {
                int rows = 0;
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
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

        public DbResult<IDataReader, string> QueryToReader(Parameter dbParameter)
        {
            try
            {
                DbConnection conn = CreateConnection(ConnectionString);
                DbCommand cmd = CreateCommand(conn, dbParameter);
                IDataReader reader = cmd.ExecuteReader();
                return DbResult<IDataReader, string>.Create(reader, string.Empty);
            }
            catch (Exception ex)
            {
                return DbResult<IDataReader, string>.Create(null, ex.ToString());
            }
        }

        public DbResult<int, string> ExecuteCmd(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
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

        public DbResult<int, string> QueryToTable(Parameter dbParameter, DataTable dstTable)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
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

        public DbResult<int, string> ExecuteTransaction(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
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

        public DbResult<int, string> ExecuteTransaction(string[] CommandTexts, int timeout = 1800, Func<int, bool> action = null)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
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

        public DbResult<T, string> ExecuteScalar<T>(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
                {
                    object obj = cmd.ExecuteScalar();
                    if (obj == DBNull.Value) obj = null;

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

        public DbResult<int, string> BulkCopy(BulkParameter dbParameter)
        {
            try
            {
                int cnt = 0;
                var copy = new CopyFactory(DbType, ConnectionString, dbParameter.ExecuteTimeout)
                {
                    BulkCopiedHandler = dbParameter.BulkCopiedHandler
                };

                if ((dbParameter.DataSource == null
                    || dbParameter.DataSource.Rows.Count == 0)
                    && dbParameter.IDataReader != null)
                {
                    copy.WriteToServer(dbParameter.IDataReader, dbParameter.TableName);
                    cnt = 1;
                }
                else
                {
                    if (dbParameter.BatchSize > dbParameter.DataSource.Rows.Count)
                        dbParameter.BatchSize = dbParameter.DataSource.Rows.Count;

                    cnt = copy.WriteToServer(dbParameter.DataSource, dbParameter.BatchSize);
                }
                return DbResult<int, string>.Create(cnt, string.Empty);
            }
            catch (Exception ex)
            {
                return DbResult<int, string>.Create(-1, ex.ToString());
            }
        }

        public DbResult<int, string> BulkCopy<T>(CopyParameter<T> dbParameter)
        {
            try
            {
                int cnt = 0;
                var copy = new CopyFactory(DbType, ConnectionString, dbParameter.ExecuteTimeout)
                {
                    BulkCopiedHandler = dbParameter.BulkCopiedHandler
                };
                if (dbParameter.dataSource is DataTable)
                {
                    var data = dbParameter.dataSource as DataTable;
                    if (dbParameter.BatchSize > data.Rows.Count)
                        dbParameter.BatchSize = data.Rows.Count;

                    copy.WriteToServer(data);
                    cnt = data.Rows.Count;
                }
                else if (dbParameter.dataSource is IDataReader)
                {
                    copy.WriteToServer(dbParameter.dataSource as IDataReader, dbParameter.TableName);
                    cnt = 1;
                }
                else if (dbParameter.dataSource is Array)
                {
                    var array = dbParameter.dataSource as Array;

                    copy.WriteToServer(array, dbParameter.TableName);
                    cnt = array.Length;
                }
                else
                    throw new Exception("Unsupported Data Type");

                return DbResult<int, string>.Create(cnt, string.Empty);
            }
            catch (Exception ex)
            {
                return DbResult<int, string>.Create(-1, ex.ToString());
            }
        }

        public DbResult<List<T>, string> Query<T>(Parameter dbParameter)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
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

        public DbResult<int, string> QueryChanged(Parameter dbParameter, Func<DataTable, bool> action)
        {
            try
            {
                using (DbConnection conn = CreateConnection(ConnectionString))
                using (DbTransaction trans = dbParameter.IsTransaction ? BeginTransaction(conn, dbParameter.IsolationLevel) : null)
                using (DbCommand cmd = CreateCommand(conn, dbParameter, trans))
                using (DbDataAdapter adapter = CreateDataAdapter())
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
        public DbResult<DataTable, string> QuerySchema()
        {
            DataTable dt = null;
            string info = string.Empty;
            try
            {
                using (DbConnection conn = CreateConnection(this.ConnectionString))
                {
                    dt = conn.GetSchema();
                }
            }
            catch (Exception ex)
            {
                info = ex.Message;
            }

            return DbResult<DataTable, string>.Create(dt, info);
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
