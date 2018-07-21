using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIStateTransition {

    public AIDecision decision;

    public AIState successState;
    public AIState failState;
    
    public void Init (AIStateController controller)
    {
        if (decision == null)
            throw new AIException("Transition with no decision set");

        if (successState == null && failState == null)
            throw new AIException("Both success state and fail state are not set");

            decision.Init(controller);

        if (failState != null)
            failState.Init(controller);

        if (successState != null)
            successState.Init(controller);
    }
}
