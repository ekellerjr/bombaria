using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GameAI/Action/CrossMovement")]
public class CrossMovementAction : AIAction
{

    private static readonly DiscreteMovement.MovingDirection[] moving_directions =
        new DiscreteMovement.MovingDirection[] {
            DiscreteMovement.MovingDirection.forward,
            DiscreteMovement.MovingDirection.right,
            DiscreteMovement.MovingDirection.backward,
            DiscreteMovement.MovingDirection.left
        };

    private static readonly int TIMER_KEY = CommonUtils.RandomHashKey("timer");
    private static readonly int STOP_TIMER_KEY   = CommonUtils.RandomHashKey("stop_timer");

    private static readonly int CUR_TIME_BEFORE_DIR_CHANGE_KEY = CommonUtils.RandomHashKey("cur_time_before_dir_change");
    private static readonly int CUR_STOP_TIME_KEY = CommonUtils.RandomHashKey("cur_stop_time");

    private static readonly int CUR_MOVING_DIR = CommonUtils.RandomHashKey("cur_moving_direction");

    [Header("Fixed Values")]
    public float speed = 3;

    [Header("Random Values")]
    public float minTimeBeforeDirectionChange = 2;
    public float maxTimeBeforeDirectionChange = 5;

    public float minStopTime = 1;
    public float maxStopTime = 2;

    public override void Init(AIStateController controller)
    {
        if (controller.GetRigidbody() == null)
            throw new AIException("Rigidbody component in AIStateController: " + controller + " is neccessary");

        InitCurStopTime();
        InitCurTimeBeforeChangeDirection();

        Resume(controller);
    }

    private void InitCurStopTime()
    {
        minStopTime = minStopTime < 0.1f ? 0.1f : minStopTime;
        maxStopTime = maxStopTime < minStopTime ? minStopTime * 2 : maxStopTime;
    }

    private void InitCurTimeBeforeChangeDirection()
    {
        minTimeBeforeDirectionChange = minTimeBeforeDirectionChange < 0.5f ? 0.5f : minTimeBeforeDirectionChange;
        maxTimeBeforeDirectionChange = maxTimeBeforeDirectionChange < minTimeBeforeDirectionChange ? minTimeBeforeDirectionChange * 2
            : maxTimeBeforeDirectionChange;
    }

    private void ResetTimer(AIStateController controller)
    {
        controller.SetFloat(TIMER_KEY, 0f);
        controller.SetFloat(STOP_TIMER_KEY, 0f);
    }

    private void ResetCurStopTime(AIStateController controller)
    {
        controller.SetFloat(CUR_STOP_TIME_KEY, UnityEngine.Random.Range(minStopTime, maxStopTime));
    }

    private void ResetCurTimeBeforeChangeDirection(AIStateController controller)
    {
        controller.SetFloat(CUR_TIME_BEFORE_DIR_CHANGE_KEY, UnityEngine.Random.Range(minTimeBeforeDirectionChange, maxTimeBeforeDirectionChange));
    }

    private void ChangeDirection(AIStateController controller)
    {
        DiscreteMovement.MovingDirection nextMovingDirection;
        int tries = 0;
        do
        {
            nextMovingDirection = (DiscreteMovement.MovingDirection)moving_directions.GetValue(UnityEngine.Random.Range(0, moving_directions.Length));

        } while (nextMovingDirection == GetCurMovingDirection(controller) && tries++ <= moving_directions.Length);

        SetCurMovingDirection(controller, nextMovingDirection);
    }

    public override void Dispose(AIStateController controller) { }

    public override void OnCollision(AIStateController controller, Collision collision, AIStateController.CollisionType collisionType)
    {
        if (collision.gameObject.CompareTag(BombariaTags.FLOOR) || collisionType != AIStateController.CollisionType.CollisionEnter )
        {
            return;
        }

        Collide(controller);
    }

    private void Collide(AIStateController controller)
    {
        controller.SetFloat(TIMER_KEY, controller.getFloat(CUR_TIME_BEFORE_DIR_CHANGE_KEY));
    }

    public override void OnTrigger(AIStateController controller, Collider other, AIStateController.TriggerType triggerType) {

        if (other.CompareTag(BombariaTags.FLOOR) || triggerType != AIStateController.TriggerType.TriggerEnter)
        {
            return;
        }

        Collide(controller);
    }

    public override void Resume(AIStateController controller)
    {
        ResetTimer(controller);

        ResetCurTimeBeforeChangeDirection(controller);

        ResetCurStopTime(controller);

        ChangeDirection(controller);
    }

    public override void UpdateAction(AIStateController controller)
    {
        controller.SetFloat(TIMER_KEY, controller.getFloat(TIMER_KEY) + Time.deltaTime);
        
        if (controller.getFloat(TIMER_KEY) >= controller.getFloat(CUR_TIME_BEFORE_DIR_CHANGE_KEY))
        {
            controller.SetFloat(STOP_TIMER_KEY, controller.getFloat(STOP_TIMER_KEY) + Time.deltaTime);

            if (controller.getFloat(STOP_TIMER_KEY) >= controller.getFloat(CUR_STOP_TIME_KEY))
            {
                Resume(controller);
            }
        }
        else
        {
            DiscreteMovement.MoveDiscrete(controller.GetRigidbody(), GetCurMovingDirection(controller), speed);
            DiscreteMovement.RotateDiscrete(controller.GetRigidbody(), GetCurMovingDirection(controller));
        }
    }

    private DiscreteMovement.MovingDirection GetCurMovingDirection(AIStateController controller)
    {
        System.Object value = controller.getValue(CUR_MOVING_DIR);
        return value == null ? default(DiscreteMovement.MovingDirection) : (DiscreteMovement.MovingDirection)value;
    }

    private void SetCurMovingDirection(AIStateController controller, DiscreteMovement.MovingDirection direction) {
        controller.SetValue(CUR_MOVING_DIR, direction);
    }

}
