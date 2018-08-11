
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;

namespace Bombaria.ECS
{

    public class BombariaBootstrap
    {

        // == Archetypes ==
        public static EntityArchetype PlayerArchetype;
        public static EntityArchetype BombSpawnArchetype;

        // == MeshInstanceRenderer ==
        public static MeshInstanceRenderer PlayerRenderer;

        // == Bombaria Settings ==
        public static BombariaSettings Settings;

        // == Manager ==
        public static BombManager BombManager;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();

            PlayerArchetype = em.CreateArchetype(
                typeof(Position), typeof(Heading), typeof(PlayerInput), typeof(TransformMatrix));

            BombSpawnArchetype = em.CreateArchetype(typeof(BombSpawnRequest));

        }

        public static void NewGame()
        {
            EntityManager em = World.Active.GetOrCreateManager<EntityManager>();

            Entity player = em.CreateEntity(PlayerArchetype);

            em.AddSharedComponentData(player, PlayerRenderer);

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterSceneLoad()
        {
            Settings = CommonUtils.GetComponentInGameObjectFoundWithTagOrPanic<BombariaSettings>(BombariaTags.SETTINGS);

            InitializeWithScene();
        }

        private static void InitializeWithScene()
        {
            PlayerRenderer = CommonUtils.GetComponentInGameObjectFoundWithTagOrPanic<MeshInstanceRendererComponent>(
                BombariaTags.PLAYER,
                CommonUtils.GetComponentPostCommand.DestroyGameObject)
                .Value;

            BombManager = BombManager.GetInstance();

            NewGame();
        }

    }

}
