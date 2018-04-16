using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : MonoBehaviour
{
    public List<Tile> FindFarthestRoom(Tile[][] map, Tile source)
    {
        //source = map[1][1];
        //Dictionary<List<Tile>, int> result = new Dictionary<List<Tile>, int>();

        // Prepare Open Set and Close Set
        // Add Source As First OpenSet Element
        List<Tile> openSet = new List<Tile>() { source };
        List<Tile> closeSet = new List<Tile>();

        // Set The First Element To 0
        // Set The First Element Parent To Null
        source.gCost = 0;
        source.parent = null;

        // Mark All Node Cost To Infinity
        for (int r = 0; r < GameSettings.MAP_HEIGHT; r++)
        {
            for (int c = 0; c < GameSettings.MAP_WIDTH; c++)
            {
                if (map[r][c] == source)
                    continue;

                map[r][c].gCost = System.Int32.MaxValue;
            }
        }

        // While We Still Don't Meet The Destination
        while (openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            openSet.Remove(currentTile);
            closeSet.Add(currentTile);

            foreach (Tile neighbour in currentTile.neighbours)
            {
                /*
                 * Real Dijkstra Will Stop When We Found The Destination
                 */

                if (neighbour == null || 
                    neighbour.isWall ||
                    closeSet.Contains(neighbour) ||
                    neighbour.lamp != null ||
                    neighbour.chair != null ||
                    neighbour.table != null)
                    continue;

                if (!openSet.Contains(neighbour) &&
                    neighbour.gCost > currentTile.gCost)
                {
                    neighbour.gCost = currentTile.gCost + 10;
                    neighbour.parent = currentTile;
                    openSet.Add(neighbour);
                }
            }
        }

        List<Tile> farthestPath = GetFarthestPath(closeSet);

        return farthestPath;
    }

    //List<Tile> path = new List<Tile>();
    public List<Tile> GetFarthestPath(List<Tile> closeSet)
    {
        closeSet = closeSet.OrderByDescending((Tile x) => x.gCost).ToList();

        Tile farthestTile = closeSet[0];

        List<Tile> path = new List<Tile>();

        while (farthestTile.parent != null)
        {
            path.Add(farthestTile);

            //farthestTile.Flash("#00cb4a");

            farthestTile = farthestTile.parent;
        }

        return path;
    }

    //public IEnumerator Wrapper(List<Tile> closeSet)
    //{
    //    yield return PaintTile(closeSet);
    //}

    //public IEnumerator PaintTile(List<Tile> closeSet)
    //{
    //    yield return null;   
    //}
}