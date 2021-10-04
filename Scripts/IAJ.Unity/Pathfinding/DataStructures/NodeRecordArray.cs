﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class NodeRecordArray : IOpenSet, IClosedSet
    {
        private NodeRecord[] NodeRecords { get; set; }
        private NodePriorityHeap Open { get; set; }

        public NodeRecordArray(List<NodeRecord> nodes)
        {
            //this method creates and initializes the NodeRecordArray for all nodes in the Navigation Graph
            this.NodeRecords = new NodeRecord[nodes.Count];
            
            for(int i = 0; i < nodes.Count; i++)
            {
                this.NodeRecords[i] = new NodeRecord(nodes[i].x, nodes[i].y) {index = i };
            }

            this.Open = new NodePriorityHeap();
        }

        public NodeRecord GetNodeRecord(NodeRecord node)
        {
            return NodeRecords[node.index];
        }

        void IOpenSet.Initialize()
        {
            this.Open.Initialize();
            //we want this to be very efficient (that's why we use for)
            for (int i = 0; i < this.NodeRecords.Length; i++)
            {
                if(NodeRecords[i].isWalkable)
                this.NodeRecords[i].status = NodeStatus.Unvisited;
            }

        }

        void IClosedSet.Initialize()
        {
        
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.Open.AddToOpen(nodeRecord);
            nodeRecord.status = NodeStatus.Open;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            //RemoveFromOpen sets status to unvisited, this leads to certain doubts...
            //TODO: Check if correct
            this.Open.RemoveFromOpen(nodeRecord);
            nodeRecord.status = NodeStatus.Closed;
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            //A* Node Array makes use of node indexes to improve efficiency
            return nodeRecord.status == NodeStatus.Open ? this.NodeRecords[nodeRecord.index] : null;
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            //A* Node Array makes use of node indexes to improve efficiency
            return nodeRecord.status == NodeStatus.Closed ? this.NodeRecords[nodeRecord.index] : null;
        }

        public NodeRecord GetBestAndRemove()
        {
            return this.Open.GetBestAndRemove();
        }

        public NodeRecord PeekBest()
        {
            return this.Open.PeekBest();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.Open.Replace(nodeToBeReplaced, nodeToReplace);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.Open.RemoveFromOpen(nodeRecord);
            nodeRecord.status = NodeStatus.Unvisited;
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            //TODO: check
            if (this.NodeRecords[nodeRecord.index].status == NodeStatus.Closed)
            {
                this.NodeRecords[nodeRecord.index].status = NodeStatus.Unvisited;
            }
        }

        ICollection<NodeRecord> IOpenSet.All()
        {
            return this.Open.All();
        }

        ICollection<NodeRecord> IClosedSet.All()
        {
            return this.NodeRecords.Where(node => node.status == NodeStatus.Closed).ToList();
        }

        public int CountOpen()
        {
            return this.Open.CountOpen();
        }

        public void ClearFill()
        {
            foreach (NodeRecord n in this.NodeRecords)
            {
                n.startingEdge = StartingEdge.None;
            }
        }
    }
}
