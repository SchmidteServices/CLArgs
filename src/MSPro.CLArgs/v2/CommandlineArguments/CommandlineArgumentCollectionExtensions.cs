﻿using System.Linq;
using JetBrains.Annotations;

namespace MSPro.CLArgs;

[PublicAPI]
public static class CommandlineArgumentCollectionExtensions
{
    public static IArgumentCollection AddArguments(this IArgumentCollection arguments,
        string[] args, Settings2 settings)
    {
        var cp =  CommandLineParser2.Parse(args.Skip(1).ToArray(),settings);
        foreach (string name in cp.Verbs)
        {
            arguments.Add( CommandlineArgument.Verb( name));
        } 
        
        foreach (var option in cp.Options.Values)
        {
            arguments.Add( CommandlineArgument.Option( option.Key, option.Value));
        }     
        
        foreach (var target in cp.Targets)
        {
            arguments.Add( CommandlineArgument.Target( target));
        }

        return arguments;
    }
}