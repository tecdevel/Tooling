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

namespace Steeltoe.Cli.Test
{
    [Label("list")]
    public class ListFeature : FeatureSpecs
    {
        [Scenario]
        [Label("help")]
        public void ListHelp()
        {
            Runner.RunScenario(
                given => a_dotnet_project("list_help"),
                when => the_developer_runs_cli_command("list --help"),
                then => the_cli_should_output(new[]
                {
                    "List services, service types, or deployment environments.",
                    $"Usage: {Program.Name} list [options]",
                    "Options:",
                    "-e|--environments List deployment environments",
                    "-s|--services List services",
                    "-t|--service-types List service types",
                    "-v|--verbose Verbose",
                    "-?|-h|--help Show help information",
                    "If run with no options, list services.",
                })
            );
        }

        [Scenario]
        public void ListTooManyArgs()
        {
            Runner.RunScenario(
                given => a_dotnet_project("list_too_many_args"),
                when => the_developer_runs_cli_command("list arg1"),
                then => the_cli_should_fail_parse("Unrecognized command or argument 'arg1'")
            );
        }

        [Scenario]
        public void ListMutallyExclusiveOptions()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_mutually_exclusive_options"),
                when => the_developer_runs_cli_command("list -e -t"),
                then => the_cli_should_error(ErrorCode.Argument,
                    "Specify at most one of: -e|--environments, -s|--services, -t|--service-types")
            );
        }

        [Scenario]
        public void ListUninitializedProject()
        {
            Runner.RunScenario(
                given => a_dotnet_project("list_uninitialized_project"),
                when => the_developer_runs_cli_command("list"),
                then => the_cli_should_error(ErrorCode.Tooling,
                    "Project has not been initialized for Steeltoe Developer Tools")
            );
        }

        [Scenario]
        public void ListEnvironments()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_environments"),
                when => the_developer_runs_cli_command("list -e"),
                then => the_cli_should_output(new[]
                {
                    "cloud-foundry",
                    "docker",
                    "dummy-env",
                })
            );
        }

        [Scenario]
        public void ListEnvironmentsVerbose()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_environments_verbose"),
                when => the_developer_runs_cli_command("list -e -v"),
                then => the_cli_should_output(new[]
                {
                    "cloud-foundry Cloud Foundry",
                    "docker Docker",
                    "dummy-env A Dummy Environment",
                })
            );
        }

        [Scenario]
        public void ListServiceTypes()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_service_types"),
                when => the_developer_runs_cli_command("list -t"),
                then => the_cli_should_output(new[]
                {
                    "config-server",
                    "dummy-svc",
                    "eureka-server",
                    "hystrix-dashboard",
                    "mssql",
                    "redis",
                    "zipkin",
                })
            );
        }

        [Scenario]
        public void ListServiceTypesVerbose()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_service_types_verbose"),
                when => the_developer_runs_cli_command("list -t -v"),
                then => the_cli_should_output(new[]
                {
                    "config-server 8888 Cloud Foundry Config Server",
                    "dummy-svc 0 A Dummy Service",
                    "eureka-server 8761 Netflix Eureka Server",
                    "hystrix-dashboard 7979 Netflix Hystrix Dashboard",
                    "mssql 1433 Microsoft SQL Server",
                    "redis 6379 Redis In-Memory Datastore",
                    "zipkin 9411 Zipkin Tracing Collector and UI",
                })
            );
        }

        [Scenario]
        public void ListServices()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_services"),
                when => the_developer_runs_cli_command("add dummy-svc c-service"),
                and => the_developer_runs_cli_command("add dummy-svc b-service"),
                and => the_developer_runs_cli_command("add dummy-svc a-service"),
                and => the_developer_runs_cli_command("list"),
                then => the_cli_should_output(new[]
                {
                    "a-service",
                    "b-service",
                    "c-service",
                })
            );
        }

        [Scenario]
        public void ListServicesVerbose()
        {
            Runner.RunScenario(
                given => a_steeltoe_project("list_services_verbose"),
                and => the_developer_runs_cli_command("add dummy-svc b-service"),
                and => the_developer_runs_cli_command("add dummy-svc c-service"),
                when => the_developer_runs_cli_command("add dummy-svc a-service"),
                and => the_developer_runs_cli_command("list -s -v"),
                then => the_cli_should_output(new[]
                {
                    "a-service 0 dummy-svc",
                    "b-service 0 dummy-svc",
                    "c-service 0 dummy-svc",
                })
            );
        }
    }
}
