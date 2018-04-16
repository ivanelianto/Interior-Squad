using UnityEngine;

public class SniperController : Player
{
    public AudioClip attackSound;

    public Material material;

    public SniperController()
    {
        this.playerName = "Sniper";
        this.maxRange = 12;
        this.minDamage = 70;
        this.maxDamage = 100;
        this.bulletQty = 1;
        this.grenadeQty = 0;
        this.maxHp = 100;
        this.playerHp = 100;
    }
}
