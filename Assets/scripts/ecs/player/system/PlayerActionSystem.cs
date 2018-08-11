using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using System;

namespace Bombaria.ECS
{
    public class PlayerActionSystem : ComponentSystem
    {

        struct PlayerInputData
        {
            public readonly int Length;

            public ComponentDataArray<PlayerInput> Input;
            [ReadOnly] public ComponentDataArray<Position> Position;
            [ReadOnly] public ComponentDataArray<Heading> Heading;
        }

        [Inject] private PlayerInputData players;

        protected override void OnUpdate()
        {
            for (int i = 0; i < players.Length; i++)
            {
                HandleActions(
                    i,
                    players.Input[i],
                    players.Position[i],
                    players.Heading[i]
                    );
            }
        }

        private void HandleActions(int i, PlayerInput input, Position position, Heading heading)
        {
            HandlePlaceBombAction(i, input, position, heading);
        }

        private void HandlePlaceBombAction(int i, PlayerInput input, Position position, Heading heading)
        {
            if (!input.PlaceBomb) return;

            // TODO: Find out which bomb type is currently selected from PlayerManager
            BombAsset bombAsset = BombariaBootstrap.BombManager.GetBombAsset(BombType.Normal);
            
            // TODO: Get placeBombActionCooldown from PlayerManager
            input.PlaceBombActionCooldown = BombariaBootstrap.Settings.placeBombActionCooldown;
            players.Input[i] = input;

            PostUpdateCommands.CreateEntity(BombariaBootstrap.BombSpawnArchetype);
            PostUpdateCommands.SetComponent(new BombSpawnRequest
            {
                Bomb = new Bomb
                {
                    Type = bombAsset.Type,
                    TimeBeforeIgnition = bombAsset.TimeBeforeIgnition,
                    Energy = bombAsset.Energy
                },

                Position = position,
                Heading = heading
            });
        }
    }
}