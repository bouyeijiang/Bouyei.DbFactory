using System;
using Bouyei.DbFactoryCore;

namespace DbEntity
{
    using Bouyei.DbFactoryCore.DbEntityProvider;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("user")]
    public class User : DbEntity
    {
        [Key]
      //  [IgnoreWrite]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }

        public string remark { get; set; } = "";

        public int no { get; set; }

        public int age { get; set; }

        public byte[] stamp { get; set; }

    }
}
