using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using Bovender.UserSettings;

namespace Bovender.UnitTests.UserSettings
{
    [TestFixture]
    class UserSettingsTest
    {
        [Test]
        public void WriteSettings()
        {
            UserSettings o = new UserSettings();
            o.Save();
            string yaml = File.ReadAllText(UserSettings.SettingsFileName);
            Assert.IsTrue(yaml.Contains("LastVersionSeen:"));
        }

        [Test]
        public void LoadSettings()
        {
            string fn = UserSettings.SettingsFileName;
            string yaml = "# Comment\nLastVersionSeen:\n  Major: 3\n  Minor: 2\n  Patch: 1\n";
            File.WriteAllText(fn, yaml);
            UserSettings o = UserSettings.FromFileOrDefault();
            Assert.IsNull(o.Exception);
            Assert.IsTrue(o.WasFromFile);
            Assert.AreEqual("3.2.1", o.LastVersionSeen.ToString());
        }

        [Test]
        public void LoadCorruptedSettings()
        {
            string fn = UserSettings.SettingsFileName;
            string yaml = "LastVersionSeen:\n  Major: this_should_be_a_number\n  Minor: 2\n  Patch: 1\n";
            File.WriteAllText(fn, yaml);
            UserSettings o = UserSettings.FromFileOrDefault();
            Assert.IsNotNull(o.Exception);
            Assert.IsInstanceOf<YamlDotNet.Core.YamlException>(o.Exception);
            Assert.IsFalse(o.WasFromFile);
        }
    }
}
