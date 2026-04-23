using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Listens to gameplay event channels and drives the neon aesthetic: applies HDR colors to sprites,
    /// attaches a TrailRenderer to the ball, spawns bounce/goal particles, pulses score text on points,
    /// shakes the camera on paddle impacts, and lerps ball color with speed. All visuals are code-generated.
    /// </summary>
    public class NeonVFXManager : MonoBehaviour
    {
        [Header("Palette")]
        [SerializeField] private NeonPaletteSO m_Palette;
        [SerializeField] private GameDataSO m_GameData;

        [Header("Listen")]
        [SerializeField] private VoidEventChannelSO m_GameStarted;
        [SerializeField] private Vector2EventChannelSO m_BallCollided;
        [SerializeField] private PlayerIDEventChannelSO m_GoalHit;
        [SerializeField] private PlayerIDEventChannelSO m_PointScored;

        [Header("Trail")]
        [SerializeField] private float m_TrailTime = 0.28f;
        [SerializeField] private float m_TrailStartWidth = 0.35f;

        [Header("Shake")]
        [SerializeField] private float m_ShakeAmplitude = 0.18f;
        [SerializeField] private float m_ShakeDuration = 0.18f;

        private Ball m_Ball;
        private SpriteRenderer m_BallSprite;
        private Rigidbody2D m_BallRb;
        private readonly List<Paddle> m_Paddles = new List<Paddle>();
        private readonly Dictionary<PlayerIDSO, Color> m_PaddleColors = new Dictionary<PlayerIDSO, Color>();
        private readonly Dictionary<PlayerIDSO, ScoreView> m_ScoreViewsByPlayer = new Dictionary<PlayerIDSO, ScoreView>();

        private ParticleSystem m_BounceBurst;
        private ParticleSystem m_GoalBurst;
        private Material m_AdditiveSpriteMaterial;

        private Camera m_Camera;
        private Vector3 m_CameraRestPos;
        private float m_ShakeT;
        private float m_MaxSpeedScalar;

        private void Awake()
        {
            m_AdditiveSpriteMaterial = new Material(Shader.Find("Sprites/Default"));
            m_BounceBurst = CreateBurstSystem("BounceBurst", 0.4f, 0.05f);
            m_GoalBurst = CreateBurstSystem("GoalBurst", 0.9f, 0.09f);
            m_Camera = Camera.main;
            if (m_Camera != null)
                m_CameraRestPos = m_Camera.transform.localPosition;
            // GameDataSO.MaxSpeed is compared as sqrMagnitude in Ball.cs, so convert.
            m_MaxSpeedScalar = Mathf.Sqrt(Mathf.Max(0.01f, m_GameData.MaxSpeed));
        }

        private void OnEnable()
        {
            m_GameStarted.OnEventRaised += OnGameStarted;
            m_BallCollided.OnEventRaised += OnBallCollided;
            m_GoalHit.OnEventRaised += OnGoalHit;
            m_PointScored.OnEventRaised += OnPointScored;
        }

        private void OnDisable()
        {
            m_GameStarted.OnEventRaised -= OnGameStarted;
            m_BallCollided.OnEventRaised -= OnBallCollided;
            m_GoalHit.OnEventRaised -= OnGoalHit;
            m_PointScored.OnEventRaised -= OnPointScored;
        }

        private void OnGameStarted()
        {
            StartCoroutine(ApplyThemeNextFrame());
        }

        private IEnumerator ApplyThemeNextFrame()
        {
            yield return null;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (m_Camera != null)
                m_Camera.backgroundColor = m_Palette.CameraBackground;

            m_Ball = FindFirstObjectByType<Ball>();
            if (m_Ball != null)
            {
                m_BallSprite = m_Ball.GetComponent<SpriteRenderer>();
                m_BallRb = m_Ball.GetComponent<Rigidbody2D>();
                if (m_BallSprite != null) m_BallSprite.color = m_Palette.Ball;
                AttachBallTrail(m_Ball);
            }

            m_Paddles.Clear();
            m_PaddleColors.Clear();
            foreach (var paddle in FindObjectsByType<Paddle>(FindObjectsSortMode.None))
            {
                m_Paddles.Add(paddle);
                var sr = paddle.GetComponentInChildren<SpriteRenderer>();
                if (sr == null) continue;

                // Identify paddle by checking PlayerIDs through GameDataSO. The Paddle doesn't expose its
                // id publicly, so we rely on X position vs. level layout start positions.
                var layout = m_GameData.LevelLayout;
                bool isPlayer1 = Vector2.Distance(paddle.transform.position, layout.Paddle1StartPosition)
                                 < Vector2.Distance(paddle.transform.position, layout.Paddle2StartPosition);
                PlayerIDSO id = isPlayer1 ? m_GameData.Player1 : m_GameData.Player2;
                Color c = isPlayer1 ? m_Palette.Player1 : m_Palette.Player2;
                sr.color = c;
                m_PaddleColors[id] = c;
            }

            foreach (var bouncer in FindObjectsByType<Bouncer>(FindObjectsSortMode.None))
            {
                if (bouncer.GetComponent<Paddle>() != null) continue; // skip paddle-bouncers
                foreach (var sr in bouncer.GetComponentsInChildren<SpriteRenderer>())
                    sr.color = m_Palette.Wall;
            }

            foreach (var goal in FindObjectsByType<ScoreGoal>(FindObjectsSortMode.None))
            {
                foreach (var sr in goal.GetComponentsInChildren<SpriteRenderer>())
                    sr.color = m_Palette.Goal;
            }

            // Map ScoreViews to PlayerIDs via ScoreManager.
            m_ScoreViewsByPlayer.Clear();
            var scoreManager = FindFirstObjectByType<ScoreManager>();
            if (scoreManager != null)
            {
                var field = typeof(ScoreManager).GetField("m_PlayerScores",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null && field.GetValue(scoreManager) is List<PlayerScore> list)
                {
                    foreach (var ps in list)
                    {
                        if (ps.scoreUI != null && ps.playerID != null)
                        {
                            m_ScoreViewsByPlayer[ps.playerID] = ps.scoreUI;
                            if (m_PaddleColors.TryGetValue(ps.playerID, out var pc))
                            {
                                var tmp = ps.scoreUI.GetComponent<TMPro.TextMeshPro>();
                                if (tmp != null) tmp.color = pc;
                            }
                        }
                    }
                }
            }
        }

        private void AttachBallTrail(Ball ball)
        {
            var existing = ball.GetComponent<TrailRenderer>();
            var trail = existing != null ? existing : ball.gameObject.AddComponent<TrailRenderer>();
            trail.time = m_TrailTime;
            trail.startWidth = m_TrailStartWidth;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.02f;
            trail.material = m_AdditiveSpriteMaterial;
            trail.numCornerVertices = 2;
            trail.numCapVertices = 2;
            trail.sortingOrder = -1;

            var grad = new Gradient();
            Color head = m_Palette.Ball;
            Color tail = head;
            grad.SetKeys(
                new[] { new GradientColorKey(head, 0f), new GradientColorKey(tail, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            trail.colorGradient = grad;
        }

        private ParticleSystem CreateBurstSystem(string label, float lifetime, float startSize)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform, false);
            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.loop = false;
            main.playOnAwake = false;
            main.startLifetime = lifetime;
            main.startSpeed = 6f;
            main.startSize = startSize;
            main.startColor = Color.white;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 400;

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 25f;
            shape.radius = 0.05f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });
            col.color = grad;

            var sizeOverLife = ps.sizeOverLifetime;
            sizeOverLife.enabled = true;
            var curve = new AnimationCurve(
                new Keyframe(0f, 1f, 0f, 0f),
                new Keyframe(1f, 0f, -1f, 0f));
            sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, curve);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = m_AdditiveSpriteMaterial;
            renderer.sortingOrder = 10;
            return ps;
        }

        private void OnBallCollided(Vector2 bounceDirection)
        {
            if (m_Ball == null) return;

            Vector3 pos = m_Ball.transform.position;
            Vector2 dir = bounceDirection.sqrMagnitude > 0.001f ? bounceDirection.normalized : Vector2.up;

            Paddle nearPaddle = FindPaddleNear(pos, 1.2f);
            Color burstColor;
            if (nearPaddle != null && m_PaddleColors.Count > 0)
            {
                var layout = m_GameData.LevelLayout;
                bool isP1 = Vector2.Distance(nearPaddle.transform.position, layout.Paddle1StartPosition)
                            < Vector2.Distance(nearPaddle.transform.position, layout.Paddle2StartPosition);
                burstColor = isP1 ? m_Palette.Player1 : m_Palette.Player2;
                StartShake(m_ShakeAmplitude, m_ShakeDuration);
            }
            else
            {
                burstColor = m_Palette.Wall;
            }

            EmitBurst(m_BounceBurst, pos, dir, burstColor, count: 16, speedMin: 3.5f, speedMax: 7f);
        }

        private Paddle FindPaddleNear(Vector3 pos, float radius)
        {
            float r2 = radius * radius;
            for (int i = 0; i < m_Paddles.Count; i++)
            {
                if (m_Paddles[i] == null) continue;
                if ((m_Paddles[i].transform.position - pos).sqrMagnitude <= r2)
                    return m_Paddles[i];
            }
            return null;
        }

        private void OnGoalHit(PlayerIDSO scoringPlayer)
        {
            if (m_Ball == null) return;
            Color c = m_PaddleColors.TryGetValue(scoringPlayer, out var pc) ? pc : m_Palette.Ball;
            EmitBurst(m_GoalBurst, m_Ball.transform.position, Vector2.zero, c,
                count: 70, speedMin: 6f, speedMax: 14f);
            StartShake(m_ShakeAmplitude * 1.8f, m_ShakeDuration * 1.5f);
        }

        private void OnPointScored(PlayerIDSO scoringPlayer)
        {
            if (m_ScoreViewsByPlayer.TryGetValue(scoringPlayer, out var view) && view != null)
                StartCoroutine(PulseTransform(view.transform, 1.45f, 0.35f));
        }

        private void EmitBurst(ParticleSystem ps, Vector3 pos, Vector2 dir, Color color, int count, float speedMin, float speedMax)
        {
            if (ps == null) return;
            bool directional = dir.sqrMagnitude > 0.001f;
            Vector2 baseDir = directional ? dir.normalized : Vector2.up;
            float spread = directional ? 35f : 180f;

            var emitParams = new ParticleSystem.EmitParams
            {
                position = pos,
                startColor = color,
            };

            for (int i = 0; i < count; i++)
            {
                float angleOffset = Random.Range(-spread, spread) * Mathf.Deg2Rad;
                float cos = Mathf.Cos(angleOffset);
                float sin = Mathf.Sin(angleOffset);
                Vector2 rotated = new Vector2(
                    baseDir.x * cos - baseDir.y * sin,
                    baseDir.x * sin + baseDir.y * cos);
                float speed = Random.Range(speedMin, speedMax);
                emitParams.velocity = rotated * speed;
                ps.Emit(emitParams, 1);
            }
        }

        private void StartShake(float amplitude, float duration)
        {
            if (m_Camera == null) return;
            StopCoroutine("ShakeRoutine");
            StartCoroutine(ShakeRoutine(amplitude, duration));
        }

        private IEnumerator ShakeRoutine(float amplitude, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float decay = 1f - Mathf.Clamp01(t / duration);
                Vector2 offset = Random.insideUnitCircle * amplitude * decay;
                m_Camera.transform.localPosition = m_CameraRestPos + (Vector3)offset;
                yield return null;
            }
            m_Camera.transform.localPosition = m_CameraRestPos;
        }

        private IEnumerator PulseTransform(Transform target, float peakScale, float duration)
        {
            if (target == null) yield break;
            Vector3 baseScale = target.localScale;
            float half = duration * 0.5f;
            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, t / half);
                target.localScale = Vector3.Lerp(baseScale, baseScale * peakScale, k);
                yield return null;
            }
            t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                float k = Mathf.SmoothStep(0f, 1f, t / half);
                target.localScale = Vector3.Lerp(baseScale * peakScale, baseScale, k);
                yield return null;
            }
            target.localScale = baseScale;
        }

        private void LateUpdate()
        {
            if (m_BallSprite == null || m_BallRb == null) return;
            float speed = m_BallRb.linearVelocity.magnitude;
            float t = Mathf.Clamp01(speed / m_MaxSpeedScalar);
            m_BallSprite.color = Color.Lerp(m_Palette.Ball, m_Palette.BallFast, t);
        }
    }
}
