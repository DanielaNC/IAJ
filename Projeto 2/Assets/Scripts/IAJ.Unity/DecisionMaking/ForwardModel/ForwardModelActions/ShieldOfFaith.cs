using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : WalkToTargetAndExecuteAction
    {
        public ShieldOfFaith(AutonomousCharacter character) : base("CastShieldOfFaith", character, null)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana >= 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana >= 5;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.CastShieldOfFaith();
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= this.Character.GameManager.characterData.HP - this.Character.GameManager.characterData.ShieldHP;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var shield = worldModel.GetProperty(Properties.ShieldHP);
            worldModel.SetProperty(Properties.ShieldHP, shield);
            int mana = (int)worldModel.GetProperty(Properties.MANA);
            mana -= 5;
            worldModel.SetProperty(Properties.MANA, mana);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 0);

            //disables the target object so that it can't be reused again
            //worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //TODO implement
            return 0.0f;
        }
    }
}
