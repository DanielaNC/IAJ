using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class GOBDecisionMaking
    {
        public bool InProgress { get; set; }
        private List<Goal> goals { get; set; }
        private List<Action> actions { get; set; }

        // Utility based GOB
        public GOBDecisionMaking(List<Action> _actions, List<Goal> goals)
        {
            this.actions = _actions;
            this.goals = goals;
        }


        public static float CalculateDiscontentment(Action action, List<Goal> goals)
        {
            var discontentment = 0.0f;
            var duration = action.GetDuration();

            foreach (var goal in goals)
            {
                var newValue = goal.InsistenceValue + action.GetGoalChange(goal) + duration * goal.ChangeRate;
               
                //Discontentment varies between 0-10
                if (newValue > 10.0f)
                {
                    newValue = 10.0f;
                }
                else if (newValue < 0.0f)
                {
                    newValue = 0.0f;
                }
                discontentment += goal.GetDiscontentment(newValue);
            }

            return discontentment;
        }

        public Action ChooseAction()
        {
            InProgress = true;
            Action bestAction = null;
            var bestValue = float.PositiveInfinity;

            //TODO implement

            InProgress = false;
            return bestAction;
        }
    }
}
