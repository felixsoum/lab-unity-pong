using System.Collections.Generic;
using UnityEngine;

namespace GameSystemsCookbook.Demos.PaddleBall
{
    public class PowerUpContext
    {
        public Ball MainBall;
        public Paddle Paddle1;
        public Paddle Paddle2;
        public GameDataSO GameData;
        public MonoBehaviour CoroutineRunner;
        public Ball BallPrefab;
        public Transform BallParent;
        public readonly List<Ball> ExtraBalls = new List<Ball>();

        public Paddle LastHitPaddle => MainBall != null ? MainBall.LastHitter : null;
    }
}
