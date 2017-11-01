using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : Movement {
    
    void Start()
    {
        setup();
    }

    void Update()
    {
        processMovement();
    }

    public override void setup()
    {
        base.setup();
    }

    public override void processMovement()
    {
        base.processMovement();
    }

    public override void changeState(NPCStateMachine newState)
    {
        base.changeState(newState);
    }
}
