using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.ForwardModelActions;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using System;
using System.Collections.Generic;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel.Action;
using Assets.Scripts.Manager;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<Action> BestActionSequence { get; private set; }
        public WorldModel BestActionSequenceWorldState { get; private set; }
        protected int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }
        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }
        protected bool UseUCT = false;
        protected int NrPlayouts = 1;
        protected int nrActions = 0;
        protected bool LimitedPlayout = false;




        public MCTS(CurrentStateWorldModel currentStateWorldModel, bool useUCT, int nrPlayouts, bool limitedPlayout)
        {
            this.InProgress = false;
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.RandomGenerator = new System.Random();
            this.UseUCT = useUCT;
            this.NrPlayouts = nrPlayouts;
            this.LimitedPlayout = limitedPlayout;
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 7;
            this.MaxSelectionDepthReached = 7;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();

            // create root node n0 for state s0
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<Action>();
        }

        public Action Run()
        {
            MCTSNode selectedNode = this.InitialNode;
            Reward reward;
            this.TotalProcessingTime = Time.time;
            this.CurrentIterationsInFrame = 0;

            while(CurrentIterationsInFrame < MaxIterationsProcessedPerFrame)
            {
                selectedNode = Selection(selectedNode);
                for (int i = 0; i < this.NrPlayouts; i++)
                {
                    var state = new FutureStateWorldModel(selectedNode.State.GenerateChildWorldModel());
                    reward = Playout(state);
                    Backpropagate(selectedNode, reward);
                }
                this.CurrentIterationsInFrame++;
            }

            // return best initial child
            if (!UseUCT)
            {
                if (BestInitialChild(this.InitialNode) != null)
                    return BestInitialChild(this.InitialNode).Action;
                return null;
            }
            else
            {
                if (BestInitialUCTChild(this.InitialNode) != null)
                    return BestInitialUCTChild(this.InitialNode).Action;
                return null;
            }
        }

        // Selection and Expantion
        protected MCTSNode Selection(MCTSNode initialNode)
        {
            Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;
            int depth = 0;

            while (currentNode != null && !currentNode.State.IsTerminal())
            {
                if (LimitedPlayout && depth > MaxSelectionDepthReached)
                {
                    break;
                }
                nextAction = currentNode.State.GetNextAction();

                while (nextAction!= null && !nextAction.CanExecute(currentNode.State))
                    nextAction = currentNode.State.GetNextAction();

                // if not fully expanded
                if (nextAction != null)
                    return Expand(initialNode, nextAction);

                if (!UseUCT)
                    currentNode = this.BestChild(currentNode);
                else
                {
                    currentNode = this.BestUCTChild(currentNode);
                }
                depth++;
            }

            if (currentNode == null)
            {
                currentNode = initialNode;
            }

            return currentNode;
        }

        protected virtual Reward Playout(FutureStateWorldModel initialPlayoutState)
        {
            Action[] executableActions = initialPlayoutState.GetExecutableActions();
            int depth = 0;
            
            while (!initialPlayoutState.IsTerminal())
            {

                if (LimitedPlayout && depth >= MaxPlayoutDepthReached)
                {
                    break;
                }

                List<Action> feasibleActions = new List<Action>();
                //check if any actions leads to win scenario

                foreach(var possible_action in executableActions)
                {
                    var state = (FutureStateWorldModel)initialPlayoutState.GenerateChildWorldModel();
                    possible_action.ApplyActionEffects(state);
                    state.CalculateNextPlayer();
                    if (state.IsWin()) return new Reward(initialPlayoutState, initialPlayoutState.GetNextPlayer() == 0 ? 1 : 0, false);
                    feasibleActions.Add(possible_action);
                }

                var action = feasibleActions[RandomGenerator.Next(0, feasibleActions.Count)];
                initialPlayoutState = (FutureStateWorldModel) initialPlayoutState.GenerateChildWorldModel();
                action.ApplyActionEffects(initialPlayoutState);
                initialPlayoutState.CalculateNextPlayer();
                executableActions = initialPlayoutState.GetExecutableActions();
                depth++;
            }

            return new Reward(initialPlayoutState, initialPlayoutState.GetNextPlayer() == 0 ? 1 : 0, LimitedPlayout);
        }

        protected virtual void Backpropagate(MCTSNode node, Reward reward)
        {
           while(node != null)
           {
                node.N += 1;
                node.Q += reward.Value;
                node = node.Parent;
           }
        }

        protected MCTSNode Expand(MCTSNode parent, Action action)
        {
            WorldModel newState = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(newState);
            newState.CalculateNextPlayer();
            MCTSNode child = new MCTSNode(newState);
            child.Action = action;
            parent.ChildNodes.Add(child);
            return child;
        }

        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            MCTSNode bestChild = null;

            float score = 0.0f;
            float previousScore = float.MinValue;

            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.Parent != null && child.N != 0)
                {
                    score = (child.Q / child.N) + C * (float)Math.Sqrt(Math.Log(node.Parent.N != 0 ? node.Parent.N : 1)/child.N);

                    if (score > previousScore)
                    {
                        bestChild = child;
                        previousScore = score;
                    }
                }

                if(child.Parent == null && child.N != 0)
                {
                    score = (child.Q / child.N) + C;

                    if (score > previousScore)
                    {
                        bestChild = child;
                        previousScore = score;
                    }
                }
            }
            return bestChild;
        }

        protected virtual MCTSNode BestInitialUCTChild(MCTSNode node)
        { 
            MCTSNode bestChild = null;

            float score = 0.0f;
            float previousScore = float.MinValue;

            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.N != 0 && child.Action.CanExecute() && child.Action != null)
                {
                    score = (child.Q / child.N) + C;

                        if (score > previousScore)
                        {
                            bestChild = child;
                            previousScore = score;
                        }
                    }
                }
            this.TotalProcessingTime = Time.time - this.TotalProcessingTime;
            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            MCTSNode bestChild = null;
            float score = 0.0f;
            float previousScore = float.MinValue;

            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.N != 0)
                {
                    score = child.Q / child.N;

                    if (score > previousScore)
                    {
                        bestChild = child;
                        previousScore = score;
                    }
                }
            }

            return bestChild;
        }

        protected MCTSNode BestInitialChild(MCTSNode node)
        {
            MCTSNode bestChild = null;
            float score = 0.0f;
            float previousScore = float.MinValue;

            foreach (MCTSNode child in node.ChildNodes)
            {
                if (child.N != 0 && child.Action.CanExecute() && child.Action != null)
                {
                    score = child.Q / child.N;

                    if (score > previousScore)
                    {
                        bestChild = child;
                        previousScore = score;
                    }
                }
            }
            this.TotalProcessingTime = Time.time - this.TotalProcessingTime;
            return bestChild;
        }


        protected Action BestFinalAction(MCTSNode node)
        {
            var bestChild = this.BestChild(node);
            if (UseUCT)
                bestChild = this.BestUCTChild(node);
            if (bestChild == null) return null;

            this.BestFirstChild = bestChild;

            //this is done for debugging proposes only
            this.BestActionSequence = new List<Action>();
            this.BestActionSequence.Add(bestChild.Action);
            node = bestChild;

            while(!node.State.IsTerminal())
            {
                if (!UseUCT)
                    bestChild = this.BestChild(node);
                else
                    bestChild = this.BestUCTChild(node);
                if (bestChild == null) break;
                this.BestActionSequence.Add(bestChild.Action);
                node = bestChild;
                this.BestActionSequenceWorldState = node.State;
            }

            return this.BestFirstChild.Action;
        }

    }
}
