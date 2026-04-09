using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using GameSystemsCookbook;
using GameSystemsCookbook.Demos.PaddleBall;

namespace PaddleBall.Tests
{
    public class ScoreObjectiveTests
    {
        private ScoreObjectiveSO m_Objective;
        private ScoreListEventChannelSO m_ScoreUpdatedChannel;
        private PlayerScoreEventChannelSO m_TargetReachedChannel;
        private VoidEventChannelSO m_ObjectiveCompletedChannel;

        [SetUp]
        public void SetUp()
        {
            // Create channels
            m_ScoreUpdatedChannel = ScriptableObject.CreateInstance<ScoreListEventChannelSO>();
            m_TargetReachedChannel = ScriptableObject.CreateInstance<PlayerScoreEventChannelSO>();
            m_ObjectiveCompletedChannel = ScriptableObject.CreateInstance<VoidEventChannelSO>();

            // Suppress NullRefChecker during CreateInstance (fields aren't assigned yet)
            NullRefChecker.SuppressValidation = true;
            m_Objective = ScriptableObject.CreateInstance<ScoreObjectiveSO>();
            NullRefChecker.SuppressValidation = false;

            // Assign private serialized fields via reflection
            SetField(m_Objective, "m_ScoreManagerUpdated", m_ScoreUpdatedChannel);
            SetField(m_Objective, "m_TargetScoreReached", m_TargetReachedChannel);
            SetField(m_Objective, "m_TargetScore", 3);
            SetField(m_Objective, "m_ObjectiveCompleted", m_ObjectiveCompletedChannel);
            SetField(m_Objective, "m_Description", "Test objective");

            // Re-trigger OnEnable so it subscribes with the now-assigned fields
            InvokePrivateMethod(m_Objective, "OnEnable");
        }

        [TearDown]
        public void TearDown()
        {
            NullRefChecker.SuppressValidation = false;
            Object.DestroyImmediate(m_Objective);
            Object.DestroyImmediate(m_ScoreUpdatedChannel);
            Object.DestroyImmediate(m_TargetReachedChannel);
            Object.DestroyImmediate(m_ObjectiveCompletedChannel);
        }

        [Test]
        public void ScoreObjective_BelowTarget_DoesNotComplete()
        {
            var score = new Score();
            score.IncrementScore();
            score.IncrementScore();

            var playerScores = new List<PlayerScore>
            {
                new PlayerScore { score = score }
            };

            m_ScoreUpdatedChannel.RaiseEvent(playerScores);

            Assert.IsFalse(m_Objective.IsCompleted);
        }

        [Test]
        public void ScoreObjective_ReachesTarget_RaisesTargetScoreReached()
        {
            var score = new Score();
            score.IncrementScore();
            score.IncrementScore();
            score.IncrementScore();

            bool eventRaised = false;
            m_TargetReachedChannel.OnEventRaised += (_) => eventRaised = true;

            var playerScores = new List<PlayerScore>
            {
                new PlayerScore { score = score }
            };

            m_ScoreUpdatedChannel.RaiseEvent(playerScores);

            Assert.IsTrue(eventRaised);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var type = target.GetType();
            FieldInfo field = null;

            while (type != null && field == null)
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                type = type.BaseType;
            }

            Assert.IsNotNull(field, $"Field '{fieldName}' not found on {target.GetType().Name}");
            field.SetValue(target, value);
        }

        private static void InvokePrivateMethod(object target, string methodName)
        {
            var type = target.GetType();
            MethodInfo method = null;

            while (type != null && method == null)
            {
                method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                type = type.BaseType;
            }

            Assert.IsNotNull(method, $"Method '{methodName}' not found on {target.GetType().Name}");
            method.Invoke(target, null);
        }
    }
}
