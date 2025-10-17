using QuanLib.Core;
using QuanLib.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public class VersionList : ISingleton<VersionList, InstantiateArgs>
    {
        private VersionList(IList<MinecraftVersion.Model> models)
        {
            ArgumentNullException.ThrowIfNull(models, nameof(models));

            List<ReleaseVersion> releaseVersionsList = [];
            List<ReleaseCandidateVersion> releaseCandidateVersionsList = [];
            List<PreReleaseVersion> preReleaseVersionsList = [];
            List<OldPreReleaseVersion> oldPreReleaseVersions = [];
            List<SnapshotVersion> snapshotVersionsList = [];
            List<AprilFoolsDayVersion> aprilFoolsDayVersionsList = [];
            List<AncientVersion> oldBetaVersionsList = [];
            List<AncientVersion> oldAlphaVersionsList = [];
            List<AncientVersion> infdevVersionsList = [];
            List<AncientVersion> indevVersionsList = [];
            List<AncientVersion> classicVersionsList = [];
            List<AncientVersion> preClassicVersionsList = [];

            foreach (var model in models)
            {
                VersionType type = Enum.Parse<VersionType>(model.Type);
                DateTime time = new(model.ReleaseTime, DateTimeKind.Utc);
                switch (type)
                {
                    case VersionType.Release:
                        releaseVersionsList.Add(new ReleaseVersion(model.Version, time));
                        break;
                    case VersionType.ReleaseCandidate:
                        releaseCandidateVersionsList.Add(new ReleaseCandidateVersion(model.Version, time));
                        break;
                    case VersionType.PreRelease:
                        preReleaseVersionsList.Add(new PreReleaseVersion(model.Version, time));
                        break;
                    case VersionType.OldPreRelease:
                        oldPreReleaseVersions.Add(new OldPreReleaseVersion(model.Version, time));
                        break;
                    case VersionType.Snapshot:
                        snapshotVersionsList.Add(new SnapshotVersion(model.Version, time));
                        break;
                    case VersionType.AprilFoolsDay:
                        aprilFoolsDayVersionsList.Add(new AprilFoolsDayVersion(model.Version, time));
                        break;
                    case VersionType.OldBeta:
                        oldBetaVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    case VersionType.OldAlpha:
                        oldAlphaVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    case VersionType.Infdev:
                        infdevVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    case VersionType.Indev:
                        indevVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    case VersionType.Classic:
                        classicVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    case VersionType.PreClassic:
                        preClassicVersionsList.Add(new AncientVersion(model.Version, type, time));
                        break;
                    default:
                        throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(VersionType));
                }
            }

            ReleaseVersions = releaseVersionsList.AsReadOnly();
            ReleaseCandidateVersions = releaseCandidateVersionsList.AsReadOnly();
            PreReleaseVersions = preReleaseVersionsList.AsReadOnly();
            OldPreReleaseVersions = oldPreReleaseVersions.AsReadOnly();
            SnapshotVersions = snapshotVersionsList.AsReadOnly();
            AprilFoolsDayVersions = aprilFoolsDayVersionsList.AsReadOnly();
            OldBetaVersions = oldBetaVersionsList.AsReadOnly();
            OldAlphaVersions = oldAlphaVersionsList.AsReadOnly();
            InfdevVersions = infdevVersionsList.AsReadOnly();
            IndevVersions = indevVersionsList.AsReadOnly();
            ClassicVersions = classicVersionsList.AsReadOnly();
            PreClassicVersions = preClassicVersionsList.AsReadOnly();

            List<MinecraftVersion> allVersions = [];
            allVersions.AddRange(releaseVersionsList);
            allVersions.AddRange(releaseCandidateVersionsList);
            allVersions.AddRange(preReleaseVersionsList);
            allVersions.AddRange(oldPreReleaseVersions);
            allVersions.AddRange(snapshotVersionsList);
            allVersions.AddRange(aprilFoolsDayVersionsList);
            allVersions.AddRange(oldBetaVersionsList);
            allVersions.AddRange(oldAlphaVersionsList);
            allVersions.AddRange(infdevVersionsList);
            allVersions.AddRange(indevVersionsList);
            allVersions.AddRange(classicVersionsList);
            allVersions.AddRange(preClassicVersionsList);
            AllVersions = allVersions.OrderBy(version => version.ReleaseTime).ToArray().AsReadOnly();
        }

        private static readonly object _slock = new();

        public static bool IsInstanceLoaded => _Instance is not null;

        public static VersionList Instance => _Instance ?? throw new InvalidOperationException("实例未加载");
        private static VersionList? _Instance;

        public ReadOnlyCollection<ReleaseVersion> ReleaseVersions { get; }

        public ReadOnlyCollection<ReleaseCandidateVersion> ReleaseCandidateVersions { get; }

        public ReadOnlyCollection<PreReleaseVersion> PreReleaseVersions { get; }

        public ReadOnlyCollection<OldPreReleaseVersion> OldPreReleaseVersions { get; }

        public ReadOnlyCollection<SnapshotVersion> SnapshotVersions { get; }

        public ReadOnlyCollection<AprilFoolsDayVersion> AprilFoolsDayVersions { get; }

        public ReadOnlyCollection<AncientVersion> OldBetaVersions { get; }

        public ReadOnlyCollection<AncientVersion> OldAlphaVersions { get; }

        public ReadOnlyCollection<AncientVersion> InfdevVersions { get; }

        public ReadOnlyCollection<AncientVersion> IndevVersions { get; }

        public ReadOnlyCollection<AncientVersion> ClassicVersions { get; }

        public ReadOnlyCollection<AncientVersion> PreClassicVersions { get; }

        public ReadOnlyCollection<MinecraftVersion> AllVersions { get; }

        public MinecraftVersion GetVersion(string versionNumber)
        {
            ArgumentNullException.ThrowIfNull(versionNumber, nameof(versionNumber));

            return AllVersions.FirstOrDefault(version => version.VersionNumber == versionNumber) ?? throw new ArgumentException($"找不到游戏版本“{versionNumber}”", nameof(versionNumber));
        }

        public bool TryGetVersion(string versionNumber, [MaybeNullWhen(false)]out MinecraftVersion result)
        {
            ArgumentNullException.ThrowIfNull(versionNumber, nameof(versionNumber));

            MinecraftVersion? version = AllVersions.FirstOrDefault(version => version.VersionNumber == versionNumber);
            if (version is null)
            {
                result = null;
                return false;
            }
            else
            {
                result = version;
                return true;
            }
        }

        public static VersionList LoadInstance(InstantiateArgs args)
        {
            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".SystemResource.version_list.json") ?? throw new InvalidOperationException();
                string text = stream.ReadAllText();

                List<MinecraftVersion.Model> models = JsonSerializer.Deserialize<List<MinecraftVersion.Model>>(text) ?? throw new FormatException();
                _Instance = new VersionList(models);
                return _Instance;
            }
        }
    }
}
