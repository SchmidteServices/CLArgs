﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using MSPro.CLArgs;



[PublicAPI]
public abstract class CommandBase2<TContext> : ICommand2 where TContext : class, new()
{
    private readonly IServiceProvider _serviceProvider;



    private IOptionCollection _commandOptions;



    protected CommandBase2(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }



    protected TContext Context { get; private set; }



    public IOptionCollection CommandOptions => _commandOptions ??= new OptionCollection().AddContextType<TContext>();



    void ICommand2.Execute()
    {
        var builder = _serviceProvider.GetRequiredService<ContextBuilder>();
        //builder.ConfigureConverters( (converters)=>{});
        this.Context = builder.Build<TContext>(
            _serviceProvider.GetRequiredService<IArgumentCollection>(), this.CommandOptions,
            out var unresolvedPropertyNames,
            out var errors);

        if (!errors.HasErrors())
        {
            BeforeExecute(unresolvedPropertyNames, errors);
            if (!errors.HasErrors())
            {
                try
                {
                    Execute();
                }
                catch (Exception exception)
                {
                    errors.AddError("CommandExecution", exception.Message);
                }
            }
        }

        if (errors.HasErrors()) OnError(errors, false);
    }



    /// <summary>
    ///     Error handler in case of any error.
    /// </summary>
    /// <remarks>
    ///     The default implementation of <see cref="OnError" /> simply throws an
    ///     <see cref="AggregateException" /> in case of any error. You can avoid this by overriding this method.
    /// </remarks>
    /// <param name="errors">The errors that have occurred.</param>
    /// <param name="handled">If <c>true</c> the method does nothing anymore, because it expects the errors have been handled.</param>
    /// <exception cref="AggregateException">Always</exception>
    protected virtual void OnError(ErrorDetailList errors, bool handled)
    {
        if (handled) return;
        if (errors.Details.Count > 1)
            throw new AggregateException(errors.Details.Select(
                                             e => new ArgumentException(e.ErrorMessages[0], e.AttributeName)));

        throw new ArgumentException(errors.Details[0].ErrorMessages[0], errors.Details[0].AttributeName);
    }



    protected virtual void BeforeExecute(
        HashSet<string> unresolvedPropertyNames,
        ErrorDetailList errors)
    {
    }



    protected abstract void Execute();
}