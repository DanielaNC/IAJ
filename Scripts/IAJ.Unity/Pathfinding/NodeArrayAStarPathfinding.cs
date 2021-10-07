using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class NodeArrayAStarPathfinding : AStarPathfinding
    {
        private static int index = 0;
        protected NodeRecordArray NodeRecordArray { get; set; }

        public NodeArrayAStarPathfinding(IOpenSet open, IClosedSet closed, IHeuristic heuristic) : base(null, null, heuristic)
        {

            grid = new Grid<NodeRecord>((Grid<NodeRecord> global, int x, int y) => new NodeRecord(x, y, index++));
            this.InProgress = false;
            this.Heuristic = heuristic;
            this.NodesPerSearch = 15;

            this.NodeRecordArray = new NodeRecordArray(grid.getAll());
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray; 

        }
       
        // In Node Array A* the only thing that changes is how you process the child node, the search occurs the exact same way so you can the parent's method
        protected override void ProcessChildNode(NodeRecord parentNode, NodeRecord neighbourNode)
        {

            var childNode = NodeRecordArray.GetNodeRecord(neighbourNode); //this is needed to use the algorithm node instance instead of the grid node 

            float g = parentNode.gCost + CalculateDistanceCost(parentNode, childNode);
            float h = MOVE_STRAIGHT_COST * this.Heuristic.H(childNode, this.GoalNode);
            float f = g + h;

            if (childNode != null)
            {
                if (childNode.status == NodeStatus.Unvisited)
                {
                    childNode.parent = parentNode;
                    childNode.gCost = g;
                    childNode.fCost = f;
                    childNode.CalculateFCost();
                    NodeRecordArray.AddToOpen(childNode);
                }
                else if (childNode.status == NodeStatus.Open && f < childNode.fCost)
                {
                    childNode.parent = parentNode;
                    childNode.gCost = g;
                    childNode.CalculateFCost();
                    this.Open.Replace(this.Open.SearchInOpen(childNode), childNode);
                }
                else if (childNode.status == NodeStatus.Closed && f < childNode.fCost)
                {
                    childNode.parent = parentNode;
                    childNode.gCost = g;
                    childNode.CalculateFCost();
                    NodeRecordArray.AddToOpen(childNode); //TODO: check
                }

                grid.SetGridObject(childNode.x, childNode.y, childNode);
            }
        }
    }


       
}
