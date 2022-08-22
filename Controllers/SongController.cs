using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LipService.Models;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LipService.Controllers;
    
public class SongController : Controller
{
    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("uid");
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }

    private string ApiKey
    {
        get
        {
            DotNetEnv.Env.Load();
            var message = Environment.GetEnvironmentVariable("API_KEY");
            if(message != null)
            {
                return message;
            }
            return "";
        }
    }
    private string SecretKey
    {
        get
        {
            DotNetEnv.Env.Load();
            var message = Environment.GetEnvironmentVariable("SECRET_API_KEY");
            if(message != null)
            {
                return message;
            }
            return "";
        }
    }
    private string AccessToken
    {
        get
        {
            DotNetEnv.Env.Load();
            var message = Environment.GetEnvironmentVariable("ACCESS_TOKEN");
            if(message != null)
            {
                return message;
            }
            return "";
        }
    }
    private string SecretAccess
    {
        get
        {
            DotNetEnv.Env.Load();
            var message = Environment.GetEnvironmentVariable("SECRET_ACCESS");
            if(message != null)
            {
                return message;
            }
            return "";
        }
    }

    private LipServiceContext _context;

    public SongController(LipServiceContext context)
    {
        _context = context;
    }

    [HttpGet("/get/tweets/{lyric}")]
    public async Task<IActionResult> GetTweets(string lyric){
        Query newQuery = new Query();
        newQuery.Lyric = lyric;

        TwitterClient UserClient = new TwitterClient(ApiKey, SecretKey, AccessToken, SecretAccess);

        var tweetList = await UserClient.SearchV2.SearchTweetsAsync(newQuery.Lyric);
        
        List<LipTweet> ListTweets = new List<LipTweet>();

        foreach (var tweet in tweetList.Tweets)
        {
            TweetV2 newTweet = tweet;
            var author = await UserClient.UsersV2.GetUserByIdAsync(newTweet.AuthorId);
            LipTweet newLipTweet = new LipTweet();
            newLipTweet.Tweet = newTweet;
            newLipTweet.Author = author;
            ListTweets.Add(newLipTweet);

        }


        return View("AllTweets", ListTweets);
    }

}