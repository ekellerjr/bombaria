using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameAI/Action/CrossMovement")]
public class CrossMovementAction : AIAction
{
    private static readonly string[] movingDirectionNames = Enum.GetNames(typeof(MovingDirection));

    private enum MovingDirection
    {
        Up, Right, Down, Left
    }

    [Header("Fixed Values")]
    public float speed = 3;

    [Header("Random Values")]
    public float minTimeBeforeChangeDirection = 2;
    public float maxTimeBeforeChangeDirection = 5;

    public float minStopTime = 1;
    public float maxStopTime = 2;

    private float curTimeBeforeChangeDirection;
    private float curStopTime;

    private float timer;
    private float stopTimer;

    private Vector3 movement;
    private MovingDirection curNovingDirection;

    private Rigidbody rb;

    public override void Init(AIStateController controller)
    {
        rb = CommonUtils.GetComponentOrPanic<Rigidbody>(controller.gameObject);

        movement = Vector3.zero;

        InitTimer();

        InitCurTimeBeforeChangeDirection();

        InitCurStopTime();

        ChangeDirection();

    }

    private void InitTimer()
    {
        timer = 0;
        stopTimer = 0;
    }

    private void InitCurStopTime()
    {
        minStopTime = minStopTime < 0.1f ? 0.1f : minStopTime;

        curStopTime = UnityEngine.Random.Range(minStopTime, maxStopTime);
    }

    private void InitCurTimeBeforeChangeDirection()
    {
        minTimeBeforeChangeDirection = minTimeBeforeChangeDirection < 0.5f ? 0.5f : minTimeBeforeChangeDirection;

        curTimeBeforeChangeDirection = UnityEngine.Random.Range(minTimeBeforeChangeDirection, maxTimeBeforeChangeDirection);
    }

    private void ChangeDirection()
    {
        MovingDirection nextMovingDirection;
        int tries = 0;
        do
        {
            string movingDirectionName = movingDirectionNames[UnityEngine.Random.Range(0, movingDirectionNames.Length)];

            nextMovingDirection = (MovingDirection)Enum.Parse(typeof(MovingDirection), movingDirectionName);

        } while (nextMovingDirection != curNovingDirection && tries++ <= movingDirectionNames.Length);

        curNovingDirection = nextMovingDirection;
    }

    public override void Dispose(AIStateController controller) { }

    public override void OnCollision(AIStateController controller, Collider other, AIStateController.CollisionType collisionType) { }

    public override void OnTrigger(AIStateController controller, Collider other, AIStateController.TriggerType triggerType) {

        switch (triggerType)
        {
            case AIStateController.TriggerType.TriggerEnter:
                timer = curTimeBeforeChangeDirection;
                break;
        }
    }

    public override void Resume(AIStateController controller)
    {
        timer = 0;
    }

    public override void UpdateAction(AIStateController controller)
    {
        timer += Time.deltaTime;

        if (timer >= curTimeBeforeChangeDirection)
        {
            stopTimer += Time.deltaTime;

            if (stopTimer >= curStopTime)
            {
                ChangeDirection();
                InitTimer();
                InitCurStopTime();
                InitCurTimeBeforeChangeDirection();
            }
        }
        else
        {
            switch (curNovingDirection)
            {
                case MovingDirection.Up:
                    CommonUtils.Move(rb, Vector3.forward, speed);
                    break;
                case MovingDirection.Right:
                    CommonUtils.Move(rb, Vector3.right, speed);
                    break;
                case MovingDirection.Down:
                    CommonUtils.Move(rb, -Vector3.forward, speed);
                    break;
                case MovingDirection.Left:
                    CommonUtils.Move(rb, -Vector3.right, speed);
                    break;
                default:
                    break;
            }
        }
    }
}
