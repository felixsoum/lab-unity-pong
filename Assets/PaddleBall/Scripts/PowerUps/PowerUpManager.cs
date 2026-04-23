using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    public class PowerUpManager : MonoBehaviour
    {
        [Header("Listen to Event Channels")]
        [SerializeField] private PowerUpEventChannelSO m_Collected;
        [SerializeField] private VoidEventChannelSO m_GameOver;

        [Header("Broadcast on Event Channels")]
        [SerializeField, Optional] private PowerUpEventChannelSO m_Activated;
        [SerializeField, Optional] private PowerUpEventChannelSO m_Expired;

        private readonly PowerUpContext m_Context = new PowerUpContext();
        private readonly Dictionary<PowerUpSO, Coroutine> m_Active = new Dictionary<PowerUpSO, Coroutine>();

        private void Awake()
        {
            NullRefChecker.Validate(this);
        }

        private void OnEnable()
        {
            m_Collected.OnEventRaised += OnCollected;
            m_GameOver.OnEventRaised += RevertAll;
        }

        private void OnDisable()
        {
            m_Collected.OnEventRaised -= OnCollected;
            m_GameOver.OnEventRaised -= RevertAll;
        }

        public void Initialize(Ball ball, Ball ballPrefab, Paddle p1, Paddle p2, GameDataSO gameData)
        {
            m_Context.MainBall = ball;
            m_Context.BallPrefab = ballPrefab;
            m_Context.BallParent = ball != null ? ball.transform.parent : null;
            m_Context.Paddle1 = p1;
            m_Context.Paddle2 = p2;
            m_Context.GameData = gameData;
            m_Context.CoroutineRunner = this;
        }

        private void OnCollected(PowerUpSO powerUp)
        {
            if (powerUp == null) return;

            if (m_Active.TryGetValue(powerUp, out Coroutine existing))
            {
                if (existing != null) StopCoroutine(existing);
                m_Active[powerUp] = StartCoroutine(RunTimer(powerUp, isRefresh: true));
            }
            else
            {
                powerUp.Apply(m_Context);
                if (m_Activated != null) m_Activated.RaiseEvent(powerUp);
                m_Active[powerUp] = StartCoroutine(RunTimer(powerUp, isRefresh: false));
            }
        }

        private IEnumerator RunTimer(PowerUpSO powerUp, bool isRefresh)
        {
            yield return new WaitForSeconds(powerUp.Duration);
            powerUp.Revert(m_Context);
            m_Active.Remove(powerUp);
            if (m_Expired != null) m_Expired.RaiseEvent(powerUp);
        }

        private void RevertAll()
        {
            foreach (KeyValuePair<PowerUpSO, Coroutine> kv in m_Active)
            {
                if (kv.Value != null) StopCoroutine(kv.Value);
                if (kv.Key != null) kv.Key.Revert(m_Context);
                if (m_Expired != null && kv.Key != null) m_Expired.RaiseEvent(kv.Key);
            }
            m_Active.Clear();
        }
    }
}
