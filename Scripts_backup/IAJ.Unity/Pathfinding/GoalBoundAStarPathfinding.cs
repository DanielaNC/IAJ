using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Grid;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class GoalBoundAStarPathfinding : NodeArrayAStarPathfinding
    {
        // You can create a bounding box in several differente ways, this is simply suggestion
        // Goal Bounding Box for each Node  direction - Bounding limits: minX, maxX, minY, maxY
        public Dictionary<Vector2, Dictionary<StartingEdge, Vector4>> goalBounds;

        public GoalBoundAStarPathfinding(IOpenSet open, IClosedSet closed, IHeuristic heuristic) : base(open, closed, heuristic)
        {
            goalBounds = new Dictionary<Vector2, Dictionary<StartingEdge, Vector4>>();

        }

        public void MapPreprocess()
        {
            var timer = Time.realtimeSinceStartup;
            for (int i = 0; i < grid.getHeight(); i++)
            {
                for (int j = 0; j < grid.getWidth(); j++)
                {
                    //If current tile is not walkable we don't preprocess it
                    if (!grid.GetGridObject(j, i).isWalkable)
                    {
                        continue;
                    }

                    //Initialize goal
                    goalBounds.Add(new Vector2(j, i), new Dictionary<StartingEdge, Vector4>());

                    //Initialize the Floodfill the grid for each direction..
                    if (i < grid.getHeight() - 1 && grid.GetGridObject(j, i + 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j, i + 1)).startingEdge = StartingEdge.Top;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.Top, new Vector4(j, j, i + 1, i + 1));
                    }

                    if (j < grid.getWidth() - 1 && grid.GetGridObject(j + 1, i).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j + 1, i)).startingEdge = StartingEdge.Right;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.Right, new Vector4(j + 1, j + 1, i, i));
                    }

                    if (i > 0 && grid.GetGridObject(j, i - 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j, i - 1)).startingEdge = StartingEdge.Bottom;
                        grid.GetGridObject(j, i - 1).startingEdge = StartingEdge.Bottom;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.Bottom, new Vector4(j, j, i - 1, i - 1));
                    }

                    if (j > 0 && grid.GetGridObject(j - 1, i).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j - 1, i)).startingEdge = StartingEdge.Left;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.Left, new Vector4(j - 1, j - 1, i, i));
                    }

                    //TODO: In our case, a path can also be composed of diagonals, this begs the question: in goal bound A* are bottom-left/bottom-right/top-left/top-right valid directions?
                    //TODO: Check this with the teacher, but for now, lets consider this is the case

                    if (j < grid.getWidth() - 1 && i < grid.getHeight() - 1 && grid.GetGridObject(j + 1, i + 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j + 1, i + 1)).startingEdge = StartingEdge.TopRight;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.TopRight, new Vector4(j + 1, j + 1, i + 1, i + 1));
                    }

                    if (j > 0 && i < grid.getHeight() - 1 && grid.GetGridObject(j - 1, i + 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j - 1, i + 1)).startingEdge = StartingEdge.TopLeft;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.TopLeft, new Vector4(j - 1, j - 1, i + 1, i + 1));
                    }

                    if (j < grid.getWidth() - 1 && i > 0 && grid.GetGridObject(j + 1, i - 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j + 1, i - 1)).startingEdge = StartingEdge.BottomRight;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.BottomRight, new Vector4(j + 1, j + 1, i - 1, i - 1));
                    }

                    if (j > 0 && i > 0 && grid.GetGridObject(j - 1, i - 1).isWalkable)
                    {
                        this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j - 1, i - 1)).startingEdge = StartingEdge.BottomLeft;
                        goalBounds[new Vector2(j, i)].Add(StartingEdge.BottomLeft, new Vector4(j - 1, j - 1, i - 1, i - 1));
                    }

                    // Floodfill the grid
                    FloodFill(this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j, i)));

                    // Calculate the bounding box and repeat
                    ComputeBoundingBox(this.NodeRecordArray.GetNodeRecord(grid.GetGridObject(j, i)));
                    this.NodeRecordArray.ClearFill();
                    /*Debug.Log("Debugging Node: " + j + " | " + i);

                    foreach (StartingEdge e in goalBounds[new Vector2(j, i)].Keys)
                    {
                        Debug.Log("Bouding Box " + e + " : (" + goalBounds[new Vector2(j, i)][e].x + " | " + goalBounds[new Vector2(j, i)][e].y + " | " + goalBounds[new Vector2(j, i)][e].z + " | " + goalBounds[new Vector2(j, i)][e].w + ")");
                    }*/

                }
            }
            Debug.Log("Map processing time: " + (Time.realtimeSinceStartup - timer));

        }

        // You can change the arguments of the following method....
        public void FloodFill(NodeRecord original)
        {
            NodeRecord currentNode = original;
            currentNode.startingEdge = StartingEdge.Source;
            currentNode.gCost = 0;
            currentNode.CalculateFCost();
            this.Closed.AddToClosed(currentNode);

            while (true)
            {
                var neighbours = GetNeighbourList(currentNode);
                foreach (var neighbourNode in neighbours)
                {
                    if (neighbourNode.isWalkable)
                    {
                        Fill(currentNode, this.NodeRecordArray.GetNodeRecord(neighbourNode));
                    }
                }

                if (this.Open.CountOpen() > 0)
                {
                    currentNode = Open.GetBestAndRemove();
                    this.Open.RemoveFromOpen(currentNode);
                    this.Closed.AddToClosed(currentNode);
                }
                else
                {
                    break;
                }
            }

            foreach (NodeRecord n in this.Closed.All())
            {
                GetNode(n.x, n.y).status = NodeStatus.Closed;
            }

            //At the end it is important to "clean" the Open and Closed Set
            this.Open.Initialize();
            this.Closed.Initialize();
        }

        public void Fill(NodeRecord node, NodeRecord neighbour)
        {
            if (neighbour.startingEdge == StartingEdge.None)
            {
                neighbour.startingEdge = node.startingEdge;
            }

            float g = node.gCost + CalculateDistanceCost(node, neighbour);
            if (neighbour.status == NodeStatus.Unvisited)
            {
                neighbour.parent = node;
                neighbour.gCost = g;
                neighbour.CalculateFCost();
                this.Open.AddToOpen(neighbour);
            }
            else if (neighbour.status == NodeStatus.Open && g < neighbour.fCost)
            {
                neighbour.startingEdge = node.startingEdge;
                neighbour.parent = node;
                neighbour.gCost = g;
                neighbour.CalculateFCost();
                this.Open.Replace(this.Open.SearchInOpen(neighbour), neighbour);
                //TODO: maybe missing replace, not sure
            }
            else if (neighbour.status == NodeStatus.Closed && g < neighbour.fCost)
            {
                neighbour.startingEdge = node.startingEdge;
                neighbour.parent = node;
                neighbour.gCost = g;
                neighbour.CalculateFCost();
                NodeRecordArray.AddToOpen(neighbour); //TODO: check
            }
        }

        public void ComputeBoundingBox(NodeRecord source)
        {
            foreach (NodeRecord r1 in grid.getAll())
            {
                var r = this.NodeRecordArray.GetNodeRecord(r1);
                if (r.startingEdge == StartingEdge.None || r.startingEdge == StartingEdge.Source)
                {
                    continue;
                }
                var box = goalBounds[new Vector2(source.x, source.y)][r.startingEdge];

                if (r.x < box.x)
                {
                    box.x = r.x;
                }

                if (r.x > box.y)
                {
                    box.y = r.x;
                }

                if (r.y < box.z)
                {
                    box.z = r.y;
                }

                if (r.y > box.w)
                {
                    box.w = r.y;
                }

                goalBounds[new Vector2(source.x, source.y)][r.startingEdge] = box;
            }
        }

        protected override void ProcessChildNode(NodeRecord parentNode, NodeRecord neighbourNode)
        {
            if (!IsValidNode(StartNode, GoalNode, neighbourNode)) return; //ignores nodes outside scope of path

            base.ProcessChildNode(parentNode, neighbourNode);
        }

        // Checks is if node(x,Y) is in the node(startx, starty) bounding box for the direction: direction
        public bool InsindeGoalBoundBox(int startX, int startY, int x, int y, StartingEdge direction)
        {
            if (!this.goalBounds.ContainsKey(new Vector2(startX, startY)))
                return false;

            if (!this.goalBounds[new Vector2(startX, startY)].ContainsKey(direction))
                return false;

            var box = this.goalBounds[new Vector2(startX, startY)][direction];

            if (box.x >= -1 && box.y >= -1 && box.z >= -1 && box.w >= -1)
                if (x >= box.x && x <= box.y && y >= box.z && y <= box.w)
                    return true;

            return false;
        }

        //Checks if node is in same bounding boxe(s) as goal
        public bool IsValidNode(NodeRecord start, NodeRecord goal, NodeRecord node)
        {
            List<StartingEdge> directions = new List<StartingEdge>();
            directions.Add(StartingEdge.Top);
            directions.Add(StartingEdge.Bottom);
            directions.Add(StartingEdge.Left);
            directions.Add(StartingEdge.Right);
            directions.Add(StartingEdge.TopLeft);
            directions.Add(StartingEdge.TopRight);
            directions.Add(StartingEdge.BottomLeft);
            directions.Add(StartingEdge.BottomRight);

            foreach (var direction in directions)
            {
                if (InsindeGoalBoundBox(start.x, start.y, goal.x, goal.y, direction) && InsindeGoalBoundBox(start.x, start.y, node.x, node.y, direction))
                {
                    return true;
                }
            }

            return false;

            /*var direction = GetBestBoundingBox(start, goal);
            return InsindeGoalBoundBox(start.x, start.y, node.x, node.y, direction);*/
        }

        public StartingEdge GetBestBoundingBox(NodeRecord start, NodeRecord goal)
        {
            //otimize this thing eventually
            //maybe return list of directions ordered by size of bounding box and search each box until goal
            List<StartingEdge> directions = new List<StartingEdge>();
            directions.Add(StartingEdge.Top);
            directions.Add(StartingEdge.Bottom);
            directions.Add(StartingEdge.Left);
            directions.Add(StartingEdge.Right);
            directions.Add(StartingEdge.TopLeft);
            directions.Add(StartingEdge.TopRight);
            directions.Add(StartingEdge.BottomLeft);
            directions.Add(StartingEdge.BottomRight);

            List<StartingEdge> directionsList = new List<StartingEdge>();

            int index = 0;
            float size = float.MaxValue;
            Vector2 startingPos = new Vector2(start.x, start.y);

            foreach (var direction in directions)
            {
                if (InsindeGoalBoundBox(start.x, start.y, goal.x, goal.y, direction))
                    directionsList.Add(direction);
            }

            for (int i = 0; i < directionsList.Count; i++)
            {
                var box = goalBounds[startingPos][directionsList.ElementAt(i)];
                var hedge1 = new Vector2(box.x, box.y).magnitude;
                var hedge2 = new Vector2(box.z, box.w).magnitude;

                var newSize = hedge1 * hedge2; //compute area of box;

                if (newSize < size)
                {
                    index = i;
                    size = newSize;
                }
            }

            return directionsList.ElementAt(index);
        }


    }
}
