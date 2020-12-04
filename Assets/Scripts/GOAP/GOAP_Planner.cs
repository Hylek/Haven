using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Written by Daniel Cumbor in 2020. Thanks Covid-19!

/*
 * PLANNER LOGIC
 * The planner takes in all possible agent actions and produces
 * a tree to determine all the different ways the given goal is achieveable
 * It will then return 1 of the plans, the one that is the shortest and cheapest
 * (If more than 1 plan has the same length and cost then it will be chosen at random)
 * Planner can return no plan if the agent's possible actions do not combine to reach the given goal.
 * (E.G Baker wants to make bread but the prerequisite for that action is that they need wheat
 * which they do not have, as the world state reports there is no wheat in the storehouse).
 */

// Class that represents the different nodes of the tree graph.
public class Node
{
    public Node root;
    public float cost;
    public Dictionary<string, int> states;
    public GOAP_BaseAction action;

    public Node(Node root, float cost, GOAP_BaseAction action, Dictionary<string, int> states)
    {
        this.root = root;
        this.cost = cost;
        this.action = action;
        this.states = new Dictionary<string, int>(states); // Create new copy for the node.
    }
}

public class GOAP_Planner
{
    public Queue<GOAP_BaseAction> createPlan(List<GOAP_BaseAction> possibleActions, Dictionary<string, int> goal, GOAP_WorldStates worldStates)
    {
        List<Node> nodes = new List<Node>();
        GOAP_WorldStates currentWorldStates = GOAP_World.getWorldInstance().getWorldStates();
        Node root = new Node(null, 0, null, currentWorldStates.getStates());

        // Generate tree and determine if there is a possible plan
        if(createTree(root, nodes, possibleActions, goal))
        {
            // Get the cheapest possible path to the goal and return the last node in that branch.
            Node cheapest = getCheapestPath(nodes);

            // Once we have the cheapest node, work backwards on the branch to add each actions that achieve the goal
            return getPlan(cheapest);
        }
        else
        {
            Debug.Log("Planner failed to obtain plan!");

            return null;
        }
    }

    private bool createTree(Node root, List<Node> nodes, List<GOAP_BaseAction> possibleActions, Dictionary<string, int> goal)
    {
        bool foundPossiblePlan = false;
        foreach(GOAP_BaseAction currentAction in possibleActions)
        {
            if(currentAction.checkPlausibility(root.states))
            {
                // Copy world states to keep track of changing prerequisites when searching for plans.
                Dictionary<string, int> worldStates = new Dictionary<string, int>(root.states);
                foreach(KeyValuePair<string, int> consequence in currentAction.getConsequences())
                {
                    if(!worldStates.ContainsKey(consequence.Key))
                    {
                        worldStates.Add(consequence.Key, consequence.Value);
                    }
                }

                // Create the next node in the current branch
                Node nextNode = new Node(root, root.cost + currentAction.actionCost, currentAction, worldStates);

                // If goal was reached a plan was discovered! Return true, else create a branch.
                // Keep recursively calling until all options are exhausted. 
                if(isGoalReached(goal, worldStates))
                {
                    nodes.Add(nextNode);

                    foundPossiblePlan = true;
                }
                else
                {
                    List<GOAP_BaseAction> newPath = removeTriedAction(possibleActions, currentAction);

                    bool newPlan = createTree(nextNode, nodes, newPath, goal);
                    if(newPlan)
                    {
                        foundPossiblePlan = true;
                    }
                }
            }
        }

        return foundPossiblePlan;
    }

    private bool isGoalReached(Dictionary<string, int> goal, Dictionary<string, int> states)
    {
        foreach(KeyValuePair<string, int> g in goal)
        {
            if(!states.ContainsKey(g.Key))
            {
                return false;
            }
        }

        return true;
    }

    private List<GOAP_BaseAction> removeTriedAction(List<GOAP_BaseAction> actions, GOAP_BaseAction actionToRemove)
    {
        List<GOAP_BaseAction> newActionsList = new List<GOAP_BaseAction>();
        foreach(GOAP_BaseAction action in actions)
        {
            if(!action.Equals(actionToRemove))
            {
                newActionsList.Add(action);
            }
        }

        return newActionsList;
    }

    private Node getCheapestPath(List<Node> nodes)
    {
        Node cheapest = null;

        foreach(Node node in nodes)
        {
            if(cheapest == null)
            {
                cheapest = node;
            }
            else
            {
                if(node.cost < cheapest.cost)
                {
                    cheapest = node;
                }
            }
        }

        return cheapest;
    }

    private Queue<GOAP_BaseAction> getPlan(Node cheapestNode)
    {
        List<GOAP_BaseAction> actions = new List<GOAP_BaseAction>();

        // Gather all actions that make up the cheapest plan
        while(cheapestNode != null)
        {
            if(cheapestNode.action != null)
            {
                actions.Insert(0, cheapestNode.action);
            }
            cheapestNode = cheapestNode.root;
        }

        // Put them in an executable queue for the agent to use.
        Queue<GOAP_BaseAction> plan = new Queue<GOAP_BaseAction>();
        foreach(GOAP_BaseAction action in actions)
        {
            plan.Enqueue(action);
        }

        Debug.Log("The Plan is: ");
        foreach (GOAP_BaseAction action in plan)
        {
            Debug.Log("Q: " + action.actionName);
        }

        return plan;
    }
}
