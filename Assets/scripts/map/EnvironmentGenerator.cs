using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour {

    [Header("Settings")]
    public GameObject environment;

    [Header("Prefabs")]
    public GameObject stonePrefab;

    private MapGenerator.Room mainRoom;

    private MeshGenerator.ControlNode playerStartNode;

    private GameObject player;

    private string seed; 

    private void Init(string seed)
    {
        this.seed = seed;
    }

    internal void GenerateEnvorionment(
        ushort[,] map,
        MeshGenerator.SquareGrid squareGrid,
        ICollection<MapGenerator.Room> rooms,
        ICollection<MapGenerator.Passage> passages,
        string seed)
    {

        Init(seed);

        PrepareMainRoom(rooms);
        PreparePlayerStartPosition(squareGrid);

        foreach (MapGenerator.Passage passage in passages)
        {
            foreach (MapGenerator.Coord coord in passage.passageCoords)
            {
                Vector3 stonePosition = squareGrid.controlNodes[coord.tileX,coord.tileY].position;

                GameObject stoneObject = Instantiate(stonePrefab, stonePosition + stonePrefab.transform.position, stonePrefab.transform.rotation);
                stoneObject.transform.parent = environment.transform;
                stoneObject.layer = environment.layer;
                stoneObject.isStatic = environment.isStatic;

            }
        }   
    }

    private void PreparePlayerStartPosition(MeshGenerator.SquareGrid squareGrid)
    {
        if (mainRoom == null)
            return;

        Random.InitState(seed.GetHashCode());
        
        int tries = 0;

        do
        {
            MapGenerator.Coord playerStartCoord = mainRoom.tiles[Random.Range(0, mainRoom.tiles.Count - 1)];
            playerStartNode = squareGrid.controlNodes[playerStartCoord.tileX, playerStartCoord.tileY];

        } while (playerStartNode != null && (!playerStartNode.active || tries++ <= 5));

        if (playerStartNode == null || !playerStartNode.active)
        {
            Debug.Log("Can't find player start node");
        }
    }

    private void PrepareMainRoom(ICollection<MapGenerator.Room> rooms)
    {
        foreach (MapGenerator.Room room in rooms)
        {
            if (room.isMainRoom)
            {
                mainRoom = room;
                break;
            }
        }

        if(mainRoom == null)
        {
            Debug.Log("Can´t find main room");
        }
    }

    internal MeshGenerator.ControlNode GetPlayerStartNode()
    {
        return this.playerStartNode;
    }
}
