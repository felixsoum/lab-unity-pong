using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    /// <summary>
    /// Procedural Tron-style neon grid built from LineRenderers. Lines scroll on X and pulse intensity
    /// on a sine wave. Draws behind gameplay via negative sorting order on the Default sprite layer.
    /// </summary>
    public class TronGridBackground : MonoBehaviour
    {
        [SerializeField] private NeonPaletteSO m_Palette;

        [Header("Grid")]
        [SerializeField] private float m_Width = 26f;
        [SerializeField] private float m_Height = 14f;
        [SerializeField] private float m_Spacing = 1.0f;
        [SerializeField] private float m_LineThickness = 0.035f;
        [SerializeField] private float m_ZDepth = 5f;

        [Header("Animation")]
        [SerializeField] private float m_ScrollSpeed = 0.6f;
        [SerializeField] private float m_PulseHz = 0.5f;
        [SerializeField] private float m_PulseAmount = 0.35f;

        private readonly System.Collections.Generic.List<LineRenderer> m_Verticals =
            new System.Collections.Generic.List<LineRenderer>();
        private readonly System.Collections.Generic.List<LineRenderer> m_Horizontals =
            new System.Collections.Generic.List<LineRenderer>();
        private Material m_SharedMaterial;
        private float m_ScrollOffset;

        private void Start()
        {
            m_SharedMaterial = new Material(Shader.Find("Sprites/Default"));
            BuildGrid();
        }

        private void BuildGrid()
        {
            float halfW = m_Width * 0.5f;
            float halfH = m_Height * 0.5f;

            // Vertical lines — create N + 2 so we can wrap one for scrolling.
            int verticalCount = Mathf.CeilToInt(m_Width / m_Spacing) + 2;
            for (int i = 0; i < verticalCount; i++)
            {
                float x = -halfW + i * m_Spacing;
                m_Verticals.Add(CreateLine($"V_{i}",
                    new Vector3(x, -halfH, m_ZDepth),
                    new Vector3(x, halfH, m_ZDepth)));
            }

            int horizontalCount = Mathf.CeilToInt(m_Height / m_Spacing) + 1;
            for (int i = 0; i < horizontalCount; i++)
            {
                float y = -halfH + i * m_Spacing;
                m_Horizontals.Add(CreateLine($"H_{i}",
                    new Vector3(-halfW, y, m_ZDepth),
                    new Vector3(halfW, y, m_ZDepth)));
            }
        }

        private LineRenderer CreateLine(string label, Vector3 a, Vector3 b)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform, false);
            var lr = go.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.positionCount = 2;
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
            lr.startWidth = m_LineThickness;
            lr.endWidth = m_LineThickness;
            lr.sharedMaterial = m_SharedMaterial;
            lr.startColor = m_Palette.Grid;
            lr.endColor = m_Palette.Grid;
            lr.sortingOrder = -20;
            lr.numCornerVertices = 0;
            lr.numCapVertices = 0;
            return lr;
        }

        private void Update()
        {
            m_ScrollOffset = (m_ScrollOffset + m_ScrollSpeed * Time.deltaTime) % m_Spacing;

            float halfW = m_Width * 0.5f;
            float halfH = m_Height * 0.5f;
            for (int i = 0; i < m_Verticals.Count; i++)
            {
                float baseX = -halfW + i * m_Spacing;
                float x = baseX + m_ScrollOffset;
                if (x > halfW) x -= m_Verticals.Count * m_Spacing;
                m_Verticals[i].SetPosition(0, new Vector3(x, -halfH, m_ZDepth));
                m_Verticals[i].SetPosition(1, new Vector3(x, halfH, m_ZDepth));
            }

            float pulse = 1f + Mathf.Sin(Time.time * m_PulseHz * Mathf.PI * 2f) * m_PulseAmount;
            Color pulsed = m_Palette.Grid * pulse;
            pulsed.a = 1f;
            for (int i = 0; i < m_Verticals.Count; i++)
            {
                m_Verticals[i].startColor = pulsed;
                m_Verticals[i].endColor = pulsed;
            }
            for (int i = 0; i < m_Horizontals.Count; i++)
            {
                m_Horizontals[i].startColor = pulsed;
                m_Horizontals[i].endColor = pulsed;
            }
        }
    }
}
