using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    class Teleport : Action
    {
        public AutonomousCharacter Character { get; private set; }
        public Vector3 initialPosition { get; private set; }

        public Teleport(AutonomousCharacter character, Vector3 initialPosition) : base("Rest")
        {
            this.Character = character;
            this.initialPosition = initialPosition;
        }

        public override bool CanExecute()
        {
            var hp = this.Character.GameManager.characterData.HP;
            var mana = this.Character.GameManager.characterData.Mana;

            return hp > 10 && mana >= 5;
        }


        public override bool CanExecute(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var mana = (int)worldModel.GetProperty(Properties.MANA);

            return hp > 0 && mana >= 5;
        }

        public override void Execute()
        {
            this.Character.GameManager.Teleport();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            worldModel.SetProperty(Properties.POSITION, initialPosition);
            worldModel.SetGoalValue(AutonomousCharacter.BE_QUICK_GOAL, 2); //TO DO: not sure, but yeah, after teleport the player probably doesn't need to be quick
        }

        public override float GetGoalChange(Goal goal)
        {
            float change = 0.0f;

            //TODO: implement

            //if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            //{
            //    change = -2;
            //}

            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //TODO: implement
            return 0f;
        }
    }
}
