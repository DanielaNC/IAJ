using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class ShieldOfFaith : Action
    {
        public AutonomousCharacter Character { get; private set; }

        public ShieldOfFaith(AutonomousCharacter character) : base("CastShieldOfFaith")
        {
            this.Character = character;
        }

        public override bool CanExecute()
        {
            //if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Mana >= 5 && this.Character.GameManager.characterData.ShieldHP < 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //if (!base.CanExecute(worldModel)) return false;
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var shield = (int)worldModel.GetProperty(Properties.ShieldHP);
            return mana >= 5 && shield < 5;
        }

        public override void Execute()
        {
            //base.Execute();
            this.Character.GameManager.CastShieldOfFaith();
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = 0.0f;

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= this.Character.GameManager.characterData.ShieldHP;

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
            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue - 5);

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
