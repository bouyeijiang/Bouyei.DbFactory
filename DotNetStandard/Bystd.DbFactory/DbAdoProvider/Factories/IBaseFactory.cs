using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bystd.DbFactory.DbAdoProvider
{
    public interface IBaseFactory : IDisposable
    {
        string ConnectionString { get; set; }

        int ExecuteTimeout { get; set; }

        FactoryType FactoryType { get; set; }

    }
}
