// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentParser.cs" company="CatenaLogic">
//   Copyright (c) 2012 - 2014 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitHubLink
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;

    public static class ArgumentParser
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        public static Context ParseArguments(string commandLineArguments)
        {
            return ParseArguments(commandLineArguments.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).ToList());
        }

        public static Context ParseArguments(params string[] commandLineArguments)
        {
            return ParseArguments(commandLineArguments.ToList());
        }

        public static Context ParseArguments(List<string> commandLineArguments)
        {
            var context = new Context();

            if (commandLineArguments.Count == 0)
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Invalid number of arguments");
            }

            var firstArgument = commandLineArguments.First();
            if (IsHelp(firstArgument))
            {
                context.IsHelp = true;
                return context;
            }

            if (commandLineArguments.Count < 3)
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Invalid number of arguments");
            }

            context.SolutionDirectory = firstArgument;

            var namedArguments = commandLineArguments.Skip(1).ToList();

            EnsureArgumentsEvenCount(commandLineArguments, namedArguments);

            for (var index = 0; index < namedArguments.Count; index = index + 2)
            {
                var name = namedArguments[index];
                var value = namedArguments[index + 1];

                if (IsSwitch("url", name))
                {
                    context.TargetUrl = value;
                    continue;
                }

                if (IsSwitch("b", name))
                {
                    context.TargetBranch = value;
                    continue;
                }

                Log.ErrorAndThrowException<Exception>("Could not parse command line parameter '{0}'.", name);
            }

            return context;
        }

        private static bool IsSwitch(string switchName, string value)
        {
            if (value.StartsWith("-"))
            {
                value = value.Remove(0, 1);
            }

            if (value.StartsWith("/"))
            {
                value = value.Remove(0, 1);
            }

            return (string.Equals(switchName, value));
        }

        private static void EnsureArgumentsEvenCount(IEnumerable<string> commandLineArguments, List<string> namedArguments)
        {
            if (namedArguments.Count.IsOdd())
            {
                Log.ErrorAndThrowException<Exception>("Could not parse arguments: '{0}'.", string.Join(" ", commandLineArguments));
            }
        }

        private static bool IsHelp(string singleArgument)
        {
            return (singleArgument == "?") ||
                   IsSwitch("h", singleArgument) ||
                   IsSwitch("help", singleArgument) ||
                   IsSwitch("?", singleArgument);
        }

        #endregion
    }
}