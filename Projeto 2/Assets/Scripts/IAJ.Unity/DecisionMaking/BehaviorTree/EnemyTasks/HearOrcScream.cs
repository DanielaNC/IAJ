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
    class HearOrcScream : Task
    {
        protected NPC character { get; set; }

        public GameObject target { get; set; }

        public float range;

        public HearOrcScream(NPC character, GameObject target, float _range)
        {
            this.character = character;
            this.target = target;
            range = _range;
        }

        public override Result Run()
        {
            
            if (target == null)
                return Result.Failure;

            if (GameObject.FindObjectOfType<GameManager>().orcShout == true && character.isShouting == false)
            {
                character.MoveTo((GameObject.FindObjectOfType<GameManager>().orcShoutPosition));
                return Result.Success;
            }

            return Result.Failure;

        }

    }
}
