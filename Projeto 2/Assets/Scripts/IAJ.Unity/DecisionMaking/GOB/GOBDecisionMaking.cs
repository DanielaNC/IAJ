using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class GOBDecisionMaking
    {
        public bool InProgress { get; set; }
        private List<Goal> goals { get; set; }
        private List<Action> actions { get; set; }
        private float bestDiscontentment = float.MaxValue;
        private int nrActions = 0;

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
            var topGoal = goals[0];
            var start = Time.time;

            foreach (var goal in goals)
            {
                if (goal.InsistenceValue > topGoal.InsistenceValue)
                {
                    topGoal = goal;
                }

                if(goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL && goal.InsistenceValue > 0.5 && GameObject.FindObjectOfType<GameManager>().characterData.HP > GameObject.FindObjectOfType<GameManager>().characterData.MaxHP/2)
                {
                    topGoal = goal;
                    break;
                }
            }

            //Debug.Log(topGoal.Name);
            bestAction = actions.Find(x => x.CanExecute());
            var bestUtility = -bestAction.GetGoalChange(topGoal);
            foreach (var action in actions)
            {
                if (!action.CanExecute())
                {
                    continue;
                }

                var utility = -action.GetGoalChange(topGoal);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestAction = action;
                }
            }

            var discontentment = CalculateDiscontentment(bestAction, this.goals);
            if(discontentment < bestDiscontentment)
            {
                bestDiscontentment = discontentment;
                Debug.Log("Disc: " + bestDiscontentment);
            }

            InProgress = false;
            nrActions++;
            Debug.Log("Actions: " + nrActions);
            Debug.Log("Processing time: " + (Time.time - start));
            return bestAction;
        }
    }
}
