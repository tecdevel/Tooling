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

namespace Steeltoe.Tooling.Dummy
{
    public class DummyServiceBackend : IServiceBackend
    {
        private readonly string _path;

        private DummyServiceDatabase _database;

        public DummyServiceBackend(string path)
        {
            _path = path;
            _database = DummyServiceDatabase.Load(_path);
        }

        public void DeployService(string serviceName, string serviceTypeName)
        {
            _database.Services[serviceName] = ServiceLifecycle.State.Starting;
            Store();
        }

        public void UndeployService(string name)
        {
            _database.Services[name] = ServiceLifecycle.State.Stopping;
            Store();
        }

        public ServiceLifecycle.State GetServiceLifecleState(string name)
        {
            if (!_database.Services.ContainsKey(name))
            {
                return ServiceLifecycle.State.Offline;
            }

            var state = _database.Services[name];
            switch (state)
            {
                case ServiceLifecycle.State.Starting:
                    _database.Services[name] = ServiceLifecycle.State.Online;
                    Store();
                    break;
                case ServiceLifecycle.State.Stopping:
                    _database.Services.Remove(name);
                    Store();
                    break;
            }

            return state;
        }

        private void Store()
        {
            DummyServiceDatabase.Store(_path, _database);
        }
    }
}
