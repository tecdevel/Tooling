// Copyright 2018 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.IO;
using Shouldly;
using Steeltoe.Tooling.CloudFoundry;
using Xunit;

namespace Steeltoe.Tooling.Test.CloudFoundry
{
    public class CloudFoundryBackendTest : ToolingTest
    {
        private readonly CloudFoundryBackend _backend;

        public CloudFoundryBackendTest()
        {
            Context.Configuration.Target = "cloud-foundry";
            _backend = Context.Target.GetBackend(Context) as CloudFoundryBackend;
        }

        [Fact]
        public void TestDeployApp()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Context.Configuration.AddApp("my-app");
            _backend.DeployApp("my-app");
            Shell.Commands.Count.ShouldBe(2);
            Shell.Commands[0].ShouldBe("dotnet publish -f netcoreapp2.1 -r win10-x64");
            Shell.Commands[1].ShouldBe("cf push -f manifest-steeltoe.yml -p bin/Debug/netcoreapp2.1/win10-x64/publish");
            var manifestFile =
                new CloudFoundryManifestFile(Path.Combine(Context.ProjectDirectory, "manifest-steeltoe.yml"));
            manifestFile.Exists().ShouldBeTrue();
            manifestFile.CloudFoundryManifest.Applications.Count.ShouldBe(1);
            var app = manifestFile.CloudFoundryManifest.Applications[0];
            app.Name.ShouldBe("my-app");
            app.Command.ShouldBe($"cmd /c .\\{Path.GetFileName(Context.ProjectDirectory)}");
            app.BuildPacks.Count.ShouldBe(1);
            app.BuildPacks[0].ShouldBe("binary_buildpack");
            app.Stack.ShouldBe("windows2016");
            app.Memory.ShouldBe("512M");
            app.Environment["ASPNETCORE_ENVIRONMENT"].ShouldBe("development");
            app.ServiceNames[0].ShouldBe("my-service");
            app.ServiceNames.Count.ShouldBe(1);
        }

        [Fact]
        public void TestUndeployApp()
        {
            Context.Configuration.AddApp("my-app");
            _backend.UndeployApp("my-app");
            Shell.LastCommand.ShouldBe("cf delete my-app -f");
        }

        [Fact]
        public void TestDeployService()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service dummy-server dummy-plan my-service");
        }

        [Fact]
        public void TestDeployConfigServer()
        {
            Context.Configuration.AddService("my-service", "config-server");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service p-config-server standard my-service");
        }

        [Fact]
        public void TestDeployEurekaServer()
        {
            Context.Configuration.AddService("my-service", "eureka-server");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service p-service-registry standard my-service");
        }

        [Fact]
        public void TestDeployHystrixDashboard()
        {
            Context.Configuration.AddService("my-service", "hystrix-dashboard");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service p-circuit-breaker-dashboard standard my-service");
        }

        [Fact]
        public void TestDeployMySql()
        {
            Context.Configuration.AddService("my-service", "mysql");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service p-mysql 100mb my-service");
        }

        [Fact]
        public void TestDeployRedis()
        {
            Context.Configuration.AddService("my-service", "redis");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe("cf create-service p-redis shared-vm my-service");
        }

        [Fact]
        public void TestUndeployService()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            _backend.UndeployService("my-service");
            Shell.LastCommand.ShouldBe("cf delete-service my-service -f");
        }

        [Fact]
        public void TestCheckApp()
        {
            Context.Configuration.AddApp("my-app");
            _backend.GetAppStatus("my-app");
            Shell.LastCommand.ShouldBe("cf app my-app");
        }

        [Fact]
        public void TestCheckService()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            _backend.GetServiceStatus("my-service");
            Shell.LastCommand.ShouldBe("cf service my-service");
        }

        [Fact]
        public void TestServiceStarting()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Shell.AddResponse("status:    create in progress");
            var status = _backend.GetServiceStatus("my-service");
            status.ShouldBe(Lifecycle.Status.Starting);
        }

        [Fact]
        public void TestAppOnline()
        {
            Context.Configuration.AddApp("my-app");
            Shell.AddResponse("#0   running   2018-11-02T16:32:37Z   16.9%   129.2M of 512M   124.8M of 1G");
            var status = _backend.GetAppStatus("my-app");
            status.ShouldBe(Lifecycle.Status.Online);
        }

        [Fact]
        public void TestServiceOnline()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Shell.AddResponse("status:    create succeeded");
            var status = _backend.GetServiceStatus("my-service");
            status.ShouldBe(Lifecycle.Status.Online);
        }

        [Fact]
        public void TestServiceOffline()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Shell.AddResponse("Service instance my-service not found", 1);
            var status = _backend.GetServiceStatus("my-service");
            status.ShouldBe(Lifecycle.Status.Offline);
        }
    }
}
