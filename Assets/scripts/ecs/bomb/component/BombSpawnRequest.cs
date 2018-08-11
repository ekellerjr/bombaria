using Unity.Entities;
using Unity.Transforms;

namespace Bombaria.ECS
{
    public struct BombSpawnRequest : IComponentData
    {
        public Bomb Bomb;
        public Position Position;
        public Heading Heading;
    }
}