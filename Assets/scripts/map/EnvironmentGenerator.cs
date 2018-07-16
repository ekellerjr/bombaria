using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour {

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject stonePrefab;

    private string seed; 

    private void Init()
    {

    }


    internal void GenerateEnvorionment(
        ushort[,] map, MeshGenerator.SquareGrid squareGrid, ICollection<MapGenerator.Room> rooms, ICollection<MapGenerator.Passage> passages)
    {

        Init();

        MapGenerator.Room mainRoom = PrepareMainRoom(map, squareGrid, rooms);

        
        foreach (MapGenerator.Passage passage in passages)
        {
            foreach (MapGenerator.Coord coord in passage.passageCoords)
            {
                Vector3 stonePosition = squareGrid.controlNodes[coord.tileX,coord.tileY].position;

                Instantiate(stonePrefab, stonePosition + stonePrefab.transform.position, stonePrefab.transform.rotation);
            }
        }
        
    }

    private MapGenerator.Room PrepareMainRoom(ushort[,] map,  MeshGenerator.SquareGrid squareGrid, ICollection<MapGenerator.Room> rooms)
    {
        MapGenerator.Room mainRoom = null;

        foreach (MapGenerator.Room room in rooms)
        {
            if (room.isMainRoom)
            {
                mainRoom = room;
                break;
            }
        }

        Random.InitState(seed.GetHashCode());
        
        MapGenerator.Coord playerStartCoord = mainRoom.tiles[Random.Range(0, mainRoom.tiles.Count - 1)];

        // Vector3 playerStartPosition = squareGrid.GetPosition(playerStartCoord);

        Vector3 playerStartPosition = squareGrid.controlNodes[playerStartCoord.tileX, playerStartCoord.tileY].position;

        if (playerStartPosition == Vector3.negativeInfinity)
        {
            Debug.Log("Can not find player start position from playerStartCoord: " + playerStartCoord);
        }
        else
        {
            Instantiate(playerPrefab, playerStartPosition + playerPrefab.transform.position, playerPrefab.transform.rotation);
        }

        return mainRoom;
    }

    internal void SetSeed(string seed)
    {
        this.seed = seed;
    }

}
