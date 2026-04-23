using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    public abstract class PowerUpSO : DescriptionSO
    {
        [Header("Identity")]
        [SerializeField] private string m_DisplayName = "PowerUp";
        [ColorUsage(true, true)]
        [SerializeField] private Color m_NeonColor = Color.white;
        [SerializeField, Optional] private Sprite m_Icon;

        [Header("Timing")]
        [Min(0.1f)]
        [SerializeField] private float m_Duration = 5f;

        [Header("Spawner")]
        [Min(0f)]
        [SerializeField] private float m_SpawnWeight = 1f;

        public string DisplayName => m_DisplayName;
        public Color NeonColor => m_NeonColor;
        public Sprite Icon => m_Icon;
        public float Duration => m_Duration;
        public float SpawnWeight => m_SpawnWeight;

        public abstract void Apply(PowerUpContext ctx);
        public abstract void Revert(PowerUpContext ctx);
    }
}
