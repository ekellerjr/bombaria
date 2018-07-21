using System;
using UnityEngine;

public abstract class AIAction : ScriptableObject {

    public abstract void Init(AIStateController controller);

    public abstract void UpdateAction(AIStateController controller);

    public abstract void Dispose(AIStateController controller);

    public abstract void Resume(AIStateController controller);

    public abstract void OnTrigger(AIStateController controller, Collider other, AIStateController.TriggerType triggerType);

    public abstract void OnCollision(AIStateController controller, Collider other, AIStateController.CollisionType collisionType);

}
