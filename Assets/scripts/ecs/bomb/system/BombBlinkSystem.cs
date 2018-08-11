using Unity.Entities;
using Unity.Rendering;
using Unity.Collections;
using UnityEngine;

namespace Bombaria.ECS
{

    [UpdateAfter(typeof(BombSpawnSystem))]
    public class BombBlinkSystem : ComponentSystem
    {

        public struct BombBlinkData
        {
            public readonly int Length;

            [ReadOnly] public EntityArray Entities;
            [ReadOnly] public ComponentDataArray<Bomb> Bomb;
            public ComponentDataArray<BombBlink> BombBlink;
            [ReadOnly] public SharedComponentDataArray<MeshInstanceRenderer> BombLook;
        }

        [Inject] private BombBlinkData bombBlinkData;

        protected override void OnUpdate()
        {
            float dt = Time.deltaTime;

            for (int i = 0; i < bombBlinkData.Length; i++)
            {
                HandleBlinking(
                    i,
                    dt,
                    bombBlinkData.Entities[i],
                    bombBlinkData.Bomb[i],
                    bombBlinkData.BombBlink[i],
                    bombBlinkData.BombLook[i]);
            }
        }

        private void HandleBlinking(int i, float dt, Entity bombEntity, Bomb bomb, BombBlink bombBlink, MeshInstanceRenderer bombLook)
        {
            BombAsset bombAsset = BombariaBootstrap.BombManager.GetBombAsset(bomb.Type);

            bombBlink.OverallTimer += dt;
            bombBlink.BlinkPeriodTimer += dt;

            if (bombBlink.OverallTimer < bombBlink.Duration025)
            {
                bombBlink.Period = bombBlink.Duration;
            }
            else if (bombBlink.OverallTimer >= bombBlink.Duration025 && bombBlink.OverallTimer < bombBlink.Duration05)
            {
                bombBlink.Period = bombBlink.DurationDiv4;
            }
            else if (bombBlink.OverallTimer >= bombBlink.Duration05 && bombBlink.OverallTimer < bombBlink.Duration075)
            {
                bombBlink.Period = bombBlink.DurationDiv8;
            }
            else if (bombBlink.OverallTimer >= bombBlink.Duration075 && bombBlink.OverallTimer < bombBlink.Duration)
            {
                bombBlink.Period = bombBlink.DurationDiv16;
            }
            else if (bombBlink.OverallTimer >= bombBlink.Duration)
            {
                return;
            }

            if (bombBlink.BlinkPeriodTimer >= bombBlink.Period)
            {
                bombBlink.BlinkPeriodTimer = 0;

                MeshInstanceRenderer newLook;

                if(bombLook.Equals(bombAsset.IdleLook))
                {
                    newLook = bombAsset.BlinkLook;
                }
                else if (bombLook.Equals(bombAsset.BlinkLook))
                {
                    newLook = bombAsset.IdleLook;
                }
                else
                {
                    throw new System.Exception("Undefined blinking behavior occurred");
                }

                PostUpdateCommands.RemoveComponent<MeshInstanceRenderer>(bombEntity);
                PostUpdateCommands.AddSharedComponent(bombEntity, newLook);

            }

            bombBlinkData.BombBlink[i] = bombBlink;
        }
    }
}