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

// -----------------------------VIEW ROUTES---------------------------------------------


    [HttpGet("/songs")]
    public IActionResult AllSongs()
    {
        // if(!loggedIn)
        // {
        //     return RedirectToAction("Index", "User");
        // }

        List<Song> allSongs = _context.Songs.ToList();

        return View("AllSongs", allSongs);
    }

    [HttpGet("/songs/{songId}")]
    public IActionResult OneSong(int songId){
        Song? dbSong = _context.Songs.FirstOrDefault(s => s.SongId == songId);

        if(dbSong == null){
            return RedirectToAction("AllSongs");
        }
        string temp = "";
        char space = ' ';
        List<List<string>> lyricsList = new List<List<string>>();
        List<string> lyricsSubList = new List<string>();


        for(int i = 0; i< dbSong.Lyrics.Length; i++){
            if(dbSong.Lyrics[i] == space)
            {
                lyricsSubList.Add(temp);
                if(lyricsSubList.Count == 9)
                {
                    lyricsList.Add(lyricsSubList);
                    lyricsSubList = new List<string>();  
                }
                temp = "";
                i++;
            }
            if(i == dbSong.Lyrics.Length -1)
            {
                temp += dbSong.Lyrics[i];
                lyricsSubList.Add(temp);
                lyricsList.Add(lyricsSubList);
            }
            temp += dbSong.Lyrics[i];
        }

        ViewBag.LyricsList = lyricsList;
        return View("OneSong", dbSong);
    }

    [HttpGet("/get/tweets/{lyric}/{songId}")]
    public async Task<IActionResult> GetTweets(string lyric, int songId){
        Query newQuery = new Query();
        newQuery.Lyric = lyric;
        Console.WriteLine(newQuery.Lyric, "++++++++++++++++++++++++++++++++++++");

        TwitterClient UserClient = new TwitterClient(ApiKey, SecretKey, AccessToken, SecretAccess);

        var tweetList = await UserClient.SearchV2.SearchTweetsAsync(newQuery.Lyric);
<<<<<<< HEAD
        
=======

>>>>>>> 0f66b2a40303bb602a4ffea83bac5ca012d82acb
        if(tweetList.Tweets.Length == 0){

            return RedirectToAction("OneSong", new {songId = songId});
        }

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

    [HttpGet("/songs/new")]
    public IActionResult AddSong()
    {
        // if(!loggedIn)
        // {
        //     return RedirectToAction("Index", "User");
        // }
        return View("AddSong");
    }


// -------------------------------------POST ROUTES-------------------------------------
    [HttpPost("/songs/create")]
    public IActionResult CreateSong(Song newSong)
    {
        // if(!loggedIn)
        // {
        //     return RedirectToAction("Index", "User");
        // }

        if(ModelState.IsValid == false)
        {
            return AddSong();
        }
        // Console.WriteLine(newSong.Lyrics);
        newSong.Lyrics = newSong.Lyrics.Replace("\n", "").Replace("\r", " ").Replace("\r\n", "");
        // newSong.Lyrics.Trim();
        // Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++");
        // Console.WriteLine(newSong.Lyrics);
        
        if(uid != null)
        {
            newSong.UserId = (int)uid;
        }
        _context.Songs.Add(newSong);
        _context.SaveChanges();
        
        return RedirectToAction("OneSong", new {songId = newSong.SongId});
    }


}