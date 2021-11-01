using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP > 0 && this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            return hp > 0 && hp < maxHP;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP;

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var maxHP = worldModel.GetProperty(Properties.MAXHP);
            worldModel.SetProperty(Properties.HP, maxHP);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, 0);

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHp = (int)worldModel.GetProperty(Properties.MAXHP);

            if (hp <= maxHp / 3 && !this.Character.GameManager.SleepingNPCs)
                return -300 + base.GetHValue(worldModel) - (maxHp-hp); // choose the closest one

            return base.GetHValue(worldModel) / (1 / hp);
        }
    }
}
