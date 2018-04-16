using UnityEngine;

public class CloseQuartersController : Player
{
    public AudioClip attackSound, grenadeExplosionSound;

    public Material material;

    public CloseQuartersController()
    {
        this.playerName = "Close Quarters";
        this.maxRange = 4;
        this.minDamage = 20;
        this.maxDamage = 35;
        this.bulletQty = System.Int32.MaxValue;
        this.grenadeQty = 1;
        this.maxHp = 80;
        this.playerHp = 80;
    }
}
