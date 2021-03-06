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

namespace Steeltoe.Tooling.Executor
{
    public abstract class GroupExecutor : Executor
    {
        private readonly bool _appsFirst;

        protected GroupExecutor(Boolean appsFirst)
        {
            _appsFirst = appsFirst;
        }

        protected override void Execute()
        {
            var services = Context.Configuration.GetServices();
            var apps = Context.Configuration.GetApps();

            if (_appsFirst)
            {
                foreach (var app in apps)
                {
                    ExecuteForApp(app);
                }

                foreach (var service in services)
                {
                    ExecuteForService(service);
                }
            }
            else
            {
                foreach (var service in services)
                {
                    ExecuteForService(service);
                }

                foreach (var app in apps)
                {
                    ExecuteForApp(app);
                }
            }
        }

        protected abstract void ExecuteForApp(string app);

        protected abstract void ExecuteForService(string service);
    }
}
