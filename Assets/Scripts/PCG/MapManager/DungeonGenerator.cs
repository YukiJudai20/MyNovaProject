
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class DungeonGenerator : MonoBehaviour {
    public enum CellType {
        None,
        Room,
        Hallway,
        Stairs
    }

    class Room {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size) {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    [SerializeField]
    public Vector3Int mapSize;
    [SerializeField]
    public int roomCount;
    [SerializeField]
    public Vector3Int roomMaxSize;
    [SerializeField]
    public GameObject roomPrefab;
    [SerializeField]
    public GameObject hallWayPrefab;
    [SerializeField]
    public GameObject stairsPrefab;
    [SerializeField]
    public GameObject spherePrefab;


    public int seed;
    public GameObject dungeonRoot;
    public double wayAddProbability = 0.125;

    Random random;
    Grid3D<CellType> grid;
    List<Room> rooms = new List<Room>();
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    

    void Start() {
        random = new Random(seed);
        grid = new Grid3D<CellType>(mapSize, Vector3Int.zero);
        rooms = new List<Room>();
        if (dungeonRoot == null)
        {
            dungeonRoot = GameObject.Find("DungeonRoot");
            if (dungeonRoot == null)
            {
                dungeonRoot = new GameObject("DungeonRoot");   
            }
        }
        CreateDungeon();
    }

    public void CreateDungeon()
    {
        PlaceRooms();
        EventCenter.Instance.EventTrigger("加载进度条更新", 0.92f);
        Triangulate();
        EventCenter.Instance.EventTrigger("加载进度条更新", 0.94f);
        CreateHallways();
        EventCenter.Instance.EventTrigger("加载进度条更新", 0.96f);
        PathfindHallways();
        EventCenter.Instance.EventTrigger("加载进度条更新", 0.99f);
        dungeonRoot.transform.localScale = new Vector3(4, 4, 4);
        EventCenter.Instance.EventTrigger<UnityEngine.Vector3>("DungeonGenerateDone",roomsGoList[0].transform.GetChild(0).position);
    }
    
    
    //随机放置房间
    void PlaceRooms()
    {
        PerlinNoiseGenerator perlinNoiseGenerator = new PerlinNoiseGenerator();
        List<Vector3> noisePosition = perlinNoiseGenerator.GenerateNoiseMap(mapSize, Vector3.zero);
        for (int i = 0; i < roomCount; i++) {
            // Vector3Int location = new Vector3Int(
            //     random.Next(0, mapSize.x),
            //     random.Next(0, mapSize.y),
            //     random.Next(0, mapSize.z)
            // );
            Vector3 randomNoisePos = noisePosition[random.Next(0, noisePosition.Count)];
            //Vector3Int location = new Vector3Int((int)noisePosition[i].x,(int)noisePosition[i].y,(int)noisePosition[i].z);
            Vector3Int location = new Vector3Int((int)randomNoisePos.x,(int)randomNoisePos.y,(int)randomNoisePos.z);
            //Debug.Log(location);
            Vector3Int roomSize = new Vector3Int(
                random.Next(5, roomMaxSize.x + 1),
                random.Next(3, roomMaxSize.y + 1),
                random.Next(5, roomMaxSize.z + 1)
            );
            if (roomSize.x % 2 == 0)
            {
                roomSize = new Vector3Int(roomSize.x - 1, roomSize.y, roomSize.z);
            }
            if (roomSize.z % 2 == 0)
            {
                roomSize = new Vector3Int(roomSize.x, roomSize.y, roomSize.z-1);
            }

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(3, 1, 3));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }
            
            //检查房间是否在地图边界外
            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= mapSize.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= mapSize.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= mapSize.z) {
                // Debug.Log("xMin"+newRoom.bounds.xMin+" xMax"+newRoom.bounds.xMax +" mapSizeX"+mapSize.x);
                // Debug.Log("yMin"+newRoom.bounds.yMin+" yMax"+newRoom.bounds.yMax+" mapSizeY"+mapSize.y);
                // Debug.Log("zMin"+newRoom.bounds.zMin+" zMax"+newRoom.bounds.zMax+" mapSizeZ"+mapSize.z);
                // Debug.Log("房间在地图边界外");
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);
                //在对应位置生成房间预制体
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);
                // Debug.Log(newRoom.bounds.position+" "+newRoom.bounds.size);
                //在三维网格中记录单元格类型为房间
                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    //GameObject sphere = Instantiate(spherePrefab, pos, Quaternion.identity,dungeonRoot.transform);
                    //grid[new Vector3Int(pos.x, pos.y + 1, pos.z)] = CellType.Room;
                    //grid[new Vector3Int(pos.x, pos.y + 2, pos.z)] = CellType.Room;
                    grid[pos] = CellType.Room;
                }
            }
        }
    }
    
    //德劳内三角剖分
    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();
        
        //将每个房间的中心加入到点集中  + ((Vector3)room.bounds.size) / 2
        foreach (var room in rooms)
        {
            // Vector3 pos = (Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2;
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }
        //以所有房间的中心为点集，进行德劳内三角剖分
        delaunay = Delaunay3D.Triangulate(vertices);
    }
    
    //最小生成树
    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
            //Debug.DrawLine(edge.U.Position , edge.V.Position , Color.red, 100, false);
        }
        //将德劳内三角剖分的边传入Prim算法，得到最小生成树
        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        // foreach (var edge in selectedEdges)
        // {
        //     Debug.DrawLine(edge.U.Position , edge.V.Position , Color.blue, 100, false);
        // }
        
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);
        //最小生成树以外的每条边，按一定概率加入到树中
        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < wayAddProbability) {
                selectedEdges.Add(edge);
            }
        }
    }

    private Vector3 stairHallwayOffset = new Vector3(0.5f, 0, 0.5f);
    void PathfindHallways() {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(mapSize);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = new Vector3(startRoom.bounds.center.x,startRoom.bounds.position.y,startRoom.bounds.center.z);
            var endPosf = new Vector3(endRoom.bounds.center.x,endRoom.bounds.position.y,endRoom.bounds.center.z);
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);
            Debug.DrawLine(startPos , endPos , Color.blue, 1000, false);
            float xOffset = startRoom.bounds.center.x - startRoom.bounds.position.x;
            float zOffset = startRoom.bounds.center.z - startRoom.bounds.position.z;
            var path = aStar.FindPath(startPos, endPos,xOffset,zOffset, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                //代价函数
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0) {
                    //点和其邻居在同一层
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    

                    if (grid[b.Position] == CellType.Stairs) {
                        //楼梯不能通过
                        return pathCost;
                    } 
                    if (grid[b.Position] == CellType.Room) {
                        pathCost.cost += 100;
                    } else if (grid[b.Position] == CellType.None) {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                } else {
                    //点和其邻居不在同一层 需要放楼梯
                    //如果当前点或者其邻居是房间或楼梯 则不能通过 只有空地和走廊可以放楼梯，房间内和楼梯本身不能再放楼梯
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 10 + Vector3Int.Distance(b.Position, endPos);    

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);
                    
                    //边界检查
                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset)) {
                        return pathCost;
                    }
                    
                    //检查是否有足够空间放楼梯 放楼梯需要水平两个格子 垂直一个格子 总共占四个格子
                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None) {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None) {
                        grid[current] = CellType.Hallway;
                    }
                    
                    //处理楼梯连接
                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                        //如果路径上的两个点之间需要楼梯进行移动，则构建楼梯
                        if (delta.y != 0) {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);
                            
                            grid[prev + horizontalOffset] = CellType.Stairs;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                            StairRotate stairRotate = CheckStairRotateType(current, prev);

                            PlaceStairs(prev + horizontalOffset+stairHallwayOffset,stairRotate);
                            // PlaceStairs(prev + horizontalOffset * 2);
                            // PlaceStairs(prev + verticalOffset + horizontalOffset);
                            // PlaceStairs(prev + verticalOffset + horizontalOffset * 2);
                            
                        }

                        //Debug.DrawLine(prev , current , Color.blue, 1000, false);
                    }
                }
                //放置走廊
                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway) {
                        PlaceHallway(pos+stairHallwayOffset);
                    }
                }
            }
        }
        foreach (var hallway in hallwaysGoList)
        {
            Vector3Int location = new Vector3Int((int)hallway.transform.position.x, (int)hallway.transform.position.y,
                (int)hallway.transform.position.z);
            hallway.GetComponent<HallwayCheck>().CheckAndDelWall(grid,location);
        }
        foreach (var stairs in stairsGoList)
        {
            Vector3Int location = new Vector3Int((int)stairs.transform.position.x, (int)stairs.transform.position.y,
                (int)stairs.transform.position.z);
            stairs.GetComponent<StairsCheck>().CheckAndDelStairsWall(grid,location);
        }
        // for (int i = roomsGoList.Count - 1; i >= 0; i--)
        // {
        //     RoomCheck roomCheck = roomsGoList[i].GetComponent<RoomCheck>();
        //     if (roomCheck.IsRoomValid())
        //     {
        //         continue;
        //     }
        //     roomsGoList.RemoveAt(i);
        // }
    }
    private List<GameObject> roomsGoList = new List<GameObject>();
    private List<GameObject> hallwaysGoList = new List<GameObject>();
    private List<GameObject> stairsGoList = new List<GameObject>();
    enum StairRotate
    {
        None,
        XUpYUp,
        ZUpYDown,
        XDownYDown,
        YUpZDown,
        YUpZUp,
        YDownZDown,
        XDownYUp,
        XUpYDown,
    }

    private StairRotate CheckStairRotateType(Vector3Int current, Vector3Int prev)
    {
        StairRotate stairRotate = StairRotate.None;
        if (current.x - prev.x == 3 && current.y - prev.y == 1)
        {
            stairRotate = StairRotate.XUpYUp;
        }
        else if (current.y - prev.y == -1 && current.z - prev.z == 3)
        {
            stairRotate = StairRotate.ZUpYDown;
        }
        else if (current.x - prev.x == -3 && current.y - prev.y ==-1)
        {
            stairRotate = StairRotate.XDownYDown;
        }
        else if (current.y - prev.y ==1 && current.z - prev.z ==-3)
        {
            stairRotate = StairRotate.YUpZDown;
        }
        else if(current.y - prev.y ==1 && current.z - prev.z == 3)
        {
            stairRotate = StairRotate.YUpZUp;
        }
        else if(current.y - prev.y ==-1 && current.z - prev.z == -3)
        {
            stairRotate = StairRotate.YDownZDown;
        }
        else if(current.x - prev.x == -3 && current.y - prev.y == 1)
        {
            stairRotate = StairRotate.XDownYUp;
        }
        else if (current.x - prev.x == 3 && current.y - prev.y == -1)
        {
            stairRotate = StairRotate.XUpYDown;
        }
        return stairRotate;
    }

    void PlaceGrid(Vector3 location, Vector3Int size, GridType type,StairRotate stairRotate = StairRotate.None) {
        switch (type)
        {
            case GridType.Room:
                GameObject room = Instantiate(roomPrefab, location, Quaternion.identity,dungeonRoot.transform);
                Vector3 roomScale = room.GetComponent<Transform>().localScale;
                room.GetComponent<Transform>().localScale = new Vector3(roomScale.x * size.x,roomScale.y * size.y,roomScale.z*size.z);
                roomsGoList.Add(room);
                //room.GetComponent<Transform>().localScale = size;
                break;
            case GridType.Stairs:
                Quaternion quaternion = Quaternion.identity;
                GameObject stairs = Instantiate(stairsPrefab, location, quaternion,dungeonRoot.transform);
                stairsGoList.Add(stairs);
                switch (stairRotate)
                {
                    case StairRotate.XUpYUp:
                        quaternion = Quaternion.Euler(0, 270, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0.25f);
                        break;
                    case StairRotate.ZUpYDown:
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, -0.5f, 0.75f);
                        break;
                    case StairRotate.XDownYDown:
                        quaternion = Quaternion.Euler(0, 270, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, -0.5f, 0.75f);
                        break;
                    case StairRotate.YUpZDown:
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0.25f);
                        break;
                    case StairRotate.YUpZUp:
                        quaternion = Quaternion.Euler(0, 180, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0.25f);
                        break;
                    case StairRotate.YDownZDown:
                        quaternion = Quaternion.Euler(0, 180, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0f, -0.5f, 0.75f);
                        break;
                    case StairRotate.XDownYUp:
                        quaternion = Quaternion.Euler(0, 90, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0.25f);
                        break;
                    case StairRotate.XUpYDown:
                        quaternion = Quaternion.Euler(0, 90, 0);
                        stairs.transform.localRotation = quaternion;
                        stairs.transform.GetChild(0).transform.localPosition = new Vector3(0, -0.5f, 0.75f);
                        break;
                }
                Vector3 stairsScale = stairs.GetComponent<Transform>().localScale;
                stairs.GetComponent<Transform>().localScale = new Vector3(stairsScale.x * size.x,stairsScale.y * size.y,stairsScale.z*size.z);
                //stairs.GetComponent<Transform>().localScale = size;
                break;
            case GridType.Hallway:
                GameObject hallway = Instantiate(hallWayPrefab, location, Quaternion.identity,dungeonRoot.transform);
                Vector3 hallwayScale = hallway.GetComponent<Transform>().localScale;
                hallway.GetComponent<Transform>().localScale = new Vector3(hallwayScale.x * size.x,hallwayScale.y * size.y,hallwayScale.z*size.z);
                hallwaysGoList.Add(hallway);
                //hallway.GetComponent<Transform>().localScale = size;
                break;
        }
    }

    void PlaceRoom(Vector3 location, Vector3Int size) {
        PlaceGrid(location, size, GridType.Room);
    }

    void PlaceHallway(Vector3 location) {
        PlaceGrid(location, new Vector3Int(1, 1, 1), GridType.Hallway);
    }

    void PlaceStairs(Vector3 location,StairRotate stairRotate = StairRotate.None) {
        
        PlaceGrid(location, new Vector3Int(1, 1, 1), GridType.Stairs,stairRotate);
    }
}

public enum GridType
{
    None,
    Room,
    Stairs,
    Hallway
}
