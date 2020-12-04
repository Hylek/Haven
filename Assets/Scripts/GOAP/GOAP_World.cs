using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Daniel Cumbor in 2020. Thanks Covid-19!

// Singleton for the world and it's states.
public sealed class GOAP_World : MonoBehaviour
{
    private static readonly GOAP_World singleton = new GOAP_World();
    private static GOAP_World_States worldStates;

    static GOAP_World()
    {
        worldStates = new GOAP_World_States();
    }

    public GOAP_World_States getWorldStates()
    {
        return worldStates;
    }

    public static GOAP_World getWorldInstance()
    {
        return singleton;
    }
}
