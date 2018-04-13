using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    // Initial Available Moves
    #region Move States
    private int availableMoves = 2,
                maxMoves = 2,
                enemyAvailableMoves = 2,
                enemyMaxMoves = 2;
    #endregion

    #region Constant
    public const int LEFT_BUTTON = 0,
                     RIGHT_BUTTON = 1,
                     BODY_MATERIAL = 0,
                     HEAD_MATERIAL = 1,
                     OPERATOR = 0,
                     SNIPER = 1,
                     CLOSE_QUARTERS = 2;
    #endregion

    #region Gameplay State
    private bool isPaused = false,
                 isPlayerTurn = false;
    #endregion

    #region Camera Influences Field
    private Vector3 newCameraPosition;
    
    private float scrollSpeed = 15;
    #endregion

    #region UI
    public GameObject mainCanvas,
                      menuUI,
                      hoveredCharacterUI,
                      selectedCharacterUI;

    public Text pauseText,
                subPauseText,
                movesText, // Availble Moves Left
                playerGameplayStateText, // Bottom Right Corner Text
                anyText;

    public LineRenderer line;
    #endregion

    #region Game Object Influencer
    public GameObject character;

    public Material[] swatClassMaterial;

    private Tile[][] map;
    #endregion

    private List<GameObject> players = new List<GameObject>(),
                            enemies = new List<GameObject>();

    void Update()
    {

    }

    IEnumerator PlayerGamePlay()
    {
        yield return null;
    }

    IEnumerator BotGameplay()
    {
        yield return null;
    }

    public IEnumerator StartGamePlay()
    {
        // Get Map
        map = GetComponent<MazeLoader>().map;

        InitializePlayer();

        while (!GameIsOver())
        {
            if (isPlayerTurn)
            {
                yield return StartCoroutine(PlayerGamePlay());
            }
            else
            {
                yield return StartCoroutine(BotGameplay());
            }

            // Toggle Turn
            isPlayerTurn = !isPlayerTurn;
        }

        // Clear UI

        if (isPlayerTurn)
        {
            // Player Lose
        }
        else if (!isPlayerTurn)
        {
            // Player Win
        }
    }

    bool GameIsOver()
    {
        return players.Count < 1 || enemies.Count < 1;
    }

    public void ResetUI()
    {
        menuUI.SetActive(false);

        hoveredCharacterUI.SetActive(false);

        selectedCharacterUI.SetActive(false);

        anyText.gameObject.SetActive(false);
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            availableMoves = maxMoves;

            enemyAvailableMoves = enemyMaxMoves;

            // Reset Sniper Ammo To 1

            // Reset Close Quarters Grenate To 1
        }
    }

    IEnumerator DrawPath(Tile origin, Tile destination)
    {
        yield return null;
    }

    IEnumerator ShowDamage(int damage, Transform parentObject)
    {
        //GameObject ui = stateCanvas.transform.Find("anyText").gameObject;
        //CanvasGroup canvasGroup = ui.GetComponent<CanvasGroup>();
        //Text uiText = ui.GetComponentInChildren<Text>();
        //ui.SetActive(true);
        //uiText.text = "-" + damage;

        //float percent = 0.1f;

        //while (percent < 1f)
        //{
        //    Vector3 start = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 5);
        //    Vector3 finish = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 10);
        //    Vector3 newPosition = Vector3.Lerp(start, finish, percent);
        //    ui.transform.position = Camera.main.WorldToScreenPoint(newPosition);

        //    canvasGroup.alpha -= 0.02f;

        //    percent += 0.02f;

        yield return null;
        //}

        //ui.SetActive(false);
        //canvasGroup.alpha = 1f;
    }

    IEnumerator PanCameraWrapper(int randomX, int randomY)
    {
        //yield return StartCoroutine(PanCameraToPlayer(map[randomY][randomX].swat));

        //if (DevelopmentMode)
        //{
        //    Camera.main.transform.position = GameObject.Find("GoodDebugCameraPosition").transform.position;
        //    Camera.main.transform.rotation = GameObject.Find("GoodDebugCameraPosition").transform.rotation;
        //}

        yield return null;
    }

    IEnumerator PanCameraToPlayer(GameObject swat)
    {
        //float x = swat.transform.position.x - 10;
        //float y = swat.transform.position.y + 30;
        //float z = swat.transform.position.z - 10;
        //newCameraPosition = new Vector3(x, y, z);

        //while (Vector3.Distance(Camera.main.transform.position, newCameraPosition) >= 0.01f &&
        //       Camera.main.transform.rotation != playerCameraTransform.rotation)
        //{
        //    Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCameraPosition, Time.deltaTime * 4);
        //    Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, playerCameraTransform.rotation, Time.deltaTime * 2);

        yield return null;
        //}
    }

    public void InitializePlayer()
    {
        // Get Room
        List<Room> rooms = GetComponent<MazeLoader>().rooms;

        // Get Random Room
        Room playerRoom = rooms[Random.Range(1, rooms.Count)];
  
        int randomX = 0,
            randomY = 0;

        // If Debugging Directly Frokm This Game Scene
        if (GameSettings.players.Count < 1)
        {
            GameSettings.players.AddRange(new int[]
            {
                OPERATOR,
                OPERATOR,
                SNIPER,
                CLOSE_QUARTERS
            });
        }

        GameObject playersWrapperGameObject = new GameObject() 
        {
            name = "Players"
        };

        foreach (int characterType in GameSettings.players)
        {
            // Get Random Floor
            do
            {
                randomX = Random.Range(playerRoom.x + 1, playerRoom.x + playerRoom.width - 1);
                randomY = Random.Range(playerRoom.y + 1, playerRoom.y + playerRoom.height - 1);
            }
            while (map[randomY][randomX].swat != null || 
                   map[randomY][randomX].isFilled);

            Vector3 characterPosition = map[randomY][randomX].gameObject.transform.position;

            GameObject characterObject = Instantiate(character, characterPosition, Quaternion.identity);
            characterObject.transform.parent = playersWrapperGameObject.transform;

            GameObject head = character.transform.Find("Geo").gameObject.transform.Find("Soldier_head").gameObject,
                       body = character.transform.Find("Geo").gameObject.transform.Find("Soldier_body").gameObject;

            Player characterController = characterObject.GetComponent<Player>();
            characterController.characterType = characterType;

            if (characterType == CLOSE_QUARTERS)
            {
                maxMoves += 3;

                RenderMaterial(head, body, BODY_MATERIAL, CLOSE_QUARTERS);
                SetPlayerIdentity(ref characterController, "Close Quaters", 4, 20, 35, System.Int32.MaxValue, 0, 80, 80);
            }
            else
            {
                maxMoves += 2;

                if (characterType == SNIPER)
                {
                    RenderMaterial(head, body, BODY_MATERIAL, SNIPER);
                    SetPlayerIdentity(ref characterController, "Sniper", 12, 70, 100, 1, 0, 100, 100);
                }
                else if (characterType == OPERATOR)
                {
                    RenderMaterial(head, body, BODY_MATERIAL, OPERATOR);
                    SetPlayerIdentity(ref characterController, "Operator", 6, 20, 50, System.Int32.MaxValue, 0, 100, 100);
                }
            }

            map[randomY][randomX].isFilled = true;
            map[randomY][randomX].swat = characterObject;
            map[randomY][randomX].swat.GetComponent<Player>().isPlayer = true;

            // character. <= Is Need Maze Loader Anymore ?

            Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.isKinematic = true;

            // Starting Node
            characterController.currentStandingTile = map[randomY][randomX];

            players.Add(characterObject);
        }
    }

    private void RenderMaterial(GameObject head, GameObject body, int materialIndex, int characterType)
    {
        body.GetComponent<Renderer>().material = swatClassMaterial[materialIndex + characterType * 2];
        head.GetComponent<Renderer>().material = swatClassMaterial[materialIndex + characterType * 2];
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

    private void InitializeEnemies(Room roomToExclude)
    {
        //List<Room> rooms = new List<Room>();

        //foreach (Room room in this.rooms)
        //{
        //    if (room != roomToExclude)
        //        rooms.Add(room);
        //}

        //// Get Random Room
        //Room randomEnemyRoom = rooms[UnityEngine.Random.Range(1, rooms.Count)];

        //for (int r = randomEnemyRoom.y; r < randomEnemyRoom.y + randomEnemyRoom.height; r++)
        //{
        //    for (int c = randomEnemyRoom.x; c < randomEnemyRoom.x + randomEnemyRoom.width; c++)
        //    {
        //        map[r][c].Flash("#E91E63");
        //    }
        //}

        //// When Debugging Directly From This GameScene
        //if (GameSettings.players.Count < 1)
        //{
        //    GameSettings.players.AddRange(new int[] { 0, 0, 1, 2 });
        //}

        //int randomX = 0, randomY = 0;

        //foreach (int i in GameSettings.players)
        //{
        //    // Get Random Floor
        //    do
        //    {
        //        randomY = UnityEngine.Random.Range(randomEnemyRoom.y + 1, randomEnemyRoom.y + randomEnemyRoom.height - 1);
        //        randomX = UnityEngine.Random.Range(randomEnemyRoom.x + 1, randomEnemyRoom.x + randomEnemyRoom.width - 1);
        //    }
        //    while (map[randomY][randomX].swat != null);

        //    if (i == 0 || i == 1) // Operator and Sniper
        //        maxMoves += 2;
        //    else if (i == 2) // Close Quarters
        //        maxMoves += 3;

        //    map[randomY][randomX].isFilled = true;

        //    GameObject player = Instantiate(characters[i], map[randomY][randomX].gameObject.transform.position, Quaternion.identity);
        //    map[randomY][randomX].swat = player;
        //    map[randomY][randomX].swat.GetComponent<Player>().isPlayer = false;
        //    player.GetComponent<Player>().mazeLoader = this;
        //    player.GetComponent<Rigidbody>().isKinematic = true;

        //    // Buat Cek 5 Disekitar, Jika Tidak Ada Light, Tidak Render
        //    //player.GetComponentInChildren<Light>().gameObject.SetActive(false);

        //    // Starting Node
        //    player.GetComponent<Player>().currentStandingTile = map[randomY][randomX];

        //    enemies.Add(map[randomY][randomX].swat);
        //}

        // Initialize availableMoves
        //enemyAvailableMoves = enemyMaxMoves;
    }

    public void LateUpdate()
    {
        if (!isPaused) // Game Is Not In Pause Mode
        {
            if (!GetComponent<MazeLoader>().DevelopmentMode)
            {
                if (Input.mousePosition.y >= Screen.height)
                {
                    // Move North
                    Camera.main.transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
                    Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);
                }
                else if (Input.mousePosition.y <= 5)
                {
                    // Move South
                    Camera.main.transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
                    Camera.main.transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed, Space.World);
                }
                else if (Input.mousePosition.x >= Screen.width * 0.98)
                {
                    // Move East
                    Camera.main.transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);
                    Camera.main.transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed, Space.World);

                    if (Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                    {
                        // Rotate To East
                        Camera.main.transform.Rotate(0.0f,
                            Time.deltaTime * 100f,
                            0.0f, Space.World);
                    }
                }
                else if (Input.mousePosition.x <= 0)
                {
                    // Move West
                    Camera.main.transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
                    Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);

                    if (Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                    {
                        // Rotate To West
                        Camera.main.transform.Rotate(0.0f,
                            -Time.deltaTime * 100f,
                            0.0f, Space.World);
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                // Show Pause Menu
                pauseText.text = "Game is Paused";
                pauseText.gameObject.SetActive(true);

                subPauseText.text = "Press Escape To Resumed or ENTER To Exit Game";
                subPauseText.gameObject.SetActive(true);

                movesText.gameObject.SetActive(false);
            }
        }
        else if (isPaused)// Is Paused
        {
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                GameSettings.SCENE_HELPER = 1;
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                pauseText.gameObject.SetActive(false);
                subPauseText.gameObject.SetActive(false);
                movesText.gameObject.SetActive(true);
            }
        }
    }
}
