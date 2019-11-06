using System;
using System.Collections.Generic;
using System.Data;
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
            //string connstr = "Data Source=127.0.0.1;Initial Catalog=B;User ID=sa;Password=123456;";

            //using (var provider = OrmProvider.CreateProvider(connstr))
            //{
            //    var usr= provider.GetById<User>(1);
            //    return View();
            //}


            DataTable dt = new DataTable();
            dt.Columns.Add("uname", typeof(string));
            dt.Columns.Add("upwd", typeof(string));
            dt.Columns.Add("age", typeof(int));
            dt.Columns.Add("score", typeof(float));

            dt.Rows.Add("bouyei", "232a", 12, 239.4);
            dt.Rows.Add("hell哦", "232a", 12, 239.4);
            dt.TableName = "luser";

            string str = "server=127.0.0.1;port=3306;user=root;password=123456; database=gdzl;";
             IAdoProvider dbProvider = AdoProvider.CreateProvider(str, FactoryType.MySql);

            //var brt = dbProvider.BulkCopy(new CopyParameter<DataTable>(dt));

            List<luser> ls = new List<luser>();
            ls.Add(new luser()
            {
                uname = "bouyei",
                age = 28,
                score = 34.4f,
                upwd = "dfs"
            });
            ls.Add(new luser()
            {
                uname = "hkj",
                age = 30,
                score = 34.56f,
                upwd = "地方"
            });

           var arraybrt= dbProvider.BulkCopy(new  CopyParameter<Array>(ls.ToArray()));

            return View();
        }
    }

    public class luser
    {
        public string uname { get; set; }

        public string upwd { get; set; }

        public int age { get; set; }

        public float score { get; set; }
    }
}