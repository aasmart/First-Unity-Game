using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies.Scripts.Pathfinding
{
    public class AStarPathfinder : MonoBehaviour
    {
        [SerializeField] private Tilemap tileMap;
        private PathfindingGrid _grid;
        private PathfindingGrid Grid => _grid ??= new PathfindingGrid(tileMap);

        private void Start()
        {
            if(tileMap == null)
                return;
            _grid = new PathfindingGrid(tileMap);
        }

        public List<Node> GetEntityPath(Vector3 entityPosition, Vector3 targetPosition, GameObject objectKey)
        {
            var startNode = Grid.GetNode(entityPosition);
            var endNode = Grid.GetNode(targetPosition);
            var path = FindPath(startNode, endNode, objectKey);

            return path;
        }

        private List<Node> FindPath(Node start, Node end, GameObject objectKey)
        {
            /*if (!start.isNavigable() || !end.isNavigable())
            {
                System.out.println("Start and end nodes must both be navigable!");
                return null;
            }*/
            var openNodes = new AStarHeap(Grid.Size);
            openNodes.Add(start, objectKey);
            var closedNodes = new HashSet<Node>();
 
            while (openNodes.GetItemCount() > 0)
            {
                var currentNode = openNodes.RemoveFirst(objectKey);
                closedNodes.Add(currentNode);

                if (currentNode == end)
                    break;
                
                foreach (var neighbor in FindNeighbors(currentNode))
                {
                    if(!neighbor.IsWalkable || closedNodes.Contains(neighbor))
                        continue;
                    
                    var newCostToNeighbor = neighbor.GetData(objectKey).GCost + CalculateDistance(currentNode, neighbor);
                    if (newCostToNeighbor >= neighbor.GetData(objectKey).GCost && openNodes.Contains(neighbor, objectKey)) continue;
                    
                    // Update costs
                    neighbor.GetData(objectKey).GCost = newCostToNeighbor;
                    neighbor.GetData(objectKey).HCost = CalculateDistance(neighbor, end);
                    neighbor.GetData(objectKey).Parent = currentNode;

                    if (!openNodes.Contains(neighbor, objectKey))
                        openNodes.Add(neighbor, objectKey);
                    else
                        openNodes.UpdateItem(neighbor, objectKey);
                }
            }

            var path = new List<Node>();
            var endNode = end;
            
            while (endNode != start)
            {
                if (endNode == null)
                {
                    return null;
                }

                path.Add(endNode);
                var endParent = endNode.GetData(objectKey).Parent;
                    endNode.ClearKey(objectKey);
                endNode = endParent;
            }

            path.Add(endNode);
            endNode.ClearKey(objectKey);

            // Reverse path
            for (int i = 0, j = path.Count - 1; i < j; i++, j--)
            {
                (path[i], path[j]) = (path[j], path[i]);
            }

            return path;
        }

        private List<Node> FindNeighbors(Node node)
        {
            var nodes = new List<Node>();

            for (var y = -1; y <= 1; y++)
            {
                var yPos = node.GridY + y;
                // Determine if the y position is in bounds
                if (yPos < 0 || yPos >= Grid.SizeY)
                    continue;

                for (var x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var xPos = node.GridX + x;
                    // Determine if the x position is in bounds
                    if (xPos < 0 || xPos >= Grid.SizeX)
                        continue;

                    // Get the neighboring node and check if it can be navigated
                    var checkNode = Grid._grid[yPos, xPos];
                    nodes.Add(checkNode);
                }
            }

            return nodes;
        }

        private static int CalculateDistance(Node nodeA, Node nodeB)
        {
            var distanceX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var distanceY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            /* If the distance in the x direction is greater than the y, calculate the distance along the diagonal the
            path must take. Then, add it to the horizontal distance the path must take. Otherwise, do the opposite*/
            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        public PathfindingGrid GetGrid()
        {
            return Grid;
        }

        public static void DrawPath(List<Node> path)
        {
            if (!Application.isPlaying || path is null)
                return;

            Gizmos.color = new Color(0.75f, 0, 0, 0.75f);
            Gizmos.DrawCube(path[0].WorldPos, Vector3.one);
            Gizmos.color = new Color(0, 0.75f, 0, 0.75f);
            Gizmos.DrawCube(path[^1].WorldPos, Vector3.one);
            
            Gizmos.color = Color.white;
            for(var i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i].WorldPos, path[i+1].WorldPos);
            }
        }

        private void OnDrawGizmos()
        {
            if(_grid is null)
                return;
            
            for (var y = 0; y < _grid.SizeY; y++)
            {
                for (var x = 0; x < _grid.SizeX; x++)
                {
                    var node = _grid._grid[y, x];
                    Gizmos.color = node.IsWalkable ? new Color(0.0f, 0.75f, 0.0f, 0.25f + (node.ExtraCost/100.0f)) : new Color(0.75f, 0.0f, 0.0f, 0.25f);;
                    Gizmos.DrawCube(node.WorldPos, Vector3.one);
                }
            }
        }
    }
}