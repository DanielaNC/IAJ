using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        private int enemySimpleDamage;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite", character, target)
        {
            if (target.tag.Equals("Skeleton"))
            {
                this.dmgRoll = () => RandomHelper.RollD6();
                this.enemySimpleDamage = 3;
                this.expectedHPChange = 3.5f;
                this.xpChange = 3;
                this.expectedXPChange = 2.7f;
                this.enemyAC = 10;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            
            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -this.expectedXPChange;
            }

            return change;
        }

        public override void Execute()
        {
            Vector3 delta = this.Target.transform.position - this.Character.transform.position;

            if (!(delta.sqrMagnitude < 15 && delta.sqrMagnitude > 7))
                this.Character.StartPathfinding(this.Target.transform.position);

            this.Character.GameManager.DivineSmite(this.Target);
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return (this.Character.GameManager.characterData.Mana >= 2 && this.Target != null && this.Target.active);
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute()) return false;
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana >= 2;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            int hp = (int)worldModel.GetProperty(Properties.HP);
            int shieldHp = (int)worldModel.GetProperty(Properties.ShieldHP);
            int xp = (int)worldModel.GetProperty(Properties.XP);

            int damage = 0;
            if (this.Character.GameManager.StochasticWorld)
            {
                //execute the lambda function to calculate received damage based on the creature type
                damage = this.dmgRoll.Invoke();
            }
            else
            {
                damage = this.enemySimpleDamage;
            }
            //calculate player's damage
            int remainingDamage = damage - shieldHp;
            int remainingShield = Mathf.Max(0, shieldHp - damage);
            int remainingHP;

            if (remainingDamage > 0)
            {
                remainingHP = (hp - remainingDamage);
                worldModel.SetProperty(Properties.HP, remainingHP);
            }

            worldModel.SetProperty(Properties.ShieldHP, remainingShield);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);


            //calculate Hit
            //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
            int attackRoll = RandomHelper.RollD20() + 7;

            if (attackRoll >= enemyAC || !this.Character.GameManager.StochasticWorld)
            {
                //there was an hit, enemy is destroyed, gain xp
                //disables the target object so that it can't be reused again
                worldModel.SetProperty(this.Target.name, false);
                worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            }
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var shield = (int)worldModel.GetProperty(Properties.ShieldHP);

            if (hp + shield > this.expectedHPChange && !this.Character.GameManager.SleepingNPCs)
            {
                return base.GetHValue(worldModel) / 2.5f; // prefer it over swordAttack(skelleton);
            }
            return 200.0f;
        }
    }
}
