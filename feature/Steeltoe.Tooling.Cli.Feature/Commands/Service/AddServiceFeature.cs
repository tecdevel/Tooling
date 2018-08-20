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

namespace Steeltoe.Tooling.Cli.Feature.Commands.Service
{
    [Label("service")]
    public class AddServiceFeature : CliFeatureSpecs
    {
        [Scenario]
        [Label("help")]
        public void AddServiceHelp()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_service_help"),
                when => the_developer_runs_steeltoe_command("add-service --help"),
                then => the_command_should_succeed(),
                and => the_developer_should_see(@"^\s*Add a service\.\n"),
                and => the_developer_should_see(@"\n\s*name\s+The service name\n"),
                and => the_developer_should_see(@"\n\s*-t\|--type\s+The service type\n")
            );
        }

        [Scenario]
        public void AddServiceNotEnoughArgs()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_service_not_enough_args"),
                when => the_developer_runs_steeltoe_command("add-service"),
                then => the_command_should_fail(),
                and => the_developer_should_see_the_error("Service name not specified")
            );
        }

        [Scenario]
        public void AddServiceTooManyArgs()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_service_too_many_args"),
                when => the_developer_runs_steeltoe_command("add-service arg1 arg2"),
                then => the_command_should_fail(),
                and => the_developer_should_see_the_error("Unrecognized command or argument 'arg2'")
            );
        }

        [Scenario]
        public void AddServiceMissingType()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_service_missing_type"),
                when => the_developer_runs_steeltoe_command("add-service foo"),
                then => the_command_should_fail(),
                and => the_developer_should_see_the_error("Service type not specified")
            );
        }

        [Scenario]
        public void AddUnknownServiceType()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_unknown_service_type"),
                when => the_developer_runs_steeltoe_command("add-service foo --type no-such-type"),
                then => the_command_should_fail(),
                and => the_developer_should_see_the_error("Unknown service type 'no-such-type'")
            );
        }

        [Scenario]
        public void AddAlreadyExistingService()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_already_existing_service"),
                and => a_service("existing-service", "existing-service-type"),
                when => the_developer_runs_steeltoe_command("add-service existing-service --type cloud-foundry-config-server"),
                then => the_command_should_fail(),
                and => the_developer_should_see_the_error("Service 'existing-service' already exists")
            );
        }

        [Scenario]
        public void AddCloudFoundryConfigServerService()
        {
            Runner.RunScenario(
                given => a_dotnet_project("add_cloud_foundry_config_server_service"),
                and => a_target("keep-this-target"),
                when => the_developer_runs_steeltoe_command("add-service MyConfigServer --type cloud-foundry-config-server"),
                then => the_command_should_succeed(),
                and => the_developer_should_see("Added cloud-foundry-config-server service 'MyConfigServer'"),
                and => the_service_config_should_exist("MyConfigServer", "cloud-foundry-config-server"),
                and => the_target_config_should_exist("keep-this-target")
            );
        }
    }
}