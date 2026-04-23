using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    public class PowerUpSpawner : MonoBehaviour
    {
        [Header("Listen to Event Channels")]
        [SerializeField] private VoidEventChannelSO m_GameStarted;
        [SerializeField] private VoidEventChannelSO m_GameOver;

        [Header("Broadcast on Event Channels")]
        [SerializeField] private PowerUpEventChannelSO m_PowerUpCollected;

        [Header("Config")]
        [Tooltip("All available power-up types. Adding new ones requires no code change to the spawner.")]
        [SerializeField] private List<PowerUpSO> m_PowerUps = new List<PowerUpSO>();
        [Tooltip("Single pickup prefab; tinted per PowerUpSO at spawn time")]
        [SerializeField] private PowerUpPickup m_PickupPrefab;

        [Tooltip("Random interval between spawns (seconds)")]
        [SerializeField] private Vector2 m_SpawnIntervalRange = new Vector2(8f, 12f);
        [Tooltip("How long a pickup stays on field before disappearing")]
        [Min(0.5f)]
        [SerializeField] private float m_PickupLifetime = 6f;
        [Tooltip("Random X range for spawn positions (world units)")]
        [SerializeField] private Vector2 m_SpawnXRange = new Vector2(-6f, 6f);
        [Tooltip("Random Y range for spawn positions (world units)")]
        [SerializeField] private Vector2 m_SpawnYRange = new Vector2(-3f, 3f);

        private Coroutine m_Loop;

        private void Awake()
        {
            NullRefChecker.Validate(this);
        }

        private void OnEnable()
        {
            m_GameStarted.OnEventRaised += StartSpawning;
            m_GameOver.OnEventRaised += StopSpawning;
        }

        private void OnDisable()
        {
            m_GameStarted.OnEventRaised -= StartSpawning;
            m_GameOver.OnEventRaised -= StopSpawning;
        }

        private void StartSpawning()
        {
            StopSpawning();
            m_Loop = StartCoroutine(SpawnLoop());
        }

        private void StopSpawning()
        {
            if (m_Loop != null)
            {
                StopCoroutine(m_Loop);
                m_Loop = null;
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                float wait = Random.Range(m_SpawnIntervalRange.x, m_SpawnIntervalRange.y);
                yield return new WaitForSeconds(wait);
                SpawnOne();
            }
        }

        private void SpawnOne()
        {
            if (m_PowerUps == null || m_PowerUps.Count == 0 || m_PickupPrefab == null) return;

            PowerUpSO choice = PickWeighted();
            if (choice == null) return;

            Vector3 pos = new Vector3(
                Random.Range(m_SpawnXRange.x, m_SpawnXRange.y),
                Random.Range(m_SpawnYRange.x, m_SpawnYRange.y),
                0f);

            PowerUpPickup pickup = Instantiate(m_PickupPrefab, pos, Quaternion.identity);
            pickup.Initialize(choice, m_PowerUpCollected, m_PickupLifetime);
        }

        private PowerUpSO PickWeighted()
        {
            float total = 0f;
            foreach (PowerUpSO so in m_PowerUps)
                if (so != null) total += Mathf.Max(0f, so.SpawnWeight);
            if (total <= 0f) return null;

            float r = Random.Range(0f, total);
            float acc = 0f;
            foreach (PowerUpSO so in m_PowerUps)
            {
                if (so == null) continue;
                acc += Mathf.Max(0f, so.SpawnWeight);
                if (r <= acc) return so;
            }
            return null;
        }
    }
}
