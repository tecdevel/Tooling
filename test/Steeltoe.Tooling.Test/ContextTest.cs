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

using Shouldly;
using Xunit;

namespace Steeltoe.Tooling.Test
{
    public class ContextTest : ToolingTest
    {
        [Fact]
        public void TestContext()
        {
            var cfg = new Configuration();
            cfg.Target = "dummy-target";
            var ctx = new Context(null, cfg, Console, Shell);
            ctx.Configuration.ShouldBe(cfg);
            ctx.Shell.ShouldBe(Shell);
            ctx.Target.Name.ShouldBe("dummy-target");
        }
    }
}
