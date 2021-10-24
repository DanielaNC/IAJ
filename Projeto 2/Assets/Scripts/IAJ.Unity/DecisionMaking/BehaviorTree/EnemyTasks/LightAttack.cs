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

        protected float range { get; set; } = -1.0f;

        public GameObject target { get; set; }

        public LightAttack(NPC character)
        {
            this.character = character;
        }

        public LightAttack(NPC character, float _range)
        {
            this.character = character;
            this.range = _range;
            this.target = GameObject.FindObjectOfType<GameManager>().autonomousCharacter.gameObject;
        }

        public override Result Run()
        {
            if (range > 0.0f)
            {
                if (Vector3.Distance(character.transform.position, this.target.transform.position) > range)
                    return Result.Failure;
            }

            character.AttackPlayer();
            return Result.Success;
        }

    }
}
