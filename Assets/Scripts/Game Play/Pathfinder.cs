using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private MazeLoader mazeLoader;

    private void Start()
    {
        mazeLoader = GetComponent<MazeLoader>();
    }
    // Theta* Algorithm
    public Dictionary<List<Tile>, int> PathFinding(Tile start, Tile destination)
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
                if (n == null || n.isFilled || n.isWall || closedSet.Contains(n))
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
                //    n.isLineOfSight = true;
                //    n.parent = curr.parent;
                //}

                if (LineOfSightTipuTipu(n, curr.parent))
                {
                    n.isLineOfSight = true;
                    n.parent = curr.parent;
                }

                openSet.Add(n);

                if (n == destination)
                {
                    return Backtracking(start, destination);
                }
            }

            // sort list ascending
            openSet.Sort((a, b) => a.fCost.CompareTo(b.fCost));
        }

        return null;
    }

    public bool LineOfSightTipuTipu(Tile start, Tile destination)
    {
        Vector3 direction = destination.position - start.position;

        if (Physics.Raycast(start.position + Vector3.up, direction, direction.magnitude))
            return false;

        return true;
    }

    public bool LineOfSight(Tile start, Tile destination)
    {
        int x1 = (int)start.GetX();
        int y1 = (int)start.GetY();
        int x2 = (int)destination.GetX();
        int y2 = (int)destination.GetY();

        int dx = x2 - x1;
        int dy = y2 - y1;

        int stepx = (dx > 0) ? 1 : -1;
        int stepy = (dy > 0) ? 1 : -1;

        int limiter = (dx > dy) ? dx : dy;

        int currx = x1;
        int curry = y1;

        int error = 0;
        int errorprev = 0;
        error = errorprev = (dx < dy) ? dx : dy;

        for (int i = 0; i < limiter; i++)
        {
            if (dx > dy)
            {
                currx += stepx;
                error += dy;
                if (error > dx)
                {
                    curry += stepy;
                    error -= dx;
                    if (error + errorprev < dx)
                    {
                        if (mazeLoader.map[curry - stepy][currx].isFilled)
                            return false;
                    }
                    else if (error + errorprev > dx)
                    {
                        if (mazeLoader.map[curry][currx - stepx].isFilled)
                            return false;
                    }
                    else
                    {
                        if (mazeLoader.map[curry - stepy][currx].isFilled)
                            return false;
                        if (mazeLoader.map[curry][currx - stepx].isFilled)
                            return false;
                    }
                }
            }
            else
            {
                curry += stepy;
                error += dx;
                if (error > dy)
                {
                    currx += stepx;
                    error -= dy;
                    if (error + errorprev < dy)
                    {
                        if (mazeLoader.map[curry][currx - stepx].isFilled)
                            return false;
                    }
                    else if (error + errorprev > dy)
                    {
                        if (mazeLoader.map[curry - stepy][currx].isFilled)
                            return false;
                    }
                    else
                    {
                        if (mazeLoader.map[curry - stepy][currx].isFilled)
                            return false;
                        if (mazeLoader.map[curry][currx - stepx].isFilled)
                            return false;
                    }
                }
            }
            errorprev = error;
        }

        return true;
    }

    public Dictionary<List<Tile>, int> Backtracking(Tile start, Tile destination)
    {
        List<Tile> nodes = new List<Tile>() { destination };

        int totalCost = 0;

        while (destination.parent != destination)
        {
            if (destination.isLineOfSight)
                totalCost++;

            totalCost++;

            nodes.Add(destination.parent);

            destination = destination.parent;
        }

        nodes.Reverse();

        Dictionary<List<Tile>, int> result = new Dictionary<List<Tile>, int>();

        result.Add(nodes, totalCost - 1);

        return result;
    }
}
