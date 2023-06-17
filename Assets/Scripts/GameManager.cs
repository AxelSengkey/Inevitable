using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, highScoreText, level, turningCountdown, newLevelInfoText;

    public GameObject turningCountdownCanvas, turningCountdownInfo, newLevelInfoCanvas, passageInfo;
    public GameObject gameOver, youWin, pointsCleared, pauseMenu, pauseBtn, help;
    public static bool GObool; // A boolean for whether Game Over or not
    public static bool isWin = false;
    public static bool isWinPanel = true;
    public static ParticleSystem newPar;

    public Sprite[] livesSprites;
    public Image livesImage;

    private PointScript[] pointsObjects;

    // Create the variables for the AudioClip and AudioSource component
    private AudioSource audioSource;

    public static int curLevel;

    private static float potionDurationThreshold = 10;
    public static float potionDuration, potionTimeRemaining;
    // public static Dictionary<string, float> levelPotionDurations = new Dictionary<string, float>()
    // {
    //     { "Level1", 10f },
    //     { "Level2", 20f },
    //     { "Level3", 30f }
    // };

    // Start is called before the first frame update
    void Start()
    {
        // To get the highest score from previous game in the same computer
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore " + SceneManager.GetActiveScene().name, 0).ToString();

        // To get the current level name/number
        level.text = "Level " + CarouselView.curLevelIndex.ToString();

        // Set potion duration based on active scene
        for (int i = 1; i <= 3; i++)
        {
            if (SceneManager.GetActiveScene().name == "Level" + i)
            {
                potionDuration = i * potionDurationThreshold;
                potionTimeRemaining = potionDuration;
                Debug.Log(potionDuration);
            }
        }
        // potionDuration = levelPotionDurations[SceneManager.GetActiveScene().name];
        // potionTimeRemaining = levelPotionDurations[SceneManager.GetActiveScene().name];
        // Debug.Log(levelPotionDurations[SceneManager.GetActiveScene().name]);

        // Hide the Turning Countdown
        turningCountdownCanvas.SetActive(false);
        turningCountdownInfo.SetActive(false);

        newLevelInfoCanvas.SetActive(false);

        // Assign the AudioSource component to the audioSource variable
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        GObool = false; // false -> Indicates that is not Game Over yet
        gameOver.SetActive(false); // Hide the Game Over screen
        youWin.SetActive(false); // Hide the You Win screen
        pointsCleared.SetActive(false); // Hide the Points Cleared screen

        pauseMenu.SetActive(false);
        pauseBtn.SetActive(true);
    }

    void FixedUpdate()
    {
        if (isWin)
        {
            YouWin();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // To update the score text and change the variable to string
        scoreText.text = "Score: " + PlayerScript.scoreCount.ToString();

        // To manage and change the highscore if the player get higher score
        if (PlayerScript.scoreCount > PlayerPrefs.GetInt("HighScore " + SceneManager.GetActiveScene().name))
        {
            // Set the new high score to be save in the next game
            PlayerPrefs.SetInt("HighScore " + SceneManager.GetActiveScene().name, PlayerScript.scoreCount);
            highScoreText.text = "High Score: " + PlayerScript.scoreCount.ToString();
        }

        for (int i = 1; i <= 3; i++)
        {
            if (i < 3)
            {
                if (SceneManager.GetActiveScene().name == "Level" + i)
                {
                    if (PlayerPrefs.GetInt("HighScore " + SceneManager.GetActiveScene().name, 0) >= i * MainMenu.levelUnlockThreshold)
                    {
                        newLevelInfoText.text = "Level " + (i + 1) + " Unlocked!";
                        newLevelInfoCanvas.SetActive(true);
                    }
                }
            }
            else
            {
                if (SceneManager.GetActiveScene().name == "Level" + i)
                {
                    if (PlayerPrefs.GetInt("HighScore " + SceneManager.GetActiveScene().name, 0) >= i * MainMenu.levelUnlockThreshold)
                    {
                        if (isWinPanel == true)
                        {
                            isWin = true;
                        }
                        else
                        {
                            isWin = false;
                            youWin.SetActive(false);
                        }
                    }
                }
            }
        }

        pointsObjects = FindObjectsOfType<PointScript>();
        Debug.Log("pointsObjects.Length = " + pointsObjects.Length);
        if (pointsObjects == null)
        {
            PointsCleared();
        }

        // To manage and change the lives if the player is damaged
        livesImage.sprite = livesSprites[PlayerScript.curLives];

        turningCountdown.text = AllyScript.seconds.ToString();

        if (AllyScript.turn)
        {
            turningCountdownCanvas.SetActive(true);
            turningCountdownInfo.SetActive(true);

            audioSource.mute = false; // Unmute the sound effect
        }
        else
        {
            turningCountdownCanvas.SetActive(false);
            turningCountdownInfo.SetActive(false);

            audioSource.mute = true; // Mute the sound effect
        }

        if (GObool)
        {
            pauseBtn.SetActive(false);
            GameOver();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        // // Screenshot (Editor Only)
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     ScreenshotGenerator.GenerateScreenshot();
        // }
    }

    // Unhide the Game Over screen when Game Over, and set the game to paused state
    public void GameOver()
    {
        Time.timeScale = 0; // Time.timeScale = 0 -> Time stopped (paused)
        gameOver.SetActive(true);
        passageInfo.SetActive(false);
        turningCountdownInfo.SetActive(false);
    }

    // Unhide the You Win screen when You Win, and set the game to paused state
    public void YouWin()
    {
        Time.timeScale = 0; // Time.timeScale = 0 -> Time stopped (paused)
        youWin.SetActive(true);
        passageInfo.SetActive(false);
        turningCountdownInfo.SetActive(false);
        pauseBtn.SetActive(false);
    }

    // Unhide the Points Cleared screen when the Points Cleared, and set the game to paused state
    public void PointsCleared()
    {
        Time.timeScale = 0; // Time.timeScale = 0 -> Time stopped (paused)
        pointsCleared.SetActive(true);
        passageInfo.SetActive(false);
        turningCountdownInfo.SetActive(false);
        pauseBtn.SetActive(false);
    }

    // Reload the main game scene to play again
    public void Retry()
    {
        PlayerScript.scoreCount = 0; // Set score to 0
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GObool = false;
        isWin = false;
        Time.timeScale = 1; // Time.timeScale = 1 -> Time passes as fast as real time (unpaused)
    }

    // Unhide the pause menu and hide the pause button, and set the game to paused state
    public void Pause()
    {
        Debug.Log("Pause");
        pauseMenu.SetActive(true);
        pauseBtn.SetActive(false);
        Time.timeScale = 0; // Time.timeScale = 0 -> Time stopped (paused)
    }

    // Hide the pause menu and unhide the pause button, and set the game to unpaused state
    public void Resume()
    {
        Debug.Log("Resume");
        pauseMenu.SetActive(false);
        pauseBtn.SetActive(true);
        isWin = false;
        Time.timeScale = 1; // Time.timeScale = 1 -> Time passes as fast as real time (unpaused)
    }

    // Back to main menu
    public void Home()
    {
        Debug.Log("Home");
        SceneManager.LoadScene("MainMenu");
    }

    // Hide the pause menu and unhide the help
    // (Show help window)
    public void Help()
    {
        pauseMenu.SetActive(false);
        help.SetActive(true);
    }

    // Unhide the pause menu and hide the help
    // (Return to pause menu from showing the help window)
    public void Return()
    {
        pauseMenu.SetActive(true);
        help.SetActive(false);
    }

    public static void Particles(ParticleSystem particle, Transform attachedObject)
    {
        // To make the particle effect
        newPar = Instantiate(particle, attachedObject.position, Quaternion.Euler(-90, 0, 0));
        newPar.transform.SetParent(attachedObject);
        if (newPar.isPlaying) newPar.Stop();
        if (!newPar.isPlaying) newPar.Play();
    }
}
