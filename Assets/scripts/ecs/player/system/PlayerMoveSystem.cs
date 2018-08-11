using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;


namespace Bombaria.ECS
{
    public class PlayerMovementSystem : ComponentSystem
    {

        public struct PlayerMovementData
        {
            public readonly int Length;

            public ComponentDataArray<Position> Position;
            public ComponentDataArray<Heading> Heading;
            [ReadOnly] public ComponentDataArray<PlayerInput> Input;
        }

        [Inject] private PlayerMovementData data;


        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;

            for (int i = 0; i < data.Length; i++)
            {
                PlayerInput input = data.Input[i];

                Position position = data.Position[i];
                Heading heading = data.Heading[i];

                position.Value += input.Movement * dt * BombariaBootstrap.Settings.playerMovementSpeed;
                heading.Value = DiscreteMovement.GetHeadingXZDiscrete45(input.Movement);

                data.Heading[i] = heading;
                data.Position[i] = position;
            }
        }
    }
}