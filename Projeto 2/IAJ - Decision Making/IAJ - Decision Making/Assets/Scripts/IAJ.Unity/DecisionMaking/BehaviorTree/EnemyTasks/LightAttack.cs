using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks
{
    class LightAttack : Task
    {
        protected NPC character { get; set; }

        public GameObject target { get; set; }

        public LightAttack(NPC character)
        {
            this.character = character;
        }
        public override Result Run()
        {
            character.AttackPlayer();
            return Result.Success;
        }

    }
}
