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

        public Pursue(NPC character, GameObject target, float _range)
        {
            this.character = character;
            this.target = target;
            range = _range;
        }

        public override Result Run()
        {
            // sees the player
            if (Vector3.Distance(character.transform.position, this.target.transform.position) <= range)
            {
                character.Shout();
                character.PursuePlayer();
                return Result.Success;
            }

            else
            {
                if (character.isShouting)
                {
                    character.isShouting = false;
                    GameObject.FindObjectOfType<GameManager>().orcShout = false;
                    character.ShoutText.text = "";
                }

                return Result.Failure;
            }

        }

    }
}
