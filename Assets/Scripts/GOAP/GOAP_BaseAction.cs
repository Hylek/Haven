using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Written by Daniel Cumbor in 2020. Thanks Covid-19!

// Base class for all AI actions
public abstract class GOAP_BaseAction : MonoBehaviour
{
    public string actionName = "";
    public float actionCost = 0.0f;
    public float actionDuration = 0.0f;
    public string tagString;
    public GameObject target;
    public NavMeshAgent agent;
    public GOAP_WorldStates agentStates;
    public bool isActionRunning = false;

    // Inspectable properties in editor
    public InspectableState[] inspectable_prerequisites;
    public InspectableState[] inspectable_consequences;

    private Dictionary<string, int> prerequisites;
    private Dictionary<string, int> consequences;

    public GOAP_BaseAction()
    {
        prerequisites = new Dictionary<string, int>();
        consequences = new Dictionary<string, int>();
    }

    public Dictionary<string, int> getPrerequisites()
    {
        return prerequisites;
    }

    public Dictionary<string, int> getConsequences()
    {
        return consequences;
    }

    // Override to perform custom logic before action begins.
    public abstract bool onActionStart();

    // Override to perform custom logc after action is complete.
    public abstract bool onActionComplete();

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        prerequisites = setupStateMap(inspectable_prerequisites);
        consequences = setupStateMap(inspectable_consequences);
    }

    // Takes all the inputted states from the editor and stores them in a Dictionary.
    private Dictionary<string, int> setupStateMap(InspectableState[] states)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();

        if(states != null)
        {
            foreach(InspectableState state in states)
            {
                dictionary.Add(state.key, state.value);
            }
        }

        return dictionary;
    }

    public bool checkPlausibility(Dictionary<string, int> conditions)
    {
        foreach(KeyValuePair<string, int> prerequisite in prerequisites)
        {
            // Prerequisites for action are not met, action cannot proceed.
            if (!conditions.ContainsKey(prerequisite.Key))
            {
                return false;
            }
        }

        return true;
    }
}
