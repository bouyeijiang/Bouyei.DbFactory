using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bouyei.DbFactory.DbEntityProvider;

namespace Bouyei.DbEntities
{
    [Table("user")]
    public class User: DbEntity<User>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string name { get; set; }

        public int age { get; set; }

        public int no { get; set; }
    }
}
