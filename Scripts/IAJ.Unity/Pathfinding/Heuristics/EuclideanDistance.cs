using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using UnityEngine;


namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclideanDistance : IHeuristic
    {
        public float H(NodeRecord node, NodeRecord goalNode)
        {
            return Mathf.Sqrt(Mathf.Pow(goalNode.x - node.x, 2f) + Mathf.Pow(goalNode.y - node.y, 2f));
        }
    }
}