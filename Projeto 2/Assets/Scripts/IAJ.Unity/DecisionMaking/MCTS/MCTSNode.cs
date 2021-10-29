using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSNode
    {
        public FutureStateWorldModel State { get; private set; }
        public MCTSNode Parent { get; set; }
        public Action Action { get; set; }
        public int PlayerID { get; set; }
        public List<MCTSNode> ChildNodes { get; private set; }
        public int N { get; set; }
        public float Q { get; set; }

        public MCTSNode(WorldModel state)
        {
            this.State = new FutureStateWorldModel(state);
            this.ChildNodes = new List<MCTSNode>();
        }
    }
}
