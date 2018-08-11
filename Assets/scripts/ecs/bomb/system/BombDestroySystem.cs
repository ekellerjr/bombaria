using System;
using Unity.Entities;
using UnityEngine;

namespace Bombaria.ECS
{
    [UpdateAfter(typeof(BombSpawnSystem))]
    [UpdateAfter(typeof(BombBlinkSystem))]
    public class BombDestroySystem : ComponentSystem
    {

        public struct BombDestroyData
        {
            public readonly int Length;

            public EntityArray Entities;
            public ComponentDataArray<Bomb> Bomb;
        }

        [Inject] private BombDestroyData bombDestroyData;

        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;

            for (int i = 0; i < bombDestroyData.Length; i++)
            {
                DestroyBomb(
                    i,
                    bombDestroyData.Entities[i],
                    bombDestroyData.Bomb[i],
                    dt
                    );
            }
        }

        private void DestroyBomb(int i, Entity bombEntity, Bomb bomb, float dt)
        {
            bomb.TimeBeforeIgnition -= dt;

            if(bomb.TimeBeforeIgnition <= 0.0f)
            {
                PostUpdateCommands.DestroyEntity(bombEntity);
            }

            bombDestroyData.Bomb[i] = bomb;
        }
    }
}