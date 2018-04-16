using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    public bool enableCameraMovement = false;

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

    public const string OPERATOR_WEAPON = "operator_weapon";
    public const string SNIPER_WEAPON = "sniper_weapon";
    public const string CQ_WEAPON = "cq_weapon";
    public const string GRENADE = "m26";
    #endregion

    #region Gameplay State
    private bool isPaused = false,
                 isPlayerTurn = true,
                 isSelectingTile = false,
                 isValidMoveCost = false,
                 isMoving = false,
                 isShooting = false,
                 isThrowingGrenade = false;
    #endregion

    #region Camera Influences Field
    private Vector3 newCameraPosition;
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

    private Tile hoveredTile, prevTile, selectedTile;

    private List<Tile> currentPath;

    private static GameObject hoveredSwat, selectedSwat, prevSwat;

    private static Player hoveredSwatController, selectedSwatController;
    #endregion

    public GameObject grenade;

    [HideInInspector]
    public List<GameObject> players = new List<GameObject>(),
                            enemies = new List<GameObject>();

    [HideInInspector]
    public int pathCost = 0;

    private void Start()
    {
        // Get Map
        map = GetComponent<MazeLoader>().map;

        // Initialize Player
        Room playerRoom = InitializeCharacter();
        Room enemyRoom = null;
        //StartCoroutine(FlashPlayerRoom(playerRoom));

        // Dijkstra's Algorithm To Find Farthest Path
        Dijkstra dijkstra = GetComponent<Dijkstra>();

        List<Tile> pathToFarthestRoom = dijkstra.FindFarthestRoom(map, map[playerRoom.y + 1][playerRoom.x + 1]);

        GetComponent<MazeLoader>().rooms.ForEach(r =>
        {
            for (int i = 0; i < r.height; i++)
            {
                for (int j = 0; j < r.width; j++)
                {
                    if (r.roomTile[i][j] == pathToFarthestRoom[0])
                    {
                        //StartCoroutine(FlashEnemyRoom(r));
                        enemyRoom = r;
                        return;
                    }
                }
            }
        });

        //GetComponent<MazeLoader>().map[playerRoom.y + 1][playerRoom.x + 1].Flash("#FF9800");

        // Initialize Enemy
        InitializeCharacter(_isPlayer: false, room: enemyRoom);

        availableMoves = maxMoves;

        enemyAvailableMoves = enemyMaxMoves;

        movesText.text += availableMoves;
    }

    private IEnumerator FlashPlayerRoom(Room playerRoom)
    {
        for (int r = playerRoom.y; r < playerRoom.y + playerRoom.height; r++)
        {
            for (int c = playerRoom.x; c < playerRoom.x + playerRoom.width; c++)
            {
                GetComponent<MazeLoader>().map[r][c].Flash("#009688");
                yield return null;
            }
        }
    }

    private IEnumerator FlashEnemyRoom(Room enemyRoom)
    {
        for (int r = enemyRoom.y; r < enemyRoom.y + enemyRoom.height; r++)
        {
            for (int c = enemyRoom.x; c < enemyRoom.x + enemyRoom.width; c++)
            {
                GetComponent<MazeLoader>().map[r][c].Flash("#F44336");
                yield return null;
            }
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hittedPlane, hittedSwat;

        if (isPlayerTurn)
        {
            if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) &&
                !isPaused)
            {
                StartCoroutine(ChangeTurn());
            }

            if (!isSelectingTile)
            {
                #region Hover Swat
                if (Physics.Raycast(ray, out hittedSwat, 1000, LayerMask.GetMask("Swat")))
                {
                    //print(hittedSwat.collider.gameObject.layer);
                    //if (hittedSwat.collider.gameObject.layer == )

                    if (hittedSwat.collider != null)
                    {
                        hoveredSwat = hittedSwat.collider.gameObject;
                        hoveredSwatController = hoveredSwat.GetComponent<Player>();

                        if (selectedSwat == null || selectedSwat != hoveredSwat)
                            ShowHoveredCharacterUI(hoveredSwatController);

                        if (isShooting)
                        {
                            if (availableMoves >= 2)
                            {
                                Vector3 direction = hoveredSwat.transform.position - selectedSwat.transform.position;

                                RaycastHit hittedObject;

                                // Validasi Raycast Obstructing
                                if (Physics.Raycast(selectedSwat.transform.position + (Vector3.up * 5f), direction, out hittedObject, 1000))
                                {
                                    if (hittedObject.collider.gameObject.layer != 0 && hittedObject.collider.gameObject.layer != 8)
                                    {
                                        HideHoveredCharacterUI();

                                        anyText.text = "Obstructing";
                                        anyText.gameObject.SetActive(true);
                                        anyText.transform.position = Camera.main.WorldToScreenPoint(hoveredSwat.transform.position + (Vector3.up * 10f));
                                    }
                                    else if (hittedObject.collider.gameObject.layer == 8)
                                    {
                                        if (anyText.gameObject.activeInHierarchy)
                                            anyText.gameObject.SetActive(false);

                                        ShowSelectedCharacterUI(selectedSwatController);
                                    }
                                }

                                Debug.DrawRay(selectedSwat.transform.position + (Vector3.up * 2.5f), direction * 1000, Color.red);
                            }
                            else
                            {
                                GameObject placeholder =
                                    hoveredSwat.transform.GetChild(2).gameObject;

                                // Show Insufficient Move
                                anyText.text = "Insufficient Moves";
                                anyText.gameObject.SetActive(true);
                                anyText.transform.position = Camera.main.WorldToScreenPoint(placeholder.transform.position);

                                HideHoveredCharacterUI();
                            }
                        }
                    }

                    SetPreviousSwat(hittedSwat);
                }
                else if (hoveredSwat != null)
                {
                    HideHoveredCharacterUI();
                    hoveredSwat = null;
                }
                #endregion
            }
            else if (isSelectingTile)
            {
                #region Hover Tile
                if (Physics.Raycast(ray, out hittedPlane, 1000, LayerMask.GetMask("Tiles")))
                {
                    if (prevTile != null)
                        prevTile.Unflash();

                    if (!isShooting)
                    {
                        hoveredTile = hittedPlane.collider.gameObject.GetComponent<Tile>();
                        hoveredTile.Flash();
                    }
                }

                SetPreviousTile(hittedPlane);
                #endregion

                // Between Move, Attack and Throw Grenade
                if (isMoving)
                {
                    if (!hoveredTile.isFilled)
                        StartCoroutine(DrawPath(selectedSwatController.currentStandingTile, hoveredTile));
                }
            }
        }
        else
        {
            // Bot Logic
        }

        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {
            if (Input.GetMouseButtonDown(LEFT_BUTTON))
            {
                if (!isSelectingTile)
                {
                    if (hoveredSwat != null && hoveredSwat != selectedSwat)
                    {
                        if (isShooting)
                        {
                            if (hoveredSwat.name == "swat:Head")
                            {
                                print("Caught Head : " + selectedSwat);
                                selectedSwat = selectedSwat.transform.parent.gameObject;
                            }

                            isShooting = false;

                            if (availableMoves >= 2)
                            {
                                Vector3 direction = hoveredSwat.transform.position - selectedSwat.transform.position;

                                RaycastHit hittedObject;

                                if (Physics.Raycast(selectedSwat.transform.position + (Vector3.up * 5f), direction, out hittedObject, 1000))
                                {
                                    if (hittedObject.collider.gameObject.layer == 8)
                                    {
                                        // Each Attack Requires 2 Moves
                                        availableMoves -= 2;

                                        movesText.text = "Available Moves : " + availableMoves;

                                        if (selectedSwatController is OperatorController)
                                            this.transform.Find("OperatorShooting").GetComponent<AudioSource>().Play();
                                        else if (selectedSwatController is SniperController)
                                            this.transform.Find("SniperShooting").GetComponent<AudioSource>().Play();
                                        else if (selectedSwatController is CloseQuartersController)
                                            this.transform.Find("CloseQuartersShooting").GetComponent<AudioSource>().Play();

                                        int damage = CalculateDamage(selectedSwatController, hoveredSwatController);

                                        selectedSwat.transform.LookAt(hoveredSwat.transform.position);

                                        selectedSwatController.ShootPlayer(hoveredSwat, damage);

                                        // Play Muzzle Flash
                                        Transform[] objects = selectedSwat.GetComponentsInChildren<Transform>();

                                        foreach (Transform obj in objects)
                                        {
                                            if (obj.CompareTag("Weapon"))
                                            {
                                                if (obj.gameObject.activeInHierarchy)
                                                {
                                                    StartCoroutine(ShowMuzzleFlash(obj.gameObject));
                                                }
                                            }
                                        }

                                        StartCoroutine(ShowDamage(damage, hoveredSwat.transform));

                                        if (hoveredSwatController.playerHp <= 0)
                                        {
                                            hoveredSwatController.GetComponentInChildren<Light>().enabled = false;

                                            hoveredSwatController.currentStandingTile.isFilled = false;

                                            if (hoveredSwatController.isPlayer)
                                                players.Remove(hoveredSwat);
                                            else
                                                enemies.Remove(hoveredSwat);
                                        }
                                    }
                                }

                                ResetAllCharacterRendererColor();
                            }

                            selectedSwat = null;

                            HideHoveredCharacterUI();

                            HideSelectedCharacterUI();
                        }
                        else
                        {
                            selectedSwat = hoveredSwat;
                            selectedSwatController = selectedSwat.GetComponent<Player>();
                        }

                        if (selectedSwatController != null)
                        {
                            if (selectedSwatController.isPlayer)
                            {
                                ResetAllCharacterRendererColor();

                                HideHoveredCharacterUI();

                                // Penghijau
                                ShowSelectedCharacterUI(selectedSwatController);

                                if (selectedSwat != null && !menuUI.activeInHierarchy)
                                {
                                    ShowMenuUI(selectedSwatController);
                                }
                            }
                        }
                    }
                    else if (hoveredSwat == null && !menuUI.activeInHierarchy)
                    {
                        ResetUI();
                    }
                }
                else if (isSelectingTile)
                {
                    selectedTile = hoveredTile;

                    if (isMoving &&
                        line.positionCount > 0 &&
                        isValidMoveCost)
                    {
                        availableMoves -= pathCost;
                        movesText.text = "Available Moves : " + availableMoves;

                        isMoving = isSelectingTile = false;

                        ResetAllCharacterRendererColor();

                        selectedTile.Unflash();

                        StartCoroutine(MoveCharacter(selectedSwatController, currentPath));
                    }
                    else if (isThrowingGrenade)
                    {
                        if (availableMoves > 1)
                        {
                            selectedSwatController.grenadeQty -= 1;

                            availableMoves -= 2;
                            movesText.text = "Available Moves : " + availableMoves;

                            selectedSwat.transform.LookAt(selectedTile.transform);

                            StartCoroutine(ThrowGrenade(selectedSwatController, selectedTile));

                            selectedSwat.GetComponent<Animator>().SetTrigger("throw");
                        }
                        else
                        {
                            anyText.text = "Insufficent Move";
                            anyText.gameObject.SetActive(true);
                            anyText.transform.position = Camera.main.WorldToScreenPoint(selectedTile.transform.position + (Vector3.up * 10f));
                            selectedTile.Flash("#FF0000");
                        }

                        ResetUI();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(RIGHT_BUTTON))
        {
            ResetUI();
        }

        Debug.DrawRay(ray.origin, ray.direction * 1000);
    }

    private IEnumerator ShowMuzzleFlash(GameObject w)
    {
        Light flash = w.GetComponentInChildren<Light>();
        GameObject muzzleFlash = flash.GetComponentInChildren<Transform>().Find("MuzzleFlash").gameObject;

        flash.enabled = true;
        muzzleFlash.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        flash.enabled = false;
        muzzleFlash.SetActive(false);
    }

    private IEnumerator ThrowGrenade(Player thrower, Tile targetTile)
    {
        yield return new WaitForSeconds(1.5f);

        Vector3 grenadePosition = thrower.transform.position;
        grenadePosition.y += 6f;
        grenadePosition += thrower.transform.forward * 1.5f;

        Vector3 deltaPosition = targetTile.transform.position - grenadePosition;
        
        float height = 2f;

        GameObject grenadeObject = Instantiate(grenade, grenadePosition, Quaternion.identity);

        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();

        Vector3 parabolaPosition = Parabola(Vector3.zero, deltaPosition / 10, height, 1f);
        //rb.AddForce(parabolaPosition * (deltaPosition.magnitude / 5f), ForceMode.Impulse);
        rb.AddForce(parabolaPosition * (deltaPosition.magnitude * .2f), ForceMode.Impulse);
    }

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    IEnumerator PlayerGamePlay()
    {
        while (true)
        {


            yield return null;
        }
    }

    private void SetPreviousSwat(RaycastHit hittedSwat)
    {
        if (prevSwat != null)
        {
            if (prevSwat != hittedSwat.collider.gameObject)
            {
                prevSwat = hittedSwat.collider.gameObject;
            }
        }
        else
        {
            prevSwat = hittedSwat.collider.gameObject;
        }
    }

    private void SetPreviousTile(RaycastHit hittedTile)
    {
        if (prevTile != null)
        {
            if (hittedTile.collider.gameObject != null)
                if (prevTile != hittedTile.collider.gameObject)
                    prevTile = hittedTile.collider.gameObject.GetComponent<Tile>();
        }
        else
        {
            prevTile = hittedTile.collider.gameObject.GetComponent<Tile>();
        }
    }

    IEnumerator BotGameplay()
    {
        yield return null;
    }

    bool GameIsOver()
    {
        /* TODO : Enabled This After InitializeEnemies */
        //return players.Count < 1 || enemies.Count < 1;

        return players.Count < 1;
    }

    public void ResetUI()
    {
        menuUI.SetActive(false);

        hoveredCharacterUI.SetActive(false);

        selectedCharacterUI.SetActive(false);

        anyText.gameObject.SetActive(false);

        menuUI.SetActive(false);

        ResetAllCharacterRendererColor();

        isSelectingTile = isMoving = isShooting = isThrowingGrenade = false;

        if (selectedTile != null)
            selectedTile.Unflash();

        if (prevTile != null)
            prevTile.Unflash();

        selectedSwat = null;

        line.positionCount = 0;
    }

    public void SelectMenuMove()
    {
        menuUI.SetActive(false);
        isSelectingTile = true;
        isMoving = true;
    }

    public void SelectMenuAttack()
    {
        menuUI.SetActive(false);
        isShooting = true;
    }

    public void SelectMenuThrow()
    {
        menuUI.SetActive(false);
        isSelectingTile = true;
        isThrowingGrenade = true;
    }

    public void SelectMenuOpenDoor()
    {

    }

    public void SelectMenuCloseDoor()
    {

    }

    public void ShowHoveredCharacterUI(Player character)
    {
        if (character == null)
            return;

        NameLoader placeholder = character.GetComponentInChildren<NameLoader>();

        hoveredCharacterUI.SetActive(true);

        hoveredCharacterUI.GetComponentInChildren<Text>().text = character.playerName;
        hoveredCharacterUI.GetComponentInChildren<Slider>().maxValue = character.maxHp;
        hoveredCharacterUI.GetComponentInChildren<Slider>().value = character.playerHp;

        if (!character.isPlayer)
            hoveredCharacterUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 0;
        else
            hoveredCharacterUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 1;

        Vector3 newPosition = Camera.main.WorldToScreenPoint(placeholder.transform.position);
        hoveredCharacterUI.transform.position = newPosition;
    }

    public void HideHoveredCharacterUI()
    {
        hoveredCharacterUI.SetActive(false);
    }

    public void ShowSelectedCharacterUI(Player character)
    {
        if (character == null)
            return;

        NameLoader placeholder = character.GetComponentInChildren<NameLoader>();

        selectedCharacterUI.SetActive(true);

        selectedCharacterUI.GetComponentInChildren<Text>().text = character.playerName;
        selectedCharacterUI.GetComponentInChildren<Slider>().maxValue = character.maxHp;
        selectedCharacterUI.GetComponentInChildren<Slider>().value = character.playerHp;

        if (!character.isPlayer)
            selectedCharacterUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 0;
        else
            selectedCharacterUI.GetComponentInChildren<Slider>().GetComponent<CanvasGroup>().alpha = 1;

        Vector3 newPosition = Camera.main.WorldToScreenPoint(placeholder.transform.position);
        selectedCharacterUI.transform.position = newPosition;

        foreach (Renderer r in character.gameObject.GetComponentsInChildren<Renderer>())
        {
            r.material.color = Color.green;
        }
    }

    public void HideSelectedCharacterUI()
    {
        selectedCharacterUI.SetActive(false);

        if (selectedSwat == null)
            return;

        foreach (Renderer r in selectedSwat.gameObject.GetComponentsInChildren<Renderer>())
        {
            r.material.color = Color.white;
        }
    }

    private void ShowMenuUI(Player selectedPlayer)
    {
        if (selectedPlayer == null)
            return;

        menuUI.SetActive(true);

        if (selectedPlayer.grenadeQty <= 0)
            menuUI.transform.Find("GrenadeButton").gameObject.SetActive(false);
        else
            menuUI.transform.Find("GrenadeButton").gameObject.SetActive(true);

        foreach (Tile neighbour in selectedPlayer.currentStandingTile.GetComponent<Tile>().neighbours)
        {
            if (neighbour.isVerticalDoor || neighbour.isHorizontalDoor)
            {
                DoorController doorObject = neighbour.door.GetComponent<DoorController>();

                if (doorObject.state == DoorController.DoorState.Close)
                {
                    menuUI.transform.Find("ClosedDoorButton").gameObject.SetActive(false);
                }
                else
                {
                    menuUI.transform.Find("OpenDoorButton").gameObject.SetActive(false);
                }
            }
        }

        // Position
        Vector3 newPosition = selectedCharacterUI.transform.position;
        newPosition.x += 130;
        newPosition.y -= 50;

        menuUI.transform.position = newPosition;
    }

    public IEnumerator ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if (isPlayerTurn)
        {

        }
        else
        {
            // Bot Logic
            // ChangeTurn() Again
        }

        playerGameplayStateText.text = "Waiting For Player Turn...";

        yield return new WaitForSeconds(3);

        availableMoves = maxMoves;
        movesText.text = "Available Moves : " + availableMoves;

        enemyAvailableMoves = enemyMaxMoves;

        foreach (GameObject p in players)
        {
            Player controller = p.GetComponent<Player>();

            if (controller is SniperController)
                controller.bulletQty = 1;
            else if (controller is CloseQuartersController)
                controller.grenadeQty = 1;
        }

        playerGameplayStateText.text = "Press [ENTER] To End Turn";
    }

    IEnumerator DrawPath(Tile origin, Tile destination)
    {
        if (!destination.isFilled && destination != origin)
        {
            Pathfinder pathfinder = GetComponent<Pathfinder>();

            Dictionary<List<Tile>, int> result = pathfinder.PathFinding(origin, destination);

            if (result != null && result.Count > 0)
            {
                List<Tile> path = currentPath = result.FirstOrDefault().Key;

                //int totalCost = pathCost = result.FirstOrDefault().Value;

                float cost = 0;

                if (path != null)
                {
                    if (path.Count > 1)
                    {
                        line.positionCount = path.Count;

                        for (int i = 0; i < line.positionCount; i++)
                        {
                            if (i > 0)
                            {
                                cost += ((line.GetPosition(i) - line.GetPosition(i - 1)).magnitude / 10);
                            }

                            line.SetPosition(i, path[i].position + Vector3.up);
                        }
                    }
                }

                ShowMoveCostStatus(cost, destination, path);
            }
        }
        // else
        //     line.positionCount = 0;

        yield return null;
    }

    public void ShowMoveCostStatus(float totalCost, Tile destination, List<Tile> path)
    {
        pathCost = Mathf.RoundToInt(totalCost);

        if (availableMoves > pathCost)
        {
            anyText.text = "Cost : " + pathCost;
            currentPath = path;
            isValidMoveCost = true;
        }
        else
        {
            anyText.text = "Insufficient Moves";
            destination.Flash("#FF0000");
            isValidMoveCost = false;
        }

        anyText.gameObject.SetActive(true);

        // Position
        anyText.transform.position = Camera.main.WorldToScreenPoint(destination.transform.position + Vector3.up * 3);
    }

    IEnumerator MoveCharacter(Player character, List<Tile> path)
    {
        if (line.positionCount > 0)
        {
            character.currentStandingTile.isFilled = false;
            character.currentStandingTile.swat = null;

            character.GetComponent<Animator>().SetTrigger("run");

            bool openDoor = false;

            for (int i = 0; i < path.Count; i++)
            {
                while (character.transform.position != line.GetPosition(i) + Vector3.down)
                {
                    character.transform.LookAt(line.GetPosition(i) + Vector3.down);

                    Vector3 edge = Vector3.MoveTowards(character.transform.position, line.GetPosition(i) + Vector3.down, Time.deltaTime * 20f);

                    character.transform.position = edge;

                    RaycastHit door;

                    if (Physics.Raycast(character.transform.position, character.transform.forward, out door, 20f, LayerMask.GetMask("Door")))
                    {
                        yield return StartCoroutine(OpenDoorWrapper(door.collider.gameObject));
                    }

                    Debug.DrawRay(character.transform.position, character.transform.forward * 20f, Color.black);

                    yield return null;
                }
            }

            character.currentStandingTile = path[path.Count - 1];
            character.currentStandingTile.isFilled = true;
            character.currentStandingTile.swat = character.gameObject;
        }

        character.GetComponent<Animator>().SetTrigger("idle");

        ResetUI();

        yield return null;
    }

    private IEnumerator OpenDoorWrapper(GameObject door)
    {
        print("OpenTheDoor");
        DoorController doorController = door.GetComponent<DoorController>();

        doorController.PrepareDoorHinge();

        yield return StartCoroutine(doorController.OpenDoor());
    }

    private int CalculateDamage(Player attacker, Player target)
    {
        Vector3 positionAttacker = selectedSwat.transform.position;
        Vector3 positionVictim = hoveredSwat.transform.position;
        Vector3 positionDelta = positionVictim - positionAttacker;

        float damage = Mathf.Lerp(attacker.maxDamage,
                                  attacker.minDamage,
                                  (positionDelta.magnitude / 10) / attacker.maxRange);

        return (int)System.Math.Round((double)damage);
    }

    IEnumerator ShowDamage(int damage, Transform parentObject)
    {
        anyText.gameObject.SetActive(true);

        anyText.text = "-" + damage;

        float percent = 0.1f;

        while (percent < 1f)
        {
            Vector3 start = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 1);
            Vector3 finish = parentObject.Find("PlayerNamePlaceholder").transform.position + (Vector3.up * 5);

            Vector3 newPosition = Vector3.Lerp(start, finish, percent);

            anyText.transform.position = Camera.main.WorldToScreenPoint(newPosition);

            anyText.GetComponent<CanvasGroup>().alpha -= 0.02f;

            percent += 0.02f;

            yield return null;
        }

        anyText.GetComponent<CanvasGroup>().alpha = 1f;
        ResetUI();
    }

    IEnumerator PanCameraWrapper(int randomX, int randomY)
    {
        yield return StartCoroutine(PanCameraToPlayer(map[randomY][randomX].swat));

        if (GetComponent<MazeLoader>().DevelopmentMode)
        {
            Camera.main.transform.position = GameObject.Find("GoodDebugCameraPosition").transform.position;
            Camera.main.transform.rotation = GameObject.Find("GoodDebugCameraPosition").transform.rotation;
        }
    }

    IEnumerator PanCameraToPlayer(GameObject swat)
    {
        float x = swat.transform.position.x - 10;
        float y = swat.transform.position.y + 25;
        float z = swat.transform.position.z - 10;
        newCameraPosition = new Vector3(x, y, z);

        while (Vector3.Distance(Camera.main.transform.position, newCameraPosition) >= 0.01f)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCameraPosition, Time.deltaTime * 2f);

            Camera.main.transform.LookAt(swat.transform);
            //Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, playerCameraTransform.rotation, Time.deltaTime * 2);
            yield return null;
        }
    }

    public void ResetAllCharacterRendererColor()
    {
        foreach (GameObject p in players)
        {
            foreach (Renderer r in p.GetComponentsInChildren<Renderer>())
            {
                r.material.color = Color.white;
            }
        }
    }

    public Room InitializeCharacter(bool _isPlayer = true, Room room = null)
    {
        Room selectedRoom = null;

        // Get Room
        if (room == null)
        {
            List<Room> rooms = GetComponent<MazeLoader>().rooms;

            selectedRoom = rooms[Random.Range(1, rooms.Count)];

            if (room != null &&
                selectedRoom == room)
            {
                do
                {
                    selectedRoom = rooms[Random.Range(1, rooms.Count)];
                }
                while (selectedRoom == room);
            }
        }
        else
            selectedRoom = room;

        // Get Random Room

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
            name = _isPlayer ? "Players" : "Enemies"
        };

        foreach (int characterType in GameSettings.players)
        {
            // Get Random Floor
            do
            {
                randomX = Random.Range(selectedRoom.x + 1, selectedRoom.x + selectedRoom.width - 1);
                randomY = Random.Range(selectedRoom.y + 1, selectedRoom.y + selectedRoom.height - 1);
            }
            while (map[randomY][randomX].swat != null ||
                   map[randomY][randomX].isFilled);

            Vector3 characterPosition = map[randomY][randomX].gameObject.transform.position;

            GameObject characterObject = Instantiate(character, characterPosition, Quaternion.identity);
            characterObject.transform.parent = playersWrapperGameObject.transform;

            GameObject head = character.transform.Find("Geo").gameObject.transform.Find("Soldier_head").gameObject,
                       body = character.transform.Find("Geo").gameObject.transform.Find("Soldier_body").gameObject;

            if (characterType == CLOSE_QUARTERS)
            {
                if (_isPlayer)
                    maxMoves += 3;
                else
                    enemyMaxMoves += 3;

                characterObject.GetComponent<WeaponController>().weapons[CLOSE_QUARTERS].SetActive(true);

                CloseQuartersController controller = characterObject.AddComponent<CloseQuartersController>();
                controller.isPlayer = _isPlayer;
                controller.currentStandingTile = map[randomY][randomX];

                Destroy(characterObject.GetComponent<Player>());
            }
            else
            {
                if (_isPlayer)
                    maxMoves += 2;
                else
                    enemyMaxMoves += 2;

                if (characterType == SNIPER)
                {
                    characterObject.GetComponent<WeaponController>().weapons[SNIPER].SetActive(true);

                    SniperController controller = characterObject.AddComponent<SniperController>();
                    controller.isPlayer = _isPlayer;
                    controller.currentStandingTile = map[randomY][randomX];

                }
                else if (characterType == OPERATOR)
                {
                    characterObject.GetComponent<WeaponController>().weapons[OPERATOR].SetActive(true);

                    OperatorController controller = characterObject.AddComponent<OperatorController>();
                    controller.isPlayer = _isPlayer;
                    controller.currentStandingTile = map[randomY][randomX];
                }

                Destroy(characterObject.GetComponent<Player>());
            }

            RenderMaterial(head, body, characterType);

            map[randomY][randomX].isFilled = true;
            map[randomY][randomX].swat = characterObject;

            // character. <= Is Need Maze Loader Anymore ?

            Rigidbody[] rigidbodies = character.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidbody in rigidbodies)
                rigidbody.isKinematic = true;

            if (_isPlayer)
                players.Add(characterObject);
            else
                enemies.Add(characterObject);
        }

        if (_isPlayer)
            StartCoroutine(PanCameraWrapper(randomX, randomY));

        return selectedRoom;
    }

    private void RenderMaterial(GameObject head, GameObject body, int characterType)
    {
        body.GetComponent<Renderer>().material = swatClassMaterial[characterType];
        head.GetComponent<Renderer>().material = swatClassMaterial[characterType];
    }

    public void LateUpdate()
    {
        if (!isPaused) // Game Is Not In Pause Mode
        {
            if (!GetComponent<MazeLoader>().DevelopmentMode)
            {
                if (this.enableCameraMovement)
                {
                    if (Input.mousePosition.y >= Screen.height)
                    {
                        // Move North
                        Camera.main.transform.Translate(Vector3.right * Time.deltaTime * 25f, Space.World);
                        Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * 25f, Space.World);
                    }

                    else if (Input.mousePosition.y <= 5)
                    {
                        // Move South
                        Camera.main.transform.Translate(Vector3.left * Time.deltaTime * 25f, Space.World);
                        Camera.main.transform.Translate(Vector3.back * Time.deltaTime * 25f, Space.World);
                    }
                    else if (Input.mousePosition.x >= Screen.width * 0.98)
                    {
                        // Move East
                        Camera.main.transform.Translate(Vector3.right * Time.deltaTime * 25f, Space.World);
                        Camera.main.transform.Translate(Vector3.back * Time.deltaTime * 25f, Space.World);

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
                        Camera.main.transform.Translate(Vector3.left * Time.deltaTime * 25f, Space.World);
                        Camera.main.transform.Translate(Vector3.forward * Time.deltaTime * 25f, Space.World);

                        if (Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                        {
                            // Rotate To West
                            Camera.main.transform.Rotate(0.0f,
                                -Time.deltaTime * 100f,
                                0.0f, Space.World);
                        }
                    }
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
