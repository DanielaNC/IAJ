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
            if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL) // account for possibility of teleport
            {
                change = -0.2f;
            }

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) // account for possibility of shield of faith
            {
                if (this.Character.GameManager.characterData.ShieldHP == 0 && this.Character.GameManager.characterData.Mana <= 5)
                    return -5.0f;

                else if (this.Character.GameManager.characterData.Mana >= 5) { 
                        return 0.0f;
                }
                    
            }

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL) // account for possibility of divine smite
            {
                if (this.Character.GameManager.characterData.Mana < 2)
                    return -2.0f;

                else if (this.Character.GameManager.characterData.Mana >= 2)
                {
                    return 0.0f;
                }

            }

            return change;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + 10);
            var goalChange = worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, goalChange - 0.2f);  //----> Implement mana goal

            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var mana = (int)worldModel.GetProperty(Properties.MANA);

            if (mana < 2 && !this.Character.GameManager.SleepingNPCs)
                return base.GetHValue(worldModel); // choose the closest one

            return base.GetHValue(worldModel);
        }
    }
}
