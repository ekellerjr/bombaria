using System;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour
{

    public static readonly string[] environmentObjectTypeNames = Enum.GetNames(typeof(EnvironmentObjectType));

    [Header("Settings")]
    public GameObject environment;

    [Header("Prefabs")]
    public GameObject stonePrefab;

    private MapGenerator.Room mainRoom;

    private MeshGenerator.ControlNode playerStartNode;

    private GameObject player;

    private void Init() { }

    internal void GenerateEnvorionment(
        ushort[,] map,
        MeshGenerator.SquareGrid squareGrid,
        ICollection<MapGenerator.Room> rooms,
        ICollection<MapGenerator.Passage> passages)
    {

        Init();

        PrepareMainRoom(rooms);

        foreach (MapGenerator.Passage passage in passages)
        {
            foreach (MapGenerator.Coord coord in passage.passageCoords)
            {
                MeshGenerator.ControlNode stoneNode = squareGrid.controlNodes[coord.tileX, coord.tileY];
                Vector3 stonePosition = stoneNode.position;

                GameObject stoneObject = Instantiate(stonePrefab, stonePosition + stonePrefab.transform.position, stonePrefab.transform.rotation);
                stoneObject.transform.parent = environment.transform;
                stoneObject.layer = environment.layer;
                stoneObject.isStatic = environment.isStatic;

                map[stoneNode.mapCoords.tileX, stoneNode.mapCoords.tileY] = (ushort)EnvironmentObjectType.Stone;

            }
        }

        PreparePlayerStartPosition(map, squareGrid);
    }

    private void PreparePlayerStartPosition(ushort[,] map, MeshGenerator.SquareGrid squareGrid)
    {
        if (mainRoom == null)
            return;

        int tries = 0;
        bool valid = false;

        do
        {
            MapGenerator.Coord playerStartCoord = mainRoom.tiles[UnityEngine.Random.Range(0, mainRoom.tiles.Count - 1)];
            playerStartNode = squareGrid.controlNodes[playerStartCoord.tileX, playerStartCoord.tileY];

            valid = playerStartNode != null
                && !playerStartNode.active // active means wall
                && map[playerStartNode.mapCoords.tileX, playerStartNode.mapCoords.tileY] == (ushort)EnvironmentObjectType.Void;

        } while (!valid && tries++ <= mainRoom.tiles.Count);

        if (playerStartNode == null || playerStartNode.active)
        {
            Debug.Log("Can't find player start node");
        }
        else
        {
            map[playerStartNode.mapCoords.tileX, playerStartNode.mapCoords.tileY] = (ushort)EnvironmentObjectType.PlayerStart;
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

        if (mainRoom == null)
        {
            Debug.Log("Can´t find main room");
        }
    }

    internal MeshGenerator.ControlNode GetPlayerStartNode()
    {
        return this.playerStartNode;
    }
}
