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
        public Dictionary<Vector2,Dictionary<string, Vector4>> goalBounds;

        public GoalBoundAStarPathfinding(IOpenSet open, IClosedSet closed, IHeuristic heuristic) : base(open, closed, heuristic)
        {
            goalBounds = new Dictionary<Vector2, Dictionary<string, Vector4>>();

        }

        public void MapPreprocess()
        {
           
            for (int i = 0; i < grid.getHeight(); i++)
            {
                for (int j = 0; j < grid.getWidth(); j++)
                {
                    grid.GetGridObject(i + 1, j).startingEdge = StartingEdge.Top;
                    grid.GetGridObject(i, j + 1).startingEdge = StartingEdge.Left;
                    grid.GetGridObject(i - 1, j).startingEdge = StartingEdge.Bottom;
                    grid.GetGridObject(i, j - 1).startingEdge = StartingEdge.Right;

                    FloodFill(grid.GetGridObject(i, j));

                   // Floodfill the grid for each direction..

                   // Calculate the bounding box and repeat
                }
            }
            
        }

        // You can change the arguments of the following method....
        public void FloodFill(NodeRecord original)
        {
            NodeRecord currentNode = original;

            while (true)
            {
                var neighbours = GetNeighbourList(currentNode);
                foreach (var neighbourNode in neighbours)
                {
                    if (neighbourNode.isWalkable)
                    {
                        /*
                        TODO
                        */
                    }
                }

                if (this.Open.CountOpen() > 0)
                {
                    currentNode = Open.GetBestAndRemove();
                }
                else
                {
                    break;
                }
            }

            //At the end it is important to "clean" the Open and Closed Set
            this.Open.Initialize();
            this.Closed.Initialize();
            this.NodeRecordArray.ClearFill();
        }

    

        // Checks is if node(x,Y) is in the node(startx, starty) bounding box for the direction: direction
        public bool InsindeGoalBoundBox(int startX, int startY, int x, int y, string direction)
        {
            if (!this.goalBounds.ContainsKey(new Vector2(startX, startY)))
                return false;

            if (!this.goalBounds[new Vector2(startX, startY)].ContainsKey(direction))
                return false;

            var box = this.goalBounds[new Vector2(startX, startY)][direction];
            
            if(box.x >= -1 && box.y >= -1 && box.z >= -1 && box.w >= -1)
                if (x >= box.x && x <= box.y && y >= box.z && y <= box.w)
                    return true;

            return false;
        }
    }
}
