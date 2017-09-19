using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bouyei.DbFactory;

namespace Bouyei.DbFactoryWebDemo
{
    public class Manager
    {
        public static IOrmProvider dbProvider { get; set; }
    }
}