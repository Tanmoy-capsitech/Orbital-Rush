using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
     private bool _isInitializing = false;

    private void Awake()
    {
        if (Instance == null)
        {
            _isInitializing = true;
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
            _isInitializing = false;
            
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // if (playerShield == null)
        //     {
        //         playerShield = FindAnyObjectByType<PlayerShield>();
        //         Debug.Log("Auto-assigned playerShield: " + (playerShield ? playerShield.name : "none found"));
        //     }


    }
    
     public static bool IsReady()
    {
        return Instance != null && !Instance._isInitializing;
    }

    public bool IsIntialized{get; set;}
    public int CurrentScore { get; set; }

    private string highScoreKey = "HighScore";
    public int HighScore
    {
        get
        {
            int score = PlayerPrefs.GetInt(highScoreKey, 0);
            Debug.Log($"Retrieving High Score: {score}");
            return score;
        }

            
        set
        {
            Debug.Log($"Setting High Score: {value}");
            PlayerPrefs.SetInt(highScoreKey, value);
            PlayerPrefs.Save();
        }
    }
    private void Init()
    {
        IsIntialized = false;
        CurrentScore = 0;
    }

    private const string MainMenu = "MainMenu";
    private const string Gameplay = "GamePlay";

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenu);
    }

    public void GoToGameplay()
    {   
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(Gameplay);
        
    }
     
     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    if (scene.name == "GamePlay")
    {
        // Find the PlayerShield in the Gameplay scene
        playerShield = FindAnyObjectByType<PlayerShield>();

        if (playerShield != null)
            Debug.Log("✅ PlayerShield found and linked after GamePlay loaded: " + playerShield.name);
        else
            Debug.LogWarning("⚠️ PlayerShield NOT found in GamePlay scene!");
    }

    // Unsubscribe so this only runs once
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

    public PlayerShield playerShield;
    // public void AddScore(int amount)
    //         {
    //             CurrentScore += amount;
    //             Debug.Log("AddScore called: " + CurrentScore);

    //             // if we haven't linked playerShield yet, try to find it now
    //             if (playerShield == null)
    //             {
    //                 playerShield = FindAnyObjectByType<PlayerShield>();
    //                 Debug.Log("Lazy-find playerShield in AddScore: " + (playerShield ? playerShield.name : "none found"));
    //             }

    //             if (playerShield != null)
    //             {
    //                 playerShield.CheckShieldByScore(CurrentScore);
    //             }
    //             else
    //             {
    //                 // safe fallback: log a warning and do not crash
    //                 Debug.LogWarning("playerShield is still null when AddScore called. Shield won't activate.");
    //             }
    //         }


    public void AddScore(int amount)
    {
        CurrentScore += amount;                     // UI / high-score

        // ---- NEW: always get the *current* shield ----
        PlayerShield shield = FindAnyObjectByType<PlayerShield>();

        // ---- Individual points for the shield logic ----
        GameplayManager gm = FindAnyObjectByType<GameplayManager>();
        int individualPoints = gm ? gm.GetIndividualPoints() : CurrentScore;

        // ---- Call the shield if we have one ----
        shield?.CheckShieldByScore(individualPoints);

        Debug.Log($"Total: {CurrentScore} | Shield Points: {individualPoints}");
    }
            

        

}
