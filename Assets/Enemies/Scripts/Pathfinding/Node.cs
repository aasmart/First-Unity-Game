using System.Collections.Generic;
using UnityEngine;

namespace Enemies.Scripts.Pathfinding
{
    public class Node
    {
        private Dictionary<GameObject, NodeData> _nodeDataDict;

        public Vector3 WorldPos;
        public readonly int ExtraCost;
        public readonly int GridX, GridY;
        public readonly bool IsWalkable;

        public Node(int xPos, int yPos, int extraCost, bool isWalkable, Vector3 worldPos)
        {
            _nodeDataDict = new Dictionary<GameObject, NodeData>();
            
            GridX = xPos;
            GridY = yPos;
            ExtraCost = extraCost;
            IsWalkable = isWalkable;
            WorldPos = worldPos;
        }

        public int CompareTo(Node other, GameObject objectKey)
        {
            var compare = GetData(objectKey).FCost.CompareTo(other.GetData(objectKey).FCost);
            if (compare == 0)
                compare = GetData(objectKey).HCost.CompareTo(other.GetData(objectKey).HCost);
            return -compare;
        }

        public void AddKey(GameObject objectKey)
        {
            if (_nodeDataDict.ContainsKey(objectKey) || !objectKey)
                return;
            _nodeDataDict.Add(objectKey, new NodeData(ExtraCost));
        }
        
        public void ClearKey(GameObject objectKey)
        {
            if (_nodeDataDict.ContainsKey(objectKey))
                _nodeDataDict.Remove(objectKey);
        }

        public NodeData GetData(GameObject objectKey)
        {
            AddKey(objectKey);
            return _nodeDataDict[objectKey];
        }
    }

    public class NodeData
    {
        public int HCost, GCost, ExtraCost;
        public int FCost => HCost + GCost + ExtraCost;
        public Node Parent { get; set; }
        public int HeapIndex { get; set; }

        public NodeData(int extraCost)
        {
            ExtraCost = extraCost;
        }
    }
}