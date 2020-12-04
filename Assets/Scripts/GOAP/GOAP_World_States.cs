using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Daniel Cumbor in 2020. Thanks Covid-19!

[System.Serializable]
public class WORLD_STATE
{
    public string key;
    public int value;
}

// Helper logic for handling GOAP's world states.
public class GOAP_World_States
{
    private Dictionary<string, int> states;

    public GOAP_World_States()
    {
        states = new Dictionary<string, int>();
    }

    public Dictionary<string, int> getStates()
    {
        return states;
    }

    // Check if a state exists
    protected bool hasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Checks if a state exists, if it does modify, if not add it.
    public void setState(string key, int value)
    {
        if(hasState(key))
        {
            states[key] = value;
        }
        else
        {
            states.Add(key, value);
        }
    }

    public void editState(string key, int value)
    {
        if(hasState(key))
        {
            states[key] += value;
            if(states[key] <= 0)
            {
                deleteState(key);
            }
        }
        else
        {
            setState(key, value);
        }
    }

    public void deleteState(string key)
    {
        if(hasState(key))
        {
            states.Remove(key);
        }
        else
        {
            Debug.Log("CAUTION: Cannot delete state, it does not exist!");
        }
    }
}
