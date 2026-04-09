using NUnit.Framework;
using UnityEngine;
using GameSystemsCookbook;

namespace PaddleBall.Tests
{
    public class EventChannelTests
    {
        [Test]
        public void VoidEventChannel_RaiseEvent_NotifiesSubscriber()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannelSO>();
            bool eventReceived = false;
            channel.OnEventRaised += () => eventReceived = true;

            channel.RaiseEvent();

            Assert.IsTrue(eventReceived);
        }

        [Test]
        public void VoidEventChannel_RaiseEvent_NoSubscribers_DoesNotThrow()
        {
            var channel = ScriptableObject.CreateInstance<VoidEventChannelSO>();

            Assert.DoesNotThrow(() => channel.RaiseEvent());
        }

        [Test]
        public void IntEventChannel_RaiseEvent_PassesCorrectValue()
        {
            var channel = ScriptableObject.CreateInstance<IntEventChannelSO>();
            int receivedValue = 0;
            channel.OnEventRaised += (value) => receivedValue = value;

            channel.RaiseEvent(42);

            Assert.AreEqual(42, receivedValue);
        }
    }
}
