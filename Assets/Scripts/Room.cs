public class Room
{
    public int x, y, width, height;

    public Room leftRoom, rightRoom, parent, sibling;

    public Tile[][] roomTile;

    public Room(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.leftRoom = this.rightRoom = this.parent = this.sibling = null;
    }
}
