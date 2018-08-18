﻿// Copyright 2018 the original author or authors.
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

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

// ReSharper disable InconsistentNaming

namespace Steeltoe.Tooling.Cli
{
    public class ToolingConfiguration
    {
        private static readonly ILogger Logger = Logging.LoggerFactory.CreateLogger<ToolingConfiguration>();

        public const string DefaultFileName = ".steeltoe.tooling.yml";

        public string target { get; set; }

        public SortedDictionary<string, Service> services { get; set;  } = new SortedDictionary<string, Service>();

        public static ToolingConfiguration Load(string path)
        {
            var realPath = Directory.Exists(path) ? Path.Combine(path, DefaultFileName) : path;
            Logger.LogDebug($"loading tooling configuration from {realPath}");
            using (var reader = new StreamReader(realPath))
            {
                var cfg = Load(reader);
                if (cfg == null)
                {
                    cfg = new ToolingConfiguration();
                }

                return cfg;
            }
        }

        public static ToolingConfiguration Load(TextReader reader)
        {
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<ToolingConfiguration>(reader);
        }

        public void Store(string path)
        {
            var realPath = Directory.Exists(path) ? Path.Combine(path, DefaultFileName) : path;
            Logger.LogDebug($"storing tooling configuration to {realPath}");
            using (var writer = new StreamWriter(realPath))
            {
                Store(writer);
            }
        }

        public void Store(TextWriter writer)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(this);
            writer.Write(yaml);
            writer.Flush();
        }

        public class Service
        {
            public string type { get; set; }

            public Service()
            {
            }

            public Service(string type)
            {
                this.type = type;
            }
        }
    }
}