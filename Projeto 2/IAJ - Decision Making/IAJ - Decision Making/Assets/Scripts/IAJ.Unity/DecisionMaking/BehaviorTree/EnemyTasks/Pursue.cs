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
    class Pursue : Task
    {
        protected NPC character { get; set; }

        public float range;

        public GameObject target { get; set; }

        public bool hasScreamed { get; set; }

        public bool isPursuing { get; set; } = false;

        public Pursue(NPC character, GameObject target, float _range)
        {
            this.character = character;
            this.target = target;
            range = _range;
            this.hasScreamed = character.hasScreamed;
        }

        public override Result Run()
        {
            // sees the player
            if (Vector3.Distance(character.transform.position, this.target.transform.position) <= range)
            {
                character.hasScreamed = true;
                GameObject.FindObjectOfType<GameManager>().orcScream = true;
                GameObject.FindObjectOfType<GameManager>().orcScreamPosition = character.transform.position;
                character.PursuePlayer();
                isPursuing = true;
                return Result.Success;
            }

            else
            {
                if (hasScreamed)
                {
                    character.hasScreamed = false;
                    GameObject.FindObjectOfType<GameManager>().orcScream = false;
                }

                return Result.Failure;
            }

        }

    }
}
