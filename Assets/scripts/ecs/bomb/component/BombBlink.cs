using Unity.Entities;

namespace Bombaria.ECS
{
    public struct BombBlink : IComponentData
    {
        public float Duration;

        public float Duration025;
        public float Duration05;
        public float Duration075;

        public float DurationDiv4;
        public float DurationDiv8;
        public float DurationDiv16;

        public float OverallTimer;
        public float BlinkPeriodTimer;
        public float Period;

        public BombBlink(float _duration)
        {
            Duration = _duration;

            Duration025 = _duration * 0.25f;
            Duration05 = _duration * 0.50f;
            Duration075 = _duration * 0.75f;

            DurationDiv4 = _duration / 16;
            DurationDiv8 = _duration / 32;
            DurationDiv16 = _duration / 128;

            OverallTimer = 0.0f;
            BlinkPeriodTimer = 0.0f;
            Period = 0.0f;
        }
    }
}