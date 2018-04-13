using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isFilled,
                isWall,
                isHorizontalDoor,
                isVerticalDoor;

    // gCost = Cost Dari Start Node Ke Current Hovered Node
    // hCost = Ini Heuristic
    // fCost = gCost + hCost
    public float gCost, hCost, fCost;

    public Vector3 position;

    public Tile parent;
    public Tile[] neighbours;

    public GameObject plane,
                      wall,
                      door,
                      swat,
                      lamp,
                      chair,
                      table,
                      sofa2,
                      sofa3;

    public bool isDoorClosed = false;

    public Tile(Tile _tile)
    {
        this.isFilled = _tile.isFilled;
        this.isWall = _tile.isWall;
        this.isHorizontalDoor = _tile.isHorizontalDoor;
        this.isVerticalDoor = _tile.isVerticalDoor;
        this.gCost = _tile.gCost;
        this.hCost = _tile.hCost;
        this.fCost = _tile.fCost;
        this.position = _tile.position;
        this.parent = _tile.parent;
        this.neighbours = _tile.neighbours;
        this.plane = _tile.plane;
        this.wall = _tile.wall;
        this.door = _tile.door;
        this.swat = _tile.swat;
        this.lamp = _tile.lamp;
        this.chair = _tile.chair;
        this.table = _tile.table;
        this.sofa2 = _tile.sofa2;
        this.sofa3 = _tile.sofa3;
    }

    public void Flash()
    {
        if (this.swat != null)
            Unflash();
        else if (this.isFilled)
            this.gameObject.GetComponent<Renderer>().material.color = Color.red;
        else
            this.gameObject.GetComponent<Renderer>().material.color = Color.green;
    }

    public void Flash(string hexColorCode)
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(hexColorCode, out newColor);
        this.gameObject.GetComponent<Renderer>().material.color = newColor;
    }

    public void Unflash()
    {
        this.gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
}