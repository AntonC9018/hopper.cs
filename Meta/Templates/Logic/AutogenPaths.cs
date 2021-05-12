using System;
using System.Collections.Generic;
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
            {
                Directory.CreateDirectory(AutogenFolder);
            }

            foreach (var subfolder in GetOutputSubFolders())
            {
                CreateOrEmptyDirectory(subfolder);
            }

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
        public string StatAutogenFolder => $@"{AutogenFolder}/Stats";
        public string FlagsAutogenFolder => $@"{AutogenFolder}/Flags";
        
        // I know this is sort duplicate code
        public IEnumerable<string> GetOutputSubFolders()
        {
            yield return BehaviorAutogenFolder;
            yield return ComponentAutogenFolder;
            yield return TagsAutogenFolder;
            yield return HandlersAutogenFolder;
            yield return StatAutogenFolder;
            yield return FlagsAutogenFolder;
        }
        

        public string MainAutogenFile => $@"{AutogenFolder}/Main.cs";
        public string GitignorePath => $@"{AutogenFolder}/.gitignore";
        public string SlotExtensionsPath => $@"{AutogenFolder}/SlotExtensions.cs";

        public IEnumerable<string> GetFiles()
        {
            yield return MainAutogenFile;
            yield return GitignorePath;
            yield return SlotExtensionsPath;
        }
    }
}