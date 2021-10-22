using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.EnemyTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees
{
    class OrcPatrolTree : Selector
    {
        public OrcPatrolTree(NPC character, GameObject target)
        {
            // Orc should patrol between two points
            // If orc sees player then pursue
            // If player is too far away go back to patrol
            // Make patrol and pursue scripts?

            this.children = new List<Task>()
            {
                new Pursue(character, target, character.awakeDistance),
                new HearOrcScream(character, target, character.weaponRange),
                new IsPlayerNear(character, target, character.awakeDistance),
                new Patrol(character, character.weaponRange),
                new LightAttack(character)
            };

        }

    }
}
