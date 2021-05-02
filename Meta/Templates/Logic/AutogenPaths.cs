using System.IO;

namespace Hopper.Meta
{
    public class AutogenPaths
    {
        public string projectRoot;

        public void Reset(string projectRoot)
        {
            this.projectRoot = projectRoot;
        }

        public static void CreateOrEmptyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            else
            foreach (var file in new DirectoryInfo(directory).GetFiles())
                file.Delete(); 
        }

        public void CreateOrEmpty()
        {
            if (!Directory.Exists(AutogenFolder))
                Directory.CreateDirectory(AutogenFolder);

            CreateOrEmptyDirectory(BehaviorAutogenFolder);
            CreateOrEmptyDirectory(ComponentAutogenFolder);
            CreateOrEmptyDirectory(HandlersAutogenFolder);
            CreateOrEmptyDirectory(TagsAutogenFolder);
            CreateOrEmptyDirectory(StatAutogenFolder);
            CreateOrEmptyDirectory(FlagsAutogenFolder);

            if (!File.Exists(GitignorePath))
            {
                File.WriteAllText(GitignorePath, "*");
            }
        }

        public string StatJsonsFolder => $@"{projectRoot}/Stats/Json";
        public string AutogenFolder => $@"{projectRoot}/Autogen";
        public string BehaviorAutogenFolder => $@"{AutogenFolder}/Behaviors";
        public string ComponentAutogenFolder => $@"{AutogenFolder}/Components";
        public string TagsAutogenFolder => $@"{AutogenFolder}/Tags";
        public string HandlersAutogenFolder => $@"{AutogenFolder}/Handlers";
        public string MainAutogenFile => $@"{AutogenFolder}/Main.cs";
        public string StatAutogenFolder => $@"{AutogenFolder}/Stats";
        public string GitignorePath => $@"{AutogenFolder}/.gitignore";
        public string SlotExtensionsPath => $@"{AutogenFolder}/SlotExtensions.cs";
        public string FlagsAutogenFolder => $@"{AutogenFolder}/Flags";
    }

}