using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    [Header("Debug")]
    public bool debugMode = true;
    public KeyCode rebuildMapKey = KeyCode.R;

    [Header("World")]
    public string seed;
    public bool useRandomSeed;
    public GameObject mapGeneratorPrefab;

    [Header("Player")]
    public GameObject playerPrefab;

    private Player player;

    private MapGenerator mapGenerator;

    private bool gameRunning;

    private bool aiActive;

    public bool IsGameRunning()
    {
        return gameRunning;
    }

    public bool IsAIActive()
    {
        return aiActive;
    }

	// Use this for initialization
	void Start () {

        Init();
        StartGame();
	}

    private void StartGame()
    {
        gameRunning = true;
        aiActive = true;
    }

    private void Init()
    {
        InitGameState();
        InitMapGenerator();
        InitPlayer();
        InitWorld();
    }

    private void InitGameState()
    {
        gameRunning = false;
        aiActive = false;

        if (useRandomSeed)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        UnityEngine.Random.InitState(seed.GetHashCode());
    }

    private void InitPlayer()
    {
        player = CommonUtils.GetComponentOrPanic<Player>(Instantiate(playerPrefab));
        player.gameObject.SetActive(false);
    }

    private void InitMapGenerator()
    {
        mapGenerator = CommonUtils.GetComponentOrPanic<MapGenerator>(Instantiate(mapGeneratorPrefab));
    }

	// Update is called once per frame
	void Update ()
    {
        if (!gameRunning)
            return;

		if(debugMode && Input.GetKeyDown(rebuildMapKey))
        {
            Destroy(mapGenerator.gameObject);
            InitMapGenerator();
            InitWorld();
        }
	}

    private void InitWorld()
    {
        mapGenerator.GenerateMap();

        Vector3 playerStartPosition = mapGenerator.GetPlayerStartPosition();

        if(playerStartPosition.Equals(Vector3.negativeInfinity))
        {
            player.gameObject.SetActive(false);
        }
        else
        {
            player.gameObject.transform.position = playerStartPosition + playerPrefab.transform.position;
            player.gameObject.SetActive(true);
        }
    }
}