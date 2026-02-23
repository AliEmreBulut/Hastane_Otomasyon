using System.ComponentModel.DataAnnotations.Schema;

namespace Hastane_Otomasyon.Models
{
    [Table("admin")]
    public class Admin : User
    {
        
        public override string GetRoleName()
        {
            return "Admin";
        }
        public override string GetFullName()
        {
            return "Admin";
        }
        public override int GetRoleId() => 1;
        
    }
}
