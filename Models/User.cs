#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LipService.Models;

public class User
{
    [Key]
    public int UserId {get;set;}

    [Required(ErrorMessage = "First name is required!")]
    [MinLength(2, ErrorMessage = "Must be at least 2 characters!")]
    [Display(Name = "First Name:")]
    public string FirstName {get;set;}

    [Required(ErrorMessage = "Last name is required!")]
    [MinLength(2, ErrorMessage = "Must be at least 2 characters!")]
    [Display(Name = "Last Name:")]
    public string LastName {get;set;}

    [Required(ErrorMessage = "Username is required!")]
    [StringLength(15, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 15 characters!")]
    [Display(Name = "Username:")]
    public string Username {get;set;}

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress]
    public string Email {get;set;}

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters!")]
    [DataType(DataType.Password)]
    [Display(Name = "Password:")]
    public string Password {get;set;}

    [NotMapped]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Doesn't match password!")]
    [Display(Name = "Confirm Password:")]
    public string ConfirmPassword {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public List<Song> CreatedSongs {get;set;} = new List<Song>();

    public string FullName()
    {
        return FirstName + " " + LastName;
    }
}