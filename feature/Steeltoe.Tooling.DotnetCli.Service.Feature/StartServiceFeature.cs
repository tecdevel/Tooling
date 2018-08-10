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

using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

namespace Steeltoe.Tooling.DotnetCli.Service.Feature
{
    [Label("service")]
    public partial class StartServiceFeature
    {
        [Scenario]
        public void RunStartNoArgs()
        {
            Runner.RunScenario(
                given => a_dotnet_project("start_no_args"),
                when => the_developer_runs_steeltoe_("start-service"),
                then => the_command_succeeds()
            );
        }
    }
}