using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MazeLoader : MonoBehaviour
{
    public static LineRenderer line;

    public static GameObject hoveredSwat = null,
                             prevHoveredSwat = null,
                             selectedSwat = null,
                             previousHittedTile = null;

    public bool DevelopmentMode = false;

    public Light mainCameraLight;

    public GameObject plane,
                      door,
                      character;

    public GameObject[] wall, obstacles;

    public Material[] floorTypeMaterial;

    public Tile[][] map;

    private const int MIN_LENGTH = 4, // Smallest Room Size
                      OBS_LIGHT = 0,
                      OBS_CHAIR = 1,
                      OBS_TABLE = 2,
                      OBS_SOFA_2 = 3,
                      OBS_SOFA_3 = 4,
                      HEAD_MATERIAL = 1, // Swat's Head Material Index
                      BODY_MATERIAL = 0; // Swat's Body Material Index

    private Room biggestRoom;

    public List<Room> rooms = new List<Room>();

    private GameObject tilesWrapper;

    void Start()
    {
        tilesWrapper = new GameObject
        {
            name = "Map"
        };

        line = GetComponent<LineRenderer>();

        // Initialize Map
        InitializeMap();

        biggestRoom = new Room(0, 0, GameSettings.MAP_WIDTH, GameSettings.MAP_HEIGHT);

        SplitRoom(biggestRoom);

        MakeWall(biggestRoom);

        //StartCoroutine(Wrapper());
        InitializeObjectInstant();

        //Room playerRoom = InitializePlayers();

        //InitializeEnemies(playerRoom);

        //FlashObjectTile();
    }

    private void FlashObjectTile()
    {
        for (int r = 0; r < GameSettings.MAP_HEIGHT; r++)
        {
            for (int c = 0; c < GameSettings.MAP_WIDTH; c++)
            {
                map[r][c].Unflash();
            }
        }

        for (int r = 0; r < GameSettings.MAP_HEIGHT; r++)
        {
            for (int c = 0; c < GameSettings.MAP_WIDTH; c++)
            {
                if (map[r][c].isFilled)
                {
                    map[r][c].Flash("#9C27B0");
                }
            }
        }
    }

    private void InitializeMap()
    {
        map = new Tile[GameSettings.MAP_HEIGHT][];
        for (int i = 0; i < GameSettings.MAP_HEIGHT; i++)
        {
            map[i] = new Tile[GameSettings.MAP_WIDTH];

            for (int j = 0; j < GameSettings.MAP_WIDTH; j++)
            {
                GameObject tile = Instantiate(plane, new Vector3(j * 10, 0, i * 10), Quaternion.identity);
                map[i][j] = tile.GetComponent<Tile>();
                map[i][j].x = j;
                map[i][j].y = i;
                map[i][j].position = new Vector3(j * 10, 0, i * 10);
                map[i][j].neighbours = new Tile[4];
                tile.transform.parent = tilesWrapper.transform;
            }
        }

        bool isRight, isLeft, isBottom, isTop;

        for (int r = 0; r < GameSettings.MAP_HEIGHT; r++)
        {
            for (int c = 0; c < GameSettings.MAP_WIDTH; c++)
            {
                isRight = c == GameSettings.MAP_WIDTH - 1;
                isLeft = c == 0;
                isBottom = r == 0;
                isTop = r == GameSettings.MAP_HEIGHT - 1;

                if (!isTop)
                {
                    map[r][c].neighbours[2] = map[r + 1][c];
                }
                if (!isRight)
                {
                    map[r][c].neighbours[1] = map[r][c + 1];
                }
                if (!isBottom)
                {
                    map[r][c].neighbours[0] = map[r - 1][c];
                }
                if (!isLeft)
                {
                    map[r][c].neighbours[3] = map[r][c - 1];
                }
            }
        }
    }

    public void SplitRoom(Room room)
    {
        int door,
            randomPoint;

        if (room.width >= room.height &&
            room.width > MIN_LENGTH * 2)
        {
            do randomPoint = new System.Random().Next() % room.width;
            while (randomPoint < MIN_LENGTH ||
                     room.width - randomPoint < MIN_LENGTH ||
                     map[room.y][room.x + randomPoint - 1].isHorizontalDoor ||
                     map[room.y + room.height - 1][room.x + randomPoint - 1].isHorizontalDoor);

            do
            {
                door = new System.Random().Next() % room.height;
            }
            while (door % (room.height - 1) == 0);

            map[room.y + door][room.x + randomPoint - 1].isVerticalDoor = true;

            room.leftRoom = new Room(room.x, room.y, randomPoint, room.height);
            room.rightRoom = new Room(room.x + randomPoint - 1, room.y, room.width - randomPoint + 1, room.height);

            room.leftRoom.sibling = room.rightRoom;
            room.leftRoom.parent = room;
            room.rightRoom.sibling = room.leftRoom;
            room.rightRoom.parent = room;

            SplitRoom(room.leftRoom);
            SplitRoom(room.rightRoom);
        }
        else if (room.height > MIN_LENGTH * 2)
        {
            do
            {
                randomPoint = new System.Random().Next() % room.height;
            }
            while (randomPoint < MIN_LENGTH ||
                   room.height - randomPoint < MIN_LENGTH ||
                   map[room.y + randomPoint - 1][room.x].isVerticalDoor ||
                   map[room.y + randomPoint - 1][room.x + room.width - 1].isVerticalDoor);

            do
            {
                door = new System.Random().Next() % room.width;
            }
            while (door % (room.width - 1) == 0);

            map[room.y + randomPoint - 1][room.x + door].isHorizontalDoor = true;

            room.leftRoom = new Room(room.x, room.y, room.width, randomPoint);
            room.rightRoom = new Room(room.x, room.y + randomPoint - 1, room.width, room.height - randomPoint + 1);

            room.leftRoom.sibling = room.rightRoom;
            room.leftRoom.parent = room;
            room.rightRoom.sibling = room.leftRoom;
            room.rightRoom.parent = room;

            SplitRoom(room.leftRoom);
            SplitRoom(room.rightRoom);
        }

        if (room.leftRoom == null && room.rightRoom == null)
            rooms.Add(room);
    }

    public void MakeWall(Room room)
    {
        if (room != null)
        {
            for (int i = 0; i < room.height; i++)
            {
                for (int j = 0; j < room.width; j++)
                {
                    // Check If Now Is The Border (Start and End)
                    if (i == 0 ||
                        i == room.height - 1 ||
                        j == 0 ||
                        j == room.width - 1)
                    {
                        if (!map[i + room.y][j + room.x].isVerticalDoor &&
                            !map[i + room.y][j + room.x].isHorizontalDoor)
                            map[i + room.y][j + room.x].isWall = true;
                    }
                }
            }

            MakeWall(room.leftRoom);
            MakeWall(room.rightRoom);
        }
    }

    public IEnumerator Wrapper()
    {
        yield return StartCoroutine(InitializeObject());
    }

    public IEnumerator InitializeObject()
    {
        // Random Room, Random Tiles
        foreach (Room room in rooms)
        {
            // 0 = Steel512
            // 1 = Tile1024
            int floorType = UnityEngine.Random.Range(0, 2);

            for (int r = room.y; r < room.y + room.height; r++)
            {
                for (int c = room.x; c < room.x + room.width; c++)
                {
                    // Initialize Floor Material
                    map[r][c].gameObject.GetComponent<MeshRenderer>().material = floorTypeMaterial[floorType];

                    yield return null;

                    // Initialize Wall
                    if (map[r][c].isWall &&
                        map[r][c].wall == null &&
                        (!map[r][c].isVerticalDoor && !map[r][c].isHorizontalDoor))
                    {
                        map[r][c].wall = Instantiate(wall[0], new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                        map[r][c].isFilled = true;
                        map[r][c].wall.transform.parent = tilesWrapper.transform;
                    }

                    // Initialize Door (20% Chance)
                    //if (UnityEngine.Random.Range(1, 11) >= 1)
                    //{
                        if (map[r][c].isVerticalDoor && 
                            map[r][c].door == null &&
                            !map[r][c].isFilled)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.Euler(0, 90, 0));
                        }
                        else if (map[r][c].isHorizontalDoor && 
                                 map[r][c].door == null &&
                                 !map[r][c].isFilled)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                        }
                        map[r][c].isFilled = true;
                    //}
                }
            }

            int randX, randY;

            // Will This Room Instantiate Lamp (20% Chance)
            if (UnityEngine.Random.Range(1, 11) <= 2)
            {
                randX = getRandomXInRoom(room);
                randY = getRandomYInRoom(room);

                if (!map[randY][randX].isFilled)
                {
                    map[randY][randX].lamp = Instantiate(obstacles[OBS_LIGHT], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));
                    map[randY][randX].lamp.transform.localScale = new Vector3(5, 5, 5);
                    map[randY][randX].isFilled = true;
                    //map[randY][randX].lamp.transform.parent = tilesWrapper.transform;
                }
            }

            // Instantiate Another Obstacles with 20% Chance Table and Chair
            for (int i = 0; i < 2; i++)
            {
                int spawnObstacle = UnityEngine.Random.Range(1, 11);

                if (spawnObstacle <= 2)
                {
                    randX = getRandomXInRoom(room);
                    randY = getRandomYInRoom(room);

                    if (map[randY][randX].isFilled)
                        break;

                    if (UnityEngine.Random.Range(1, 3) == OBS_TABLE)
                    {
                        map[randY][randX].table = Instantiate(obstacles[OBS_TABLE], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));
                        //map[randY][randX].table.transform.parent = tilesWrapper.transform;
                    }
                    else
                    {
                        map[randY][randX].chair = Instantiate(obstacles[OBS_CHAIR], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));

                        // Chance 50% Random Chair Rotation
                        if (UnityEngine.Random.Range(1, 3) == 2)
                        {
                            // Chair Random Rotation
                            map[randY][randX].chair.transform.rotation = Quaternion.Euler(-90, UnityEngine.Random.Range(0, 361), 0);
                            //map[randY][randX].chair.transform.parent = tilesWrapper.transform;
                        }
                    }

                    map[randY][randX].isFilled = true;
                }
            }


        }
    }

    public void InitializeObjectInstant()
    {
        // Random Room, Random Tiles
        foreach (Room room in rooms)
        {
            // 0 = Steel512
            // 1 = Tile1024
            int floorType = UnityEngine.Random.Range(0, 2);

            room.roomTile = new Tile[room.height][];

            for (int r = room.y, 
                     roomTileRow = 0; 
                     r < room.y + room.height; 
                     r++, roomTileRow++)
            {
                room.roomTile[roomTileRow] = new Tile[room.width];

                for (int c = room.x,
                         roomTileColumn = 0; 
                         c < room.x + room.width;
                         c++, roomTileColumn++)
                {
                    room.roomTile[roomTileRow][roomTileColumn] = map[r][c];

                    // Initialize Floor Material
                    map[r][c].gameObject.GetComponent<MeshRenderer>().material = floorTypeMaterial[floorType];

                    // Initialize Wall
                    if (map[r][c].isWall &&
                        map[r][c].wall == null &&
                        (!map[r][c].isVerticalDoor && !map[r][c].isHorizontalDoor))
                    {
                        map[r][c].wall = Instantiate(wall[0], new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                        map[r][c].isFilled = true;
                        map[r][c].wall.transform.parent = tilesWrapper.transform;
                    }

                    // Initialize Door (30% Chance)
                    if (UnityEngine.Random.Range(1, 11) <= 3)
                    {
                        if (map[r][c].isVerticalDoor)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.Euler(0, 90, 0));
                        }
                        else if (map[r][c].isHorizontalDoor)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                        }
                    }
                }
            }

            int randX, randY;

            // Will This Room Instantiate Lamp (20% Chance)
            if (UnityEngine.Random.Range(1, 11) <= 2)
            {
                randX = getRandomXInRoom(room);
                randY = getRandomYInRoom(room);

                if (!map[randY][randX].isFilled &&
                    !map[randY][randX].isVerticalDoor &&
                    !map[randY][randX].isHorizontalDoor)
                {
                    map[randY][randX].lamp = Instantiate(obstacles[OBS_LIGHT], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));
                    map[randY][randX].lamp.transform.localScale = new Vector3(5, 5, 5);
                    map[randY][randX].isFilled = true;
                }
            }

            // Instantiate Another Obstacles with 20% Chance Table and Chair
            for (int i = 0; i < 2; i++)
            {
                int spawnObstacle = UnityEngine.Random.Range(1, 11);

                if (spawnObstacle <= 2)
                {
                    randX = getRandomXInRoom(room);
                    randY = getRandomYInRoom(room);

                    if (map[randY][randX].isFilled ||
                        map[randY][randX].isVerticalDoor ||
                        map[randY][randX].isHorizontalDoor)
                        continue;

                    if (UnityEngine.Random.Range(1, 3) == OBS_TABLE)
                    {
                        map[randY][randX].table = Instantiate(obstacles[OBS_TABLE], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));
                        map[randY][randX].isFilled = true;
                    }
                    else
                    {
                        map[randY][randX].chair = Instantiate(obstacles[OBS_CHAIR], new Vector3(randX * 10, 0, randY * 10), Quaternion.Euler(-90, 0, 0));
                        map[randY][randX].isFilled = true;

                        // Chance 50% Random Chair Rotation
                        if (UnityEngine.Random.Range(1, 3) == 2)
                        {
                            // Chair Random Rotation
                            map[randY][randX].chair.transform.rotation = Quaternion.Euler(-90, UnityEngine.Random.Range(0, 361), 0);
                        }
                    }
                }
            }
        }
    }

    private void InitSofaBelumJadi()
    {
        // Instatiate Sofa 10% Chance
        //int sofaType = UnityEngine.Random.Range(0, 2);
        //for (int r = room.y; r < room.y + room.height; r++)
        //{
        //    for (int c = room.x; c < room.x + room.width; c++)
        //    {
        //        //map[r][c].Flash("#66BB6A");
        //        if (r == room.y ||
        //            c == room.x ||
        //            r == (room.y + room.height) - 1 ||
        //            c == (room.x + room.width) - 1)
        //        {
        //            //Top
        //            if (r == 0 &&
        //                map[r][c].neighbours[2] != null)
        //            {
        //                map[r][c].neighbours[2].Flash("#66BB6A");
        //            }

        //            //Bottom
        //            if (r == rooms[0].height - 1 &&
        //                map[r][c].neighbours[0] != null)
        //            {
        //                map[r][c].neighbours[0].Flash("#66BB6A");
        //            }

        //            //Left
        //            if (c == 0 &&
        //                map[r][c].neighbours[1] != null)
        //            {
        //                map[r][c].neighbours[1].Flash("#66BB6A");
        //            }

        //            //Right
        //            if (c == rooms[0].width - 1 &&
        //                map[r][c].neighbours[3] != null)
        //            {
        //                map[r][c].neighbours[3].Flash("#66BB6A");
        //            }

        //for (int i = 0; i < map[r][c].neighbours.Length; i++)
        //{
        //    if (map[r][c].neighbours[i] == null ||
        //        map[r][c].neighbours[i].isWall ||
        //        map[r][c].neighbours[i].isHorizontalDoor ||
        //        map[r][c].neighbours[i].isVerticalDoor)
        //    {
        //        continue;
        //    }
        //    else
        //    {
        //        map[r][c].neighbours[i].Flash("#66BB6A");
        //    }
        //}
        //        }
        //    }
        //}
        //int  = UnityEngine.Random.Range(room.y + 1, room.height),
        //        j = UnityEngine.Random.Range(room.x + 1, room.width);
        //if (UnityEngine.Random.Range(1, 11) <= 1)
        //{
        //    int i = UnityEngine.Random.Range(room.y + 1, room.height),
        //        j = UnityEngine.Random.Range(room.x + 1, room.width);

        //    bool isHorizontal = UnityEngine.Random.Range(0, 2) == 0 ? true : false;

        //    if (sofaType == 0) // Sofa 2
        //    {
        //        if (!isHorizontal)
        //        {
        //            if (!map[room.y + 1][j].isFilled && !map[room.y + 2][j].isFilled)
        //            {
        //                map[i][j].isFilled = true;
        //                map[i + 1][j].isFilled = true;
        //                map[i][j].sofa2 = Instantiate(obstacles[sofaType], new Vector3(j * 10, 0, i * 10), Quaternion.Euler(-90, 0, 0));
        //            }
        //        }
        //        else
        //        {
        //            if (!map[i][room.x + 1].isFilled && !map[i][room.x + 2].isFilled)
        //            {
        //                map[i][j].isFilled = true;
        //                map[i][j + 1].isFilled = true;
        //                map[i][j].sofa2 = Instantiate(obstacles[OBS_SOFA_2], new Vector3(j * 10, 0, i * 10), Quaternion.Euler(-90, 90, 0));
        //            }
        //        }
        //    }
        //    else // Sofa 3
        //    {
        //        if (!isHorizontal)
        //        {
        //            if (!map[room.y + 1][j].isFilled && !map[room.y + 2][j].isFilled && !map[room.y + 3][j].isFilled)
        //            {
        //                map[i][j].isFilled = true;
        //                map[i + 1][j].isFilled = true;
        //                map[i + 2][j].isFilled = true;
        //                map[i][j].sofa2 = Instantiate(obstacles[sofaType], new Vector3(j * 10, 0, i * 10), Quaternion.Euler(-90, 0, 0));
        //            }
        //        }
        //        else
        //        {
        //            if (!map[i][room.x + 1].isFilled && !map[i][room.x + 2].isFilled && !map[i][room.x + 3].isFilled)
        //            {
        //                map[i][j].isFilled = true;
        //                map[i][j + 1].isFilled = true;
        //                map[i][j + 2].isFilled = true;
        //                map[i][j].sofa2 = Instantiate(obstacles[OBS_SOFA_2], new Vector3(j * 10, 0, i * 10), Quaternion.Euler(-90, 90, 0));
        //            }
        //        }
        //    }
        //}
    }

    private int getRandomXInRoom(Room room)
    {
        return UnityEngine.Random.Range(room.x + 1, room.width - 1);
    }

    private int getRandomYInRoom(Room room)
    {
        return UnityEngine.Random.Range(room.y + 1, room.height - 1);
    }
 
    private void SetPreviousTile(RaycastHit hittedPlane)
    {
        if (previousHittedTile != null)
        {
            if (previousHittedTile != hittedPlane.collider.gameObject)
            {
                previousHittedTile.GetComponent<Tile>().Unflash();
                previousHittedTile = hittedPlane.collider.gameObject;
            }
        }
        else
        {
            previousHittedTile = hittedPlane.collider.gameObject;
        }
    }

    private void SetPreviousSwat(RaycastHit hittedSwat)
    {
        if (prevHoveredSwat != null)
        {
            if (prevHoveredSwat != hittedSwat.collider.gameObject)
            {
                prevHoveredSwat = hittedSwat.collider.gameObject;
            }
        }
        else
        {
            prevHoveredSwat = hittedSwat.collider.gameObject;
        }
    }

    private void HideHoveredPlayerState(Player player)
    {
        if (player == null)
            return;

        //playerStateUI.gameObject.SetActive(false);
    }

    private void HideSelectedPlayerState(Player player)
    {
        if (player == null)
            return;

        //selectedPlayerStateUI.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        
    }

    public void ResetAllPlayersRendererColor()
    {
        //foreach (GameObject p in players)
        //{
        //    GameObject geo = p.transform.Find("Geo").gameObject;
        //    Material head = geo.transform.Find("Soldier_head").gameObject.GetComponent<SkinnedMeshRenderer>().material;
        //    Material body = geo.transform.Find("Soldier_body").gameObject.GetComponent<SkinnedMeshRenderer>().material;

        //    head.color = body.color = Color.white;
        //}
    }

    public void MenuMove()
    {
        selectedSwat.GetComponent<Player>().Move();
    }

    public void MenuAttack()
    {
        selectedSwat.GetComponent<Player>().Attack();
    }

    public void SelectMenuThrow()
    {
        selectedSwat.GetComponent<Player>().ThrowGrenade();
    }
}
