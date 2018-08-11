using Unity.Entities;

namespace Bombaria.ECS
{
    public struct Bomb : IComponentData
    {
        public BombType Type;
        public float TimeBeforeIgnition;
        public float Energy;
    }
}