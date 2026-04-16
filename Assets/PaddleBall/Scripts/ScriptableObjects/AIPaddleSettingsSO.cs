using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Configuration for the AI-controlled paddle. Adjust these values in the Inspector
    /// to tune difficulty — lower reaction time and accuracy offset make the AI harder to beat.
    /// </summary>
    [CreateAssetMenu(menuName = "PaddleBall/AIPaddleSettings", fileName = "AIPaddleSettings")]
    public class AIPaddleSettingsSO : DescriptionSO
    {
        [Header("AI Behavior")]
        [Tooltip("Time in seconds before the AI reacts to ball position changes")]
        [Range(0.05f, 1f)]
        [SerializeField] private float m_ReactionTime = 0.3f;

        [Tooltip("Random offset range applied to the AI's target position (higher = less accurate)")]
        [Range(0f, 2f)]
        [SerializeField] private float m_AccuracyOffset = 0.5f;

        [Tooltip("Dead zone threshold — AI won't move if ball is within this distance")]
        [Range(0.05f, 1f)]
        [SerializeField] private float m_DeadZone = 0.2f;

        public float ReactionTime => m_ReactionTime;
        public float AccuracyOffset => m_AccuracyOffset;
        public float DeadZone => m_DeadZone;
    }
}
