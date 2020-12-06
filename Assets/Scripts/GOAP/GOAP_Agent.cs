using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

// Written by Daniel Cumbor in 2020. Thanks Covid-19!

public class GOAP_Agent : MonoBehaviour
{
    public GOAP_Planner planner;

    // A list of this agent's available possible actions
    public List<GOAP_BaseAction> possibleActions = new List<GOAP_BaseAction>();

    // An executable list of actions
    public Queue<GOAP_BaseAction> actionQueue;

    // Map of this agent's main goals
    public Dictionary<GOAP_Goal, int> mainGoals = new Dictionary<GOAP_Goal, int>();

    // Current action and goal
    public GOAP_BaseAction currentAction;
    private GOAP_Goal currentGoal;

    private bool isBusy = true;

    public void Start()
    {
        // Get all possible actions attached to this agent and put them in the list
        GOAP_BaseAction[] receivedActions = this.GetComponents<GOAP_BaseAction>();
        foreach(GOAP_BaseAction action in receivedActions)
        {
            possibleActions.Add(action);
        }
    }

    public void LateUpdate()
    {
        // If the plan is not yet complete then don't make a new plan.
        if (currentlyExecutingPlan())
        {
            if(atActionLocation())
            {
                if (isBusy)
                {
                    Invoke("actionCompleted", currentAction.actionDuration);
                    isBusy = false;
                }
            }

            return;
        }

        makePlansforGoals();
        removeAchievedGoal();
        startNewAction();
    }

    private void startNewAction()
    {
        // If we still have items in the queue then our goal is not yet reached.
        if(actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if(currentAction.onActionStart())
            {
                if(currentAction.target == null && currentAction.tag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.tagString);
                }

                if(currentAction.target != null)
                {
                    currentAction.isActionRunning = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }

    private void removeAchievedGoal()
    {
        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (currentGoal.shouldRemove)
            {
                mainGoals.Remove(currentGoal);
            }
            planner = null;
        }
    }

    private void actionCompleted()
    {
        currentAction.isActionRunning = false;
        currentAction.onActionComplete();

        isBusy = true;
    }

    private bool currentlyExecutingPlan()
    {
        return currentAction != null && currentAction.isActionRunning;
    }

    private bool atActionLocation()
    {
        return currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f;
    }

    private bool hasNoPlan()
    {
        return planner == null || actionQueue == null;
    }

    private void makePlansforGoals()
    {
        if (hasNoPlan())
        {
            planner = new GOAP_Planner();

            // Order all agent's desired goals and create a plan for each to see which are achievable.
            IOrderedEnumerable<KeyValuePair<GOAP_Goal, int>> orderedGoals = from goal in mainGoals orderby goal.Value descending select goal;
            foreach (KeyValuePair<GOAP_Goal, int> orderedGoal in orderedGoals)
            {
                actionQueue = planner.createPlan(possibleActions, orderedGoal.Key.goals, null);
                if (actionQueue != null)
                {
                    // We have a plan!
                    currentGoal = orderedGoal.Key;

                    break;
                }
            }
        }
    }
}
