using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class Reward
    {
        // TODO

        public float Value { get; set; }
        public int PlayerID { get; set; }

        public Reward() { }

        public Reward(WorldModel state, int playerID, bool useHeuristic)
        {
            this.PlayerID = playerID;
            if (useHeuristic)
            {
                this.Value = state.GetScore();
            }
            else
            {
                this.Value = state.StateQuality();
            }
        }

        public Reward(float value, int playerID)
        {
            this.Value = value;
            this.PlayerID = playerID;
        }
    }
}
