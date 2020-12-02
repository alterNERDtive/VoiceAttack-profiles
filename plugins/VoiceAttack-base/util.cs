#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace alterNERDtive.util
{
    public class Configuration
    {
        private readonly dynamic VA;
        private readonly string ID;
        private readonly VoiceAttackLog Log;
        private static readonly Dictionary<string, OptDict<string, Option>> Defaults = new Dictionary<string, OptDict<string, Option>>
        {
            {
                "alterNERDtive-base",
                new OptDict<string, Option>{
                    { new Option("eddi.quietMode", true, voiceTrigger: "eddi quiet mode", description: "Whether or not to make EDDI shut up.") },
                    { new Option("keyPressDuration", (decimal)0.01, voiceTrigger: "key press duration", description: "The time keys will be held down for.") },
                    { new Option("elite.pasteKey", "v", voiceTrigger: "elite paste key", description: "The key used to paste in conjunction with CTRL. The key that would be 'V' on QWERTY.") }
                }
            },
            {
                "EliteAttack",
                new OptDict<string, Option>{

                }
            },
            {
                "RatAttack",
                new OptDict<string, Option>{

                }
            },
            {
                "SpanshAttack",
                new OptDict<string, Option>{

                }
            },
            {
                "StreamAttack",
                new OptDict<string, Option>{

                }
            }
        };

        private class Option
        {
            public readonly string Name;
            public readonly dynamic DefaultValue;
            public readonly string VoiceTrigger;
            public string TtsDescription { get => ttsDescription ?? VoiceTrigger; }
            private readonly string? ttsDescription;
            public string Description { get => description ?? "No description available."; }
            public readonly string? description;

            public Option(string name, dynamic defaultValue, string voiceTrigger, string? ttsDescription = null, string? description = null)
                => (Name, DefaultValue, VoiceTrigger, this.ttsDescription, this.description) = (name, defaultValue, voiceTrigger, ttsDescription, description);

            public static implicit operator (string, Option)(Option o) => ( o.Name, o );
            public static explicit operator bool(Option o) => o.DefaultValue;
            public static explicit operator DateTime(Option o) => o.DefaultValue;
            public static explicit operator decimal(Option o) => o.DefaultValue;
            public static explicit operator int(Option o) => o.DefaultValue;
            public static explicit operator short(Option o) => o.DefaultValue;
            public static explicit operator string(Option o) => o.DefaultValue;
            public override string ToString() => DefaultValue.ToString();
        }
        private class OptDict<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public OptDict() : base() { }
            public OptDict(int capacity) : base(capacity) { }

            public void Add((TKey,TValue) tuple)
            {
                base.Add(tuple.Item1, tuple.Item2);
            }
        }

        public Configuration(dynamic vaProxy, VoiceAttackLog log, string id) => (VA, Log, ID) = (vaProxy, log, id);

        public dynamic GetDefault(string name)
        {
            return GetDefault(ID, name);
        }
        public static dynamic GetDefault(string id, string name)
        {
            return Defaults[id][name];
        }

        public void SetVoiceTriggers(System.Type type)
        {
            List<string> triggers = new List<string>();
            foreach (Dictionary<string,Option> options in Defaults.Values)
            {
                foreach (Option option in options.Values)
                {
                    if (option.DefaultValue.GetType() == type)
                    {
                        if (triggers.Contains(option.VoiceTrigger))
                        {
                            throw new ArgumentException($"Voice trigger '{option.VoiceTrigger}' is not unique, aborting …");
                        }
                        triggers.Add(option.VoiceTrigger);
                    }
                }
            }
            if (triggers.Count > 0)
            {
                string triggerString = string.Join(";", triggers);
                VA.SetText($"alterNERDtive-base.triggers.{type.Name}", triggerString);
                Log.Debug($"Voice triggers for {type.Name}: '{triggerString}'");
            }
            else
            {
                // make sure we don’t accidentally have weird things happening with empty config voice triggers
                string triggerString = $"tenuiadafesslejüsljlejutlesuivle{type.Name}";
                VA.SetText($"alterNERDtive-base.triggers.{type.Name}", triggerString);
                Log.Debug($"No voice triggers found for {type.Name}");
            }
        }

        public void SetVariablesForTrigger(dynamic vaProxy, string trigger)
        {
            _ = trigger ?? throw new ArgumentNullException("trigger");

            foreach (KeyValuePair<string,OptDict<string,Option>> options in Defaults)
            {
                try
                {
                    Option option = options.Value.First(item => item.Value.VoiceTrigger == trigger).Value;
                    vaProxy.SetText("~name", $"{options.Key}.{option.Name}");
                    vaProxy.SetText("~ttsDescription", option.TtsDescription);
                    vaProxy.SetText("~description", option.Description);
                    break;
                }
                catch (InvalidOperationException) { }
            }
        }

        public bool HasDefault(string name)
        {
            return HasDefault(ID, name);
        }
        public static bool HasDefault(string id, string name)
        {
            return Defaults[id].ContainsKey(name);
        }

        public dynamic? ApplyDefault(string name)
        {
            return ApplyDefault(ID, name);
        }
        public dynamic? ApplyDefault(string id, string name)
        {
            if (!HasDefault(id, name))
            {
                Log.Warn($"No default configuration value found for '{id}.{name}'");
                return null;
            }

            dynamic value = Defaults[id][name].DefaultValue;
            Log.Debug($"Loading default configuration value, '{id}.{name}': '{value}' …");
            string variable = $"{id}.{name}#";
            if (value is bool)
            {
                VA.SetBoolean(variable, value);
            }
            else if (value is DateTime)
            {
                VA.SetDate(variable, value);
            }
            else if (value is decimal)
            {
                VA.SetDecimal(variable, value);
            }
            else if (value is int)
            {
                VA.SetInt(variable, value);
            }
            else if (value is short)
            {
                VA.SetSmallInt(variable, value);
            }
            else if (value is string)
            {
                VA.SetText(variable, value);
            }
            else
            {
                throw new InvalidDataException($"Invalid data type for option '{id}.{name}': '{value}'");
            }
            return value;
        }
        public void ApplyDefaults()
        {
            ApplyDefaults(ID);
        }
        public void ApplyAllDefaults()
        {
            foreach (string id in Defaults.Keys)
            {
                ApplyDefaults(id);
            }
        }
        public void ApplyDefaults(string id)
        {
            foreach (string key in Defaults[id].Keys)
            {
                ApplyDefault(id, key);
            }
        }
    }

    public class PythonProxy
    {
        public static Process SetupPythonScript(string path, string arguments)
        {
            Process p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            return p;
        }
    }

    public enum LogLevel
    {
        ERROR,
        WARN,
        NOTICE,
        INFO,
        DEBUG
    }

    public class VoiceAttackCommands
    {
        private readonly dynamic VA;
        private readonly VoiceAttackLog Log;
        public VoiceAttackCommands(dynamic vaProxy, VoiceAttackLog log) => (VA, Log) = (vaProxy, log);

        public void Run(string command, bool logMissing = true, bool wait = false, bool subcommand = false, Action<Guid?>? callback = null, dynamic[]? parameters = null)
        {
            _ = command ?? throw new ArgumentNullException("command");

            if (VA.Command.Exists(command))
            {
                Log.Debug($"Parsing arguments for command '{command}' …");

                string[]? strings = null;
                int[]? integers = null;
                decimal[]? decimals = null;
                bool[]? booleans = null;
                DateTime[]? dates = null; // this might not work!

                if (parameters != null)
                {
                    foreach (var values in parameters)
                    {
                        if (values.GetType() == typeof(string[]))
                            strings = values as string[];
                        else if (values.GetType() == typeof(int[]))
                            integers = values as int[];
                        else if (values.GetType() == typeof(decimal[]))
                            decimals = values as decimal[];
                        else if (values.GetType() == typeof(bool[]))
                            booleans = values as bool[];
                        else if (values.GetType() == typeof(DateTime[]))
                            dates = values as DateTime[];
                    }
                }

                Log.Debug($"Running command '{command}' …");

                VA.Command.Execute(
                    CommandPhrase: command,
                    WaitForReturn: wait,
                    AsSubcommand: subcommand,
                    CompletedAction: callback,
                    PassedText: strings == null ? null : $@"""{String.Join<string>(@""";""", strings)}""",
                    PassedIntegers: integers == null ? null : String.Join<int>(";", integers),
                    PassedDecimals: decimals == null ? null : String.Join<decimal>(";", decimals),
                    PassedBooleans: booleans == null ? null : String.Join<bool>(";", booleans),
                    PassedDates: dates == null ? null : String.Join<DateTime>(";", dates)
                    );
            }
            else
            {
                if (logMissing)
                    Log.Warn($"Tried running missing command '{command}'.");
            }
        }
        public void RunAll(IEnumerable<string> prefixes, string command, bool logMissing = true, bool wait = false, bool subcommand = false, Action<Guid?>? callback = null, dynamic[]? parameters = null)
        {
            foreach (string prefix in prefixes)
                Run($"{prefix}.{command}", logMissing, wait, subcommand, callback, parameters);
        }

        public void TriggerEvent(string name, bool logMissing = true, bool wait = false, bool subcommand = false, Action<Guid?>? callback = null, dynamic[]? parameters = null)
        {
            Run($"(({name}))", logMissing, wait, subcommand, callback, parameters);
        }
        public void TriggerEventAll(IEnumerable<string> prefixes, string name, bool logMissing = true, bool wait = false, bool subcommand = false, Action<Guid?>? callback = null, dynamic[]? parameters = null)
        {
            foreach (string prefix in prefixes)
                Run($"(({prefix}.{name}))", logMissing, wait, subcommand, callback, parameters);
        }
    }

    public class VoiceAttackLog
    {
        private readonly dynamic VA;
        private readonly string ID;

        private static readonly string[] LogColour = { "red", "yellow", "green", "blue", "gray" };
        public LogLevel? CurrentLogLevel
        {
            get => currentLogLevel ?? LogLevel.NOTICE;
            set
            {
                currentLogLevel = value;
                Notice($"Log level set to {value ?? LogLevel.NOTICE}.");
            }
        }
        public void SetCurrentLogLevel(string level)
        {
            if (level == null)
                CurrentLogLevel = null;
            else
                CurrentLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), level.ToUpper());
        }
        private static LogLevel? currentLogLevel;

        public VoiceAttackLog(dynamic vaProxy, string id) => (VA, ID) = (vaProxy, id);

        public void Log(string message, LogLevel level = LogLevel.INFO)
        {
            Log(ID, message, level);
        }

        public void Log(string sender, string message, LogLevel level = LogLevel.INFO)
        {
            _ = sender ?? throw new ArgumentNullException("sender");
            _ = message ?? throw new ArgumentNullException("message");

            if (level <= CurrentLogLevel)
                VA.WriteToLog($"{level} | {sender}: {message}", LogColour[(int)level]);
        }

        public void Error(string message) => Log(message, LogLevel.ERROR);
        public void Warn(string message) => Log(message, LogLevel.WARN);
        public void Notice(string message) => Log(message, LogLevel.NOTICE);
        public void Info(string message) => Log(message, LogLevel.INFO);
        public void Debug(string message) => Log(message, LogLevel.DEBUG);
    }

    public interface IPipable
    {
        public void ParseString(string serialization);
        public string ToString();
    }

    public class PipeServer<Thing> where Thing : IPipable, new()
    {
        private readonly string PipeName;
        private readonly SignalHandler Handler;
        private readonly VoiceAttackLog Log;

        private bool Running = false;

        private NamedPipeServerStream? Server;

        public PipeServer(VoiceAttackLog log, string name, SignalHandler handler)
            => (Log, PipeName, Handler) = (log, name, handler);

        public delegate void SignalHandler(Thing thing);

        public PipeServer<Thing> Run()
        {
            Log.Debug($"Starting '{PipeName}' pipe …");
            if (!Running)
            {
                Running = true;
                WaitForConnection();
            }
            return this;
        }

        public PipeServer<Thing> Stop()
        {
            Log.Debug($"Stopping '{PipeName}' pipe …");
            if (Running)
            {
                Running = false;
                Server!.Close();
            }
            return this;
        }

        private void WaitForConnection()
        {
            try
            {
                Server = new NamedPipeServerStream(PipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                Server.BeginWaitForConnection(OnConnect, Server);
            }
            catch (Exception e)
            {
                Log.Error($"Error setting up pipe: {e.Message}");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            NamedPipeServerStream server = (NamedPipeServerStream)ar.AsyncState;
            try
            {
                server.EndWaitForConnection(ar);
                WaitForConnection();
                using StreamReader reader = new StreamReader(server);
                Thing thing = new Thing();
                thing.ParseString(reader.ReadToEnd());
                Handler(thing);
            }
            catch (ObjectDisposedException)
            {
                Log.Debug($"'{PipeName}' pipe has been closed.");
            }
            catch (Exception e)
            {
                Log.Error($"Error reading pipe: {e.Message}");
            }
        }
    }
}
