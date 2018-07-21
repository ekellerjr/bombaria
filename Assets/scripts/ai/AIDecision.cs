using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIDecision : ScriptableObject {

    public enum DecisionResult
    {
        None, Fail, Success
    }

    public abstract void Init(AIStateController controller);

    public abstract DecisionResult UpdateDecision(AIStateController controller);

    public abstract DecisionResult Dispose(AIStateController controller);

    public abstract DecisionResult Resume(AIStateController controller);
    
    public abstract DecisionResult OnTrigger(AIStateController controller, Collider other, AIStateController.TriggerType triggerType);

    public abstract DecisionResult OnCollision(AIStateController controller, Collision collision, AIStateController.CollisionType collisionType);

}