using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance{get{return instance;}}

    public float roadTileGap = 39.86f;
    public Transform roadTilesParent;
    public Transform obstaclesParent;
    public Transform roadTilePrefab;
    public Transform finishPrefab;
    public Vector3 firstTilePosition;
    public List<Transform> spawnedRoadTiles;
    public PlayerController player;
    public int finishLineIndex = 20;
    public int spawnedRoadLastIndex = 0;
    public int numberOfRoadsAfterFinish = 5;
    public bool isFinished = false;
    public float obstacleMinPosX = 2;
    public float obstacleMaxPosX = 3.5f;
    public List<GameObject> obstaclePrefabs;
    public List<GameObject> obstacles;
    [Header("UI")]
    public GameObject GameOverMenu;
    public GameObject LevelFinishMenu;
    public AudioClip carCrashSoundClip;
    private Vector3 tempTilePosVector;
    private int spawnedRoadFirstIndex = 0;
    private Vector3 prevRoadVec;
    private Vector3 playerPosVec;
    private Vector3 obstaclePosVec;
    private bool isSpawningRoadFinished = false;
    private bool enableObstacleSpawn = false;
    private int spawnedObstacleCount = 0;
    private AudioSource audioSource;


    void Awake()
    {
        obstacles = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        instance = this;
        for (spawnedRoadLastIndex = 0; spawnedRoadLastIndex < 10; spawnedRoadLastIndex++)
        {
            tempTilePosVector.z = roadTileGap*spawnedRoadLastIndex;
            spawnedRoadTiles.Add(Instantiate(roadTilePrefab, firstTilePosition + tempTilePosVector, Quaternion.identity, roadTilesParent));
        }

        if(obstaclePrefabs != null && obstacles != null)
        {
            for (int i = 0; i < obstaclePrefabs.Count; i++)
            {
                ColliderAction colliderAction = obstaclePrefabs[i].GetComponent<ColliderAction>();
                if(colliderAction != null)
                {
                    int max = 2;
                    if(colliderAction.colliderType == ColliderTypes.Halt)
                    {
                        max = 2;
                    }
                    else
                    {
                        max = 10;
                    }
                    for (int j = 0; j < max; j++)
                    {
                        GameObject clone =  Instantiate(obstaclePrefabs[i],obstaclePrefabs[i].transform.position, obstaclePrefabs[i].transform.rotation, obstaclesParent);
                        clone.SetActive(false);
                        obstacles.Add(clone);

                        clone = Instantiate(obstaclePrefabs[i], obstaclePrefabs[i].transform.position, obstaclePrefabs[i].transform.rotation, obstaclesParent);
                        clone.SetActive(false);
                        obstacles.Add(clone);
                    }
                }
            }
        }
        ShuffleList(obstacles);
        enableObstacleSpawn = true;
    }

    void FixedUpdate()
    {
        RoadTileSpawnControl();
        if(enableObstacleSpawn == true)
        {
            ObstaclesControl();
        }
    }
     
    public void PlayCarCrashSound()
    {
        audioSource.clip = carCrashSoundClip;
        audioSource.Play();
    }
    private void ObstaclesControl()
    {
        if(isFinished == true)
        {
            return;
        }
        float obstacleSpawnGap = Mathf.Abs(player.transform.position.z - roadTileGap*(finishLineIndex));
        float maxSpawnDist = 30 + 10*(spawnedObstacleCount+1);
        float minSpawnDist = 20 + 10*(spawnedObstacleCount+1);
        // print("------------- ");
        // print("obstacleSpawnGap "+ obstacleSpawnGap);
        // print("maxSpawnDist "+ maxSpawnDist);
        // print("minSpawnDist "+ minSpawnDist);

        if(obstacleSpawnGap <= maxSpawnDist && obstacleSpawnGap <= minSpawnDist)
        {
            return;
        }

        float randomZ = UnityEngine.Random.Range(minSpawnDist, maxSpawnDist);
        for (int i = 0; i < obstacles.Count; i++)
        {
            if(obstacles[i].gameObject.activeSelf == false)
            {
                obstaclePosVec = obstacles[i].transform.position;
                obstaclePosVec.z = player.transform.position.z + randomZ;
                
                int dir = 1;
                if(UnityEngine.Random.value > 0.5f)
                {
                    dir = -1;
                }

                obstaclePosVec.x = UnityEngine.Random.Range(obstacleMinPosX, obstacleMaxPosX)*dir;
                obstacles[i].gameObject.SetActive(true);
                obstacles[i].transform.SetPositionAndRotation(obstaclePosVec, obstacles[i].transform.rotation);
                spawnedObstacleCount++;
                return;
            }
        }
    }

    void ShuffleList(List<GameObject> list)
    {
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = UnityEngine.Random.Range(0, n);  
            GameObject value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
    void RoadTileSpawnControl()
    {
        if(spawnedRoadLastIndex + 1> finishLineIndex + numberOfRoadsAfterFinish)
        {
            if(isSpawningRoadFinished == false)
            {
                isSpawningRoadFinished = true;
                tempTilePosVector.z = roadTileGap*(finishLineIndex);
                tempTilePosVector = firstTilePosition + tempTilePosVector;
                tempTilePosVector.x = finishPrefab.transform.position.x;
                tempTilePosVector.y = finishPrefab.transform.position.y;
                Instantiate(finishPrefab, tempTilePosVector, Quaternion.identity, roadTilesParent);
            }
            return;
        }

        if(spawnedRoadTiles.Count > 0 && spawnedRoadFirstIndex < spawnedRoadTiles.Count)
        {
            prevRoadVec.z = spawnedRoadTiles[spawnedRoadFirstIndex].transform.position.z;
            playerPosVec.z = player.transform.position.z;
            if(Vector3.Distance(prevRoadVec, playerPosVec) > 35)
            {
                tempTilePosVector.z = roadTileGap*spawnedRoadLastIndex;
                spawnedRoadTiles[spawnedRoadFirstIndex].transform.position = firstTilePosition + tempTilePosVector;
                spawnedRoadLastIndex++;
                if(spawnedRoadFirstIndex + 1 < spawnedRoadTiles.Count)
                {
                    spawnedRoadFirstIndex++;
                }
                else
                {
                    spawnedRoadFirstIndex = 0;
                }
            }
        }
    }

    public void OpenGameOverMenu()
    {
        if(GameOverMenu)
        {
            GameOverMenu.SetActive(true);
        }
    }
    public void RestartLevel()
    {
        if(GameOverMenu)
        {
            GameOverMenu.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void StartNextLevel()
    {
        if(LevelFinishMenu)
        {
            LevelFinishMenu.SetActive(false);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    internal void OpenLevelFinishedMenu()
    {
        if(LevelFinishMenu)
        {
            LevelFinishMenu.SetActive(true);
        }
    }
}
