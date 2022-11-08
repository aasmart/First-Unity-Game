using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies.Scripts.Pathfinding
{
    public class PathfindingGrid
    {
        public Node[,] _grid;
        public int Size { get; }
        public int SizeX { get; }
        public int SizeY { get; }

        public PathfindingGrid(Tilemap tilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

            int xBounds = bounds.size.x;
            int yBounds = bounds.size.y;
            _grid = new Node[yBounds, xBounds];
            Size = xBounds * yBounds;
            SizeX = xBounds;
            SizeY = yBounds;
            
            for (var x = 0; x < xBounds; x++)
            {
                for (var y = 0; y < yBounds; y++)
                {
                    var tile = allTiles[x + y * xBounds];
                    
                    // Add extra cost if a surrounding node is not walkable
                    var extraCost = 0;
                    for (var nodeX = -1; nodeX <= 1; nodeX++)
                    {
                        for (var nodeY = -1; nodeY <= 1; nodeY++)
                        {
                            if (nodeX == 0 && nodeY == 0)
                                continue;
                            var check = (x + nodeX) + (y + nodeY) * xBounds;
                            if(check < 0 || check >= allTiles.Length)
                                continue;

                            if (allTiles[check] != null) 
                                continue;
                            
                            extraCost = 25;
                            break;
                        }
                    }
                    
                    var worldPosition = tilemap.origin + new Vector3Int(x, y) + new Vector3(0.5f, 0.5f, 0);
                    _grid[y, x] = new Node(x, y, extraCost, tile != null, worldPosition);
                }
            }
        }

        public Node GetNode(Vector3 worldPos)
        {
            var percentX = Mathf.Clamp01(worldPos.x / SizeX + 0.5f);
            var percentY = Mathf.Clamp01((worldPos.y + 0.5f) / SizeY + 0.5f);

            var x = Mathf.RoundToInt((SizeX) * percentX);
            var y = Mathf.RoundToInt((SizeY) * percentY);
            return _grid[y - 1, x - 1];
        }
    }
}
