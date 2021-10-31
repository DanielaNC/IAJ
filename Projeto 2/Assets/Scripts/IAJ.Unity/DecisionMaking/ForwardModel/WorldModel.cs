using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Utils;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel
{
    public class WorldModel
    {
        public object[] PropertiesArray { get; set; }

        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; } 

        private Dictionary<string, float> GoalValues { get; set; } 

        protected WorldModel Parent { get; set; }

        public WorldModel(List<Action> actions)
        {
            this.PropertiesArray = new object[24];
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(actions);
            this.Actions.Shuffle();
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.PropertiesArray = parent.PropertiesArray.ToArray();
            this.GoalValues = new Dictionary<string, float>();
            this.Actions = new List<Action>(parent.Actions);
            this.Actions.Shuffle();
            this.Parent = parent;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        public virtual object GetProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "Mana":
                    if (this.PropertiesArray[0] == null)
                    {
                        this.PropertiesArray[0] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.Mana;
                    }
                    return this.PropertiesArray[0];
                case "HP":
                    if (this.PropertiesArray[1] == null)
                    {
                        this.PropertiesArray[1] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.HP;
                    }
                    return this.PropertiesArray[1];
                case "ShieldHP":
                    if (this.PropertiesArray[2] == null)
                    {
                        this.PropertiesArray[2] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.ShieldHP;
                    }
                    return this.PropertiesArray[2];
                case "MAXHP":
                    if (this.PropertiesArray[3] == null)
                    {
                        this.PropertiesArray[3] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.MaxHP;
                    }
                    return this.PropertiesArray[3];
                case "XP":
                    if (this.PropertiesArray[4] == null)
                    {
                        this.PropertiesArray[4] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.XP;
                    }
                    return this.PropertiesArray[4];
                case "Time":
                    if (this.PropertiesArray[5] == null)
                    {
                        this.PropertiesArray[5] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.Time;
                    }
                    return this.PropertiesArray[5];
                case "Money":
                    if (this.PropertiesArray[6] == null)
                    {
                        this.PropertiesArray[6] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.Money;
                    }
                    return this.PropertiesArray[6];
                case "Level":
                    if (this.PropertiesArray[7] == null)
                    {
                        this.PropertiesArray[7] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.Level;
                    }
                    return this.PropertiesArray[7];
                case "Position":
                    if (this.PropertiesArray[8] == null)
                    {
                        this.PropertiesArray[8] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().characterData.CharacterGameObject.transform.position;
                    }
                    return this.PropertiesArray[8];
                case "Orc0":
                    if (this.PropertiesArray[9] == null)
                    {
                        this.PropertiesArray[9] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[9];
                case "Orc1":
                    if (this.PropertiesArray[10] == null)
                    {
                        this.PropertiesArray[10] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[10];
                case "Orc2":
                    if (this.PropertiesArray[11] == null)
                    {
                        this.PropertiesArray[11] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[11];
                case "Skelleton0":
                    if (this.PropertiesArray[12] == null)
                    {
                        this.PropertiesArray[12] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[12];

                case "Skelleton1":
                    if (this.PropertiesArray[13] == null)
                    {
                        this.PropertiesArray[13] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[13];

                case "Dragon(Clone)":
                    if (this.PropertiesArray[14] == null)
                    {
                        this.PropertiesArray[14] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[14];

                case "HealthPotion0":
                    if (this.PropertiesArray[15] == null)
                    {
                        this.PropertiesArray[15] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[15];

                case "HealthPotion1":
                    if (this.PropertiesArray[16] == null)
                    {
                        this.PropertiesArray[16] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[16];

                case "ManaPotion0":
                    if (this.PropertiesArray[17] == null)
                    {
                        this.PropertiesArray[17] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[17];

                case "ManaPotion1":
                    if (this.PropertiesArray[18] == null)
                    {
                        this.PropertiesArray[18] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[18];

                case "Chest0":
                    if (this.PropertiesArray[19] == null)
                    {
                        this.PropertiesArray[19] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[19];

                case "Chest1":
                    if (this.PropertiesArray[20] == null)
                    {
                        this.PropertiesArray[20] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[20];

                case "Chest2":
                    if (this.PropertiesArray[21] == null)
                    {
                        this.PropertiesArray[21] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[21];

                case "Chest3":
                    if (this.PropertiesArray[22] == null)
                    {
                        this.PropertiesArray[22] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[22];

                case "Chest4":
                    if (this.PropertiesArray[23] == null)
                    {
                        this.PropertiesArray[23] = GameObject.Find("Manager").GetComponent<Manager.GameManager>().disposableObjects.ContainsKey(propertyName);
                    }
                    return this.PropertiesArray[23];

                default:
                    Debug.Log("ERROR + " + propertyName);
                    return false;
            }

        }

        public virtual void SetProperty(string propertyName, object value)
        {
            switch (propertyName)
            {
                case "Mana":
                    this.PropertiesArray[0] = value;
                    break;
                case "HP":
                    this.PropertiesArray[1] = value;
                    break;
                case "ShieldHP":
                    this.PropertiesArray[2] = value;
                    break;
                case "MAXHP":
                    this.PropertiesArray[3] = value;
                    break;
                case "XP":
                    this.PropertiesArray[4] = value;
                    break;
                case "Time":
                    this.PropertiesArray[5] = value;
                    break;
                case "Money":
                    this.PropertiesArray[6] = value;
                    break;
                case "Level":
                    this.PropertiesArray[7] = value;
                    break;
                case "Position":
                    this.PropertiesArray[8] = value;
                    break;
                case "Orc0":
                    this.PropertiesArray[9] = value;
                    break;
                case "Orc1":
                    this.PropertiesArray[10] = value;
                    break;
                case "Orc2":
                    this.PropertiesArray[11] = value;
                    break;
                case "Skelleton0":
                    this.PropertiesArray[12] = value;
                    break;

                case "Skelleton1":
                    this.PropertiesArray[13] = value;
                    break;

                case "Dragon(Clone)":
                    this.PropertiesArray[14] = value;
                    break;

                case "HealthPotion0":
                    this.PropertiesArray[15] = value;
                    break;

                case "HealthPotion1":
                    this.PropertiesArray[16] = value;
                    break;

                case "ManaPotion0":
                    this.PropertiesArray[17] = value;
                    break;

                case "ManaPotion1":
                    this.PropertiesArray[18] = value;
                    break;

                case "Chest0":
                    this.PropertiesArray[19] = value;
                    break;

                case "Chest1":
                    this.PropertiesArray[20] = value;
                    break;

                case "Chest2":
                    this.PropertiesArray[21] = value;
                    break;

                case "Chest3":
                    this.PropertiesArray[22] = value;
                    break;

                case "Chest4":
                    this.PropertiesArray[23] = value;
                    break;

                default:
                    Debug.Log("ERROR + " + propertyName);
                    break;
            }
        }

        public virtual float GetGoalValue(string goalName)
        {
            //recursive implementation of WorldModel
            if (this.GoalValues.ContainsKey(goalName))
            {
                return this.GoalValues[goalName];
            }
            else if (this.Parent != null)
            {
                return this.Parent.GetGoalValue(goalName);
            }
            else
            {
                return 0;
            }
        }

        public virtual void SetGoalValue(string goalName, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }

            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValues[goalName] = limitedValue;
        }

        public virtual WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            foreach (var goal in goals)
            {
                var newValue = this.GetGoalValue(goal.Name);

                discontentment += goal.GetDiscontentment(newValue);
            }

            if ((int)this.GetProperty(Manager.Properties.MONEY) > 0) discontentment /= (int)this.GetProperty(Manager.Properties.MONEY);
            //if ((int)this.GetProperty(Manager.Properties.MONEY) == 25) discontentment = float.MinValue; // prioritize end goal
            if ((float)this.GetProperty(Manager.Properties.TIME) >= 200.0f) discontentment = float.MaxValue; // disregard impossible wins
            if ((int)this.GetProperty(Manager.Properties.HP) <= 0) discontentment = float.MaxValue; // disregard impossible wins

            return discontentment;
        }

        public virtual Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;    
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }

        public virtual Action[] GetExecutableActions()
        {
            return this.Actions.Where(a => a.CanExecute(this)).ToArray();
        }

        public virtual bool IsTerminal()
        {
            int HP = (int)this.GetProperty("HP");
            float time = (float)this.GetProperty("Time");
            int money = (int)this.GetProperty("Money");

            return HP <= 0 || time >= 200 || (this.GetNextPlayer() == 0 && money == 25);
        }
        

        public virtual float GetScore()
        {
            int money = (int)this.GetProperty("Money");
            int HP = (int)this.GetProperty("HP");
            float time = (float)this.GetProperty("Time");

            if (HP <= 0) return 0.0f;
            if (time >= 200) return 0.0f;
            if (money == 25) return 1.0f;
            
            return 0.0f;
        }

        public virtual int GetNextPlayer()
        {
            return 0;
        }

        public virtual void CalculateNextPlayer()
        {
        }

        public override string ToString()
        {
            string ret = "WorldModel: ";
            for (int i = 0; i < PropertiesArray.Length; i++)
            {
                ret += "Key: " + i +  " value = " + PropertiesArray[i] + " | ";
            }
            return ret;
        }
    }
}
