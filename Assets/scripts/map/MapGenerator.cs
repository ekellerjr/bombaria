using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{

    [Header("Settings")]
    public int width;
    public int height;
    public string seed;
    public bool useRandomSeed;
    [Range(0, 100)]
    public ushort randomFillPercent;
    public int wallThresholdSize = 50;
    public int roomThresholdSize = 50;
    public int roomPassageSize = 5;
    
    private ushort[,] map;

    private HashSet<Passage> passages;
    private List<Room> rooms;

    private bool generated = false;

    private MeshGenerator meshGen;
    private EnvironmentGenerator envGen;
    private NavMeshSurface navMeshSurface;

    private void Init()
    {
        map = new ushort[width, height];

        passages = new HashSet<Passage>();

        rooms = new List<Room>();

        if (useRandomSeed)
        {
            seed = System.DateTime.Now.Ticks.ToString();
        }

        meshGen = CommonUtils.GetComponentOrPanic<MeshGenerator>(this.gameObject);
        
        envGen = CommonUtils.GetComponentOrPanic<EnvironmentGenerator>(this.gameObject);

        navMeshSurface = CommonUtils.GetComponentOrPanic<NavMeshSurface>(this.gameObject);
    }

    public void GenerateMap()
    {
        Init();

        generated = false;
        
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }

        ProcessMap();

        int borderSize = 1;

        ushort[,] borderedMap = new ushort[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize)
                {
                    borderedMap[x, y] = map[x - borderSize, y - borderSize];
                }
                else
                {
                    borderedMap[x, y] = 1;
                }
            }
        }

        
        meshGen.GenerateMesh(borderedMap);
        
        envGen.GenerateEnvorionment(borderedMap, meshGen.GetSquareGrid(), rooms, passages, seed);

        navMeshSurface.BuildNavMesh();

        generated = true;
        
    }

    internal Vector3 GetPlayerStartPosition()
    {
        return envGen.GetPlayerStartNode() == null || !envGen.GetPlayerStartNode().active ? Vector3.negativeInfinity : envGen.GetPlayerStartNode().position;
    }

    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(1);
        
        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < wallThresholdSize)
            {
                foreach (Coord tile in wallRegion)
                {
                    map[tile.tileX, tile.tileY] = 0;
                }
            }
        }

        List<List<Coord>> roomRegions = GetRegions(0);
       
        foreach (List<Coord> roomRegion in roomRegions)
        {
            if (roomRegion.Count < roomThresholdSize)
            {
                foreach (Coord tile in roomRegion)
                {
                    map[tile.tileX, tile.tileY] = 1;
                }
            }
            else
            {
                rooms.Add(new Room(roomRegion, map));
            }
        }

        if(rooms.Count > 0)
        {
            rooms.Sort();
            rooms[0].isMainRoom = true;
            rooms[0].isAccessibleFromMainRoom = true;
        }
        
        ConnectClosestRooms(rooms);

    }

    void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false)
    {

        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in allRooms)
            {
                if (room.isAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = allRooms;
            roomListB = allRooms;
        }

        int bestDistance = 0;

        Coord bestTileA = new Coord();
        Coord bestTileB = new Coord();

        Room bestRoomA = new Room();
        Room bestRoomB = new Room();

        bool possibleConnectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                possibleConnectionFound = false;
                if (roomA.connectedRooms.Count > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB || roomA.IsConnected(roomB))
                {
                    continue;
                }

                for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++)
                {
                    for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++)
                    {
                        Coord tileA = roomA.edgeTiles[tileIndexA];
                        Coord tileB = roomB.edgeTiles[tileIndexB];
                        
                        int distanceBetweenRooms = 
                            (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

                        if (distanceBetweenRooms < bestDistance || !possibleConnectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            possibleConnectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (possibleConnectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }

        }

        if (possibleConnectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(allRooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(allRooms, true);
        }
    }

    void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
    {
        Room.ConnectRooms(roomA, roomB);
        //Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

        List<Coord> line = GetLine(tileA, tileB);

        Dictionary<int, Coord> passageCoords = new Dictionary<int, Coord>();

        foreach (Coord c in line)
        {
            List<Coord> circle = DrawCircle(c, roomPassageSize);

            foreach (Coord cc in circle)
            {
                if (!passageCoords.ContainsKey(cc.Key()))
                {
                    passageCoords.Add(cc.Key(), cc);
                }
            }   
        }

        Coord[] passageCoordArray = new Coord[passageCoords.Count];
        passageCoords.Values.CopyTo(passageCoordArray, 0);

        Passage passage = new Passage(roomA, roomB, passageCoordArray);
        if (passages.Contains(passage))
        {
            Debug.Log("Passage already collected ... is this bad?");
        }
        else
        {
            passages.Add(passage);
        }
        
    }

    internal class Passage
    {
        public Room source;
        public Room destination;
        public Coord[] passageCoords;

        public Passage(Room source, Room destination, Coord[] passageCoords)
        {
            this.source = source;
            this.destination = destination;
            this.passageCoords = passageCoords;
        }
        
        public override int GetHashCode()
        {
            return source.GetHashCode() + destination.GetHashCode();
        }

    }

    List<Coord> DrawCircle(Coord c, int r)
    {

        List<Coord> circle = new List<Coord>();

        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.tileX + x;
                    int drawY = c.tileY + y;

                    if (CommonUtils.IsInRange(drawX, drawY, width, height))
                    {
                        map[drawX, drawY] = 0;

                        Coord cc = new Coord(drawX, drawY);
                        circle.Add(cc);
                    }

                }
            }
        }

        return circle;
    }

    List<Coord> GetLine(Coord from, Coord to)
    {
        List<Coord> line = new List<Coord>();

        int x = from.tileX;
        int y = from.tileY;

        int dx = to.tileX - from.tileX;
        int dy = to.tileY - from.tileY;

        bool inverted = false;

        int step = (int)Mathf.Sign(dx);
        int gradientStep = (int)Mathf.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = (int)Mathf.Sign(dy);
            gradientStep = (int)Mathf.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new Coord(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;
            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
    }

    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();

        bool[,] mapFlags = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapFlags[x, y] == false && map[x, y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                    {
                        mapFlags[tile.tileX, tile.tileY] = true;
                    }
                }
            }
        }

        return regions;
    }

    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();

        bool[,] mapFlags = new bool[width, height];

        int tileType = map[startX, startY];

        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(new Coord(startX, startY));

        mapFlags[startX, startY] = true;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (CommonUtils.IsInRange(x, y, width, height) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == false && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = true;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }

    void RandomFillMap()
    {
        Random.InitState(seed.GetHashCode());
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    ushort nextRandom = (ushort)Random.Range(0, 100);
                    map[x, y] = (nextRandom <= randomFillPercent) ? (ushort) 1 :(ushort) 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (CommonUtils.IsInRange(neighbourX, neighbourY, width, height))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    internal struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }

        public int Key()
        {
            return (tileX.ToString() + tileY.ToString()).GetHashCode();
        }

        public override string ToString()
        {
            return "(x:" + tileX + ", y" + tileY + ", key: " + Key() + ")"; 
        }
    }


    internal class Room : System.IComparable<Room>
    {
        public List<Coord> tiles;
        public List<Coord> edgeTiles;

        public List<Room> connectedRooms;

        public int roomSize;

        public bool isAccessibleFromMainRoom;
        public bool isMainRoom;

        public Room()
        {
        }

        public Room(List<Coord> roomTiles, ushort[,] map)
        {
            tiles = roomTiles;
            roomSize = tiles.Count;
            connectedRooms = new List<Room>();

            edgeTiles = new List<Coord>();

            foreach (Coord tile in tiles)
            {
                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                    {
                        if (x == tile.tileX || y == tile.tileY)
                        {
                            if (map[x, y] == 1)
                            {
                                edgeTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        public void SetAccessibleFromMainRoom()
        {
            if (!isAccessibleFromMainRoom)
            {
                isAccessibleFromMainRoom = true;
                foreach (Room connectedRoom in connectedRooms)
                {
                    connectedRoom.SetAccessibleFromMainRoom();
                }
            }
        }

        public static void ConnectRooms(Room roomA, Room roomB)
        {
            if (roomA.isAccessibleFromMainRoom)
            {
                roomB.SetAccessibleFromMainRoom();
            }
            else if (roomB.isAccessibleFromMainRoom)
            {
                roomA.SetAccessibleFromMainRoom();
            }
            roomA.connectedRooms.Add(roomB);
            roomB.connectedRooms.Add(roomA);
        }

        public bool IsConnected(Room otherRoom)
        {
            return connectedRooms.Contains(otherRoom);
        }

        public int CompareTo(Room otherRoom)
        {
            return otherRoom.roomSize.CompareTo(roomSize);
        }
    }

}
