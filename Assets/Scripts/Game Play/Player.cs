using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public MazeLoader mazeLoader;

    [HideInInspector]
    public int totalCost = 0;

    [HideInInspector]
    public bool isSelectingTile = false,
                isMoving = false,
                isAttacking = false,
                isThrowingGrenade = false;

    public Tile currentStandingTile = null;

    public bool isPlayer = false;

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
        if (targetPlayer.GetComponent<Player>() == null)
            return;

        Animator attackerAnimator = this.gameObject.GetComponent<Animator>();
        attackerAnimator.SetTrigger("shoot");

        targetPlayer.GetComponent<Player>().playerHp -= damage;

        if (targetPlayer.GetComponent<Player>().playerHp <= 0)
        {
            Animator targetPlayerAnimator = targetPlayer.GetComponent<Animator>();
            targetPlayerAnimator.enabled = false;

            Rigidbody[] targetPlayerRigidbody = targetPlayer.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rb in targetPlayerRigidbody)
                rb.isKinematic = false;

            Vector3 shootingDirection;
            this.gameObject.transform.LookAt(targetPlayer.transform);

            shootingDirection = this.gameObject.transform.forward;

            targetPlayer.GetComponent<Rigidbody>().AddForce(shootingDirection * 0.01f, ForceMode.Impulse);
            
            targetPlayer.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void Attack()
    {
        isSelectingTile = true;
        isAttacking = true;
    }

    public void Move()
    {
        isSelectingTile = true;
        isMoving = true;
    }

    public void ThrowGrenade()
    {
        isSelectingTile = true;
        isThrowingGrenade = true;
    }

    public void CloseDoor()
    {
        mazeLoader.ResetAllPlayersRendererColor();
    }

    public void OpenDoor()
    {
        mazeLoader.ResetAllPlayersRendererColor();
    }
}
