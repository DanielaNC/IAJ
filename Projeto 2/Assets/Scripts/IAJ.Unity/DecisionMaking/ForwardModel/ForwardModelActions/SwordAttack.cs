using Assets.Scripts.Manager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions
{
    public class SwordAttack : WalkToTargetAndExecuteAction
    {
        private float expectedHPChange;
        private float expectedXPChange;
        private int xpChange;
        private int enemyAC;
        private int enemySimpleDamage;
        //how do you like lambda's in c#?
        private Func<int> dmgRoll;

        public SwordAttack(AutonomousCharacter character, GameObject target) : base("SwordAttack",character,target)
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
            else if (target.tag.Equals("Orc"))
            {
                this.dmgRoll = () => RandomHelper.RollD10() + 2;
                this.enemySimpleDamage = 8;
                this.expectedHPChange = 7.5f;
                this.xpChange = 10;
                this.expectedXPChange = 7.0f;
                this.enemyAC = 14;
            }
            else if (target.tag.Equals("Dragon"))
            {
                this.dmgRoll = () => RandomHelper.RollD12() + RandomHelper.RollD12();
                this.enemySimpleDamage = 15;
                this.expectedHPChange = 13.0f;
                this.xpChange = 20;
                this.expectedXPChange = 10.0f;
                this.enemyAC = 18;
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += this.expectedHPChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                if(this.Character.GameManager.characterData.XP >= this.Character.GameManager.characterData.Level * 10)
                    change = 0;
            }
            
            return change;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.SwordAttack(this.Target);
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Target.active && !this.Character.GameManager.SleepingNPCs && this.Character.GameManager.characterData.HP + this.Character.GameManager.characterData.ShieldHP > this.Target.GetComponent<NPC>().simpleDamage;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute()) return false;
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var shield = (int)worldModel.GetProperty(Properties.ShieldHP);
            var dmg = this.Target.GetComponent<NPC>().simpleDamage;
            return hp + shield > dmg && !this.Character.GameManager.SleepingNPCs;
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

            if(remainingDamage > 0)
            {
                remainingHP = (hp - remainingDamage);
                worldModel.SetProperty(Properties.HP, remainingHP);
            }

            worldModel.SetProperty(Properties.ShieldHP, remainingShield);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            hp = (int)worldModel.GetProperty(Properties.HP);
            if (hp <= 0)
            {
                worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, float.MaxValue - surviveValue);
            }
            else
            {
                worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue + remainingDamage);
            }


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
                return base.GetHValue(worldModel) + 5.0f;
            }
            return 400.0f;
        }
    }
}
