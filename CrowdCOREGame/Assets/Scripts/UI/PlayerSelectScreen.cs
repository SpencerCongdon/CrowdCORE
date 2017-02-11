using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectScreen : MonoBehaviour
{
    public List<PlayerIndicator> PlayerIndicators;
    public GameObject NoControllerNotif;

    public float NoControllerCountdown = 3.0f;

    bool noControllers = false;

    void Awake()
    {
        PlayerManager.Instance.PlayerJoined.AddListener(OnPlayerJoined);

        if(PlayerManager.Instance.NumPlayers < 1)
        {
            if (NoControllerNotif != null) NoControllerNotif.SetActive(true);
            noControllers = true;
        }
        else
        {
            foreach(SurferPlayer p in PlayerManager.Instance.Players)
            {
                int id = p.PlayerID;
                PlayerIndicators[id].gameObject.SetActive(true);
                PlayerIndicators[id].PlayerText.text = "Player: " + id;
                PlayerIndicators[id].PlayerText.color = GameManager.Instance.PlayerColors[id];
                PlayerIndicators[id].PlayerImage.color = GameManager.Instance.PlayerColors[id];
            }
            
        }
    }

    void Update()
    {
        
    }

    private void OnPlayerJoined(SurferPlayer newPlayer)
    {
        int id = newPlayer.PlayerID;
        PlayerIndicators[id].gameObject.SetActive(true);
        PlayerIndicators[id].PlayerText.text = "Player: " + id;
        PlayerIndicators[id].PlayerText.color = GameManager.Instance.PlayerColors[id];
        PlayerIndicators[id].PlayerImage.color = GameManager.Instance.PlayerColors[id];

        if(noControllers)
        {
            NoControllerNotif.SetActive(false);
            noControllers = false;
        }
    }
}
