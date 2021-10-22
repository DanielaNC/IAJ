﻿using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public Manager.CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private List<Action> Actions { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.Actions = actions;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
            var processedActions = 0;

            var startTime = Time.realtimeSinceStartup;

            float currentDiscontentment = float.MaxValue;

           while(this.CurrentDepth >= 0)
           {
                // check if we're at max depth
                if (this.CurrentDepth >= MAX_DEPTH)
                {
                    // calculate discontentment at the deepest level
                    currentDiscontentment = this.Models[this.CurrentDepth].CalculateDiscontentment(this.Goals);
                }

                if (currentDiscontentment < this.BestDiscontentmentValue) {

                    this.BestDiscontentmentValue = currentDiscontentment;
                    this.BestAction = this.ActionPerLevel[0];
                    this.CurrentDepth -= 1;
                    
                    continue;
                }

                var nextAction = this.Models[this.CurrentDepth].GetNextAction();

                if (nextAction != null)
                {
                    this.Models[CurrentDepth + 1] = this.Models[CurrentDepth].GenerateChildWorldModel();
                    this.ActionPerLevel[CurrentDepth] = nextAction;
                    nextAction.ApplyActionEffects(this.Models[CurrentDepth + 1]);
                    this.CurrentDepth += 1;
                }

                else
                    this.CurrentDepth -= 1;

           }

           this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
           this.InProgress = false;
           return this.BestAction;
        }
    }
}
