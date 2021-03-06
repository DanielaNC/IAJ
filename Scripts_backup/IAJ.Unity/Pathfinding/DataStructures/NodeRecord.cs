
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public enum NodeStatus
    {
        Unvisited,
        Open,
        Closed
    }

    public enum StartingEdge
    {
        None,
        Source,
        Top,
        Left,
        Bottom,
        Right,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class NodeRecord  : IComparable<NodeRecord>
    {
        //Coordinates
        public int x;
        public int y;
        public bool isWalkable;

        //A* Stuff
        public NodeRecord parent;
        public float gCost;
        public float hCost;
        public float fCost;

        // Node Record Array Index
        public int index;
        public NodeStatus status;

        // Node Record Starting Edge
        public StartingEdge startingEdge;
        
        public override string ToString()
        {
            return x + ":" + y;
        }

        public int CompareTo(NodeRecord other)
        {
            return this.fCost.CompareTo(other.fCost);
        }

        public NodeRecord(int x, int y)
        {
            
            this.x = x;
            this.y = y;
            gCost = int.MaxValue;
            hCost = 0;
            fCost = gCost + hCost;
            parent = null;
            index = 0;
            isWalkable = true;
            status = NodeStatus.Unvisited;
            startingEdge = StartingEdge.None;
        }

          public NodeRecord(int x, int y, int _index)
        {

            this.x = x;
            this.y = y;
            gCost = int.MaxValue;
            hCost = 0;
            fCost = gCost + hCost;
            parent = null;
            index = _index;
            isWalkable = true;
            status = NodeStatus.Unvisited;
            startingEdge = StartingEdge.None;
        }
        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        
        //two node records are equal if they refer to the same node
        public override bool Equals(object obj)
        {
            if (obj is NodeRecord target) return this.x == target.x && this.y == target.y;
            else return false;
        }


        // I wonder where this might be useful...
        public void Reset()
        {
            gCost = int.MaxValue;
            hCost = 0;
            fCost = gCost + hCost;
            parent = null;
            status = NodeStatus.Unvisited;
        }

    }
}
