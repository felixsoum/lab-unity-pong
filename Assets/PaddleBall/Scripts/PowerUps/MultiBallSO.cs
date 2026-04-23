using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [CreateAssetMenu(menuName = "PaddleBall/PowerUps/Multi-Ball", fileName = "MultiBall_SO")]
    public class MultiBallSO : PowerUpSO
    {
        [Header("Multi-Ball")]
        [Min(1)]
        [SerializeField] private int m_ExtraBallCount = 2;
        [Min(0f)]
        [SerializeField] private float m_SpawnOffset = 0.5f;

        public override void Apply(PowerUpContext ctx)
        {
            if (ctx.MainBall == null || ctx.BallPrefab == null) return;

            Vector3 origin = ctx.MainBall.transform.position;
            for (int i = 0; i < m_ExtraBallCount; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-m_SpawnOffset, m_SpawnOffset), Random.Range(-m_SpawnOffset, m_SpawnOffset), 0f);
                Ball extra = Object.Instantiate(ctx.BallPrefab, origin + offset, Quaternion.identity, ctx.BallParent);
                extra.Initialize(ctx.GameData);
                extra.IsExtra = true;
                extra.LaunchRandom();
                ctx.ExtraBalls.Add(extra);
            }
        }

        public override void Revert(PowerUpContext ctx)
        {
            foreach (Ball b in ctx.ExtraBalls)
            {
                if (b != null) Object.Destroy(b.gameObject);
            }
            ctx.ExtraBalls.Clear();
        }
    }
}
