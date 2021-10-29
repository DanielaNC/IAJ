using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    class Rest : Action
    {

        public AutonomousCharacter Character { get; private set; }

        public Rest(AutonomousCharacter character) : base("Rest")
        {
            this.Character = character;
        }

        public override bool CanExecute()
        {
            var hp = this.Character.GameManager.characterData.HP;
            var maxHP = this.Character.GameManager.characterData.MaxHP;

            return hp > 0 && hp < maxHP;
        }


        public override bool CanExecute(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxHP = (int)worldModel.GetProperty(Properties.MAXHP);
            return hp > 0 && hp < maxHP;
        }

        public override void Execute()
        {
            this.Character.GameManager.Rest();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var time = (float)worldModel.GetProperty(Properties.TIME);
            worldModel.SetProperty(Properties.TIME, time + 5);

            int hp = (int)worldModel.GetProperty(Properties.HP);
            float goalValue = (float)worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            int maxHp = (int)worldModel.GetProperty(Properties.MAXHP);

            if (maxHp - hp == 1)
            {
                worldModel.SetProperty(Properties.HP, maxHp);
                worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue - 1);
            }
            else
            {
                worldModel.SetProperty(Properties.HP, hp + 2);
                worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue - 2);
            }
            goalValue = (float)worldModel.GetGoalValue(AutonomousCharacter.BE_QUICK_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, goalValue + 5);
        }

        public override float GetGoalChange(Goal goal)
        {
            float change = 0.0f;

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL) change -= this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP;

            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //TODO: implement
            return 0f;
        }
    }
}
