using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks
{
    class IsPlayerNear : Task
    {
        protected NPC character { get; set; }

        public float range;

        public GameObject target { get; set; }

        public IsPlayerNear(NPC character, GameObject target, float _range)
        {
            this.character = character;
            this.target = target;
            range = _range;
        }

        public override Result Run()
        {
            if (Vector3.Distance(character.transform.position, this.target.transform.position) <= range)
            {
                return Result.Success;
            }
            else return Result.Failure;

        }

    }
}
