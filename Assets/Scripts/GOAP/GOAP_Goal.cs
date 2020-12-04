using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Goal class used by the Agent class
public class GOAP_Goal
{
    public Dictionary<string, int> goals;

    // If goal only needs to be achieved once, it can be removed
    public bool shouldRemove;

    public GOAP_Goal(string key, int value, bool shouldRemove)
    {
        this.shouldRemove = shouldRemove;

        goals = new Dictionary<string, int>();
        goals.Add(key, value);
    }
}
