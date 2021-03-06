using Assets.Scripts.IAJ.Unity.Pathfinding;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    //Pathfinding Manager reference
    public PathfindingManager manager;

    //Debug Components you can add your own here
    Text debugCoordinates;
    Text debugG;
    Text debugF;
    Text debugH;
    Text debugWalkable;
    Text debugtotalProcessedNodes;
    Text debugtotalProcessingTime;
    Text debugMaxNodes;
    Text debugJPU;
    Text debugJPD;
    Text debugJPL;
    Text debugJPR;
    Text debugDArray;
    Text debugBounds;

    bool useGoal;
    bool useJPS;

    private int currentX, currentY;
    VisualGridManager visualGrid;

    // Start is called before the first frame update
    void Start()
    {

        // Simple way of getting the manager's reference
        manager = GameObject.FindObjectOfType<PathfindingManager>();
        visualGrid = GameObject.FindObjectOfType<VisualGridManager>();

        // Retrieving the Debug Components
        var debugTexts = this.transform.GetComponentsInChildren<Text>();
        debugCoordinates = debugTexts[0];
        debugH = debugTexts[1];
        debugG = debugTexts[2];
        debugF = debugTexts[3];
        debugtotalProcessedNodes = debugTexts[4];
        debugtotalProcessingTime = debugTexts[5];
        debugMaxNodes = debugTexts[6];
        debugWalkable = debugTexts[7];
        debugDArray = debugTexts[8];
        useGoal = manager.useGoalBound;
        currentX = -1;
        currentY = -1;
    }

    // Update is called once per frame
    void Update()
    {
        // A Long way of printing useful information regarding the algorithm
        var currentPosition = UtilsClass.GetMouseWorldPosition();
        if (currentPosition != null)
        {
            int x, y;
            if (manager.pathfinding.grid != null)
            {
                manager.pathfinding.grid.GetXY(currentPosition, out x, out y);
                if (x == currentX && currentY == y || currentX == PathfindingManager.startingX && currentY == PathfindingManager.startingY
                    || currentX == PathfindingManager.goalX && currentY == PathfindingManager.goalY)
                    return;

                currentX = x;
                currentY = y;
                if (x != -1 && y != -1)
                {
                    var node = manager.pathfinding.grid.GetGridObject(x, y);
                    if (node != null)
                    {
                        debugCoordinates.text = " x:" + x + "; y:" + y;
                        debugG.text = "G:" + node.gCost;
                        debugF.text = "F:" + node.fCost;
                        debugH.text = "H:" + node.hCost;
                        debugWalkable.text = "IsWalkable:" + node.isWalkable;

                        if (node.isWalkable)
                        {
                           if (useGoal)
                            {
                                var array = "";
                                var goalBoundingPathfinder = (GoalBoundAStarPathfinding)manager.pathfinding;
                                if (goalBoundingPathfinder.goalBounds.ContainsKey(new Vector2(x, y)))
                                {
                                    var boundingBox = goalBoundingPathfinder.goalBounds[new Vector2(x, y)];
                                    array += "Left" + boundingBox[Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.StartingEdge.Left] + "\n";    //TODO: check
                                    array += "Right" + boundingBox[Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.StartingEdge.Right] + "\n";    //TODO: check
                                    array += "Up" + boundingBox[Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.StartingEdge.Top] + "\n";    //TODO: check
                                    array += "Down" + boundingBox[Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.StartingEdge.Bottom] + "\n";    //TODO: check
                                    debugDArray.text = array;
                                    visualGrid.fillBoundingBox(node);

                                }
                            }

                        }
                    }
                }

            }
        }

        if (this.manager.pathfinding.InProgress)
        {
                debugMaxNodes.text = "MaxNodes:" + manager.pathfinding.MaxOpenNodes;
                debugtotalProcessedNodes.text = "TotalPNodes:" + manager.pathfinding.TotalProcessedNodes;
                debugtotalProcessingTime.text = "TotalPTime:" + manager.pathfinding.TotalProcessingTime;
            }
        }
}
