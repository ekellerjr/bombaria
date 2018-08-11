using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Bombaria.ECS
{
    public struct PlayerInput : IComponentData
    {
        
        public float3 Movement;

        // == Action Triggers ==
        public byte PlaceBombActionTriggered;

        // == Action Colldowns ==
        public float PlaceBombActionCooldown;

        // == Helper Functions ==       
        public bool PlaceBomb => PlaceBombActionTriggered >= 1 && PlaceBombActionCooldown <= 0.0f;

    }

}