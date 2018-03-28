using System;

namespace DbEntity
{
    using Bouyei.DbFactoryCore.DbEntityProvider;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MemUser")]
    public class User : IDbEntity
    {
        [Key]
        public Int64 uNo { get; set; }

        public string uName { get; set; }

        public string uCity { get; set; }

        public int uAge { get; set; }

        public DateTime uBirth { get; set; }
    }
}
