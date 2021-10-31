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
            return this.Character.GameManager.characterData.Mana >= 5 && this.Character.GameManager.characterData.ShieldHP < 5;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var shield = (int)worldModel.GetProperty(Properties.ShieldHP);
            return mana >= 5 && shield < 5;
        }

        public override void Execute()
        {
            this.Character.GameManager.CastShieldOfFaith();
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = 0.0f;

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= 5 - this.Character.GameManager.characterData.ShieldHP;

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
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue - (5 - this.Character.GameManager.characterData.ShieldHP));
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHp = (int)worldModel.GetProperty(Properties.MAXHP);
            var shield = (int)worldModel.GetProperty(Properties.ShieldHP);

            if (hp <= maxHp / 2 && !this.Character.GameManager.SleepingNPCs)
                return -200 + base.GetHValue(worldModel) - (5 - shield);

            return base.GetHValue(worldModel) / (1 / (hp + shield));
        }
    }
}
