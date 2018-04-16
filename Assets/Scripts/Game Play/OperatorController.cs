using UnityEngine;

public class OperatorController : Player
{
    public AudioClip attackSound;

    public Material material;

    public OperatorController()
    {
        this.playerName = "Operator";
        this.maxRange = 6;
        this.minDamage = 20;
        this.maxDamage = 50;
        this.bulletQty = System.Int32.MaxValue;
        this.grenadeQty = 0;
        this.maxHp = 100;
        this.playerHp = 100;
    }
}
