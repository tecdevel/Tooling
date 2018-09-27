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

using System.Threading.Tasks;

namespace Steeltoe.Tooling.Executor
{
    public class DeployServicesExecutor : ServicesExecutor
    {
        public override void Execute(Context context)
        {
            base.Execute(context);
            Parallel.ForEach(ServiceNames, svcName =>
            {
                var state = context.ServiceManager.GetServiceState(svcName);
                if (state == ServiceLifecycle.State.Disabled)
                {
                    context.Shell.Console.WriteLine($"Ignoring disabled service '{svcName}'");
                }
                else
                {
                    context.Shell.Console.WriteLine($"Deploying service '{svcName}'");
                    context.ServiceManager.DeployService(svcName);
                }
            });
        }
    }
}
