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
        [IgnoreWrite]
        public int id { get; set; }

        public string name { get; set; }

        public string no { get; set; }


        public int age { get; set; }

        public byte[] stamp { get; set; }

    }
}
