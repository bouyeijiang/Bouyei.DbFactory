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
        [Ignore(AttributeType.IgnoreRead)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }

        public string remark { get; set; } = "";

        public int no { get; set; }

        public int age { get; set; }

        [Ignore(AttributeType.IgnoreWrite)]
        public byte[] stamp { get; set; }

        public DateTime gentime { get; set; }

    }
}
