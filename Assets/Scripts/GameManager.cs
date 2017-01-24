using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
public class GameManager : Singleton<GameManager>
{
    private enum GameState
    {
        COUNTDOWN,
        TITLE_SCREEN,
        HOW_TO,
        PLAYER_SELECT,
        ROUND_IN_PROGRESS,
        RESULTS,
        CREDITS,
        COUNT
    }

    [SerializeField] GameObject playerPrefab;
    [SerializeField] int playerAmount;
    [SerializeField] Text messageText;
    [SerializeField] Text messageTextSmall;
    [SerializeField] Animator messageAnimator;

    public List<Color> PlayerColors;

    private GameState currentState;
    private int alivePlayersCounter;
    private int winner;
    private string currentScene;

    public int NumPlayers = 0;

    public override void Awake()
    {
        currentState = GameState.TITLE_SCREEN;
        SceneManager.LoadScene("TitleScene");

        string[] joySticks = Input.GetJoystickNames();
        for(int i = 0; i < joySticks.Length; i++)
        {
            Debug.Log(joySticks[i]);
        }
        NumPlayers = joySticks.Length;

        if (NumPlayers > 4)
        {
            NumPlayers = 4;
        }

        Debug.Log("NumPlayers: " + NumPlayers);
        base.Awake();
    }

    void Start ()
    {
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) ||
           Input.GetButtonDown("Escape"))
        {
            Application.Quit();
            return;
        }

        switch(currentState)
        {
            case GameState.RESULTS:
                if (Input.GetButtonDown("AttackTop" + winner))
                {
                    AdvanceScreen();
                }
                break;
            case GameState.TITLE_SCREEN:
            case GameState.HOW_TO:
            case GameState.PLAYER_SELECT:
                if (Input.anyKeyDown)
                {
                    AdvanceScreen();
                }
                break;
        }
    }

    public void AdvanceScreen()
    {
        switch (currentState)
        {
            case GameState.RESULTS:
                currentState = GameState.COUNTDOWN;
                RestartGame();
                break;
            case GameState.TITLE_SCREEN:
                currentState = GameState.HOW_TO;
                SceneManager.LoadScene("HowToScene");
                break;
            case GameState.HOW_TO:
                currentState = GameState.PLAYER_SELECT;
                SceneManager.LoadScene("PlayerSelect");
                break;
            case GameState.PLAYER_SELECT:
                Initialize();
                break;
        }
    }

    private void Initialize()
    {
        currentState = GameState.COUNTDOWN;
        alivePlayersCounter = GameManager.Instance.NumPlayers;
        SceneManager.LoadScene("MainScene");
        currentScene = SceneManager.GetActiveScene().name;

        StartCoroutine(DoCountdown());
    }

    private void Restart()
    {
        currentState = GameState.COUNTDOWN;
        alivePlayersCounter = GameManager.Instance.NumPlayers;
        currentScene = SceneManager.GetActiveScene().name;

        StartCoroutine(DoCountdown());
    }

    private void ShowBigMessage(string message)
    {
        messageText.text = message;
        messageAnimator.SetTrigger("Show");
    }

    private void ShowFinalMessage(string message)
    {
        messageText.text = message;
        messageAnimator.SetTrigger("ShowDelayed");
    }

    public void OnPlayerDead(int playerID)
    {
        messageTextSmall.text = "Player "+ playerID + " just died! lol!";
        StartCoroutine(HideDiedMessage());

        if (currentState == GameState.RESULTS)
        {
            return;
        }

        alivePlayersCounter--;
        if (alivePlayersCounter <= 1)
        {
            if (GameManager.Instance.NumPlayers == 1)
            {
                ShowFinalMessage("Nobody Wins!");
                messageTextSmall.text = "Press Any button to Restart";
                currentState = GameState.RESULTS;
                winner = 1;
                Camera.main.GetComponent<CameraController>().StartCutting();
                return;
            }
            foreach (GameObject player in PlayerSpawner.Instance.SpawnedPlayers)
            {
                PlayerStats playerStat = player.GetComponent<PlayerStats>();
                if(playerStat.CurrentState == PlayerStats.PlayerState.ALIVE)
                {
                    winner = playerStat.PlayerID;
                    ShowFinalMessage("Player " + winner + " Wins!");
                    messageTextSmall.text = "Press A to Restart";
                    currentState = GameState.RESULTS;
                    Camera.main.GetComponent<CameraController>().StartCutting();
                }
            }
        }
    }

    private IEnumerator DoCountdown()
    {
        Debug.Log("Create Band");
        ShowBigMessage("3");
        yield return new WaitForSeconds(1);

        Debug.Log("Create Crowd");
        ShowBigMessage("2");
        yield return new WaitForSeconds(1);

        PlayerSpawner.Instance.Spawn(GameManager.Instance.NumPlayers, playerPrefab);

        ShowBigMessage("1");
        yield return new WaitForSeconds(1);
        float rand = Random.Range(0, 1);
        ShowBigMessage("Let's \n" + (rand == 0 ? "Thrash" : "Trash") +  "!!");
        currentState = GameState.ROUND_IN_PROGRESS;
        yield return new WaitForSeconds(1);
        messageText.text = "";
    }

    private IEnumerator RestartGameDelayed()
    {
        yield return new WaitForSeconds(5);
        messageText.text = "Restart in 3";
        yield return new WaitForSeconds(1);
        messageText.text = "Restart in 2";
        yield return new WaitForSeconds(1);
        messageText.text = "Restart in 1";
        yield return new WaitForSeconds(1);
        RestartGame();
    }

    private IEnumerator HideDiedMessage()
    {
        string originalMessage = messageTextSmall.text;
        yield return new WaitForSeconds(0.3f);
        messageTextSmall.text = "";
        yield return new WaitForSeconds(0.3f);
        messageTextSmall.text = originalMessage;
        yield return new WaitForSeconds(0.3f);
        messageTextSmall.text = "";
        yield return new WaitForSeconds(0.3f);
        messageTextSmall.text = originalMessage;
        yield return new WaitForSeconds(2);
        messageTextSmall.text = "";
    }

    private IEnumerator DelayForEnd()
    {
        while(!Camera.main.GetComponent<CameraController>().CanProgressGameState)
        {
            yield return null;
        }

        for(int i = 0; i < PlayerSpawner.Instance.SpawnedPlayers.Count; i++)
        {
            if(PlayerSpawner.Instance.SpawnedPlayers[i] != null)
            {
                GameObject.Destroy(PlayerSpawner.Instance.SpawnedPlayers[i]);
            }
        }

        yield return null;

        messageText.text = "";
        messageTextSmall.text = "";
        Restart();
    }
    private void RestartGame()
    {
        Camera.main.GetComponent<CameraController>().CameraCutting = false;
        StartCoroutine(DelayForEnd());
    }
}
