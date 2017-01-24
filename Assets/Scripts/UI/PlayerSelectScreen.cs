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
        if(GameManager.Instance.NumPlayers < 1)
        {
            if (NoControllerNotif != null) NoControllerNotif.SetActive(true);
            noControllers = true;
        }

        for(int i = 0; i < PlayerIndicators.Count; i++)
        {
            if (i < GameManager.Instance.NumPlayers)
            {
                PlayerIndicators[i].gameObject.SetActive(true);
                PlayerIndicators[i].PlayerText.text = "Player: " + (i + 1);
                PlayerIndicators[i].PlayerText.color = GameManager.Instance.PlayerColors[i];
                PlayerIndicators[i].PlayerImage.color = GameManager.Instance.PlayerColors[i];
            }
            else
            {
                PlayerIndicators[i].gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        if(noControllers)
        {
            NoControllerCountdown -= Time.deltaTime;
            if (NoControllerCountdown < 0f) GameManager.Instance.AdvanceScreen();
        }
    }
}
