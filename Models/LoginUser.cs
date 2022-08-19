#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]
public class LoginUser
{
    [Required(ErrorMessage = "Username is required!")]
    [Display(Name = "Username:")]
    public string LoginUsername {get;set;}

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters!")]
    [DataType(DataType.Password)]
    [Display(Name = "Password:")]
    public string LoginPassword {get;set;}
}