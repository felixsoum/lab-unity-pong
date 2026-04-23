using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [CreateAssetMenu(menuName = "PaddleBall/PowerUps/Giant Paddle", fileName = "GiantPaddle_SO")]
    public class GiantPaddleSO : PowerUpSO
    {
        [Header("Giant Paddle")]
        [Min(1.01f)]
        [SerializeField] private float m_ScaleFactor = 2f;

        private Paddle m_AffectedA;
        private Paddle m_AffectedB;

        public override void Apply(PowerUpContext ctx)
        {
            m_AffectedA = null;
            m_AffectedB = null;

            Paddle target = ctx.LastHitPaddle;
            if (target != null)
            {
                target.ApplyScaleMultiplier(m_ScaleFactor);
                m_AffectedA = target;
            }
            else
            {
                if (ctx.Paddle1 != null) { ctx.Paddle1.ApplyScaleMultiplier(m_ScaleFactor); m_AffectedA = ctx.Paddle1; }
                if (ctx.Paddle2 != null) { ctx.Paddle2.ApplyScaleMultiplier(m_ScaleFactor); m_AffectedB = ctx.Paddle2; }
            }
        }

        public override void Revert(PowerUpContext ctx)
        {
            float inv = 1f / m_ScaleFactor;
            if (m_AffectedA != null) m_AffectedA.ApplyScaleMultiplier(inv);
            if (m_AffectedB != null) m_AffectedB.ApplyScaleMultiplier(inv);
            m_AffectedA = null;
            m_AffectedB = null;
        }
    }
}
