using Bovender.UserSettings;
using Bovender.Versioning;
using System;
using System.IO;
using YamlDotNet.Serialization;

namespace Bovender.UnitTests.UserSettings
{
    class UserSettings : UserSettingsBase
    {
        public static UserSettings FromFileOrDefault()
        {
            return FromFileOrDefault<UserSettings>(SettingsFileName);
        }

        public static string SettingsFileName
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), "bovender-test.yaml");
            }
        }

        public static string TestVersionString
        {
            get
            {
                return "1.2.3";
            }
        }

        public SemanticVersion LastVersionSeen
        {
            get
            {
                return _lastVersionSeen != null ? _lastVersionSeen
                    : new SemanticVersion("0.0.0");
            }
            set
            {
                _lastVersionSeen = value;
            }
        }

        private SemanticVersion _lastVersionSeen;

    }
}
