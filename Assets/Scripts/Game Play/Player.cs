using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Tile currentStandingTile = null;

    [HideInInspector]
    public MazeLoader mazeLoader;

    [HideInInspector]
    public int totalCost = 0,
               characterType = -1;

    [HideInInspector]
    public bool isSelectingTile = false,
                isMoving = false,
                isAttacking = false,
                isThrowingGrenade = false,
                isPlayer = false;

    public string playerName;

    public int playerHp, maxHp, maxDamage, minDamage, maxRange, grenadeQty, bulletQty;

    // Use this for initialization
    void Start()
    {
        mazeLoader = GameObject.Find("GameManager").GetComponent<MazeLoader>();

        maxHp = playerHp;
    }

    public void ShootPlayer(GameObject targetPlayer, int damage)
    {
        Animator attackerAnimator = MazeLoader.selectedSwat.GetComponentInChildren<Animator>();
        attackerAnimator.SetTrigger("shoot");

        isAttacking = false;

        targetPlayer.GetComponentInChildren<Player>().playerHp -= damage;

        if (targetPlayer.GetComponentInChildren<Player>().playerHp <= 0)
        {
            Animator targetPlayerAnimator = targetPlayer.GetComponent<Animator>();
            Rigidbody[] targetPlayerRigidbody = targetPlayer.GetComponentsInChildren<Rigidbody>();

            targetPlayerAnimator.enabled = false;

            foreach (Rigidbody rb in targetPlayerRigidbody)
                rb.isKinematic = false;

            Vector3 shootingDirection;
            MazeLoader.selectedSwat.transform.LookAt(targetPlayer.transform);

            shootingDirection = MazeLoader.selectedSwat.transform.forward;

            targetPlayer.GetComponent<Rigidbody>().AddForce(shootingDirection);
            
            targetPlayer.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void Attack()
    {
        //mazeLoader.actionMenuUI.SetActive(false);
        //mazeLoader.ResetAllPlayersRendererColor();
        //isSelectingTile = true;
        //isAttacking = true;
    }

    public void Move()
    {
        //mazeLoader.actionMenuUI.SetActive(false);
        //isSelectingTile = true;
        //isMoving = true;
    }

    public void ThrowGrenade()
    {
        //mazeLoader.actionMenuUI.SetActive(false);
        //mazeLoader.ResetAllPlayersRendererColor();
        //isSelectingTile = true;
        //isThrowingGrenade = true;
        //animator.SetTrigger("throw");
    }

    public void CloseDoor()
    {
        //mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
    }

    public void OpenDoor()
    {
        //mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
    }

    //public IEnumerator DrawPath(Tile start, Tile destination)
    //{
        //LineRenderer l = MazeLoader.line;

        //if (
        //    !destination.isFilled
        //    &&
        //    destination != start)
        //{
        //    // 0 = Pathnya
        //    // 1 = Cost
        //    Text ui = mazeLoader.stateCanvas.transform.Find("MultifunctionalText").GetComponent<Text>();

        //    //Dictionary<List<Tile>, int> pathFindingResult = PathFinding(start, destination);
        //    //List<Tile> path = ;

        //    if (path != null && path.Count > 0)
        //    {
        //        l.positionCount = path.Count;

        //        for (int i = 0; i < path.Count; i++)
        //        {
        //            l.SetPosition(i, path[i].position + Vector3.up);
        //        }

        //        currentPath = path;
        //    }
        //    else
        //    {
        //        ui.gameObject.SetActive(false);
        //    }
        //}
        //else
        //    l.positionCount = 0;

        //yield return null;
    //}

    public IEnumerator MovePlayer(GameObject playerToMove)
    {
        //mazeLoader.selectedPlayerStateUI.gameObject.SetActive(false);

        //if (MazeLoader.line.positionCount > 0)
        //{
        //    currentStandingTile.isFilled = false;
        //    currentStandingTile.swat = null;

        //    playerToMove.GetComponent<Animator>().SetTrigger("run");

        //    for (int i = 0; i < MazeLoader.line.positionCount; i++)
        //    {
        //        while (playerToMove.transform.position != MazeLoader.line.GetPosition(i) + Vector3.down)
        //        {
        //            playerToMove.transform.position =
        //              Vector3.MoveTowards(playerToMove.transform.position, MazeLoader.line.GetPosition(i) + Vector3.down, Time.deltaTime * 25f);

        //            playerToMove.transform.LookAt(MazeLoader.line.GetPosition(i) + Vector3.down);

        //            yield return null;
        //        }
        //    }

        //    ResetManipulateUIAfterAction();

            yield return null;
        //}

        //playerToMove.GetComponent<Animator>().SetTrigger("idle");
    }

    public void ResetManipulateUIAfterAction()
    {
        //MazeLoader.line.positionCount = 0;

        //mazeLoader.ResetAllPlayersRendererColor();

        //Text ui = mazeLoader.stateCanvas.transform.Find("MultifunctionalText").GetComponent<Text>();
        //ui.gameObject.SetActive(false);

        //mazeLoader.selectedPlayerStateUI.SetActive(false);
    }

}
