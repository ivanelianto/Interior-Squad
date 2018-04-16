using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchivedCode : MonoBehaviour
{

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
        //playerScript.playerName = playerName;
        //playerScript.maxRange = maxRange;
        //playerScript.minDamage = minDamage;
        //playerScript.maxDamage = maxDamage;
        //playerScript.bulletQty = bulletQty;
        //playerScript.grenadeQty = grenadeQty;
        //playerScript.maxHp = maxHp;
        //playerScript.playerHp = currentHp;
    }

    public void OldUpdate()
    {
        //// turn
        //if (true)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    RaycastHit hittedSwat, hittedPlane;

        //    Player hoveredPlayerScript = null,
        //           selectedPlayerScript = null;

        //    #region Hover Player
        //    //if (Physics.Raycast(ray, out hittedSwat, 1000, LayerMask.GetMask("Swat")))
        //    //{
        //    //    hoveredSwat = hittedSwat.collider.gameObject;

        //    //    hoveredPlayerScript = hoveredSwat.GetComponent<Player>();

        //    //    if (selectedSwat == null &&
        //    //        selectedSwat != hoveredSwat)
        //    //    {
        //    //        ShowHoveredPlayerState(hoveredPlayerScript);
        //    //    }
        //    //    else if (selectedSwat != null)
        //    //    {
        //    //        selectedPlayerScript = selectedPlayerScript;

        //    //        if (selectedPlayerScript.isAttacking || selectedPlayerScript.isThrowingGrenade)
        //    //            ShowSelectedPlayerState(selectedPlayerScript);
        //    //    }

        //    //    SetPreviousSwat(hittedSwat);
        //    //}
        //    //else if (hoveredSwat != null && !actionMenuUI.activeInHierarchy)
        //    //{
        //    //    playerStateUI.gameObject.SetActive(false);
        //    //    hoveredSwat = null;
        //    //}
        //    #endregion

        //    if (true) // hoveredSwat != null
        //    {
        //        #region Variables
        //        //GameObject geo = hoveredSwat.transform.Find("Geo").gameObject;

        //        //Material head = geo.transform.Find("Soldier_head").gameObject.GetComponent<SkinnedMeshRenderer>().material;

        //        //Material body = geo.transform.Find("Soldier_body").gameObject.GetComponent<SkinnedMeshRenderer>().material;

        //        //Text movesState = null;

        //        //Tile targetTile = null;

        //        //bool validCost = true;
        //        #endregion

        //        if (hoveredPlayerScript.isSelectingTile)
        //        {
        //            if (Physics.Raycast(ray, out hittedPlane, 1000, LayerMask.GetMask("Tiles")))
        //            {
        //                //targetTile = hittedPlane.collider.GetComponent<Tile>();

        //                //if (selectedSwat != null &&
        //                //    !selectedPlayerScript.isAttacking &&
        //                //    !selectedPlayerScript.isThrowingGrenade &&
        //                //    selectedPlayerScript.currentStandingTile != targetTile)
        //                //{
        //                //    movesState = stateCanvas.transform.Find("TurnState").GetComponent<Text>();
        //                //    targetTile.Flash();
        //                //}

        //                #region Select Tile To Go
        //                //if (hoveredPlayerScript.isMoving)
        //                //{
        //                //    if (targetTile != null)
        //                //    {
        //                //        if (targetTile != selectedPlayerScript.currentStandingTile &&
        //                //            targetTile != hoveredPlayerScript.currentStandingTile)
        //                //        {
        //                //            StartCoroutine(hoveredPlayerScript.DrawPath(hoveredPlayerScript.currentStandingTile, targetTile));

        //                //            if (hoveredPlayerScript.totalCost < availableMoves)
        //                //            {
        //                //                anyText.text = "Cost : " + hoveredPlayerScript.totalCost;
        //                //            }
        //                //            else
        //                //            {
        //                //                if (hoveredPlayerScript.totalCost > availableMoves)
        //                //                {
        //                //                    anyText.text = "Not Enough Moves";
        //                //                    targetTile.Flash("#FF0000");
        //                //                }
        //                //                else if (targetTile.isFilled)
        //                //                {
        //                //                    anyText.text = "";
        //                //                    targetTile.Unflash();
        //                //                    line.positionCount = 0;
        //                //                }
        //                //            }

        //                //            anyText.gameObject.SetActive(true);

        //                //            Vector3 screenPoint = Camera.main.WorldToScreenPoint(targetTile.gameObject.transform.position + Vector3.up * 5 + Vector3.right * 2);
        //                //            anyText.transform.position = screenPoint;
        //                //        }
        //                //        else
        //                //        {
        //                //            targetTile.Unflash();
        //                //            line.positionCount = 0;
        //                //        }
        //                //    }
        //                //}

        //                //SetPreviousTile(hittedPlane);
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
}
