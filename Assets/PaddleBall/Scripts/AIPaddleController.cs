using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Controls a paddle automatically by tracking the ball's vertical position.
    /// Uses reaction delay and random offset to create beatable AI behavior.
    /// Feeds input through InputReaderSO.SimulateP2Input to reuse the existing paddle movement pipeline.
    /// </summary>
    public class AIPaddleController : MonoBehaviour
    {
        private AIPaddleSettingsSO m_Settings;
        private InputReaderSO m_InputReader;
        private Transform m_BallTransform;

        private float m_TargetY;
        private float m_CurrentOffset;
        private float m_ReactionTimer;

        public void Initialize(AIPaddleSettingsSO settings, InputReaderSO inputReader)
        {
            m_Settings = settings;
            m_InputReader = inputReader;

            Ball ball = FindFirstObjectByType<Ball>();
            if (ball != null)
                m_BallTransform = ball.transform;

            m_ReactionTimer = 0f;
            m_CurrentOffset = Random.Range(-m_Settings.AccuracyOffset, m_Settings.AccuracyOffset);
        }

        private void FixedUpdate()
        {
            if (m_BallTransform == null || m_Settings == null || m_InputReader == null)
                return;

            m_ReactionTimer -= Time.fixedDeltaTime;

            if (m_ReactionTimer <= 0f)
            {
                m_TargetY = m_BallTransform.position.y;
                m_CurrentOffset = Random.Range(-m_Settings.AccuracyOffset, m_Settings.AccuracyOffset);
                m_ReactionTimer = m_Settings.ReactionTime;
            }

            float targetWithOffset = m_TargetY + m_CurrentOffset;
            float difference = targetWithOffset - transform.position.y;

            float input;
            if (Mathf.Abs(difference) < m_Settings.DeadZone)
                input = 0f;
            else
                input = Mathf.Sign(difference);

            m_InputReader.SimulateP2Input(input);
        }
    }
}
