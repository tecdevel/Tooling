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
using Shouldly;
using Steeltoe.Tooling.Executor;

namespace Steeltoe.Tooling.Test.Executor
{
    public class ListTargetsExecutorTest : ToolingTest
    {
//        [Fact]
        public void TestListTargets()
        {
            new ListTargetsExecutor().Execute(Context);
            var reader = new StringReader(Console.ToString());
            reader.ReadLine().ShouldBe("cloud-foundry");
            reader.ReadLine().ShouldBe("docker");
            reader.ReadLine().ShouldBe("dummy-target");
            reader.ReadLine().ShouldBeNull();
        }

//        [Fact]
        public void TestListTargetsVerbose()
        {
            new ListTargetsExecutor(true).Execute(Context);
            var reader = new StringReader(Console.ToString());
            reader.ReadLine().ShouldBe("cloud-foundry  Cloud Foundry");
            reader.ReadLine().ShouldBe("docker         Docker");
            reader.ReadLine().ShouldBe("dummy-target   A Dummy Target");
            reader.ReadLine().ShouldBeNull();
        }
    }
}
