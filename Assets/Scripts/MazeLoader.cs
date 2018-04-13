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

        FlashObjectTile();

        StartCoroutine(GetComponent<Gameplay>().StartGamePlay());
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
                    //map[r + 1][c].parent = map[r][c];
                }
                if (!isRight)
                {
                    map[r][c].neighbours[1] = map[r][c + 1];
                    //map[r][c + 1].parent = map[r][c];
                }
                if (!isBottom)
                {
                    map[r][c].neighbours[0] = map[r - 1][c];
                    //map[r - 1][c].parent = map[r][c];
                }
                if (!isLeft)
                {
                    map[r][c].neighbours[3] = map[r][c - 1];
                    //map[r][c - 1].parent = map[r][c];
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

            do door = new System.Random().Next() % room.height;
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
                    if (UnityEngine.Random.Range(1, 11) <= 1)
                    {
                        if (map[r][c].isVerticalDoor)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.Euler(0, 90, 0));
                        }
                        else if (map[r][c].isHorizontalDoor)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                        }
                        map[r][c].isFilled = true;
                        //map[r][c].door.transform.parent = tilesWrapper.transform;
                    }
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

            for (int r = room.y; r < room.y + room.height; r++)
            {
                for (int c = room.x; c < room.x + room.width; c++)
                {
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
                            map[r][c].isFilled = true;
                        }
                        else if (map[r][c].isHorizontalDoor)
                        {
                            map[r][c].door = Instantiate(door, new Vector3(c * 10, 5, r * 10), Quaternion.identity);
                            map[r][c].isFilled = true;
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

                if (!map[randY][randX].isFilled)
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

                    if (map[randY][randX].isFilled)
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

    private Room InitializePlayers()
    {
        // Get Random Room
        //Room room = rooms[UnityEngine.Random.Range(1, rooms.Count)];

        //int randomX = 0, randomY = 0;

        // When Debugging Directly From This GameScene
        //if (GameSettings.players.Count < 1)
        //{
        //    GameSettings.players.AddRange(new int[]
        //    {
        //        Gameplay.OPERATOR,
        //        Gameplay.OPERATOR,
        //        Gameplay.SNIPER,
        //        Gameplay.CLOSE_QUARTERS
        //    });
        //}

        //GameObject playerWrapper = new GameObject()
        //{
        //    name = "Player"
        //};

        //foreach (int characterType in GameSettings.players)
        //{
        //    // Get Random Floor
        //    do
        //    {
        //        randomY = UnityEngine.Random.Range(room.y + 1, room.y + room.height - 1);
        //        randomX = UnityEngine.Random.Range(room.x + 1, room.x + room.width - 1);
        //    }
        //    while (map[randomY][randomX].swat != null);

        //    Vector3 characterPosition = 
                    // map[randomY][randomX].gameObject.transform.position;

        //    GameObject player = Instantiate(character, characterPosition, Quaternion.identity);
        //    player.transform.parent = playerWrapper.transform;

        //    GameObject head = player.transform.Find("Geo").gameObject.transform.Find("Soldier_head").gameObject,
        //               body = player.transform.Find("Geo").gameObject.transform.Find("Soldier_body").gameObject;

        //    Player playerScript = player.GetComponent<Player>();
        //    playerScript.characterType = characterType;

        //    if (characterType == Gameplay.CLOSE_QUARTERS) // Close Quarters
        //    {
        //        maxMoves += 3;

        //        body.GetComponent<Renderer>().material = swatClassMaterial[BODY_MATERIAL + CLOSE_QUARTERS * 2];
        //        head.GetComponent<Renderer>().materials[BODY_MATERIAL] = swatClassMaterial[BODY_MATERIAL + CLOSE_QUARTERS * 2];
        //        head.GetComponent<Renderer>().materials[HEAD_MATERIAL] = swatClassMaterial[HEAD_MATERIAL + CLOSE_QUARTERS * 2];

        //        SetPlayerIdentity(ref playerScript, "Close Quarters", 4, 20, 35, Int32.MaxValue, 0, 80, 80);
        //    }
        //    else
        //    {
        //        maxMoves += 2;

        //        if (characterType == OPERATOR)
        //        {
        //            body.GetComponent<Renderer>().material = swatClassMaterial[BODY_MATERIAL + OPERATOR * 2];
        //            head.GetComponent<Renderer>().material = swatClassMaterial[BODY_MATERIAL + OPERATOR * 2];

        //            SetPlayerIdentity(ref playerScript, "Operator", 6, 20, 50, Int32.MaxValue, 0, 100, 100);
        //        }
        //        else if (characterType == SNIPER)
        //        {
        //            body.GetComponent<Renderer>().material = swatClassMaterial[BODY_MATERIAL + SNIPER * 2];
        //            head.GetComponent<Renderer>().material = swatClassMaterial[BODY_MATERIAL + SNIPER * 2];

        //            SetPlayerIdentity(ref playerScript, "Sniper", 12, 70, 100, 1, 0, 100, 100);
        //        }
        //    }

        //    map[randomY][randomX].isFilled = true;
        //    map[randomY][randomX].swat = player;
        //    map[randomY][randomX].swat.GetComponent<Player>().isPlayer = true;

        //    player.GetComponent<Player>().mazeLoader = this;

        //    Rigidbody[] rigidbodies = player.GetComponentsInChildren<Rigidbody>();

        //    foreach (Rigidbody rb in rigidbodies)
        //        rb.isKinematic = true;

        //    // Starting Node
        //    player.GetComponent<Player>().currentStandingTile = map[randomY][randomX];

        //    players.Add(map[randomY][randomX].swat);
        //}

        //// Initialize availableMoves
        //availableMoves = maxMoves;

        //StartCoroutine(PanCameraWrapper(randomX, randomY));

        //return room;
        return null;
    }

    private void SetPlayerIdentity(ref Player playerScript,
                                   string playerName,
                                   int maxRange,
                                   int minDamage,
                                   int maxDamage,
                                   int bulletQty,
                                   int grenadeQty,
                                   int maxHp,
                                   int currentHp)
    {
        playerScript.playerName = playerName;
        playerScript.maxRange = maxRange;
        playerScript.minDamage = minDamage;
        playerScript.maxDamage = maxDamage;
        playerScript.bulletQty = bulletQty;
        playerScript.grenadeQty = grenadeQty;
        playerScript.maxHp = maxHp;
        playerScript.playerHp = currentHp;
    }

    public void Update()
    {
        //if (turn == Turn.Player)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hittedSwat, hittedPlane;

        //    Player hoveredPlayerScript = null,
        //           selectedPlayerScript = null;

        //    #region Hover Player
        //    if (Physics.Raycast(ray, out hittedSwat, 1000, LayerMask.GetMask("Swat")))
        //    {
        //        hoveredSwat = hittedSwat.collider.gameObject;

        //        hoveredPlayerScript = hoveredSwat.GetComponent<Player>();

        //        if (selectedSwat == null &&
        //            selectedSwat != hoveredSwat)
        //        {
        //            ShowHoveredPlayerState(hoveredPlayerScript);
        //        }
        //        else if (selectedSwat != null)
        //        {
        //            selectedPlayerScript = selectedPlayerScript;

        //            if (selectedPlayerScript.isAttacking || selectedPlayerScript.isThrowingGrenade)
        //                ShowSelectedPlayerState(selectedPlayerScript);
        //        }

        //        SetPreviousSwat(hittedSwat);
        //    }
        //    else if (hoveredSwat != null && !actionMenuUI.activeInHierarchy)
        //    {
        //        playerStateUI.gameObject.SetActive(false);
        //        hoveredSwat = null;
        //    }
        //    #endregion

        //    if (hoveredSwat != null)
        //    {
        //        #region Variables
        //        GameObject geo = hoveredSwat.transform.Find("Geo").gameObject;

        //        Material head = geo.transform.Find("Soldier_head").gameObject.GetComponent<SkinnedMeshRenderer>().material;

        //        Material body = geo.transform.Find("Soldier_body").gameObject.GetComponent<SkinnedMeshRenderer>().material;

        //        Text movesState = null;

        //        Tile targetTile = null;

        //        bool validCost = true;
        //        #endregion

        //        if (hoveredPlayerScript.isSelectingTile)
        //        {
        //            if (Physics.Raycast(ray, out hittedPlane, 1000, LayerMask.GetMask("Tiles")))
        //            {
        //                targetTile = hittedPlane.collider.GetComponent<Tile>();

        //                if (selectedSwat != null &&
        //                    !selectedPlayerScript.isAttacking &&
        //                    !selectedPlayerScript.isThrowingGrenade &&
        //                    selectedPlayerScript.currentStandingTile != targetTile)
        //                {
        //                    movesState = stateCanvas.transform.Find("TurnState").GetComponent<Text>();
        //                    targetTile.Flash();
        //                }

        //                #region Select Tile To Go
        //                if (hoveredPlayerScript.isMoving)
        //                {
        //                    if (targetTile != null)
        //                    {
        //                        if (targetTile != selectedPlayerScript.currentStandingTile &&
        //                            targetTile != hoveredPlayerScript.currentStandingTile)
        //                        {
        //                            StartCoroutine(hoveredPlayerScript.DrawPath(hoveredPlayerScript.currentStandingTile, targetTile));

        //                            if (hoveredPlayerScript.totalCost < availableMoves)
        //                            {
        //                                anyText.text = "Cost : " + hoveredPlayerScript.totalCost;
        //                            }
        //                            else
        //                            {
        //                                if (hoveredPlayerScript.totalCost > availableMoves)
        //                                {
        //                                    anyText.text = "Not Enough Moves";
        //                                    targetTile.Flash("#FF0000");
        //                                }
        //                                else if (targetTile.isFilled)
        //                                {
        //                                    anyText.text = "";
        //                                    targetTile.Unflash();
        //                                    line.positionCount = 0;
        //                                }
        //                            }

        //                            anyText.gameObject.SetActive(true);

        //                            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetTile.gameObject.transform.position + Vector3.up * 5 + Vector3.right * 2);
        //                            anyText.transform.position = screenPoint;
        //                        }
        //                        else
        //                        {
        //                            targetTile.Unflash();
        //                            line.positionCount = 0;
        //                        }
        //                    }
        //                }

        //                SetPreviousTile(hittedPlane);
        //                #endregion
        //            }
        //        }

        //        if (Input.GetMouseButtonDown(LEFT_BUTTON))
        //        {
        //            /* 
        //             * Condition To Make Sure We Don't Click A Player 
        //             * When Wnated To Choose Menu 
        //             */
        //            if (hittedSwat.collider != null &&
        //                !stateCanvas.transform.Find("ActionMenu").gameObject.activeInHierarchy)
        //            {
        //                #region Shooting
        //                if (selectedSwat != null &&
        //                    selectedPlayerScript.isAttacking)
        //                {
        //                    // Only Can Attack Enemy Validation
        //                    if (hoveredSwat != selectedSwat
        //                        //&& !hoveredSwat.GetComponent<Player>().isPlayer
        //                        )
        //                    {
        //                        //AudioSource attackSound = selectedSwat.GetComponent<AudioSource>();
        //                        //attackSound.Play();

        //                        int calculatedDamage = CalculateDamage(selectedPlayerScript, hoveredPlayerScript);

        //                        hoveredPlayerScript.ShootPlayer(hoveredSwat, calculatedDamage);

        //                        // Is Player Already Dead ?
        //                        if (hoveredSwat.GetComponent<Player>().playerHp <= 0)
        //                        {
        //                            hoveredPlayerScript.currentStandingTile.isFilled = false;
        //                            players.Remove(hoveredSwat);
        //                        }

        //                        StartCoroutine(ShowDamage(calculatedDamage, hoveredSwat.transform));
        //                    }

        //                    selectedSwat = null;

        //                    ResetAllPlayersRendererColor();

        //                    HideSelectedPlayerState(selectedPlayerScript);
        //                }
        //                else
        //                    selectedSwat = hoveredSwat;
        //                #endregion

        //                #region Show Action Menu
        //                if (selectedSwat != null &&
        //                    selectedPlayerScript.isPlayer)
        //                {
        //                    HideHoveredPlayerState(hoveredPlayerScript);

        //                    ShowSelectedPlayerState(selectedPlayerScript);

        //                    // Validate If Only Close Quarters Can Throw Grenade
        //                    if (selectedPlayerScript.grenadeQty > 0)
        //                        actionMenuUI.transform.Find("GrenadeButton").gameObject.SetActive(false);

        //                    // Validate If Around Player Has Door
        //                    Tile playerTile = selectedPlayerScript.currentStandingTile;
        //                    for (int i = 0; i < 4; i++)
        //                    {
        //                        if (playerTile.neighbours[i] != null &&
        //                            (playerTile.neighbours[i].isHorizontalDoor ||
        //                             playerTile.neighbours[i].isHorizontalDoor))
        //                        {
        //                            actionMenuUI.transform.Find("OpenDoorButton").gameObject.SetActive(true);
        //                            actionMenuUI.transform.Find("CloseDoorButton").gameObject.SetActive(true);
        //                        }
        //                    }

        //                    actionMenuUI.SetActive(true);

        //                    Vector3 hoveredSwatPosition = playerStateUI.transform.position;
        //                    hoveredSwatPosition.x += 125;
        //                    hoveredSwatPosition.y -= 50;

        //                    actionMenuUI.transform.position = hoveredSwatPosition;

        //                    ResetAllPlayersRendererColor();

        //                    head.color = body.color = Color.green;
        //                }
        //                #endregion
        //            }

        //            #region Moving
        //            if (selectedSwat != null &&
        //                selectedPlayerScript.isMoving)
        //            {
        //                Player selectedSwatScript = selectedPlayerScript;

        //                if (availableMoves < selectedSwatScript.totalCost)
        //                    validCost = false;
        //                else
        //                    validCost = true;

        //                selectedSwatScript.isSelectingTile = false;

        //                if (targetTile != null)
        //                {
        //                    targetTile.Unflash();
        //                }

        //                if (validCost &&
        //                    !targetTile.isFilled)
        //                {
        //                    /*
        //                     * No Need To Reset UI, 
        //                     * MovePlayer() Reset It
        //                     */
        //                    StartCoroutine(selectedSwatScript.MovePlayer(selectedSwat));

        //                    selectedSwatScript.isMoving = false;

        //                    availableMoves -= hoveredPlayerScript.totalCost;
        //                    movesState.text = "Available Moves : " + availableMoves;

        //                    selectedSwatScript.currentStandingTile = targetTile;
        //                    selectedSwatScript.currentStandingTile.isFilled = true;
        //                    selectedSwatScript.currentStandingTile.swat = selectedSwat;
        //                }

        //                if (selectedSwatScript.isMoving && !validCost)
        //                {
        //                    ResetUI();
        //                }
        //            }
        //            #endregion
        //        }
        //    }

        //    if (Input.GetMouseButtonDown(RIGHT_BUTTON))
        //    {
        //        ResetAllManipulatedGameState(selectedPlayerScript);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        //    {
        //        StartCoroutine(ChangeTurn());
        //    }
        //}
    }

    private void ResetAllManipulatedGameState(Player selectedPlayerScript)
    {
        // Hide Menu
        //if (actionMenuUI.activeInHierarchy)
        //    actionMenuUI.SetActive(false);

        //// Hide Selected Player State
        //if (selectedPlayerStateUI.activeInHierarchy)
        //    HideSelectedPlayerState(selectedPlayerScript);

        //// Hide Multifunctional Text
        //if (anyText.IsActive())
        //    anyText.gameObject.SetActive(false);// Reset The Player State To Not Moving
        //if (selectedPlayerScript.isMoving)
        //    selectedPlayerScript.isMoving = false;

        //// Reset The Player State To Not Attacking
        //if (selectedPlayerScript.isAttacking)
        //    selectedPlayerScript.isAttacking = false;

        //// Reset All Player Render Color
        //ResetAllPlayersRendererColor();

        //// Unflash Hovered Cell
        //if (selectedPlayerScript.isSelectingTile &&
        //    previousHittedTile != null)
        //{
        //    selectedPlayerScript.isSelectingTile = false;
        //    previousHittedTile.GetComponent<Tile>().Unflash();
        //    line.positionCount = 0;
        //}

        //// Reset Line Count
        //line.positionCount = 0;

        selectedSwat = null;
    }

    private IEnumerator ChangeTurn()
    {
        //Text waitingText = stateCanvas.transform.Find("WaitingText").GetComponent<Text>();

        //waitingText.text = "Waiting for Players turn...";

        // Bot Logic
        // turn = Turn.Enemy;
        // Do Bot Stuff

        yield return new WaitForSeconds(4);

        //waitingText.text = "Press [ENTER] To End Turn...";

        //availableMoves = maxMoves;

        //StartCoroutine(PanCameraToPlayer(prevHoveredSwat));
    }

    private void ResetUI()
    {
        //Text anyText = stateCanvas.transform.Find("anyText").GetComponent<Text>();
        //anyText.gameObject.SetActive(false);

        //ResetAllPlayersRendererColor();

        //selectedPlayerStateUI.SetActive(false);

        //line.positionCount = 0;
    }

    private int CalculateDamage(Player attacker, Player target)
    {
        Vector3 positionAttacker = selectedSwat.transform.position;
        Vector3 positionVictim = hoveredSwat.transform.position;
        Vector3 positionDelta = positionVictim - positionAttacker;

        float damage = Mathf.Lerp(attacker.maxDamage,
                                  attacker.minDamage,
                                  (positionDelta.magnitude / 10) / attacker.maxRange);

        return (int)Math.Round((double)damage);
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

    private void ShowHoveredPlayerState(Player player)
    {
        //if (player == null)
        //    return;

        //NameLoader playerUIScript = hoveredSwat.GetComponentInChildren<NameLoader>();
        //playerUIScript.playerNameUI.text = player.playerName;
        //playerUIScript.playerHpUI.maxValue = player.maxHp;
        //playerUIScript.playerHpUI.value = player.playerHp;

        //if (!player.isPlayer)
        //    playerStateUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 0;
        //else
        //    playerStateUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 1;

        //playerStateUI.gameObject.SetActive(true);

        //Vector3 newPosition = Camera.main.WorldToScreenPoint(playerUIScript.transform.position);
        //playerStateUI.transform.position = newPosition;
    }

    private void ShowSelectedPlayerState(Player player)
    {
        //if (player == null)
        //    return;

        //NameLoader playerUIScript = selectedSwat.GetComponentInChildren<NameLoader>();
        //selectedPlayerStateUI.GetComponentInChildren<Text>().text = player.playerName;
        //selectedPlayerStateUI.GetComponentInChildren<Slider>().maxValue = player.maxHp;
        //selectedPlayerStateUI.GetComponentInChildren<Slider>().value = player.playerHp;

        //if (!player.isPlayer)
        //    selectedPlayerStateUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 0;
        //else
        //    selectedPlayerStateUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 1;

        //selectedPlayerStateUI.gameObject.SetActive(true);

        //Vector3 newPosition = Camera.main.WorldToScreenPoint(playerUIScript.transform.position);
        //selectedPlayerStateUI.transform.position = newPosition;
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

    public void Move()
    {
        selectedSwat.GetComponent<Player>().Move();
    }

    public void Attack()
    {
        selectedSwat.GetComponent<Player>().Attack();
    }

    public void Throw()
    {
        selectedSwat.GetComponent<Player>().ThrowGrenade();
    }
}
