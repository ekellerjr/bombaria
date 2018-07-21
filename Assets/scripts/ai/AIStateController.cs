using System.Collections.Generic;
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

    private Rigidbody rb;

    private NavMeshAgent agent;

    private GameController gameController;

    private Dictionary<int, System.Object> valueStorage;

    private int individualHashCode;

    public NavMeshAgent GetNavMeshAgent()
    {
        return this.agent;
    }

    public Rigidbody GetRigidbody()
    {
        return this.rb;
    }

    private void Awake()
    {
        individualHashCode = GetHashCode();

        this.agent = GetComponent<NavMeshAgent>();

        this.rb = GetComponent<Rigidbody>();

        this.gameController = CommonUtils.GetComponentInGameControllerOrPanic<GameController>();

        valueStorage = new Dictionary<int, System.Object>();

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
        if (currentState == null)
            return;

        currentState.OnTrigger(this, other, TriggerType.TriggerEnter);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == null)
            return;

        currentState.OnCollision(this, collision, CollisionType.CollisionEnter);
    }

    internal void SetValue(int keyHash, System.Object value)
    {
        this.valueStorage[IndividualizeKeyHash(keyHash)] = value;
    }

    internal System.Object getValue(int keyHash)
    {
        keyHash = IndividualizeKeyHash(keyHash);

        return valueStorage.ContainsKey(keyHash) ? valueStorage[keyHash] : null ;
    }

    internal void SetFloat(int keyHash, float value)
    {   
        this.valueStorage[IndividualizeKeyHash(keyHash)] = value;
    }

    internal float getFloat(int keyHash)
    {
        keyHash = IndividualizeKeyHash(keyHash);

        return valueStorage.ContainsKey(keyHash) ? (float)valueStorage[keyHash] : float.NaN;
    }

    private int IndividualizeKeyHash(int keyHash)
    {
        return keyHash + individualHashCode;
    }
}
