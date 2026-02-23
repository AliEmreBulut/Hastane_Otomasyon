using Hastane_Otomasyon.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("user")]
public abstract class User
{
    public int UserID { get; set; }

    [Required, StringLength(50)]
    public required string UserName { get; set; }


    [Required, StringLength(60)]
    public required string Password { get; set; }

    [Required]
    public int RoleID { get; set; }
    public virtual Role Role { get; set; }

    public abstract int GetRoleId();
    public abstract string GetRoleName();

    public abstract string GetFullName();
}
