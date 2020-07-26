﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;



namespace MSPro.CLArgs
{
    /// <summary>
    ///     The top level class to easily use 'CLArgs'.
    /// </summary>
    [PublicAPI]
    public class Commander
    {
        private readonly Dictionary<string, Func<ICommand>> _commands;
        private readonly Settings _settings;



        /// <summary>
        ///     Create a new Commander instance.
        /// </summary>
        /// <param name="args">The arguments as provided to <code>void Main( string[] args)</code>.</param>
        /// <param name="settings">Settings used to control CLArgs overall behaviour.</param>
        /// <remarks>
        ///     When creating an instance the provided <see cref="args" /> will
        ///     be parsed and Command implementations will be resolved (in case <see cref="Settings.AutoResolveCommands" /> is set
        ///     to true.
        /// </remarks>
        public Commander(Settings settings = null)
        {
            _settings = settings ?? new Settings();
            _commands = new Dictionary<string, Func<ICommand>>();
            // Resolve commands and register CommandBase factories
            if (_settings.AutoResolveCommands) resolveCommandImplementations();
        }



        /// <summary>
        ///     Manually register (add or update) a Command.
        /// </summary>
        /// <remarks>
        ///     Not the command itself is registered but <b>a factory function</b> that
        ///     is used to create a new instance of the Command.<br />
        ///     If there is already a command registered for the same <paramref name="verb" />
        ///     the 'old' command is overridden.
        /// </remarks>
        /// <param name="verb">The <see cref="CLArgs.Arguments.Verbs" /> that is linked to this Command</param>
        /// <param name="factory">A factory function that return an instance of <see cref="CommandBase{TCommandParameters}" />.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="verb" /> is null or empty.</exception>
        /// <example>
        ///     <code>
        ///     Commander c = new Commander(args);
        ///     c.RegisterCommandFactory( "HelloWorld", () => new HelloWorldCommand());
        ///     c.ExecuteCommand();
        /// </code>
        /// </example>
        /// <seealso cref="Settings.AutoResolveCommands" />
        public void RegisterCommandFactory([NotNull] string verb, [NotNull] Func<ICommand> factory)
        {
            if (string.IsNullOrEmpty(verb)) throw new ArgumentNullException(nameof(verb));
            _commands[verb] = factory;
        }



        /// <summary>
        ///     Directly bind a function to a verb.
        /// </summary>
        /// <param name="verb">The Verb</param>
        /// <param name="func">
        ///     The function that is executed when the verbs passed in the
        ///     command-line (<see cref="Arguments.VerbPath" /> are equal to the <paramref name="verb" />.
        /// </param>
        /// <example>
        /// <code>
        /// string COMMAND_LINE = "word1 text2 verb3";
        /// // Arguments.VerbPath is executed
        /// var commander = new Commander(new Settings { AutoResolveCommands = false });
        /// commander.RegisterFunction("word1", word);
        /// commander.RegisterFunction("word1.text2", text);
        /// commander.RegisterFunction("word1.text2.verb3", verb);
        /// commander.ExecuteCommand(args);
        /// </code></example>
        /// <exception cref="ArgumentNullException">In case <paramref name="verb" /> is null.</exception>
        /// <seealso cref="ExecuteCommand(string[])"/>
        /// <seealso cref="Arguments.VerbPath "/>
        /// <seealso cref="Settings.AutoResolveCommands"/>
        public void RegisterFunction([NotNull] string verb, [NotNull] Action<Arguments> func)
        {
            if (string.IsNullOrEmpty(verb)) throw new ArgumentNullException(nameof(verb));
            _commands[verb] = () => new CommandWrapper(func);
        }



        /// <summary>
        ///     Resolve a Command implementation by Verb.
        /// </summary>
        /// <seealso cref="Settings.AutoResolveCommands" />
        /// <seealso cref="RegisterCommandFactory" />
        /// <param name="verb">
        ///     The verb for which and implementation should be resolved.
        ///     IF <c>verb</c> is <c>null</c> the default Command (first registered command) is returned.
        /// </param>
        public ICommand ResolveCommand(string verb)
        {
            if (_commands == null || _commands.Count == 0)
                throw new ApplicationException("No Commands have been registered");

            // Invoke Default Command
            if (verb == null) return _commands.First().Value();


            if (string.IsNullOrEmpty(verb)) throw new ArgumentNullException(nameof(verb));
            if (!_commands.ContainsKey(verb))
                throw new IndexOutOfRangeException(
                    $"There is no Command registered for verb ${verb}. "
                    + "Check if upper/lower case is correct.");

            return _commands[verb](); // call construction method
        }



        /// <summary>
        ///     Execute the command referenced by <see cref="CLArgs.Arguments.VerbPath" />.
        /// </summary>
        public void ExecuteCommand([NotNull] string[] args)
        {
            if (_commands == null || _commands.Count == 0)
                throw new ApplicationException("No Commands have been registered");

            Arguments arguments = CommandLineParser.Parse(args);
            ICommand command = ResolveCommand(arguments.VerbPath);
            command.Execute(arguments, _settings);
        }



        /// <summary>
        ///     Shortcut and preferred way to use Commander.
        /// </summary>
        public static void ExecuteCommand(string[] args, Settings settings = null) =>
            new Commander(settings).ExecuteCommand(args);



        private void resolveCommandImplementations()
        {
            Dictionary<string, Type> verbAndCommandTypes = _settings.CommandResolver.GetCommandTypes();
            if (verbAndCommandTypes.Count == 0)
                throw new ApplicationException(
                    $"{nameof(_settings.AutoResolveCommands)} is {_settings.AutoResolveCommands} " +
                    $"however the resolver {_settings.CommandResolver.GetType()} did not find any CommandBase implementation! " +
                    "Make sure the resolver can see/find the Commands.");

            foreach (var commandType in verbAndCommandTypes)
            {
                RegisterCommandFactory(commandType.Key,
                                       () => (ICommand) Activator.CreateInstance(commandType.Value));
            }
        }
    }
}