using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float roadTileGap = 39.86f;
    public Transform roadTilesParent;
    public Transform roadTilePrefab;
    public Vector3 firstTilePosition;
    public List<Transform> spawnedRoadTiles;
    private Vector3 tempTilePosVector;
    private int spawnedRoadIndex = 0;

    void Start()
    {
        for (spawnedRoadIndex = 0; spawnedRoadIndex < 400; spawnedRoadIndex++)
        {
            tempTilePosVector.z = roadTileGap*spawnedRoadIndex;
            spawnedRoadTiles.Add(Instantiate(roadTilePrefab, firstTilePosition + tempTilePosVector, Quaternion.identity));
        }
    }

}
