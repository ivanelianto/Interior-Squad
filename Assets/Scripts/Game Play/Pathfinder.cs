using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
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

                if (LineOfSight(n, curr.parent))
                {
                    // +1 Jika Diagonal Move
                    //cost += 1;
                    n.parent = curr.parent;
                }

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
        return true;
    }

    public Dictionary<List<Tile>, int> Backtracking(Tile start, Tile destination)
    {
        List<Tile> nodes = new List<Tile>() { destination };

        int totalCost = 0;

        while (destination.parent != destination)
        {
            nodes.Add(destination.parent);
            destination = destination.parent;
            totalCost++;
        }

        nodes.Reverse();

        Dictionary<List<Tile>, int> result = new Dictionary<List<Tile>, int>();
        result.Add(nodes, totalCost);

        return result;
    }
}
