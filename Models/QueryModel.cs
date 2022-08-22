#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[NotMapped]

public class Query
{
    [Required]
    public string Lyric {get; set;}
}