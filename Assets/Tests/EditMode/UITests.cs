using BelowUs;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class UITests
    {
        [Test]
        public void ImageFillSetterPasses()
        {
            FloatVariable health = ScriptableObject.CreateInstance<FloatVariable>();
            FloatVariable maxHealth = ScriptableObject.CreateInstance<FloatVariable>();
            FloatReference minimum = new FloatReference(0);
            FloatReference healthRef = new FloatReference(health);
            FloatReference maxHealthRef = new FloatReference(maxHealth);

            GameObject imageHolder = new GameObject("TestGameObject");
            ImageFillSetter imgFSetter = imageHolder.AddComponent<ImageFillSetter>();
            float expectedFill;

            //Simple test with half hp
            maxHealth.SetValue(100);
            health.SetValue(maxHealth.Value / 2);
            expectedFill = 0.5f;
            Assert.AreEqual(imgFSetter.GetFillAmount(minimum, maxHealthRef, healthRef), expectedFill);

            //Testing full hp
            maxHealth.SetValue(50);
            expectedFill = 1;
            Assert.AreEqual(imgFSetter.GetFillAmount(minimum, maxHealthRef, healthRef), expectedFill);

            //Testing zero hp
            health.SetValue(0);
            expectedFill = 0;
            Assert.AreEqual(imgFSetter.GetFillAmount(minimum, maxHealthRef, healthRef), expectedFill);

            //Testing with non zero minimum
            health.SetValue(37.5f);
            minimum.ConstantValue = 25;
            expectedFill = 0.5f;
            Assert.AreEqual(imgFSetter.GetFillAmount(minimum, maxHealthRef, healthRef), expectedFill);
        }
    }
}