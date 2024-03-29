﻿/*-------------------------------------------------------------
 *   auth: bouyei
 *   date: 2017/4/22 0:24:26
 *contact: 453840293@qq.com
 *profile: www.openthinking.cn
 *   guid: bb256df2-3fdb-4d60-acae-5b546f381130
---------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Bouyei.DbFactoryCore.DbEntityProvider
{
    internal static class EntityExtension
    {
        public static void Detach<TEntity>(this DbContext dbContext, TEntity entity) where TEntity : class
        {
            dbContext.Detach(entity);
        }

        public static void Refresh<TEntity>(this EntityProvider dbProvider, TEntity entity) where TEntity:class
        {
            dbProvider.Refresh<TEntity>(entity);
        }
    }
}
