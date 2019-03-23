using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Conan.VisualStudio.Core;
using Conan.VisualStudio.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Conan.VisualStudio.Tests.Core
{
    public class ConanRunnerTests
    {
        [Ignore ("FIXME: . disappears from path!")]
        [TestMethod]
        public async Task GeneratorShouldBeInvokedProperlyAsync()
        {
            var conan = new ConanRunner(null, ResourceUtils.ConanShim);
            var project = new ConanProject
            {
                Path = ".",
                Configurations = 
                {
                    new ConanConfiguration
                    {
                        Architecture = "x86_64",
                        BuildType = "Debug",
                        CompilerToolset = "v141",
                        CompilerVersion = "15",
                        InstallPath = "./conan"
                    }
                }
            };
            using (var process = await conan.Install(project, project.Configurations.Single(), ConanGeneratorType.visual_studio_multi, ConanBuildType.missing, true))
            {
                Assert.AreEqual("install . -g visual_studio_multi " +
                             "--install-folder ./conan " +
                             "-s arch=x86_64 " +
                             "-s build_type=Debug " +
                             "-s compiler.toolset=v141 " +
                             "-s compiler.version=15 " +
                             "--build missing --update", process.StartInfo.Arguments);
            }
        }
        [Ignore("FIXME: System.IO.FileLoadException : Could not load file or assembly 'Newtonsoft.Json, Version=12.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed' or one of its dependencies. The located assembly's manifest definition does not match the assembly ref")]
        [TestMethod]
        public async Task SettingsFileShouldBeParsedProperlyAsync()
        {
            var project = new ConanProject
            {
                Path = ResourceUtils.FakeProject,
                Configurations =
                {
                    new ConanConfiguration
                    {
                        Architecture = "x86_64",
                        BuildType = "Debug",
                        CompilerToolset = "v141",
                        CompilerVersion = "15",
                        InstallPath = "./conan"
                    }
                }
            };

            var settingsService = new VisualStudioSettingsService(null);
            var conanSettings = settingsService.LoadSettingFile(project);

            var conan = new ConanRunner(conanSettings, ResourceUtils.ConanShim);

            using (var process = await conan.Install(project, project.Configurations.Single(), ConanGeneratorType.visual_studio_multi, ConanBuildType.missing, true))
            {
                Assert.AreEqual("install -test", process.StartInfo.Arguments);
            }
        }
    }
}
