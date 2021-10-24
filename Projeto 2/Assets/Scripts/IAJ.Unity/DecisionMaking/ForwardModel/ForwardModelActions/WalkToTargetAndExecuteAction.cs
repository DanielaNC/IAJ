using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AutonomousCharacter Character { get; set; }

        public GameObject Target { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, AutonomousCharacter character, GameObject target) : base(actionName + "(" + target.name + ")")
        {
            this.Character = character;
            this.Target = target;
        }

        public override float GetDuration()
        {
            return this.GetDuration(this.Character.transform.position);
        }

        public override float GetDuration(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION);
            return this.GetDuration(position);
        }

        private float GetDuration(Vector3 currentPosition)
        {
            var distance = getDistance(currentPosition, Target.transform.position);
            var result = distance / this.Character.maxSpeed;
            return result;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                return this.GetDuration();
            }
            else return 0;
        }

        public override bool CanExecute()
        {
            return this.Character.GameManager.characterData.HP > 0 && this.Target != null;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) return false;
            var targetEnabled = (bool)worldModel.GetProperty(this.Target.name);
            var hp = (int)worldModel.GetProperty(Properties.HP);
            return hp > 0 && targetEnabled;
        }

        public override void Execute()
        {
            Vector3 delta = this.Target.transform.position - this.Character.transform.position;
            
            if (delta.sqrMagnitude > 5 )
               this.Character.StartPathfinding(this.Target.transform.position);
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);

            var quicknessValue = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, quicknessValue + duration * 0.1f);

            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + duration);

            worldModel.SetProperty(Properties.POSITION, Target.transform.position);
        }

        private float getDistance(Vector3 currentPosition, Vector3 targetPosition)
        {        
            var distance = this.Character.GetDistanceToTarget(currentPosition, targetPosition);
            return distance;
        }
    }
}