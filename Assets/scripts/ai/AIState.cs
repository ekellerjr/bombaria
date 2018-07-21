using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameAI/State")]
public class AIState : ScriptableObject {

    public AIAction[] actions;
    public AIStateTransition[] transitions;

	public void updateState(AIStateController controller)
    {
        UpdateActions(controller);
        UpdateTransitions(controller);
    }

    internal void Init(AIStateController controller)
    {
        InitActions(controller);
        InitTransitions(controller);
    }

    public void Dispose(AIStateController controller)
    {
        DisposeActions(controller);
        DisposeTransitions(controller);
    }

    public void Resume(AIStateController controller)
    {
        ResumeActions(controller);
        ResumeTransitions(controller);
    }

    private void InitTransitions(AIStateController controller)
    {
        foreach (AIStateTransition transition in transitions)
        {
            transition.Init(controller);
        }
    }

    private void InitActions(AIStateController controller)
    {
        foreach (AIAction action in actions)
        {
            action.Init(controller);
        }
    }

    public void OnTrigger(AIStateController controller, Collider other, AIStateController.TriggerType triggerType)
    {
        OnTriggerActions(controller, other, triggerType);
        OnTriggerTransitions(controller, other, triggerType);
    }

    protected void UpdateTransitions(AIStateController controller)
    {
        foreach (AIStateTransition transition in transitions)
        {
            AIDecision.DecisionResult decisionResult = transition.decision.UpdateDecision(controller);

            SwitchToState(controller, transition, decisionResult);
        }
    }

    protected void UpdateActions(AIStateController controller)
    {
        foreach (AIAction action in actions)
        {
            action.UpdateAction(controller);
        }
    }

    protected void OnTriggerTransitions(AIStateController controller, Collider other, AIStateController.TriggerType triggerType)
    {
        foreach (AIStateTransition transition in transitions)
        {
            AIDecision.DecisionResult decisionResult = transition.decision.OnTrigger(controller, other, triggerType);

            SwitchToState(controller, transition, decisionResult);
        }
    }

    private void ResumeTransitions(AIStateController controller)
    {
        foreach (AIStateTransition transition in transitions)
        {
            AIDecision.DecisionResult decisionResult = transition.decision.Resume(controller);

            SwitchToState(controller, transition, decisionResult);
        }
    }

    private void ResumeActions(AIStateController controller)
    {
        foreach (AIAction action in actions)
        {
            action.Resume(controller);
        }
    }

    private void DisposeTransitions(AIStateController controller)
    {
        foreach (AIStateTransition transition in transitions)
        {
            AIDecision.DecisionResult decisionResult = transition.decision.Dispose(controller);

            SwitchToState(controller, transition, decisionResult);
        }
    }

    private void DisposeActions(AIStateController controller)
    {
        foreach (AIAction action in actions)
        {
            action.Dispose(controller);
        }
    }

    private void SwitchToState(AIStateController controller, AIStateTransition transition, AIDecision.DecisionResult decisionResult)
    {
        switch (decisionResult)
        {
            case AIDecision.DecisionResult.None:
                break;
            case AIDecision.DecisionResult.Fail:
                controller.SwitchToState(transition.failState);
                break;
            case AIDecision.DecisionResult.Success:
                controller.SwitchToState(transition.successState);
                break;
            default:
                break;
        }
    }

    protected void OnTriggerActions(AIStateController controller, Collider other, AIStateController.TriggerType triggerType)
    {
        foreach (AIAction action in actions)
        {
            action.OnTrigger(controller, other, triggerType);
        }
    }

}