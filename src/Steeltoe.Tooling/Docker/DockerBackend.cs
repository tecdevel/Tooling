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

using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Steeltoe.Tooling.Docker
{
    public class DockerBackend : IBackend
    {
        private const string Localhost = "127.0.0.1";

        private const string HardCodedFramework = "netcoreapp2.1";

        private const int HardCodedLocalPort = 8080;

        private const int HardCodedRemotePort = 80;

        private readonly Context _context;

        private readonly DockerCli _dockerCli;

        private readonly Cli _dotnetCli;

        internal DockerBackend(Context context)
        {
            _context = context;
            _dockerCli = new DockerCli(_context.Shell);
            _dotnetCli = new Cli("dotnet", _context.Shell);
        }

        public void DeployApp(string app)
        {
            _dotnetCli.Run($"publish -f {HardCodedFramework}");
            var dotnetImage = _context.Target.GetProperty("dotnetRuntimeImage");
            var projectDll =
                $"bin/Debug/{HardCodedFramework}/publish/{Path.GetFileName(_context.ProjectDirectory)}.dll";
            const string appDir = "/app";
            var mount = $"{Path.GetFullPath(_context.ProjectDirectory)}:{appDir}";
            var portMap = $"{HardCodedLocalPort}:{HardCodedRemotePort}";
            _dockerCli.Run(
                $"run --name {app} --volume {mount} --publish {portMap} --detach --rm {dotnetImage} dotnet {appDir}/{projectDll}");
        }

        public void UndeployApp(string app)
        {
            StopContainer(app);
        }

        public Lifecycle.Status GetAppStatus(string app)
        {
            return GetContainerStatus(app, HardCodedLocalPort);
        }

        public void DeployService(string service)
        {
            var dockerInfo = new DockerCli(_context.Shell).Run("info");
            var dockerOs = new Regex(@"OSType:\s*(.+)", RegexOptions.Multiline).Match(dockerInfo).Groups[1].ToString();
            DeployService(service, dockerOs);
        }

        public void DeployService(string service, string os)
        {
            var svcInfo = _context.Configuration.GetServiceInfo(service);
            var port = Registry.GetServiceTypeInfo(svcInfo.ServiceType).Port;
            var image = LookupImage(svcInfo.ServiceType, os);
            var args = _context.Configuration.GetServiceArgs(service, "docker") ?? "";

            if (args.Length > 0)
            {
                args += " ";
            }

            _dockerCli.Run($"run --name {service} --publish {port}:{port} --detach --rm {args}{image}");
        }

        public void UndeployService(string service)
        {
            _dockerCli.Run($"stop {service}");
        }

        public Lifecycle.Status GetServiceStatus(string service)
        {
            var svcInfo = _context.Configuration.GetServiceInfo(service);
            var port = Registry.GetServiceTypeInfo(svcInfo.ServiceType).Port;
            return GetContainerStatus(service, port);
        }

        private void StopContainer(string name)
        {
            _dockerCli.Run($"stop {name}");
        }

        private Lifecycle.Status GetContainerStatus(string name, int port)
        {
            var containerInfo = _dockerCli.Run($"ps --no-trunc --filter name=^/{name}$").Split('\n');
            if (containerInfo.Length <= 2)
            {
                return Lifecycle.Status.Offline;
            }

            var statusStart = containerInfo[0].IndexOf("STATUS", StringComparison.Ordinal);
            if (!containerInfo[1].Substring(statusStart).StartsWith("Up "))
            {
                return Lifecycle.Status.Unknown;
            }

            try
            {
                new TcpClient(Localhost, port).Dispose();
                return Lifecycle.Status.Online;
            }
            catch (SocketException)
            {
                return Lifecycle.Status.Starting;
            }
        }

        private string LookupImage(string type, string os)
        {
            var images = _context.Target.Configuration.ServiceTypeProperties[type];
            return images.TryGetValue($"image-{os}", out var image) ? image : images["image"];
        }
    }
}
