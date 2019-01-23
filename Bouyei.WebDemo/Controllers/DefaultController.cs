using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bouyei.DbEntities;
using Bouyei.DbFactory;

namespace Bouyei.WebDemo.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            string connstr = "Data Source=127.0.0.1;Initial Catalog=B;User ID=sa;Password=123456;";

            using (var provider = OrmProvider.CreateProvider(connstr))
            {
                var usr= provider.GetById<User>(1);
                return View();
            }
        }
    }
}