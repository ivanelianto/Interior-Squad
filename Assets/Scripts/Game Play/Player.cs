using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public Tile currentStandingTile = null;

    [HideInInspector]
    public MazeLoader mazeLoader;

    public string playerName;

    public int playerHp, maxHp, maxDamage, minDamage, maxRange, grenadeQty, bulletQty;

    [HideInInspector]
    public int totalCost = 0,
               characterType = -1;

    [HideInInspector]
    public bool isSelectingTile = false,
                isMoving = false,
                isAttacking = false,
                isThrowingGrenade = false,
                isPlayer = false;

    private GameObject hoveredSwat = null,
                       previousHittedTile = null;

    private Animator animator;

    private List<Tile> currentPath;

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

    public void MenuAttack()
    {
        mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
        isSelectingTile = true;
        isAttacking = true;
    }

    public void MenuMove()
    {
        mazeLoader.actionMenuUI.SetActive(false);
        isSelectingTile = true;
        isMoving = true;
    }

    public void MenuThrowGrenade()
    {
        mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
        isSelectingTile = true;
        isThrowingGrenade = true;
        animator.SetTrigger("throw");
    }

    public void CloseDoor()
    {
        mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
    }

    public void OpenDoor()
    {
        mazeLoader.actionMenuUI.SetActive(false);
        mazeLoader.ResetAllPlayersRendererColor();
    }

    public IEnumerator DrawPath(Tile start, Tile destination)
    {
        LineRenderer l = MazeLoader.line;

        if (
            !destination.isFilled
            &&
            destination != start)
        {
            // 0 = Pathnya
            // 1 = Cost
            Text ui = mazeLoader.stateCanvas.transform.Find("MultifunctionalText").GetComponent<Text>();

            List<Tile> path = PathFinding(start, destination);

            if (path != null && path.Count > 0)
            {
                l.positionCount = path.Count;

                for (int i = 0; i < path.Count; i++)
                {
                    l.SetPosition(i, path[i].position + Vector3.up);
                }

                currentPath = path;
            }
            else
            {
                ui.gameObject.SetActive(false);
            }
        }
        else
            l.positionCount = 0;

        yield return null;
    }

    public IEnumerator MovePlayer(GameObject playerToMove)
    {
        mazeLoader.selectedPlayerStateUI.gameObject.SetActive(false);

        if (MazeLoader.line.positionCount > 0)
        {
            currentStandingTile.isFilled = false;
            currentStandingTile.swat = null;

            playerToMove.GetComponent<Animator>().SetTrigger("run");

            for (int i = 0; i < MazeLoader.line.positionCount; i++)
            {
                while (playerToMove.transform.position != MazeLoader.line.GetPosition(i) + Vector3.down)
                {
                    playerToMove.transform.position =
                      Vector3.MoveTowards(playerToMove.transform.position, MazeLoader.line.GetPosition(i) + Vector3.down, Time.deltaTime * 25f);

                    playerToMove.transform.LookAt(MazeLoader.line.GetPosition(i) + Vector3.down);

                    yield return null;
                }
            }

            ResetManipulateUIAfterAction();

            yield return null;
        }

        playerToMove.GetComponent<Animator>().SetTrigger("idle");
    }

    public void ResetManipulateUIAfterAction()
    {
        MazeLoader.line.positionCount = 0;

        mazeLoader.ResetAllPlayersRendererColor();

        Text ui = mazeLoader.stateCanvas.transform.Find("MultifunctionalText").GetComponent<Text>();
        ui.gameObject.SetActive(false);

        mazeLoader.selectedPlayerStateUI.SetActive(false);
    }

    // Theta* Algorithm
    public List<Tile> PathFinding(Tile start, Tile destination)
    {
        if (destination == start)
            return null;

        // For Theta LineOfSight
        start.parent = start;

        // buat 2 list, open sama closed
        // open buat daftar yang akan dituju, closed buat daftar yg dah dikunjungi
        List<Tile> openSet = new List<Tile>(),
                   closedSet = new List<Tile>();
        
        // masukkan tile pertama
        openSet.Add(start);
        start.gCost = 0;

        int cost = 0;

        // selama masih ada tile, terus buka
        while (openSet.Count > 0)
        {
            Tile curr = openSet[0];
            openSet.Remove(curr);
            closedSet.Add(curr);

            // buka tiap 4 arah tile
            foreach (Tile n in curr.neighbours)
            {
                // kalo tile itu dinding, etc..... diabaikan
                if (n == null || n.isWall || closedSet.Contains(n))
                    continue;

                // kalo udah ada di openSet, check apakah lbih hemat
                float g = curr.gCost + 10;
                float h = Vector3.Distance(n.position, destination.position);
                float f = n.gCost + n.hCost;

                if (openSet.Contains(n))
                {
                    // utk cek kalo ada rute lebih hemat ke tile ini
                    // kalo lebih hemat...
                    // set ulang semuanya(cost, parent)
                    if (n.fCost > f)
                    {
                        n.gCost = g;
                        n.hCost = h;
                        n.fCost = f;
                        n.parent = curr;
                    }
                }
                else
                {
                    n.gCost = g;
                    n.hCost = h;
                    n.fCost = f;
                    n.parent = curr;
                    openSet.Add(n);
                }
                // kasi cost ke tiap tile n

                //if (LineOfSight(n, curr.parent))
                //{
                //    // +1 Jika Diagonal Move
                //    cost += 1;
                //    n.parent = curr.parent;
                //}

                openSet.Add(n);

                if (n == destination)
                {
                    // fungsi GetPathFromDestToStart : List<Tile> -> return
                    return Backtracking(start, destination);
                }
            }

            // sort list ascending
            openSet.Sort((a, b) => a.fCost.CompareTo(b.fCost));
        }

        return null;
    }

    public bool LineOfSight(Tile start, Tile destination)
    {


        //Vector3 direction = destination.position - start.position;

        //if (Physics.Raycast(start.position + Vector3.up, direction, direction.magnitude))
        //{
        //    return false;
        //}

        //return true;

        //float num1 = start.transform.position.x - start.transform.position.x;
        //float num2 = start.transform.position.y - start.transform.position.y;
        //float num3 = num1 != 0 ? Math.Sign(num1) : 0;
        //float num4 = num2 != 0 ? Math.Sign(num2) : 0;
        //float a = Mathf.Abs(num1);
        //float b = Mathf.Abs(num2);
        //float num5 = Mathf.Max(a, b);
        //float num6 = (float)(num5 / 2);
        //float x = start.transform.position.x;
        //float y = start.transform.position.y;

        //if (a > b)
        //{
        //    for (int index = 0; index < num5; ++index)
        //    {
        //        int yIndex = (int) y;

        //        if (mazeLoader.map[(int) y][(int) x].isFilled)
        //            return false;

        //        x += num3;

        //        num6 += b;

        //        if (num6 >= a)
        //        {
        //            y += num4;
        //            num6 -= (float)a;

        //            if (mazeLoader.map[y - num4, x] == 1 || 
        //                this.map[y, x - num3] == 1 || 
        //                (this.map[y - num4, x] == 2 || this.map[y, x - num3] == 2) || 
        //                (this.map[y - num4, x] == 3 || this.map[y, x - num3] == 3))
        //                return false;
        //        }
        //    }
        //}
        //else
        //{
        //    for (int index = 0; index < num5; ++index)
        //    {
        //        if (this.map[y, x] == 1 || this.map[y, x] == 2 || this.map[y, x] == 3)
        //            return false;
        //        y += num4;
        //        num6 += (float)a;
        //        if ((double)num6 >= (double)b)
        //        {
        //            x += num3;
        //            num6 -= (float)b;
        //            if (this.map[y - num4, x] == 1 || this.map[y, x - num3] == 1 || (this.map[y - num4, x] == 2 || this.map[y, x - num3] == 2) || (this.map[y - num4, x] == 3 || this.map[y, x - num3] == 3))
        //                return false;
        //        }
        //    }
        //}
        return true;
    }

    public List<Tile> Backtracking(Tile start, Tile destination)
    {
        List<Tile> nodes = new List<Tile>() { destination };

        while (destination.parent != destination)
        {
            nodes.Add(destination.parent);
            destination = destination.parent;
        }
    
        nodes.Reverse();

        totalCost = nodes.Count - 1;

        return nodes;
    }
}
