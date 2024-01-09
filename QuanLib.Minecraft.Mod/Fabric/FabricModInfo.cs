using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class FabricModInfo : ModInfo, IDataModelOwner<FabricModInfo, FabricModInfo.DataModel>
    {
        public FabricModInfo(DataModel model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            SchemaVersion = model.schemaVersion;
            ModId = model.id;
            ModName = model.name;
            Description = model.description;
            Version = model.version;
            LogoFile = model.icon;
            License = model.license;
            Authors = model.authors.Select(s => PersonInfo.FromDataModel(s)).ToArray().AsReadOnly();
            Contributors = model.contributors.Select(s => PersonInfo.FromDataModel(s)).ToArray().AsReadOnly();
            Contact = model.contact.AsReadOnly();
            Environment = model.environment;
            Entrypoints = model.entrypoints.ToDictionary(item => item.Key, item => item.Value.AsReadOnly()).AsReadOnly();
            Mixins = model.mixins.AsReadOnly();
            Jars = model.jars.AsReadOnly();
            AccessWidener = model.accessWidener;
            Provides = model.provides.AsReadOnly();
            LanguageAdapters = model.languageAdapters.AsReadOnly();

            List<ModDependency> dependencies = [];
            dependencies.AddRange(model.depends.Select(item => new ModDependency(ModDependencyKind.Depends, item.Key, item.Value)));
            dependencies.AddRange(model.recommends.Select(item => new ModDependency(ModDependencyKind.Recommends, item.Key, item.Value)));
            dependencies.AddRange(model.suggests.Select(item => new ModDependency(ModDependencyKind.Suggests, item.Key, item.Value)));
            dependencies.AddRange(model.conflicts.Select(item => new ModDependency(ModDependencyKind.Conflicts, item.Key, item.Value)));
            dependencies.AddRange(model.breaks.Select(item => new ModDependency(ModDependencyKind.Breaks, item.Key, item.Value)));
            Dependencies = dependencies.AsReadOnly();
        }

        public override ModLoader ModLoader => ModLoader.Fabric;

        public int SchemaVersion { get; }

        public override string ModId { get; }

        public override string ModName { get; }

        public override string Description { get; }

        public override string Version { get; }

        public override string LogoFile { get; }

        public string License { get; }

        public ReadOnlyCollection<PersonInfo> Authors { get; }

        public ReadOnlyCollection<PersonInfo> Contributors { get; }

        public ReadOnlyDictionary<string, string> Contact { get; }

        public string Environment { get; }

        public ReadOnlyCollection<ModDependency> Dependencies { get; }

        public ReadOnlyDictionary<string, ReadOnlyCollection<string>> Entrypoints { get; }

        public ReadOnlyCollection<MixinInfo> Mixins { get; }

        public ReadOnlyCollection<JarInfo> Jars { get; }

        public string AccessWidener { get; }

        public ReadOnlyCollection<string> Provides { get; }

        public ReadOnlyDictionary<string, string> LanguageAdapters { get; }

        public DataModel ToDataModel()
        {
            Dictionary<string, List<string>> depends = [];
            Dictionary<string, List<string>> recommends = [];
            Dictionary<string, List<string>> suggests = [];
            Dictionary<string, List<string>> conflicts = [];
            Dictionary<string, List<string>> breaks = [];
            foreach (ModDependency dependency in Dependencies)
            {
                Dictionary<string, List<string>> dictionary = dependency.Kind switch
                {
                    ModDependencyKind.Depends => depends,
                    ModDependencyKind.Recommends => recommends,
                    ModDependencyKind.Suggests => suggests,
                    ModDependencyKind.Conflicts => conflicts,
                    ModDependencyKind.Breaks => breaks,
                    _ => throw new InvalidOperationException(),
                };

                dictionary.Add(dependency.ModId, dependency.VersionRanges.ToList());
            }

            return new()
            {
                schemaVersion = SchemaVersion,
                id = ModId,
                name = ModName,
                description = Description,
                version = Version,
                icon = LogoFile,
                license = License,
                authors = Authors.Select(s => s.ToDataModel()).ToList(),
                contributors = Contributors.Select(s => s.ToDataModel()).ToList(),
                contact = Contact.ToDictionary(),
                environment = Environment,
                depends = depends,
                recommends = recommends,
                suggests = suggests,
                conflicts = conflicts,
                breaks = breaks,
                entrypoints = Entrypoints.ToDictionary(item => item.Key, item => item.Value.ToList()),
                mixins = Mixins.ToList(),
                jars = Jars.ToList(),
                accessWidener = AccessWidener,
                provides = Provides.ToList(),
                languageAdapters = LanguageAdapters.ToDictionary()
            };
        }

        public static FabricModInfo FromDataModel(DataModel model)
        {
            return new(model);
        }

        public class DataModel : IDataModel<DataModel>
        {
            public DataModel()
            {
                schemaVersion = 1;
                id = string.Empty;
                name = string.Empty;
                description = string.Empty;
                version = string.Empty;
                icon = string.Empty;
                license = string.Empty;
                authors = [];
                contributors = [];
                contact = [];
                environment = "*";
                depends = [];
                recommends = [];
                suggests = [];
                conflicts = [];
                breaks = [];
                entrypoints = [];
                mixins = [];
                jars = [];
                accessWidener = string.Empty;
                provides = [];
                languageAdapters = [];
            }

            public int schemaVersion { get; set; }

            public string id { get; set; }

            public string name { get; set; }

            public string description { get; set; }

            public string version { get; set; }

            public string icon { get; set; }

            public string license { get; set; }

            public List<PersonInfo.DataModel> authors { get; set; }

            public List<PersonInfo.DataModel> contributors { get; set; }

            public Dictionary<string, string> contact { get; set; }

            public string environment { get; set; }

            public Dictionary<string, List<string>> depends { get; set; }

            public Dictionary<string, List<string>> recommends { get; set; }

            public Dictionary<string, List<string>> suggests { get; set; }

            public Dictionary<string, List<string>> conflicts { get; set; }

            public Dictionary<string, List<string>> breaks { get; set; }

            public Dictionary<string, List<string>> entrypoints { get; set; }

            public List<MixinInfo> mixins { get; set; }

            public List<JarInfo> jars { get; set; }

            public string accessWidener { get; set; }

            public List<string> provides { get; set; }

            public Dictionary<string, string> languageAdapters { get; set; }

            public static DataModel CreateDefault()
            {
                return new();
            }

            public static void Validate(DataModel model, string name)
            {
                ArgumentNullException.ThrowIfNull(model, nameof(model));
                ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
            }
        }
    }
}
