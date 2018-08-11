
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

namespace Bombaria.ECS
{
    public class PlayerInputSystem : ComponentSystem
    {

        struct PlayerInputData
        {
            public readonly int Length;

            public ComponentDataArray<PlayerInput> Input;
        }

        [Inject] private PlayerInputData players;

        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;

            for (int i = 0; i < players.Length; i++)
            {
                UpdatePlayerInput(i, dt);
            }

        }

        private void UpdatePlayerInput(int i, float dt)
        {

            PlayerInput pi = players.Input[i];

            // HandleMovement(pi);
            // HandleActions(pi);
            // HandleCooldowns(pi, dt);

            // == Handle Movement ==
            pi.Movement.x = Input.GetAxisRaw("Horizontal");
            pi.Movement.y = 0.0f;
            pi.Movement.z = Input.GetAxisRaw("Vertical");

            // == Handle Actions ==
            pi.PlaceBombActionTriggered = (byte)(Input.GetKeyDown(BombariaBootstrap.Settings.placeBombActionKey) ? 1 : 0);

            // == Handle Cooldowns ==
            pi.PlaceBombActionCooldown = ApplyCooldown(pi.PlaceBombActionCooldown, dt);

            players.Input[i] = pi;
        }

        /*
        private void HandleMovement(PlayerInput pi)
        {
            pi.Movement.x = Input.GetAxisRaw("Horizontal");
            pi.Movement.y = 0.0f;
            pi.Movement.z = Input.GetAxisRaw("Vertical");
        }
        */

        /*
        private void HandleActions(PlayerInput pi)
        {
            pi.PlaceBombActionTriggered = (byte) (Input.GetKeyDown(BombariaBootstrap.Settings.placeBombActionKey) ? 1 : 0);
        }
        */
        
        /*
        private void HandleCooldowns(PlayerInput pi, float dt)
        {
            pi.PlaceBombActionCooldown = ApplyCooldown(pi.PlaceBombActionCooldown, dt);
        }
        */
        
        private float ApplyCooldown(float value, float dt)
        {
            return math.max(0.0f, value - dt);
        }
    }
}