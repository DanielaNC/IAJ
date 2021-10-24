using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class Reward
    {
        // TODO

        public float Value { get; set; }
        public int PlayerID { get; set; }

        public Reward() { }

        public Reward(WorldModel state, int playerID)
        {
            this.PlayerID = playerID;
            this.Value = state.GetScore();
        }

        public Reward(float value, int playerID)
        {
            this.Value = value;
            this.PlayerID = playerID;
        }
    }
}
