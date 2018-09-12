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

using System.IO;
using Steeltoe.Tooling.Environment;
using Steeltoe.Tooling.System;

namespace Steeltoe.Tooling.Executor.Target
{
    public class ListTargetsExecutor : IExecutor
    {
        public bool Execute(Configuration config, Shell shell, TextWriter output)
        {
            foreach (var name in EnvironmentRegistry.GetNames())
            {
                output.WriteLine(name);
            }
            return false;
        }
    }
}