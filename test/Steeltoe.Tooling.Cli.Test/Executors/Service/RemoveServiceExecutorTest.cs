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

using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

namespace Steeltoe.Tooling.Cli.Test.Executors.Service
{
    public partial class RemoveServiceExecutorTest
    {
        [Scenario]
        public void RemoveUnknownService()
        {
            Runner.RunScenario(
                given => a_project(),
                when => remove_service_is_run("unknown-service"),
                then => an_exception_should_be_thrown<CommandException>("Unknown service 'unknown-service'")
            );
        }

        [Scenario]
        public void RemoveService()
        {
            Runner.RunScenario(
                given => a_project(),
                and => a_service("existing-service", "existing-service-type"),
                when => remove_service_is_run("existing-service"),
                then => the_output_should_include("Removed service 'existing-service'"),
                and => the_services_should_not_include("existing-service")
            );
        }
    }
}
