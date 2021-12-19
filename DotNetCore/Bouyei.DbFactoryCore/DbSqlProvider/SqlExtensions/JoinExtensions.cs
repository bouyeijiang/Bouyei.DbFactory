using Bouyei.DbFactoryCore.DbSqlProvider;
using Bouyei.DbFactoryCore.DbSqlProvider.SqlKeywords;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Bouyei.DbFactoryCore
{
    public static class JoinExtensions
    {
        public static Join<L, R> Join<S, L, R>(this Select select,
      Expression<Func<L, bool>> leftExp,
      Expression<Func<R, bool>> righthExp,
      Dictionary<string, string> onWhere,
      JoinType joinType = JoinType.Left)
        {
            Select<L> left = new Select<L>();
            left.SqlString = left.ToString();

            Select<R> right = new Select<R>();
            right.SqlString = right.ToString();

            Join<L, R> join = new Join<L, R>(left.From<L>().Where(leftExp),
                right.From<R>().Where(righthExp), onWhere, joinType);

            join.SqlString = select.SqlString + "From " + join.ToString();
            return join;
        }

        public static Join<L, R> Join<S, L, R>(this Select<S> select,
            Expression<Func<L, bool>> leftExp,
            Expression<Func<R, bool>> righthExp,
            Dictionary<string, string> onWhere,
            JoinType joinType = JoinType.Left)
        {
            Select<L> left = new Select<L>();
            left.SqlString = left.ToString();

            Select<R> right = new Select<R>();
            right.SqlString = right.ToString();

            Join<L, R> join = new Join<L, R>(left.From<L>().Where(leftExp),
                right.From<R>().Where(righthExp), onWhere, joinType);

            join.SqlString = select.SqlString + "From " + join.ToString();
            return join;
        }

        public static JoinOn<L, R> Join<S, L, R>(this Select<S> select,
           Expression<Func<L, bool>> leftExp,
           Expression<Func<R, bool>> righthExp,
           Expression<Func<L, R, bool>> onWhere,
           JoinType joinType = JoinType.Left)
        {
            Select<L> left = new Select<L>();
            left.SqlString = left.ToString();

            Select<R> right = new Select<R>();
            right.SqlString = right.ToString();

            JoinOn<L, R> join = new JoinOn<L, R>(left.From<L>().Where(leftExp),
                right.From<R>().Where(righthExp), new On<L, R>(onWhere), joinType);

            join.SqlString = select.SqlString + "From " + join.ToString();
            return join;
        }

        public static JoinOn<L, R> Join<S, L, R>(this Select select,
  Expression<Func<L, bool>> leftExp,
  Expression<Func<R, bool>> righthExp,
 Expression<Func<L, R, bool>> onWhere,
  JoinType joinType = JoinType.Left)
        {
            Select<L> left = new Select<L>();
            left.SqlString = left.ToString();

            Select<R> right = new Select<R>();
            right.SqlString = right.ToString();

            JoinOn<L, R> join = new JoinOn<L, R>(left.From<L>().Where(leftExp),
                 right.From<R>().Where(righthExp), new On<L, R>(onWhere), joinType);

            join.SqlString = select.SqlString + "From " + join.ToString();
            return join;
        }
    }
}
