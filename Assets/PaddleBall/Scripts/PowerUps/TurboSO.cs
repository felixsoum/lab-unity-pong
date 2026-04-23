using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [CreateAssetMenu(menuName = "PaddleBall/PowerUps/Turbo", fileName = "Turbo_SO")]
    public class TurboSO : PowerUpSO
    {
        [Header("Turbo")]
        [Min(1.01f)]
        [SerializeField] private float m_SpeedFactor = 1.6f;

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
