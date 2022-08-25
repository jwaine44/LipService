// C#, ASPNetCore & Entity Framework Imports
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// LipService Model Imports
using LipService.Models;

// Imports from the Tweetinvi Library 
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;
using Tweetinvi.Parameters.V2;

namespace LipService.Controllers;
    
public class SongController : Controller
{

    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("uid");
            // This retrieves the User Id that is stored in session when a user logs in / registers 
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
            // This returns a true or false if a user is logged in 
        }
    }

    /*
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ++++++++++++++++++++++API KEYS IMPORTED AS VARIABLES+++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    */
    private string ApiKey
    {
        get
        {
            // ALL IMPORTS USE THE DotNetEnv Library METHODS FOR IMPORTING FROM A .env FILE 
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

    /*
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ++++++++++++++++++++++END API KEY IMPORTS++++++++++++++++++++++++++++++
    +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    */

    private LipServiceContext _context;

    public SongController(LipServiceContext context)
    {
        _context = context;
    // _context = MySQL Database Schema 
    }

// -----------------------------VIEW ROUTES---------------------------------------------


    [HttpGet("/songs")]
    // This Route retrieves all songs that a particular user has saved to the database. 
    public IActionResult AllSongs()
    {
        if(!loggedIn)
        {
            // If a user is not logged in, they will be redirected to the homepage of the site.
            return RedirectToAction("Index", "User");
        }
    
        // This query retrieves a list of all the songs that only the logged in user has uploaded.
        List<Song> allSongs = _context.Songs.Where(s => s.Artist.UserId == uid).ToList();

        return View("AllSongs", allSongs);
    }

    [HttpGet("/songs/{songId}")]
    public IActionResult OneSong(int songId)
    {
        if(!loggedIn)
        {
            // If a user is not logged in, they will be redirected to the homepage of the site.
            return RedirectToAction("Index", "User");
        }

        // Looks for a song in the database that corresponds to the {songId} entered in the route 
        Song? dbSong = _context.Songs.FirstOrDefault(s => s.SongId == songId);
        
        if(dbSong == null){
        // If it can't find the song, it takes them back to AllSongs, the route above this one.
            return RedirectToAction("AllSongs");
        }

        // This begins the process of taking the song lyrics and turning them into a format that the Twitter API will accept, which is 9 keywords or less.

        // Temp will store the current word in the song the algorithm is parsing out. 
        string temp = "";

        // Space is what will designate the end of the current word 
        char space = ' ';

        // A 2-D array that holds lists of strings, 9 items long.
        List<List<string>> lyricsList = new List<List<string>>();
        
        // This is a list of 9 strings making up a query for the Twitter API
        List<string> lyricsSubList = new List<string>();

        // Starts with a loop through the lyrics, one character at a time
        for(int i = 0; i < dbSong.Lyrics.Length; i++)
        {

            // Checks if the character we are currently at is a space.
            if(dbSong.Lyrics[i] == space)
            {
                // The word stored in temp is added to the list of 9 strings, lyricsSubList
                lyricsSubList.Add(temp);
                
                if(lyricsSubList.Count == 9)
                {
                    // If the list has reached a length of 9, it will add that list to the 2-D array
                    lyricsList.Add(lyricsSubList);
                    // Resets the list of 9 to empty to start the process over again
                    lyricsSubList = new List<string>();  
                }
                // Resets temp to empty to be ready for a new word
                temp = "";
                // Makes the character we are currently looking at jump forward as to not include the space in the word being stored
                i++;
            }

            // This catches if we have reached the end of the song, but do not have 9 words to make a complete line, adding whatever we have to the 2-D array
            if(i == dbSong.Lyrics.Length - 1)
            {
                // Adds the last letter to the word inside temp, and does the same add process as before.
                temp += dbSong.Lyrics[i];
                lyricsSubList.Add(temp);
                lyricsList.Add(lyricsSubList);
            }
            // On every loop a new character is being added to temp.
            temp += dbSong.Lyrics[i];
        }

        // Viewbag holds the 2-D array to be looped through for display on the front-end. ./Views/Song/OneSong
        ViewBag.LyricsList = lyricsList;
        return View("OneSong", dbSong);
    }

    [HttpGet("/get/tweets/{lyric}/{songId}")]
    public async Task<IActionResult> GetTweets(string lyric, int songId)
    {
        // Creates a new query for Twitter API
        Query newQuery = new Query();
        // Assigns the lyric property to the lyric that was selected by the user.
        newQuery.Lyric = lyric;

        // Using the Tweetinvi Library, creates a new instance of the TwitterClient Class with the credentials imported 
        TwitterClient UserClient = new TwitterClient(ApiKey, SecretKey, AccessToken, SecretAccess);

        // Using the Tweetinvi Library, uses a Search Method to search for tweets using the Query Model 
        var tweetList = await UserClient.SearchV2.SearchTweetsAsync(newQuery.Lyric);
        
        // If it finds that there are no recent tweets about that song, it redirects them to the Song the user was using to search for tweets containing those lyrics
        if(tweetList.Tweets.Length == 0)
        {
            return RedirectToAction("OneSong", new {songId = songId});
        }

        // Using our LipTweet model, creates a new instance of a List of that Model
        List<LipTweet> ListTweets = new List<LipTweet>();
        
        // Loops through each tweet in the tweets list received from the API call
        foreach (var tweet in tweetList.Tweets)
        {
            // Casts the tweet as a TweetV2 object from the Tweetinvi library; this must be done in order to parse through the data of the tweet.
            TweetV2 newTweet = tweet;

            // Using the Tweetinvi library, gets the Author of the Tweets information to be used on the front-end; the TweetV2 object has access to the Author's Id, but not their Username and other account details
            var author = await UserClient.UsersV2.GetUserByIdAsync(newTweet.AuthorId);

            // Creates a new instance of the LipTweet model, matching up the tweet info and Author info to be accessed later
            LipTweet newLipTweet = new LipTweet();
            newLipTweet.Tweet = newTweet;
            newLipTweet.Author = author;

            // Adds the LipTweet object to the list of LipTweets
            ListTweets.Add(newLipTweet);
        }

        // Returns a view for viewing All Tweets, passing through the list of LipTweet objects.
        return View("AllTweets", ListTweets);
    }

    [HttpGet("/songs/new")]
    public IActionResult AddSong()
    {
        if(!loggedIn)
        {
            // If a user is not logged in, they will be redirected to the homepage of the site.
            return RedirectToAction("Index", "User");
        }

        // Returns a View for the form for a user to add a new song to the database.
        return View("AddSong");
    }

// -------------------------------------POST ROUTES-------------------------------------

    [HttpPost("/songs/create")]
    public IActionResult CreateSong(Song newSong)
    {
        if(!loggedIn)
        {
            // If a user is not logged in, they will be redirected to the homepage of the site.
            return RedirectToAction("Index", "User");
        }

        if(ModelState.IsValid == false)
        {
            // If all required fields don't have inputs, this will return all the error messages letting the user know what they need to add to complete the creation of a new song.
            return AddSong();
        }
        
        // This trims out all the line breaks that would be there if the lyrics were copy/pasted from Google 
        // Line Breaks are not compatible with the API call. 
        newSong.Lyrics = newSong.Lyrics.Replace("\n", "").Replace("\r", " ").Replace("\r\n", "");
                
        if(uid != null)
        {
            // Assigns the new song object with a UserID to establish the relationship inside the database. 
            newSong.UserId = (int)uid;
        }

        // Adds the song to the database and saves it.
        _context.Songs.Add(newSong);
        _context.SaveChanges();
        
        // Redirects the user to the view of the song they just added being displayed on the screen
        return RedirectToAction("OneSong", new {songId = newSong.SongId});
    }
}