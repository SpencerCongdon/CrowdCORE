using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferSpawner : Singleton<SurferSpawner>
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] SurferLight[] playerLights;
    public SurferLight[] PlayerLights { get { return playerLights; } }

    private List<GameObject> spawnedSurfers;
    public List<GameObject> SpawnedPlayers { get { return spawnedSurfers; } }

    public override void Awake ()
    {
        spawnedSurfers = new List<GameObject>();
        base.Awake();
    }

    public void Spawn(GameObject playerPrefab)
    {
        spawnedSurfers.Clear();

        int numPlayers = PlayerManager.Instance.NumPlayers;

        if (spawnPositions.Length < numPlayers) GameLog.LogWarningFormat("We have %d players, but only %d spawn positions", GameLog.Category.Surfer, numPlayers, spawnPositions.Length);

        for (int i = 0; i < numPlayers; i++)
        {
            GameObject surferPrefab = GameObject.Instantiate(playerPrefab, spawnPositions[i].position, playerPrefab.transform.rotation);
            SurferPlayer player = PlayerManager.Instance.Players[i];

            // TODO: Spawn AI Players
            HumanSurfer surfer = surferPrefab.GetComponent<HumanSurfer>();
            surfer.SetPlayer(player);

            // TODO: I feel like player lights should be tracking each player themselves - write new code for these
            SurferLight pLight = playerLights[i];
            pLight.GetComponent<Light>().color = GameManager.Instance.PlayerColors[i];
            pLight.enabled = true;
            pLight.GetComponent<Light>().enabled = true;
            pLight.gameObject.SetActive(true);
            pLight.followSurfer = surfer;
            surfer.CurrentLight = pLight;

            // Track surfers
            spawnedSurfers.Add(surferPrefab);
        }

    }
}
