/* SemanticVersion.cs
 * part of Bovender
 * 
 * Copyright 2014-2018 Daniel Kraus
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Bovender.Versioning
{
    #region Prerelease taxonomy enumeration

    public enum Prerelease
    {
        Alpha = -4,
        Beta = -3,
        RC = -2,
        Numeric = -1,
        None = 0,
    }

    #endregion

    /// <summary>
    /// Class that handles semantic versioning.
    /// </summary>
    public class SemanticVersion : Object, IComparable
    {
        #region Public properties

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public Prerelease Prerelease { get; set; }
        public int PreMajor { get; set; }
        public int PreMinor { get; set; }
        public int PrePatch { get; set; }
        public string Build { get; set; }

        #endregion

        #region Private fields

        private string _version;

        #endregion

        #region Constructor
        
        public SemanticVersion() { }

        /// <summary>
        /// Instantiates the class from a given version string.
        /// </summary>
        /// <param name="version">String that complies with semantic versioning rules.</param>
        public SemanticVersion(string version)
        {
            ParseString(version);
        }

        /// <summary>
        /// Creates an instance with
        /// the current version information, which must be contained in a file called
        /// "VERSION" that is built as an embedded resource. The first embedded "RESOURCE"
        /// file that is found in all the namespaces of <paramref name="assembly"/> will
        /// be used.
        /// </summary>
        /// <param name="assembly">Assembly that contains the VERSION file.</param>
        /// <returns>Instance of Version</returns>
        public SemanticVersion(Assembly assembly)
        {
            var versionFile = from resources in assembly.GetManifestResourceNames()
                              where resources.EndsWith(".VERSION")
                              select resources;
            Stream stream = assembly.GetManifestResourceStream(versionFile.First());
            StreamReader text = new StreamReader(stream);
            ParseString(text.ReadLine());
        }

        #endregion

        #region Operators

        public static bool operator <(SemanticVersion lower, SemanticVersion higher)
        {
            return (lower.CompareTo(higher) < 0);
        }

        public static bool operator >(SemanticVersion higher, SemanticVersion lower)
        {
            return (lower.CompareTo(higher) < 0);
        }

        public static bool operator <=(SemanticVersion lower, SemanticVersion higher)
        {
            return (lower.CompareTo(higher) <= 0);
        }

        public static bool operator >=(SemanticVersion higher, SemanticVersion lower)
        {
            return (lower.CompareTo(higher) <= 0);
        }

        public static bool operator ==(SemanticVersion v1, SemanticVersion v2)
        {
            try
            {
                return (v1.Equals(v2));
            }
            catch (NullReferenceException)
            {
                return (object)v2 == null;
            }
        }

        public static bool operator !=(SemanticVersion v1, SemanticVersion v2)
        {
            try
            {
                return (!v1.Equals(v2));
            }
            catch (NullReferenceException)
            {
                return (object)v2 != null;
            }
        }

        #endregion

        #region Comparators

        public int CompareTo(object obj)
        {
            SemanticVersion other = obj as SemanticVersion;
            if (this.Major < other.Major)
            {
                return -1;
            }
            else if (this.Major > other.Major)
            {
                return 1;
            }
            else // both major versions are the same, compare minor version
            {
                if (this.Minor < other.Minor)
                {
                    return -1;
                }
                else if (this.Minor > other.Minor)
                {
                    return 1;
                }
                else // major and minor are same, compare patch
                {
                    if (this.Patch < other.Patch)
                    {
                        return -1;
                    }
                    else if (this.Patch > other.Patch)
                    {
                        return 1;
                    }
                    else // major, minor, and path are same, compare pre-release
                    {
                        if (this.Prerelease < other.Prerelease)
                        {
                            return -1;
                        }
                        else if (this.Prerelease > other.Prerelease)
                        {
                            return 1;
                        }
                        else // prerelease type is same (alpha/beta/etc.)
                        {
                            if (this.Prerelease == Versioning.Prerelease.Numeric)
                            {
                                if (this.PreMajor < other.PreMajor)
                                {
                                    return -1;
                                }
                                else if (this.PreMajor > other.PreMajor)
                                {
                                    return 1;
                                }
                                else
                                {
                                    if (this.PreMinor < other.PreMinor)
                                    {
                                        return -1;
                                    }
                                    else if (this.PreMinor > other.PreMinor)
                                    {
                                        return 1;
                                    }
                                    else
                                    {
                                        if (this.PrePatch < other.PrePatch)
                                        {
                                            return -1;
                                        }
                                        else if (this.PrePatch> other.PrePatch)
                                        {
                                            return 1;
                                        }
                                        else
                                        {
                                            return 0;
                                        }
                                    }
                                }
                            }
                            else // prerelease type same, not numeric
                            {
                                if (this.PrePatch < other.PrePatch)
                                {
                                    return -1;
                                }
                                else if (this.PrePatch > other.PrePatch)
                                {
                                    return 1;
                                }
                                else
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                return (this.CompareTo(obj) == 0);
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region Object overrides

        /// <summary>
        /// Returns the full version string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            BuildString();
            return _version;
        }

        public override int GetHashCode()
        {
            BuildString();
            return _version.GetHashCode();
        }

        #endregion

        #region Internal logic

        /// <summary>
        /// Parses a string that complies with semantic versioning, V. 2.
        /// </summary>
        /// <param name="s">Semantic version string.</param>
        protected void ParseString(string s)
        {
            Regex r = new Regex(
                @"(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)" +
                @"(-(?<pre>((?<preMajor>\d+)\.(?<preMinor>\d+)\.|"+
                @"((?<alpha>alpha)|(?<beta>beta)|(?<rc>rc))\.)(?<prePatch>\d+)))?" +
                @"(\+(?<build>[a-zA-Z0-9]+))?");
            Match m = r.Match(s);

            if (!m.Success)
            {
                throw new InvalidVersionStringException(s);
            };

            _version = s;
            Major = Convert.ToInt32(m.Groups["major"].Value);
            Minor = Convert.ToInt32(m.Groups["minor"].Value);
            Patch = Convert.ToInt32(m.Groups["patch"].Value);

            if (m.Groups["pre"].Success)
            {
                if (m.Groups["alpha"].Success)
                {
                    Prerelease = Versioning.Prerelease.Alpha;
                }
                else if (m.Groups["beta"].Success)
                {
                    Prerelease = Versioning.Prerelease.Beta;
                }
                else if (m.Groups["rc"].Success)
                {
                    Prerelease = Versioning.Prerelease.RC;
                }
                else
                {
                    Prerelease = Versioning.Prerelease.Numeric;
                    PreMajor = Convert.ToInt32(m.Groups["preMajor"].Value);
                    PreMinor = Convert.ToInt32(m.Groups["preMinor"].Value);
                }
            }
            else
            {
                Prerelease = Versioning.Prerelease.None;
            }
            if (m.Groups["prePatch"].Success)
            {
                PrePatch = Convert.ToInt32(m.Groups["prePatch"].Value);
            };

            Build = m.Groups["build"].Value;
        }

        protected void BuildString()
        {
            string s = String.Format("{0}.{1}.{2}", Major, Minor, Patch);
            if (Prerelease != Prerelease.None)
            {
                if (Prerelease == Prerelease.Numeric)
                {
                    s += String.Format("-{0}.{1}.{2}", PreMajor, PreMinor, PrePatch);
                }
                else
                {
                    s += String.Format("-{0}.{1}", Prerelease.ToString().ToLower(), PrePatch);
                }
            }
            if (!string.IsNullOrWhiteSpace(Build))
            {
                s += String.Format("+{0}", Build);
            }
            _version = s;
        }

        #endregion
    }
}
