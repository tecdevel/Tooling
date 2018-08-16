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
using McMaster.Extensions.CommandLineUtils;

namespace Steeltoe.Tooling.DotnetCli
{
    public abstract class DotnetCliCommand
    {
        protected virtual int OnExecute(CommandLineApplication app)
        {
            try
            {
                OnCommandExecute(app);
            }
            catch (Exception e)
            {
                app.Error.WriteLine(e);
//                app.Error.WriteLine(e.Message);
                return 1;
            }

            return 0;
        }

        protected abstract void OnCommandExecute(CommandLineApplication app);
    }
}