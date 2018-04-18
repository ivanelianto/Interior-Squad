using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public MazeLoader mazeLoader;
    public Gameplay gameplay;

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

        gameplay = FindObjectOfType<Gameplay>();

        maxHp = playerHp;
    }

    public void ShootPlayer(GameObject targetPlayer, int damage)
    {
        if (targetPlayer.GetComponent<Player>() == null)
            return;

        if (true)
        {
            Animator attackerAnimator = this.gameObject.GetComponent<Animator>();
            attackerAnimator.SetTrigger("shoot");

            targetPlayer.GetComponent<Player>().playerHp -= damage;

            if (targetPlayer.GetComponent<Player>().playerHp <= 0)
            {
                if (targetPlayer.GetComponent<Player>().currentStandingTile != null)
                {
                    targetPlayer.GetComponent<Player>().currentStandingTile.isFilled = false;
                    targetPlayer.GetComponent<Player>().currentStandingTile.swat = null;
                    targetPlayer.GetComponent<Player>().currentStandingTile = null;
                }

                Animator targetPlayerAnimator = targetPlayer.GetComponent<Animator>();
                targetPlayerAnimator.enabled = false;

                Rigidbody[] targetPlayerRigidbody = targetPlayer.GetComponentsInChildren<Rigidbody>();

                foreach (Rigidbody rb in targetPlayerRigidbody)
                    rb.isKinematic = false;

                Vector3 shootingDirection;
                this.gameObject.transform.LookAt(targetPlayer.transform);

                shootingDirection = this.gameObject.transform.forward;

                targetPlayer.GetComponent<Rigidbody>().AddForce(shootingDirection);

                targetPlayer.layer = LayerMask.NameToLayer("Default");
            }
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
