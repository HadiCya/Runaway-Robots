using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine.SocialPlatforms.Impl;
using System.Net;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard instance;
    const string LeaderboardId = "Runaway-Robots-Leaderboard";

    private async void Awake()
    {
        //Keep only one instance of Leaderboard
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        await UnityServices.InitializeAsync();
        await SignInAnonymously();
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    //Will only update if new score is higher than player's best score
    public async void AddScore(int score)
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<string> GetPlayerScore()
    {
        //will catch 404 not found exception if the user doesn't have a score yet 
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            Debug.Log(JsonConvert.SerializeObject(scoreResponse));
            return ($"{scoreResponse.Rank + 1}. {scoreResponse.PlayerName}\t{scoreResponse.Score}");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        return "No score found";
    }

    public async Task<string[,]> GetScores()
    {
        string[,] scores = new string[10, 2];
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
            Debug.Log(JsonConvert.SerializeObject(scoresResponse));

            if (scoresResponse != null && scoresResponse.Results != null)
            {
                int i = 0;
                foreach (var result in scoresResponse.Results)
                {
                    scores[i, 0] = result.PlayerName;
                    scores[i, 1] = result.Score.ToString();
                    i++;
                }
                for (int e = i; e < 10; e++)
                {
                    scores[e, 0] = "No score yet";
                    scores[e, 1] = "0";
                }
            }
            else
            {
                scores[0, 0] = "No results found";
            }
            return scores;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            scores[0, 0] = "Failed to retreive scores";
            return scores;
        }
    }

    public async void UpdateName(string name)
    {
        try
        {
            var nameResponse = await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
            Debug.Log(JsonConvert.SerializeObject(nameResponse));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async Task<string> GetName()
    {
        try
        {
            var nameResponse = await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log(JsonConvert.SerializeObject(nameResponse));
            return nameResponse;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return "Failed to sign in";
        }
    }
}