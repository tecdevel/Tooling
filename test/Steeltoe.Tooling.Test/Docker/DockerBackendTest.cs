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

using Shouldly;
using Steeltoe.Tooling.Docker;
using Xunit;

namespace Steeltoe.Tooling.Test.Docker
{
    public class DockerBackendTest : ToolingTest
    {
        private readonly DockerBackend _backend;

        public DockerBackendTest()
        {
            Context.Configuration.Target = "docker";
            _backend = Context.Target.GetBackend(Context) as DockerBackend;
        }

        [Fact]
        public void TestDeployService()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 0:0 --detach --rm dummy-server:0.1");
        }

        [Fact]
        public void TestDeployServiceWithArgs()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Context.Configuration.SetServiceDeploymentArgs("my-service", "docker", "arg1 arg2");
            _backend.DeployService("my-service", "dummy-svc");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 0:0 --detach --rm arg1 arg2 dummy-server:0.1");
        }

        [Fact]
        public void TestDeployServiceForOs()
        {
            Context.Configuration.AddService("my-service", "dummy-svc");
            Shell.AddResponse("OSType: dummyos");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 0:0 --detach --rm dummy-server:for_dummyos");
        }

        [Fact]
        public void TestDeployConfigServer()
        {
            Context.Configuration.AddService("my-service", "config-server");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 8888:8888 --detach --rm steeltoeoss/config-server:2.0.1");
        }

        [Fact]
        public void TestDeployEurekaServer()
        {
            Context.Configuration.AddService("my-service", "eureka-server");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 8761:8761 --detach --rm steeltoeoss/eureka-server:2.0.1");
        }

        [Fact]
        public void TestDeployHystrixDashboard()
        {
            Context.Configuration.AddService("my-service", "hystrix-dashboard");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 7979:7979 --detach --rm steeltoeoss/hystrix-dashboard:1.4.5");
        }

        [Fact]
        public void TestDeployMicrosoftSqlServer()
        {
            Context.Configuration.AddService("my-service", "mssql");
            _backend.DeployService("my-service", "linux");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 1433:1433 --detach --rm steeltoeoss/mssql:2017-CU11-linux");
            _backend.DeployService("my-service", "windows");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 1433:1433 --detach --rm steeltoeoss/mssql:2017-CU1-windows");
        }

        [Fact]
        public void TestDeployMySql()
        {
            Context.Configuration.AddService("my-service", "mysql");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 3306:3306 --detach --rm steeltoeoss/mysql:5.7.24");
        }

        [Fact]
        public void TestDeployRedis()
        {
            Context.Configuration.AddService("my-service", "redis");
            _backend.DeployService("my-service", "linux");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 6379:6379 --detach --rm steeltoeoss/redis:4.0.11-linux");
            _backend.DeployService("my-service", "windows");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 6379:6379 --detach --rm steeltoeoss/redis:3.0.504-windows");
        }

        [Fact]
        public void TestDeployZipkin()
        {
            Context.Configuration.AddService("my-service", "zipkin");
            _backend.DeployService("my-service");
            Shell.LastCommand.ShouldBe(
                "docker run --name my-service --publish 9411:9411 --detach --rm steeltoeoss/zipkin:2.11.6");
        }

        [Fact]
        public void TestUndeployService()
        {
            _backend.UndeployService("my-service");
            Shell.LastCommand.ShouldBe("docker stop my-service");
        }

        [Fact]
        public void TestGetServiceLifecycleStateCommand()
        {
            _backend.GetServiceStatus("my-service");
            Shell.LastCommand.ShouldBe("docker ps --no-trunc --filter name=^/my-service$");
        }

        [Fact]
        public void TestGetServiceLifecycleStateOffline()
        {
            var state = _backend.GetServiceStatus("my-service");
            state.ShouldBe(Lifecycle.Status.Offline);
        }

        [Fact]
        public void TestGetServiceLifecycleStateStarting()
        {
            Shell.AddResponse(
                @"CONTAINER ID                                                       IMAGE                           COMMAND                                                                 CREATED             STATUS              PORTS                    NAMES
0000000000000000000000000000000000000000000000000000000000000000   dummy-server:0.1                 java -Djava.security.egd=file:/dev/./urandom -jar config-server.jar    37 seconds ago      Up 36 seconds       0.0.0.0:0000->0000/tcp   my-service
");
            var state = _backend.GetServiceStatus("my-service");
            state.ShouldBe(Lifecycle.Status.Starting);
        }

        [Fact]
        public void TestGetServiceLifecycleStateStartingForOs()
        {
            Shell.AddResponse(
                @"CONTAINER ID                                                       IMAGE                           COMMAND                                                                 CREATED             STATUS              PORTS                    NAMES
0000000000000000000000000000000000000000000000000000000000000000   dummy-server:for_dummyos         java -Djava.security.egd=file:/dev/./urandom -jar config-server.jar    37 seconds ago      Up 36 seconds       0.0.0.0:0000->0000/tcp   my-service
");
            Shell.AddResponse("OSType: dummyos");
            var state = _backend.GetServiceStatus("my-service");
            state.ShouldBe(Lifecycle.Status.Starting);
        }
    }
}
