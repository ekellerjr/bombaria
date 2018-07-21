using System;
using UnityEngine;
using UnityEngine.AI;

public class AIStateController : MonoBehaviour {

    public enum TriggerType
    {
        TriggerEnter, TriggerStay, TriggerExit
    }

    public enum CollisionType
    {
        CollisionEnter, CollisionStay, CollisionExit
    }

    [Header("States")]
    public AIState startState;

    [Header("Readonly")]
    public AIState currentState;

    private NavMeshAgent agent;

    private GameController gameController;

    public NavMeshAgent GetNavMeshAgent()
    {
        return this.agent;
    }

    private void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();

        this.gameController = CommonUtils.GetComponentInGameControllerOrPanic<GameController>();

        startState.Init(this);

        SwitchToState(this.startState);

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!gameController.IsGameRunning() || !gameController.IsAIActive())
            return;

        currentState.updateState(this);
	}

    internal void SwitchToState(AIState nextState)
    {
        if (nextState == null || nextState == currentState)
            return;

        if(currentState != null)
            this.currentState.Dispose(this);

        nextState.Resume(this);

        this.currentState = nextState;

    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTrigger(this, other, TriggerType.TriggerEnter);
    }

}
