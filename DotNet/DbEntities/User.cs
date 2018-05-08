using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Bouyei.DbFactory.DbEntityProvider;

namespace Bouyei.DbEntities
{
    [Table("user")]
    public class User:DbEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }
    }
}
