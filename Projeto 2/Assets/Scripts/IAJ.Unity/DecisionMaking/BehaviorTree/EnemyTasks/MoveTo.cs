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
    class MoveTo : Task
    {
        protected NPC character { get; set; }

        public GameObject target { get; set; }

        public float range;

        public MoveTo(NPC character, GameObject target, float _range)
        {
            this.character = character;
            this.target = target;
            range = _range;
        }

        public override Result Run()
        {
            if (target == null)
                return Result.Failure;

            if (Vector3.Distance(character.transform.position, this.target.transform.position) <= range)
            {
                return Result.Success;
            }
            else
            {
                character.MoveTo(target.transform.position);
                return Result.Running;
            }

        }

    }
}
