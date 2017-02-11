﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurferSpawner : Singleton<SurferSpawner>
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] PlayerLight[] playerLights;
    public PlayerLight[] PlayerLights { get { return playerLights; } }

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

        if (spawnPositions.Length < numPlayers) Debug.LogWarningFormat("We have %d players, but only %d spawn positions", numPlayers, spawnPositions.Length);

        for (int i = 0; i < numPlayers; i++)
        {
            GameObject surferPrefab = GameObject.Instantiate(playerPrefab, spawnPositions[i].position, playerPrefab.transform.rotation);
            SurferPlayer player = PlayerManager.Instance.Players[i];
            Surfer surfer = surferPrefab.GetComponent<Surfer>();
            surfer.SetPlayer(player);

            SurferControl control = surferPrefab.GetComponent<SurferControl>();

            // TODO: I feel like player lights should be tracking each player themselves - write new code for these
            PlayerLight pLight = playerLights[i];
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