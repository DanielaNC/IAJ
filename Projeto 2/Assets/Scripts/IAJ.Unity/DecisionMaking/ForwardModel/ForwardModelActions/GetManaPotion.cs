using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        public GetManaPotion(AutonomousCharacter character, GameObject target) : base("GetManaPotion", character, target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana < 10;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            int mana = (int)worldModel.GetProperty(Properties.MANA);

            return (mana < 10);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetManaPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, 10);
            var goalChange = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, goalChange + this.GetDuration());  //----> Implement mana goal

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //TODO implement
            return 0.0f;
        }
    }
}
