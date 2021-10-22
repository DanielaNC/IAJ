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
    class Patrol : Task
    {
        protected NPC character { get; set; }

        public Vector3[] target { get; set; }

        public float range { get; set; }

        protected int index = 0;

        public Patrol(NPC character, float range)
        {
            this.target = new Vector3[2];
            this.range = range;
            this.character = character;
            target[0] = character.initialPos;
            target[1] = character.patrolPoint;

        }

        public override Result Run()
        {
            if (target[index] == null)
                return Result.Failure;

            if (Vector3.Distance(character.transform.position, this.target[index]) <= range)
            {
                index = (index == 0 ? 1 : 0);
                //Debug.Log("character " + character.Name + " moving to index " + index);
                return Result.Success;
            }

            else
            {
                character.MoveTo(target[index]);
                return Result.Success;
            }

        }

    }
}
