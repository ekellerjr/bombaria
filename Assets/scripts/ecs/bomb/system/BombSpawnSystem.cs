using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

namespace Bombaria.ECS
{
    public class BombSpawnBarrier : BarrierSystem { }

    public class BombSpawnSystem : ComponentSystem
    {

        public struct BombSpawnData
        {
            public readonly int Length;

            public EntityArray SpawnedEntities;
            [ReadOnly] public ComponentDataArray<BombSpawnRequest> BombSpawn;
        }

        [Inject] private BombSpawnData bombSpawnData;

        protected override void OnUpdate()
        {
            for (int i = 0; i < bombSpawnData.Length; i++)
            {
                SpawnBomb(i,
                    bombSpawnData.SpawnedEntities[i],
                    bombSpawnData.BombSpawn[i]
                    );
            }
        }

        private void SpawnBomb(int i, Entity bombEntity, BombSpawnRequest bombSpawn)
        {
            BombAsset bombAsset = BombariaBootstrap.BombManager.GetBombAsset(bombSpawn.Bomb.Type);

            PostUpdateCommands.RemoveComponent<BombSpawnRequest>(bombEntity);

            PostUpdateCommands.AddComponent(
                bombEntity,
                bombSpawn.Bomb);

            PostUpdateCommands.AddComponent(
                bombEntity,
                bombSpawn.Position);

            PostUpdateCommands.AddComponent(
                bombEntity,
                bombSpawn.Heading);

            PostUpdateCommands.AddComponent(
                bombEntity,
                new BombBlink(bombAsset.TimeBeforeIgnition));

            PostUpdateCommands.AddComponent(
                bombEntity,
                default(TransformMatrix));

            PostUpdateCommands.AddSharedComponent(
                bombEntity,
                bombAsset.IdleLook);

        }
    }
}