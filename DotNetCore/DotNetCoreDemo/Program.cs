using System;
using System.Data;
using Bouyei.DbFactoryCore;
using System.Linq;
using Bouyei.DbFactoryCore.DbEntityProvider;
using Bouyei.DbFactoryCore.DbAdoProvider;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=127.0.0.1;Initial Catalog=B;User ID=sa;Password=123456;";
            IAdoProvider adoProvider = AdoProvider.CreateProvider(connectionString);

            var users = adoProvider.Query<user>(x => 1 == 1);
            //var rt = adoProvider.Query(new Parameter()
            //{
            //    CommandText = "select * from MemUser"
            //});

            //foreach (DataRow dr in rt.Result.Rows)
            //{
            //    Console.WriteLine(string.Join(",", dr.ItemArray));
            //}



            //DataTable dt = new DataTable();
            //dt.TableName = "std_user";
            //dt.Columns.Add("id", typeof(int));
            //dt.Columns.Add("name", typeof(string));

            //for (int i = 0; i < 500000; ++i)
            //{
            //    dt.Rows.Add(new object[] { i, "ad" + i });
            //}

            //adoProvider = AdoProvider.CreateProvider("PORT=5432;DATABASE=test;HOST=localhost;PASSWORD=bouyei;USER ID=postgres", ProviderType.PostgreSQL);
            //var rt = adoProvider.Query(new Parameter()
            //{
            //    CommandText = "select * from std_user"
            //});

            //var brt= adoProvider.BulkCopy(new BulkParameter(dt));

            IOrmProvider ormProvider = OrmProvider.CreateProvider(Bouyei.DbFactoryCore.DbType.SqlServer, connectionString);
            var items = ormProvider.Table<DbEntity.User>();
            foreach (var item in items)
            {
                Console.WriteLine(item.no);
            }
            Console.ReadKey();
        }

        //  static string ToString<T>(Predicate<T> predicate)
        //{
        //    return predicate.ToString();
        //}

        static string ToString<T>(System.Linq.Expressions.Expression<T> expression)
        {
            return expression.ToString();
        }

       public class user
        {
            public int age { get; set; }
            public string name { get; set; }
        }

        public class ReportBid
        {
            public int ID { get; set; }
            public string LoginID { get; set; }
            public string RealName { get; set; }
            public string Mobile { get; set; }
            public DateTime CreateTime { get; set; }
            public decimal? ReMoney { get; set; }
            public decimal? InMoney { get; set; }
            public decimal? WithMoney { get; set; }
        }
    }

    public class CreditOrder
    {
        public CreditOrder()
        {

        }

        /// <summary>
        /// 所属借贷项目ID
        /// </summary>
        public int LoanID { get; private set; }

        public string LoanTitle { get; set; }

        /// <summary>
        /// 虚拟标债权
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// 年利率（利率单位为小数点后2位，如 12代表的是12% ，作为利率计算需要除以100）
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// 还款方式
        /// </summary>
        public int RefundMode { get; set; }

        /// <summary>
        /// 借款期限
        /// </summary>
        public string LoanTime { get; set; }

        /// <summary>
        /// 债权人ID
        /// </summary>
        public int UserID { get; private set; }

        /// <summary>
        /// 债权订单的状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 初始购买债权
        /// </summary>
        public decimal InitialMoney { get; set; }

        /// <summary>
        /// 实际持有债权
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 实际购买金额
        /// </summary>
        public decimal BuyMoney { get; set; }

        /// <summary>
        /// 本笔债权使用的红包金额
        /// </summary>
        public decimal RewardMoney { get; set; }

        /// <summary>
        /// 已转让债权
        /// </summary>
        public decimal TransferMoney { get; set; }

        /// <summary>
        /// 已回利息
        /// </summary>
        public decimal RepayEarning { get; set; }

        /// <summary>
        /// 已回本金
        /// </summary>
        public decimal RepayPrincipal { get; set; }

        public decimal RepayTotle
        {
            get { return RepayEarning + RepayPrincipal; }
        }

        /// <summary>
        /// 剩余利息
        /// </summary>
        public decimal RemainEarning { get; set; }

        /// <summary>
        /// 总利息
        /// </summary>
        public decimal TotleEarning
        {
            get { return RepayEarning + RemainEarning; }
        }

        /// <summary>
        /// 剩余本金
        /// </summary>
        public decimal RemainPrincipal { get; set; }

        public decimal RemainTotle
        {
            get { return RemainEarning + RemainPrincipal; }
        }

        /// <summary>
        /// 下次回款
        /// </summary>
        public DateTime? NextRepayTime { get; set; }

        /// <summary>
        /// 债权计息开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 债权计息结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 当前期数
        /// </summary>
        public int CurrentStage { get; set; }

        /// <summary>
        /// 总期数
        /// </summary>
        public int TotalStage { get; set; }

        /// <summary>
        /// 债权订单类型
        /// </summary>
        public int OrderSource { get; set; }
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        /// <summary>
        /// 是否债权转让
        /// </summary>
        public bool? IsBond { get; set; }

        /// <summary>
        /// 原始债权ID
        /// </summary>
        public int? BondCreditOrderId { get; set; }

        /// <summary>
        /// 购买债权花费金额
        /// </summary>
        public decimal? BondByMoney { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MerBillNo { get; set; }

        /// <summary>
        /// 银管通流水号
        /// </summary>
        public string TxnId { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }
        /// <summary>
        /// 上上签合同编号
        /// </summary>
        public string BestSContractNo { get; set; }

        public int IsLock { get; set; }
        /// <summary>
        /// 用户红包ID
        /// </summary>
        public int RewardId { get; set; }
    }
}
