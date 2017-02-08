using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : Singleton<PlayerSpawner>
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] PlayerLight[] playerLights;
    public PlayerLight[] PlayerLights { get { return playerLights; } }

    private List<GameObject> spawnedPlayers;
    public List<GameObject> SpawnedPlayers { get { return spawnedPlayers; } }

    public override void Awake ()
    {
        spawnedPlayers = new List<GameObject>();
        base.Awake();
    }

    public void Spawn(int playerAmount, GameObject playerPrefab)
    {
        spawnedPlayers.Clear();
        if (spawnPositions.Length >= playerAmount)
        {
            for (int i = 0; i < playerAmount; i++)
            {
                GameObject newPlayer = GameObject.Instantiate(playerPrefab, spawnPositions[i].position, playerPrefab.transform.rotation);
                Surfer surfer = newPlayer.GetComponent<Surfer>();
                SurferControl control = newPlayer.GetComponent<SurferControl>();

                PlayerLight pLight = playerLights[i];
                pLight.GetComponent<Light>().color = GameManager.Instance.PlayerColors[i];
                pLight.enabled = true;
                pLight.GetComponent<Light>().enabled = true;
                pLight.gameObject.SetActive(true);
                pLight.followPlayer = control.gameObject;
                surfer.CurrentLight = pLight;

                ///surfer.PlayerID = i + 1;
                spawnedPlayers.Add(newPlayer);
            }
        }
    }
}
