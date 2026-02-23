using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hastane_Otomasyon.Models
{
    [Table("role")]
    public class Role
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public required string RoleName { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }

        

    }
}
