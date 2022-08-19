#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace LipService.Models;

public class Song
{
    [Key]
    public int SongId {get;set;}

    [Required(ErrorMessage = "Song name is required!")]
    [Display(Name = "Name:")]
    public string SongName {get;set;}

    [Required(ErrorMessage = "Lyrics are required!")]
    [Display(Name = "Lyrics:")]
    public string Lyrics {get;set;}

    public int UserId {get;set;}

    public User? Artist {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;    
}