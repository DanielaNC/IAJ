using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using Assets.Scripts.Grid;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.IO;
using System;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using UnityEngine.Networking;

public class PathfindingManager : MonoBehaviour
{

    //Struct for default positions
    [Serializable]
    public struct defaultPos
    {
        public int index;
        public Vector2 startingPos;
        public Vector2 goalPos;
    }

    public enum PathfindingAlgorithm
    {
        AStar,
        NodeArrayAStar,
        GoalBounding
    }

    // "Default Positions are quite useful for testing"
    public List<defaultPos> defaultPositions;

    [Header("Grid Settings")]
    [Tooltip("Change grid name to change grid properties")]
    public string gridName;

    [Header("Pahfinding Settings")]
    //public properties useful for testing, you can add other booleans here such as which heuristic to use
    public bool partialPath;
    public bool debugMode;
    public bool useGoalBound;
    public PathfindingAlgorithm algorithm = PathfindingAlgorithm.AStar;
    //TODO: enum to change open and closed set settings

    //Grid configuration
    public static int width;
    public static int height;
    public static float cellSize;

    //Essential Pathfind classes 
    public AStarPathfinding pathfinding { get; set; }

    //The Visual Grid
    private VisualGridManager visualGrid;
    private string[,] textLines;

    //Private fields for internal use only
    public static int startingX = -1;
    public static int startingY = -1;
    public static int goalX = -1;
    public static int goalY = -1;

    public NodeRecord startingNode;

    //Path
    List<NodeRecord> solution;
    private bool isStartingPoint = false;

    private void Start()
    {
        // Finding reference of Visual Grid Manager
        visualGrid = GameObject.FindObjectOfType<VisualGridManager>();

        // Creating the Path for the Grid and Creating it
        var gridPath = "Assets/Resources/Grid/" + gridName + ".txt";
        this.LoadGrid(gridPath);

        // Creating and Initializing the Pathfinding class, you can change the open, closed and heuristic sets here
        AStarPathfinding pathFindingAlgorithm = null;

        if (algorithm == PathfindingAlgorithm.AStar)
            pathFindingAlgorithm = new AStarPathfinding(new SimpleUnorderedNodeList(), new SimpleUnorderedNodeList(), new EuclideanDistance());

        else if (algorithm == PathfindingAlgorithm.NodeArrayAStar)
        {
            pathFindingAlgorithm = new NodeArrayAStarPathfinding(new SimpleUnorderedNodeList(), new SimpleUnorderedNodeList(), new EuclideanDistance());
        }

        else if (algorithm == PathfindingAlgorithm.GoalBounding)
        {
            pathFindingAlgorithm = new GoalBoundAStarPathfinding(new SimpleUnorderedNodeList(), new SimpleUnorderedNodeList(), new EuclideanDistance());
        }

        this.pathfinding = pathFindingAlgorithm;

        visualGrid.GridMapVisual(textLines, this.pathfinding.grid);

        if (this.pathfinding is GoalBoundAStarPathfinding)
        {
            var p = this.pathfinding as GoalBoundAStarPathfinding;
            p.MapPreprocess();
            visualGrid.ClearGrid();
        }

        //pathfinding.grid.OnGridValueChanged += visualGrid.Grid_OnGridValueChange;

    }

    // Update is called once per frame
    void Update()
    {

        // The first mouse click goes here, it defines the starting position;
        if (Input.GetMouseButtonDown(0))
        {

            //Retrieving clicked position
            var clickedPosition = UtilsClass.GetMouseWorldPosition();

            int positionX, positionY = 0;

            // Retrieving the grid's corresponding X and Y from the clicked position
            pathfinding.grid.GetXY(clickedPosition, out positionX, out positionY);

            // Getting the corresponding Node 
            var node = pathfinding.grid.GetGridObject(positionX, positionY);

            if (node != null && node.isWalkable)
            {

                if (startingX == -1)
                {
                    startingX = positionX;
                    startingY = positionY;

                    this.visualGrid.SetObjectColor(startingX, startingY, Color.cyan);
                    this.startingNode = node;
                    isStartingPoint = true;

                }
                else if (goalX == -1)
                {
                    goalX = positionX;
                    goalY = positionY;

                    this.visualGrid.SetObjectColor(startingX, startingY, Color.cyan);

                    //We can now start the search
                    isStartingPoint = false;
                    InitializeSearch(startingX, startingY, goalX, goalY);
                }

                else
                {
                    goalY = -1;
                    goalX = -1;
                    this.visualGrid.ClearGrid();
                    startingX = positionX;
                    startingY = positionY;
                    isStartingPoint = true;
                    //this.visualGrid.SetObjectColor(startingX, startingY, Color.cyan);
                }


            }

        }


        // We will use the right mouse to clean the selection and the grid
        if (Input.GetMouseButtonDown(1))
        {
            startingX = -1;
            startingY = -1;
            goalY = -1;
            goalX = -1;
            this.visualGrid.ClearGrid();
            startingNode = null;
        }

        if (algorithm == PathfindingAlgorithm.GoalBounding && useGoalBound && isStartingPoint && startingNode != null)
            this.visualGrid.fillBoundingBox(startingNode);

        // Input Handler: deals with most keyboard inputs
        InputHandler();

        // Make sure you tell the pathfinding algorithm to keep searching
        if (this.pathfinding.InProgress)
        {
            var finished = this.pathfinding.Search(out this.solution, partialPath);

            if(debugMode)
                visualGrid.UpdateGrid();

            useGoalBound = false;

            if (finished)
            {
                this.pathfinding.InProgress = false;
                this.visualGrid.DrawPath(this.solution);
            }

            this.pathfinding.TotalProcessingTime += Time.deltaTime;
        }

    }


    void InputHandler()
    {

        // Space clears the grid
        if (Input.GetKeyDown(KeyCode.Space))
            this.visualGrid.ClearGrid();


        // If you press 1-5 keys you pathfinding will use default positions
        int index = 0;
        if (Input.GetKeyDown(KeyCode.Keypad1))
            index = 1;
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            index = 2;
        else if (Input.GetKeyDown(KeyCode.Keypad3))
            index = 3;
        else if (Input.GetKeyDown(KeyCode.Keypad4))
            index = 4;
        else if (Input.GetKeyDown(KeyCode.Keypad5))
            index = 5;
        if (index != 0)
        {
            var def = defaultPositions.Find(x => x.index == index);
            startingX = (int)def.startingPos.x;
            startingY = (int)def.startingPos.y;
            goalX = (int)def.goalPos.x;
            goalY = (int)def.goalPos.y;
            InitializeSearch((int)def.startingPos.x, (int)def.startingPos.y, (int)def.goalPos.x, (int)def.goalPos.y);
        }

    }


    public void InitializeSearch(int _startingX, int _startingY, int _goalX, int _goalY)
    {
        this.visualGrid.SetObjectColor(startingX, startingY, Color.cyan);
        this.visualGrid.SetObjectColor(goalX, goalY, Color.green);

        this.pathfinding.InitializePathfindingSearch(_startingX, _startingY, _goalX, _goalY);

    }

    // Reads the text file that where the grid "definition" is stored, I don't reccomend changing this
    public void LoadGrid(string gridPath)
    {

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(gridPath);
        var fileContent = reader.ReadToEnd();
        reader.Close();
        var lines = fileContent.Split("\n"[0]);

        //Calculating Height and Width from text file
        height = lines.Length;
        width = lines[0].Length - 1;

        // CellSize Formula 
        cellSize = 650.0f / (width + 2);

        textLines = new string[height, width];
        int i = 0;
        foreach (var l in lines)
        {
            var words = l.Split();
            var j = 0;

            var w = words[0];

            foreach (var letter in w)
            {
                textLines[i, j] = letter.ToString();
                j++;

                if (j == textLines.GetLength(1))
                    break;
            }

            i++;
            if (i == textLines.GetLength(0))
                break;
        }

    }

}
