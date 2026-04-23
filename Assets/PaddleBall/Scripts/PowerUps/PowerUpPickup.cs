using System.Collections;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
    public class PowerUpPickup : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
        [SerializeField] private Collider2D m_Collider;
        [SerializeField] private Rigidbody2D m_Rigidbody;
        [Tooltip("Rotation speed for visual flourish, degrees/sec")]
        [SerializeField] private float m_SpinSpeed = 90f;

        private PowerUpSO m_PowerUp;
        private PowerUpEventChannelSO m_Collected;
        private bool m_Consumed;

        private void Reset()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Collider = GetComponent<Collider2D>();
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Initialize(PowerUpSO powerUp, PowerUpEventChannelSO collectedChannel, float lifetime)
        {
            m_PowerUp = powerUp;
            m_Collected = collectedChannel;

            if (m_SpriteRenderer != null)
            {
                m_SpriteRenderer.color = powerUp.NeonColor;
                if (powerUp.Icon != null)
                    m_SpriteRenderer.sprite = powerUp.Icon;
            }

            if (m_Collider != null)
                m_Collider.isTrigger = true;

            if (m_Rigidbody != null)
            {
                m_Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                m_Rigidbody.gravityScale = 0f;
            }

            StartCoroutine(ExpireAfter(lifetime));
        }

        private void Update()
        {
            transform.Rotate(0f, 0f, m_SpinSpeed * Time.deltaTime);
        }

        private IEnumerator ExpireAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (!m_Consumed && this != null)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_Consumed) return;

            Ball ball = other.GetComponent<Ball>();
            if (ball == null) ball = other.GetComponentInParent<Ball>();
            if (ball == null) return;

            m_Consumed = true;
            if (m_Collected != null && m_PowerUp != null)
                m_Collected.RaiseEvent(m_PowerUp);

            Destroy(gameObject);
        }
    }
}
