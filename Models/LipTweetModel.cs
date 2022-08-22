#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
[NotMapped]

public class LipTweet
{
    [Required]
    public TweetV2 Tweet {get; set;}
    public UserV2Response Author {get; set;}
}