using com.github.fredjk_gh.ObservatoryPluginAutoUpdater;
using com.github.fredjk_gh.ObservatoryPlugins.Tests.AutoUpdater;
using Observatory.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace com.github.fredjk_gh.ObservatoryPlugins.Tests
{
    [TestClass]
    public class AutoUpdaterTests
    {
        private AutoUpdaterSettings _settings;
        private Common.TestCore _core;
        private FixedResponseHttpClient _httpClient;
        private TestableAutoUpdater sutUpdater;
        private DirectoryInfo _pluginsDir;

        [TestInitialize]
        public void Setup()
        {
            _core = new Common.TestCore();
            _httpClient = new();

            sutUpdater = new TestableAutoUpdater();
            sutUpdater.PluginFolderPath = "V:\\elite-dangerous\\GitHub\\ObsCoreTesting\\plugins";
            sutUpdater.Settings = _settings;
            sutUpdater.Core = _core;
            // Tests should provide LocalPluginVersions.

            _pluginsDir = new DirectoryInfo(sutUpdater.PluginFolderPath);
            foreach(var f in _pluginsDir.GetFiles("*.eop"))
            {
                f.Delete();
            }
        }

        [TestMethod]
        public void TestLiveCurrentReleasesJson()
        {
            string jsonText = File.ReadAllText("V:\\elite-dangerous\\GitHub\\ObservatoryPlugins\\CurrentReleases.json");
            var jsonDocOptions = new JsonDocumentOptions() { AllowTrailingCommas = true };
            var jsonSerializeOptions = new JsonSerializerOptions() { AllowTrailingCommas = true };
            var json = JsonDocument.Parse(jsonText, jsonDocOptions);
            var jsonObj = json.Deserialize<List<PluginVersion>>(jsonSerializeOptions);

            Assert.IsNotNull(jsonObj);
            Assert.AreNotEqual(0, jsonObj.Count);

            foreach(PluginVersion v in jsonObj)
            {
                Assert.IsFalse(string.IsNullOrEmpty(v.PluginName));
                if (v.Production != null)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(v.Production.Version));
                    if (v.Production.Version != VersionDetail.NO_VERSION) Assert.IsFalse(string.IsNullOrEmpty(v.Production.DownloadURL));
                }
                if (v.Beta != null)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(v.Beta.Version));
                    if (v.Beta.Version != VersionDetail.NO_VERSION) Assert.IsFalse(string.IsNullOrEmpty(v.Beta.DownloadURL));
                }
            }
        }

        [TestMethod]
        public void TestVersionDetail_Compare()
        {
            int[] noVersion = VersionDetail.ParseVersion(VersionDetail.NO_VERSION);
            int[] version1 = VersionDetail.ParseVersion("0.1.2.3");
            int[] version2 = VersionDetail.ParseVersion("0.2.4.6");

            Assert.AreEqual(-1, VersionDetail.Compare(version1, version2));
            Assert.AreEqual(1, VersionDetail.Compare(version2, version1));
            Assert.AreEqual(0, VersionDetail.Compare(version1, version1));

            Assert.AreEqual(-1, VersionDetail.Compare(noVersion, version1));
            Assert.AreEqual(1, VersionDetail.Compare(version2, noVersion));
            Assert.AreEqual(0, VersionDetail.Compare(noVersion, noVersion));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersion(bool useBeta)
        {
            string latestVersion = "0.2.4.6";
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = latestVersion },
                Beta = new() { Version = latestVersion }
            };
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new() { Version = "0.1.2.3" }
            };

            var selected = PluginVersion.SelectVersion(local, latest, useBeta, _core.Version);
            Assert.IsNotNull(selected);
            Assert.AreEqual((useBeta ? "Beta" : "Production"), selected.Name);
            Assert.AreEqual(latestVersion, selected.Latest.Version);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersionNewerThanZeros(bool useBeta)
        {
            string latestVersion = "0.2.4.6";
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = latestVersion },
                Beta = new() { Version = latestVersion }
            };
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = VersionDetail.NO_VERSION },
                Beta = new() { Version = VersionDetail.NO_VERSION }
            };

            var selected = PluginVersion.SelectVersion(local, latest, useBeta, _core.Version);
            Assert.IsNotNull(selected);
            Assert.AreEqual((useBeta ? "Beta" : "Production"), selected.Name);
            Assert.AreEqual(latestVersion, selected.Latest.Version);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersion_NoneSelectedOlder(bool useBeta)
        {
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.2.4.6" },
                Beta = new() { Version = "0.2.4.6" }
            };
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new() { Version = "0.1.2.3" }
            };

            Assert.IsNull(PluginVersion.SelectVersion(local, latest, useBeta, _core.Version));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersion_NoLatestVersion(bool useBeta)
        {
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.2.4.6" },
                Beta = new() { Version = "0.2.4.6" }
            };
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = VersionDetail.NO_VERSION },
                Beta = new() { Version = VersionDetail.NO_VERSION }
            };

            Assert.IsNull(PluginVersion.SelectVersion(local, latest, useBeta, _core.Version));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersion_SameVersion(bool useBeta)
        {
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.2.4.6" },
                Beta = new() { Version = "0.2.4.6" }
            };
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = "0.2.4.6" },
                Beta = new() { Version = "0.2.4.6" }
            };

            Assert.IsNull(PluginVersion.SelectVersion(local, latest, useBeta, _core.Version));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestPluginVersion_SelectVersion_BetaAbsentButEnabledUsesMainVersion(bool useBeta)
        {
            string latestVersion = "0.2.4.6";
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = latestVersion },
            };
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
            };

            var selected = PluginVersion.SelectVersion(local, latest, useBeta, _core.Version);
            Assert.IsNotNull(selected);
            Assert.AreEqual("Production", selected.Name);
            Assert.AreEqual(latestVersion, selected.Latest.Version);
        }

        [TestMethod]
        public void TestPluginVersion_SelectVersion_BetaIncompatibleWithCore()
        {
            string latestVersion = "0.2.4.6";
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new()
                {
                    Version = latestVersion,
                    MinRequiredCoreVersion = "9.9.9.9"
                }
            };
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new() { Version = "0.1.2.3" },
            };

            var selected = PluginVersion.SelectVersion(local, latest, true, _core.Version);
            Assert.IsNull(selected);
        }

        [TestMethod]
        public void TestPluginVersion_SelectVersion_OnlyBetaNewerRespectsAllowedUsage()
        {
            string latestVersion = "0.2.4.6";
            PluginVersion local = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new() { Version = "0.1.2.3" }
            };
            PluginVersion latest = new PluginVersion()
            {
                Production = new() { Version = "0.1.2.3" },
                Beta = new() { Version = latestVersion }
            };

            // Beta allowed
            var selected = PluginVersion.SelectVersion(local, latest, true, _core.Version);
            Assert.IsNotNull(selected);
            Assert.AreEqual("Beta", selected.Name);
            Assert.AreEqual(latestVersion, selected.Latest.Version);

            // Beta disallowed -- no new version.
            selected = PluginVersion.SelectVersion(local, latest, false, _core.Version);
            Assert.IsNull(selected);
        }

        [TestMethod]
        public void TestCheckForUpdates_NoFolderPath()
        {
            sutUpdater.PluginFolderPath = "";

            Assert.IsFalse(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(1, _core.Messages.Count);
            Assert.IsTrue(_core.Messages[0].Contains("PluginFolder"));

            sutUpdater.LogMonitorStateChanged(new()
            {
                NewState = LogMonitorState.Realtime
            });
            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.First().Value.Title == "Configuration problem");
        }

        [TestMethod]
        public void TestCheckForUpdates_EmptyLatestVersions()
        {
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, new());
            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                        DownloadURL = "file:///local/TestPlugin1-v0.24.2.123.eop"
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsFalse(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);
        }

        [TestMethod]
        public void TestCheckForUpdates_NothingToUpdate()
        {
            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                        DownloadURL = "file:///local/TestPlugin1-v0.24.2.123.eop"
                    }
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                        DownloadURL = "file:///local/TestPlugin1-v0.24.2.123.eop"
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);
            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);
            Assert.AreEqual(0, _httpClient.GetStreamCalls);
        }

        [TestMethod]
        public void TestCheckForUpdates_UnknownLocalPlugin()
        {
            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                        DownloadURL = "file:///local/TestPlugin1-v0.24.2.123.eop"
                    },
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryBioInsights",
                    Production = new()
                    {
                        Version = "0.1.2",
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);
            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);
            Assert.AreEqual(0, _httpClient.GetStreamCalls);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestCheckForUpdates_MissingDownloadUrl(bool useBeta)
        {
            string updatedVersion = "0.24.3.1";
            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = updatedVersion,
                    },
                    Beta = new()
                    {
                        Version = updatedVersion,
                    },
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);
            Assert.AreEqual(0, _httpClient.GetStreamCalls);
        }

        [TestMethod]
        public void TestCheckForUpdates_ProductionUpdateAvailable()
        {
            string updatedVersion = "0.24.3.1";
            string filename = $"ObservatoryPluginAutoUpdater-v{updatedVersion}.eop";
            string downloadUrl = $"file:///local/{filename}";

            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() { 
                    PluginName = "ObservatoryPluginAutoUpdater", 
                    Production = new()
                    {
                        Version = updatedVersion,
                        DownloadURL = downloadUrl
                    }
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            _httpClient.StreamResponses.Add(downloadUrl, MakeStream(updatedVersion));

            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);

            sutUpdater.LogMonitorStateChanged(new()
            {
                NewState = LogMonitorState.Realtime
            });
            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.First().Value.Title.Contains("Pending"));

            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);
            Assert.AreEqual(1, _httpClient.GetStreamCalls);
            
            var files = _pluginsDir.GetFiles("*.eop");
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(filename, files[0].Name);
            Assert.IsTrue(files[0].Length > 0);
        }

        [TestMethod]
        public void TestCheckForUpdates_ProductionUpdateAvailableIncompatibleBeta()
        {
            string updatedVersion = "0.24.3.1";
            string incompatibleVersion = "0.24.5.7";
            string filename = $"ObservatoryPluginAutoUpdater-v{updatedVersion}.eop";
            string downloadUrl = $"file:///local/{filename}";
            string incompatibleDownloadUrl = $"file://local/ObservatoryPluginAutoUpdater-v{incompatibleVersion}.eop";

            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = updatedVersion,
                        DownloadURL = downloadUrl
                    },
                    Beta = new()
                    {
                        Version = incompatibleVersion,
                        DownloadURL = incompatibleDownloadUrl,
                        MinRequiredCoreVersion = "9.9.9.9",
                    }
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            _httpClient.StreamResponses.Add(downloadUrl, MakeStream(updatedVersion));

            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = "0.24.2.123",
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);

            sutUpdater.LogMonitorStateChanged(new()
            {
                NewState = LogMonitorState.Realtime
            });
            Assert.AreEqual(1, _core.Notifications.Count);
            Assert.IsTrue(_core.Notifications.First().Value.Title.Contains("Pending"));

            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);
            Assert.AreEqual(1, _httpClient.GetStreamCalls);

            var files = _pluginsDir.GetFiles("*.eop");
            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(filename, files[0].Name);
            Assert.IsTrue(files[0].Length > 0);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void TestCheckForUpdates_BetaUpdateAvailable(bool useBeta)
        {
            string oldVersion = "0.24.2.123";
            string updatedVersion = "0.24.3.1";
            string betaFilename = $"ObservatoryPluginAutoUpdater-v{updatedVersion}-beta.eop";
            string downloadUrl = $"file:///local/ObservatoryPluginAutoUpdater-v{oldVersion}.eop";
            string betaDownloadUrl = $"file:///local/{betaFilename}";

            List<PluginVersion> latestVersions = new()
            {
                new PluginVersion() {
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = oldVersion,
                        DownloadURL = downloadUrl,
                    },
                    Beta = new()
                    {
                        Version = updatedVersion,
                        DownloadURL = betaDownloadUrl,
                    }
                }
            };
            _httpClient.JsonResponses.Add(FredJKsPluginAutoUpdater.CURRENT_RELEASES_URL, latestVersions);
            if (useBeta) _httpClient.StreamResponses.Add(betaDownloadUrl, MakeStream(updatedVersion));

            List<PluginVersion> localVersions = new()
            {
                new PluginVersion() { 
                    PluginName = "ObservatoryPluginAutoUpdater",
                    Production = new()
                    {
                        Version = oldVersion,
                    },
                    Beta = new()
                    {
                        Version = oldVersion,
                    }
                }
            };
            sutUpdater.LocalPluginVersions = localVersions;
            _settings.UseBeta = useBeta;

            Assert.IsTrue(sutUpdater.CheckForUpdates(_httpClient));
            Assert.AreEqual(0, _core.Messages.Count);
            Assert.AreEqual(1, _httpClient.GetLatestVersionsCalls);

            sutUpdater.LogMonitorStateChanged(new()
            {
                NewState = LogMonitorState.Realtime
            });

            var files = _pluginsDir.GetFiles("*.eop");
            if (useBeta)
            {
                Assert.AreEqual(1, _core.Notifications.Count);
                Assert.IsTrue(_core.Notifications.First().Value.Title.Contains("Pending"));
                Assert.AreEqual(1, _httpClient.GetStreamCalls);
                Assert.AreEqual(1, files.Length);
                Assert.AreEqual(betaFilename, files[0].Name);
                Assert.IsTrue(files[0].Length > 0);
            }
            else
            {
                Assert.AreEqual(0, _core.Notifications.Count);
                Assert.AreEqual(0, _httpClient.GetStreamCalls);
                Assert.AreEqual(0, files.Length);
            }
        }

        private Stream MakeStream(string content)
        { 
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
    }
}
