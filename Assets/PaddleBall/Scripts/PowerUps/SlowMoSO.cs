using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [CreateAssetMenu(menuName = "PaddleBall/PowerUps/Slow-Mo", fileName = "SlowMo_SO")]
    public class SlowMoSO : PowerUpSO
    {
        [Header("Slow-Mo")]
        [Range(0.1f, 0.99f)]
        [SerializeField] private float m_SpeedFactor = 0.5f;

        public override void Apply(PowerUpContext ctx)
        {
            if (ctx.MainBall != null) ctx.MainBall.SetSpeedMultiplier(m_SpeedFactor);
            foreach (Ball b in ctx.ExtraBalls)
                if (b != null) b.SetSpeedMultiplier(m_SpeedFactor);
        }

        public override void Revert(PowerUpContext ctx)
        {
            if (ctx.MainBall != null) ctx.MainBall.SetSpeedMultiplier(1f / m_SpeedFactor);
            foreach (Ball b in ctx.ExtraBalls)
                if (b != null) b.SetSpeedMultiplier(1f / m_SpeedFactor);
        }
    }
}
