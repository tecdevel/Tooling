﻿using Microsoft.Extensions.Logging;

namespace Steeltoe.Tooling.DotnetCli.Base.Feature
{
    public static class Logging
    {
        public static ILoggerFactory LoggerFactory { get; } = new LoggerFactory().AddConsole(LogLevel.Debug);
    }
}
