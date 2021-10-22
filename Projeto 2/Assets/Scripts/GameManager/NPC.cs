using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine.AI;
using UnityEngine.UI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;

namespace Assets.Scripts.Manager
{

    public class NPC : MonoBehaviour
    {

        public string Name { get; private set; }
        public string Type { get; private set; }
        // Stats
        public int XPvalue { get; private set; }
        public int HP { get; private set; }
        public int AC { get; private set; }
        public int simpleDamage { get; private set; }
        public int weaponRange { get; private set; }
        public Vector3 patrolPoint { get; private set; }
        public Vector3 initialPos { get; private set; }

        //how do you like lambda's in c#?
        public Func<int> dmgRoll;
        public float awakeDistance { get; private set; }
        public GameObject player { get; private set; }
        public GameManager manager { get; private set; }

        public bool usingBehaviourTree;
        public bool isShouting { get; set; } = false;
        public float decisionRate = 2.0f;
        public TextMesh ShoutText { get; set; }

        private NavMeshAgent agent;
        
        //The Behavior Tree
        private Task behaviourTree;
     
        void Start()
        {
            this.Name = this.transform.gameObject.name;
            this.Type = this.transform.gameObject.tag;
            this.initialPos = this.transform.position;
            agent = this.GetComponent<NavMeshAgent>();
            manager = GameObject.FindObjectOfType<GameManager>();
            player = GameObject.FindGameObjectWithTag("Player");

          

            switch (this.Type)
            {
                case "Skeleton":
                    this.XPvalue = 3;
                    this.AC = 10;
                    this.HP = 5;
                    this.dmgRoll = () => RandomHelper.RollD6();
                    this.simpleDamage = 2;
                    this.awakeDistance = 10;
                    this.weaponRange = 2;
                    break;
                case "Orc":
                    this.XPvalue = 10;
                    this.AC = 14;
                    this.HP = 15;
                    this.dmgRoll = () => RandomHelper.RollD10() +2;
                    this.simpleDamage = 5;
                    this.awakeDistance = 15;
                    this.weaponRange = 3;
                    var canvas = Instantiate(GameObject.FindObjectOfType<GameManager>().CanvasPrefab, this.transform);
                    ShoutText = canvas.GetComponentInChildren<TextMesh>();
                    ShoutText.text = "";
                    ShoutText.color = Color.red;
                    this.gameObject.AddComponent<AudioSource>();
                    this.gameObject.GetComponent<AudioSource>().clip = GameObject.FindObjectOfType<GameManager>().OrcShout;
                    break;
                case "Dragon":
                    this.XPvalue = 20;
                    this.AC = 16;
                    this.HP = 30;
                    this.dmgRoll = () => RandomHelper.RollD12() + RandomHelper.RollD12();
                    this.simpleDamage = 10;
                    this.awakeDistance = 15;
                    this.weaponRange = 5;
                    break;
                default:
                    this.XPvalue = 3;
                    this.AC = 10;
                    this.HP = 5;
                    this.dmgRoll = () => RandomHelper.RollD6();
                    break;
            }

            // If we want the NPCs to use behavior trees
            if (manager.BehaviourTreeNPCs)
            {
                this.usingBehaviourTree = true;
                var patrols = GameObject.FindGameObjectsWithTag("Patrol");
                var lastDistance = float.MaxValue;

                foreach(var p in patrols)
                {
                    if (this.agent != null && Vector3.Distance(this.agent.transform.position, p.transform.position) < lastDistance)
                    {
                        this.patrolPoint = p.transform.position;
                        lastDistance = Vector3.Distance(this.agent.transform.position, p.transform.position);
                    }
                }

                //Debug.Log("patrol: " + this.patrolPoint +  " for character: " + this.Name) ;

                if (this.Type == "Orc")
                {
                    behaviourTree = new OrcPatrolTree(this, player);
                    //Debug.Log(this.Name);
                }

                else
                    behaviourTree = new BasicTree(this, player);
            }

            // If the NPCs are wake we call this function every 1 secons
            if (!usingBehaviourTree && !manager.SleepingNPCs)
                Invoke("CheckPlayerPosition", 1.0f);


        }


        void FixedUpdate()
        {
            if (usingBehaviourTree)
                    this.behaviourTree.Run();
        }

        // Very basic Enemy AI
        void CheckPlayerPosition()
        {
            if (Vector3.Distance(this.transform.position, player.transform.position) < awakeDistance)
            {

                if (Vector3.Distance(this.transform.position, player.transform.position) <= weaponRange)
                {
                    AttackPlayer();
                }

                else
                {
                    Debug.Log("Pursuing Player");
                    PursuePlayer();
                    Invoke("CheckPlayerPosition", 0.5f);
                }
            }
            else
            {

                Invoke("CheckPlayerPosition", 3.0f);
            }
        }


        //These are the 3 basic actions the NPCs can make
        public void PursuePlayer()
        {
            if (agent != null)
            {
                this.agent.SetDestination(player.transform.position);
                //isPursuingPlayer = true;
            }
        }

        public void StopPursuingPlayer()
        {
            this.agent.SetDestination(this.initialPos);
        }

        public void AttackPlayer()
        {
            manager.EnemyAttack(this.gameObject);
        }

        public void MoveTo(Vector3 targetPosition)
        {
            if (agent != null)
                this.agent.SetDestination(targetPosition);
        }

        public void Shout()
        {
            if (this.Type != "Orc") return;
            if(!this.isShouting)
                this.gameObject.GetComponent<AudioSource>().Play();
            this.isShouting = true;
            GameObject.FindObjectOfType<GameManager>().orcShout = true;
            GameObject.FindObjectOfType<GameManager>().orcShoutPosition = this.transform.position;
            ShoutText.text = "RAAAWR";
        }

       

     }
}
