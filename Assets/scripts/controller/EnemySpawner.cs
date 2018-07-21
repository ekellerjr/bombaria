using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public GameObject enemiesHolder;


    internal void InitSpawn(
        ushort[,] map,
        MeshGenerator.SquareGrid squareGrid,
        List<MapGenerator.Room> rooms,
        HashSet<MapGenerator.Passage> passages)
    {

        foreach (MapGenerator.Room room in rooms)
        {
            MapGenerator.Coord enemySpawnCoord = default(MapGenerator.Coord);

            int tries = 0;
            bool match = false;

            do
            {
                MapGenerator.Coord curCoord = room.tiles[UnityEngine.Random.Range(0, room.tiles.Count)];

                if (map[curCoord.tileX, curCoord.tileY] == (ushort)EnvironmentObjectType.Void)
                {
                    enemySpawnCoord = curCoord;
                    match = true;
                    break;
                }
                
            } while (tries++ <= 5);

            if (!match)
                continue;

            Vector3 enemyPosition = squareGrid.controlNodes[enemySpawnCoord.tileX, enemySpawnCoord.tileY].position;

            GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition + enemyPrefab.transform.position, enemyPrefab.transform.rotation);
            enemyObject.transform.parent = enemiesHolder.transform;
            enemyObject.layer = enemiesHolder.layer;

        }
    }
}