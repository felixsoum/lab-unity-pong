using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    [CreateAssetMenu(menuName = "PaddleBall/Neon Palette", fileName = "NeonPalette")]
    public class NeonPaletteSO : ScriptableObject
    {
        [Header("Core HDR Colors")]
        [ColorUsage(true, true)] public Color Ball = new Color(0f, 3f, 3f, 1f);
        [ColorUsage(true, true)] public Color BallFast = new Color(3f, 3f, 3f, 1f);
        [ColorUsage(true, true)] public Color Player1 = new Color(0.5f, 1f, 2.5f, 1f);
        [ColorUsage(true, true)] public Color Player2 = new Color(2.5f, 0.25f, 2f, 1f);
        [ColorUsage(true, true)] public Color Wall = new Color(1f, 0.2f, 2f, 1f);
        [ColorUsage(true, true)] public Color Goal = new Color(0.3f, 0.1f, 0.8f, 1f);
        [ColorUsage(true, true)] public Color Grid = new Color(0.3f, 0.1f, 0.9f, 1f);

        [Header("Background")]
        public Color CameraBackground = new Color(0.02f, 0.02f, 0.04f, 1f);
    }
}
