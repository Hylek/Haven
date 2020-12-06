using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumberjack : GOAP_Agent
{
    new void Start()
    {
        base.Start();

        GOAP_Goal idle = new GOAP_Goal("storeWood", 1, true);
        mainGoals.Add(idle, 1);
    }
}
