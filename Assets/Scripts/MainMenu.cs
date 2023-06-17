using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject playerNameEntry, menu, credits, help, levels, highScoreList, highScoreElementPrefab, confirmReset, confirmEnter;
    public GameObject[] levelImage, requirement;
    public TextMeshProUGUI playerNameDisplay;
    public Transform highScoreElementWrapper;
    List<GameObject> highScoreElements = new List<GameObject>();
    public TMP_InputField playerNameInput;
    private string playerName = "HLOC";
    [SerializeField] HighScoreListHandler highScoreListHandler;
    public static int levelUnlockThreshold = 100;
    private bool yes_ResetHS = false; // A boolean for whether the "Yes" button in the confirmation window is clicked or not
    private bool HighScoreAdded = false;

    private void OnEnable()
    {
        HighScoreListHandler.onHighScoreListChanged += UpdateHighScoreList;
    }

    private void OnDisable()
    {
        HighScoreListHandler.onHighScoreListChanged -= UpdateHighScoreList;
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        PlayerNameEntry();
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1; // Unpause the game when click the home button in the paused state
        credits.SetActive(false); // Hide the credits window
        help.SetActive(false); // Hide the help window
        levels.SetActive(false); // Hide the levels window
        highScoreList.SetActive(false); // Hide the high score list window
        confirmReset.SetActive(false); // Hide the reset confirmation window
        confirmEnter.SetActive(false); // Hide the enter level confirmation window

        for (int i = 0; i < 3; i++)
        {
            requirement[i].SetActive(false);
            if (i > 0)
            {
                if (PlayerPrefs.GetInt("HighScore Level" + i, 0) < i * levelUnlockThreshold)
                {
                    levelImage[i].GetComponent<Button>().interactable = false;
                    requirement[i].SetActive(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!HighScoreAdded)
        {
            AddHighScoreToList();
        }

        playerNameDisplay.text = "– — " + PlayerPrefs.GetString("Player Name", "No Name") + " — –";

        // // Screenshot (Editor Only)
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     ScreenshotGenerator.GenerateScreenshot();
        // }
    }

    // Hide the main menu and unhide the player name entry
    // (Show player name entry window) if player name
    // has not been entered
    public void PlayerNameEntry()
    {
        if (PlayerPrefs.HasKey("Player Name"))
        {
            menu.SetActive(true);
            playerNameEntry.SetActive(false);
        }
        else
        {
            menu.SetActive(false);
            playerNameEntry.SetActive(true);
        }
    }

    // Save player name
    public void SavePlayerName()
    {
        menu.SetActive(true);
        playerNameEntry.SetActive(false);

        playerName = playerNameInput.text;
        PlayerPrefs.SetString("Player Name", playerName);
    }

    // Add current high scores to the list
    public void AddHighScoreToList()
    {
        if (PlayerPrefs.HasKey("Player Name"))
        {
            highScoreListHandler.AddHighScoreIfPossible(new HighScoreElement(PlayerPrefs.GetString("Player Name", "No Name"), GetPlayerHighScores()));
        }

        HighScoreAdded = true;
    }

    // Get the array of player high scores of each level
    private int[] GetPlayerHighScores()
    {
        int[] highScores = new int[3];

        for (int i = 0; i < highScores.Length; i++)
        {
            highScores[i] = PlayerPrefs.GetInt("HighScore Level" + (i + 1), 0);
        }

        return highScores;
    }

    // Load the main game scene to start playing
    public void StartGame()
    {
        PlayerScript.scoreCount = 0; // Set score to 0
        SceneManager.LoadScene(CarouselView.curLevelIndex);
    }

    // Hide the main menu and unhide the credits
    // (Show credits window)
    public void Credits()
    {
        menu.SetActive(false);
        credits.SetActive(true);
    }

    // Hide the main menu and unhide the help
    // (Show help window)
    public void Help()
    {
        menu.SetActive(false);
        help.SetActive(true);
    }

    // Hide the main menu and unhide the levels
    // (Show levels window)
    public void Levels()
    {
        menu.SetActive(false);
        levels.SetActive(true);
    }

    // Hide the main menu and unhide the high score list
    // (Show high score list window)
    public void HighScoreList()
    {
        menu.SetActive(false);
        highScoreList.SetActive(true);
    }

    // Update the High Score List
    private void UpdateHighScoreList(List<HighScoreElement> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            HighScoreElement el = list[i];

            if (el != null)
            {
                if (i >= highScoreElements.Count)
                {
                    // Instantiate New Entry
                    var inst = Instantiate(highScoreElementPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(highScoreElementWrapper, false);

                    highScoreElements.Add(inst);
                }

                // Write or Overwrite Name and High Score
                var texts = highScoreElements[i].GetComponentsInChildren<TextMeshProUGUI>();

                texts[0].text = el.playerName;

                // Assign high score values using a loop
                for (int j = 0; j < 3; j++)
                {
                    texts[j + 1].text = el.playerHighScore[j].ToString();
                }
            }
        }
    }

    // Unhide the main menu and hide
    //  the credits, help, levels, and high score list
    // (Return to main menu from showing
    //  the credits, help, levels, or high score list window)
    public void Return()
    {
        menu.SetActive(true);
        credits.SetActive(false);
        help.SetActive(false);
        levels.SetActive(false);
        highScoreList.SetActive(false);
    }

    // Unhide the enter level confirmation 
    // (Show enter level confirmation window)
    public void Enter()
    {
        confirmEnter.SetActive(true);
    }

    // Unhide the reset confirmation 
    // (Show reset confirmation window)
    public void Quit()
    {
        confirmReset.SetActive(true);
    }

    // Assign yes_ResetHS=true when the "Yes" button
    // in the confirmation window is clicked
    public void ResetHighScore()
    {
        yes_ResetHS = true;
    }

    // Quit the game, reset all the High Scores, and reset High Score list
    //  if the "Yes" button in the confirmation window is clicked,
    // Quit the game and reset all the High Scores without reset the High Score list
    //  if the "No" button in the confirmation window is clicked
    public void QuitGame()
    {
        if (yes_ResetHS)
        {
            HighScoreListHandler.ResetHighScoreList(); // Reset the High Score list
        }

        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("HighScore Level" + (i + 1), 0);
        }

        Debug.Log("Quit Game");
        Application.Quit(); // Quit the game

        PlayerPrefs.DeleteKey("Player Name");

        yes_ResetHS = false;
    }

    // Hide the enter level confirmation window if the "No" button in the confirmation window is clicked
    public void CancelEnter()
    {
        confirmEnter.SetActive(false);
    }

    // Hide the reset confirmation window if the "Cancel" button in the confirmation window is clicked
    public void CancelQuit()
    {
        confirmReset.SetActive(false);
    }
}