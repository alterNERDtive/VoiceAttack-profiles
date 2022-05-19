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
        private readonly VoiceAttackCommands Commands;
        private static readonly Dictionary<string, OptDict<string, Option>> Defaults = new Dictionary<string, OptDict<string, Option>>
        {
            {
                "alterNERDtive-base",
                new OptDict<string, Option>{
                    { new Option<decimal>("delays.keyPressDuration", (decimal)0.01, voiceTrigger: "key press duration",
                        description: "The time keys will be held down for.") },
                    { new Option<decimal>("delays.quitToDesktop", (decimal)10.0, voiceTrigger: "quit to desktop delay",
                        description: "The delay before restarting the game after hitting “Exit to Desktop”, in seconds.\nDefault: 10.0. (Used by the `restart from desktop` command)") },
                    { new Option<bool>("eddi.quietMode", true, voiceTrigger: "eddi quiet mode",
                        description: "Make EDDI shut up. Disables all built-in speech responders.") },
                    { new Option<string>("elite.pasteKey", "v", voiceTrigger: "elite paste key",
                        description: "The key used to paste in conjunction with CTRL. The physical key in your layout that would be 'V' on QWERTY.") },
                    { new Option<bool>("enableAutoUpdateCheck", true, voiceTrigger: "auto update check",
                        description: "Automatically check Github for profiles updates when the profile loads.") },
                    { new Option<string>("log.logLevel", "NOTICE", voiceTrigger: "log level", validValues: new List<string>{ "ERROR", "WARN", "NOTICE", "INFO", "DEBUG" },
                        description: "The level of detail for logging to the VoiceAttack log.\nValid levels are \"ERROR\", \"WARN\", \"NOTICE\", \"INFO\" and \"DEBUG\".\nDefault: \"NOTICE\".") },
                }
            },
            {
                "EliteAttack",
                new OptDict<string, Option>{
                    { new Option<bool>("announceEdsmSystemStatus", true, voiceTrigger: "edsm system status",
                        description: "Pull system data from EDSM and compare it against your discovery scan.") },
                    { new Option<bool>("announceJumpsInRoute", true, voiceTrigger: "route jump count",
                        description: "Give a jump count on plotting a route.") },
                    { new Option<bool>("announceMappingCandidates", true, voiceTrigger: "mapping candidates",
                        description: "Announce bodies worth mapping when you have finished scanning a system.\n(Terraformables, Water Worlds, Earth-Like Worlds and Ammonia Worlds that have not been mapped yet.)") },
                    { new Option<bool>("announceOutdatedStationData", true, voiceTrigger: "outdated stations",
                        description: "Announce stations with outdated data in the online databases.") },
                    { new Option<int>("outdatedStationThreshold", 365, voiceTrigger: "outdated station threshold",
                        description: "The threshold for station data to count as “outdated”, in days.\nDefault: 365.") },
                    { new Option<bool>("announceR2RMappingCandidates", false, voiceTrigger: "road to riches",
                        description: "Announce bodies worth scanning if you are looking for some starting cash on the Road to Riches.") },
                    { new Option<bool>("announceRepairs", true, voiceTrigger: "repair reports",
                        description: "Report on AFMU repairs.") },
                    { new Option<bool>("announceSynthesis", true, voiceTrigger: "synthesis reports",
                        description: "Report on synthesis.") },
                    { new Option<bool>("autoHonkNewSystems", true, voiceTrigger: "auto honk new systems",
                        description: "Automatically honk upon entering a system if it is your first visit.") },
                    { new Option<bool>("autoHonkAllSystems", false, voiceTrigger: "auto honk all systems",
                        description: "Automatically honk upon entering a system, each jump, without constraints.") },
                    { new Option<int>("scannerFireGroup", 0, voiceTrigger: "scanner fire group",
                        description: "The fire group your discovery scanner is assigned to.\nDefault: 0 (the first one).") },
                    { new Option<bool>("usePrimaryFireForDiscoveryScan", false, voiceTrigger: "discovery scan on primary fire",
                        description: "Use primary fire for honking instead of secondary.") },
                    { new Option<bool>("autoRefuel", true, voiceTrigger: "auto refuel",
                        description: "Automatically refuel after docking at a station.") },
                    { new Option<bool>("autoRepair", true, voiceTrigger: "auto repair",
                        description: "Automatically repair after docking at a station.") },
                    { new Option<bool>("autoRestock", true, voiceTrigger: "auto restock",
                        description: "Automatically restock after docking at a station.") },
                    { new Option<bool>("autoHangar", true, voiceTrigger: "auto move to hangar",
                        description: "Automatically move the ship to the hangar after docking at a station.") },
                    { new Option<bool>("autoStationService", true, voiceTrigger: "auto enter station services",
                        description: "Automatically enter the Station Services menu after docking at a station.") },
                    { new Option<bool>("autoRetractLandingGear", true, voiceTrigger: "auto retract landing gear",
                        description: "Automatically retract landing gear when lifting off a planet / undocking from a station.") },
                    { new Option<bool>("autoDisableSrvLights", true, voiceTrigger: "auto disable s r v lights",
                        description: "Automatically turn SRV lights off when deploying one.") },
                    { new Option<bool>("flightAssistOff", false, voiceTrigger: "flight assist off",
                        description: "Permanent Flight Assist off mode. You should really do that, it’s great.") },
                    { new Option<bool>("hyperspaceDethrottle", true, voiceTrigger: "hyper space dethrottle",
                        description: "Throttle down after a jump and when dropping from SC. Like the SC Assist module does.") },
                    { new Option<bool>("limpetCheck", true, voiceTrigger: "limpet check",
                        description: "Do a limpet check when undocking, reminding you if you forgot to buy some.") },
                }
            },
            {
                "RatAttack",
                new OptDict<string, Option>{
                    { new Option<bool>("autoCloseCase", false, voiceTrigger: "auto close fuel rat case",
                        description: "Automatically close a rat case when sending “fuel+” via voice command or ingame chat.") },
                    { new Option<bool>("autoCopySystem", true, voiceTrigger: "auto copy rat case system",
                        description: "Automatically copy the client’s system to the clipboard when you open a rat case.") },
                    { new Option<bool>("announceNearestCMDR", false, voiceTrigger: "nearest commander to fuel rat case",
                        description: "Announce the nearest commander to incoming rat cases.") },
                    { new Option<string>("CMDRs", "", voiceTrigger: "fuel rat commanders",
                        description: "All your CMDRs that are ready to take rat cases.\nUse ‘;’ as separator, e.g. “Bud Spencer;Terrence Hill”.") },
                    { new Option<bool>("announcePlatform", false, voiceTrigger: "platform for fuel rat case",
                        description: "Announce the platform for incoming rat cases.") },
                    { new Option<string>("platforms", "PC", voiceTrigger: "fuel rat platforms", validValues: new List<string>{ "PC", "Xbox", "Playstation" },
                        description: "The platform(s) you want to get case announcements for (PC, Xbox, Playstation).\nUse ‘;’ as separator, e.g. “PC;Xbox”.") },
                    { new Option<bool>("announceSystemInfo", true, voiceTrigger: "system information for fuel rat case",
                        description: "System information provided by Mecha.")},
                    { new Option<bool>("confirmCalls", true, voiceTrigger: "fuel rat call confirmation",
                        description: "Only make calls in #fuelrats after vocal confirmation to prevent mistakes.") },
                    { new Option<bool>("onDuty", true, voiceTrigger: "fuel rat duty",
                        description: "On duty, receiving case announcements via TTS.") },
                }
            },
            {
                "SpanshAttack",
                new OptDict<string, Option>{
                    { new Option<string>("announceJumpsLeft", ";1;3;5;10;15;20;30;50;75;100;", voiceTrigger: "announce jumps left",
                        description: "Estimated jumps left to announce when reached.\nNEEDS to have leading and trailing “;”.") },
                    { new Option<bool>("announceWaypoints", true, voiceTrigger: "waypoint announcements",
                        description: "Announce each waypoint by name.") },
                    { new Option<bool>("autoJumpAfterScooping", true, voiceTrigger: "auto jump after scooping",
                        description: "Automatically jump out when fuel scooping is complete.") },
                    { new Option<bool>("autoPlot", true, voiceTrigger: "auto plot",
                        description: "Automatically plot to the next waypoint after supercharging.") },
                    { new Option<bool>("clearOnShutdown", true, voiceTrigger: "clear neutron route on shutdown",
                        description: "Clear an active neutron route when the game is shut down.") },
                    { new Option<bool>("copyWaypointToClipboard", false, voiceTrigger: "copy neutron waypoints to clipboard",
                        description: "Copy each neutron waypoint into the Windows clipboard.") },
                    { new Option<bool>("defaultToLadenRange", false, voiceTrigger: "default to laden range",
                        description: "Default to the current ship’s laden range as reported by EDDI instead of prompting for input.") },
                    { new Option<bool>("timeTrip", false, voiceTrigger: "time neutron route",
                        description: "Keep track of how long a neutron route takes you to complete.") },
                }
            },
            {
                "StreamAttack",
                new OptDict<string, Option>{
                    { new Option<string>("outputDir", @"%appdata%\StreamAttack\", voiceTrigger: "StreamAttack output directory",
                        description: "The directory the status files are written to.") }
                }
            }
        };

        public abstract class Option 
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public string Name { get; }
            public dynamic DefaultValue { get;  }
            public List<dynamic>? ValidValues { get; }
            public string VoiceTrigger { get; }
            public string TtsDescription { get; }
            public string Description { get; }
            public Type Type { get; }

            public string? TypeString { get; }
            public override string ToString() => DefaultValue!.ToString();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        }
        public class Option<T> : Option
        {
            public new string Name { get; }
            public new T DefaultValue { get; }
            public new List<T>? ValidValues { get; }
            public new string VoiceTrigger { get; }
            public new string TtsDescription { get => ttsDescription ?? VoiceTrigger; }
            private readonly string? ttsDescription;
            public new string Description { get => description ?? "No description available."; }
            private readonly string? description;

            public new Type Type { get => typeof(T); }
            public new string? TypeString
            {
                get
                {
                    string? type = null;
                    if (typeof(T) == typeof(bool))
                    {
                        type = "boolean";
                    }
                    else if (typeof(T) == typeof(DateTime))
                    {
                        type = "date";
                    }
                    else if (typeof(T) == typeof(decimal))
                    {
                        type = "decimal";
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        type = "int";
                    }
                    else if (typeof(T) == typeof(short))
                    {
                        type = "smallint";
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        type = "text";
                    }
                    return type;
                }
            }

            public Option(string name, T defaultValue, string voiceTrigger, List<T>? validValues = null, string? ttsDescription = null, string? description = null)
                => (Name, DefaultValue, VoiceTrigger, ValidValues, this.ttsDescription, this.description) = (name, defaultValue, voiceTrigger, validValues, ttsDescription, description);


            public static implicit operator (string, Option)(Option<T> o) => (o.Name, o);
            public static explicit operator T(Option<T> o) => o.DefaultValue;
        }
        public class OptDict<TKey, TValue> : Dictionary<TKey, TValue>
        {
            public OptDict() : base() { }
            public OptDict(int capacity) : base(capacity) { }

            public void Add((TKey,TValue) tuple)
            {
                base.Add(tuple.Item1, tuple.Item2);
            }
        }

        public Configuration(dynamic vaProxy, VoiceAttackLog log, VoiceAttackCommands commands, string id) => (VA, Log, Commands, ID) = (vaProxy, log, commands, id);

        public dynamic GetDefault(string name)
        {
            return GetDefault(ID, name);
        }
        public static dynamic GetDefault(string id, string name)
        {
            return Defaults[id][name];
        }

        public Option GetOption(string name)
        {
            return GetOption(ID, name);
        }
        public Option GetOption(string id, string name)
        {
            return Defaults[id][name];
        }

        public OptDict<string, Option> GetOptions(string id)
        {
            return Defaults[id];
        }

        public void SetVoiceTriggers(System.Type type)
        {
            List<string> triggers = new List<string>();
            foreach (Dictionary<string,Option> options in Defaults.Values)
            {
                foreach (dynamic option in options.Values)
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
                    dynamic option = options.Value.First(item => ((dynamic)item.Value).VoiceTrigger.ToLower() == trigger).Value;
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

        public void LoadFromProfile()
        {
            foreach (KeyValuePair<string,OptDict<string,Option>> options in Defaults)
            {
                LoadFromProfile(options.Key);
            }
        }
        public void LoadFromProfile(string id)
        {
            foreach (dynamic option in Defaults[id].Values)
            {
                string name = $"{id}.{option.Name}";
                string type = option.TypeString ?? throw new InvalidDataException($"Invalid data type for option '{name}': '{option}'");
                Log.Debug($"Loading value for option '{name}' from profile …");
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#", type } });
            }
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

            dynamic option = Defaults[id][name];
            dynamic value = option.DefaultValue;
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

        public void DumpConfig()
        {
            foreach (string id in Defaults.Keys)
            {
                DumpConfig(id);
            }
        }
        public void DumpConfig(string id)
        {
            Log.Notice($"===== {id} configuration: =====");
            foreach (string name in Defaults[id].Keys)
            {
                DumpConfig(id, name);
            }
        }
        public void DumpConfig(string id, string name)
        {
            dynamic defaultValue = ((dynamic)Defaults[id][name]).DefaultValue;
            dynamic value = GetConfig(id, name);
            Log.Notice($"{id}.{name}# = {value}{(value == defaultValue ? " (default)" : "")}");
        }
        public dynamic GetConfig(string id, string name)
        {
            dynamic defaultValue = ((dynamic)Defaults[id][name]).DefaultValue;
            string variable = $"{id}.{name}#";
            dynamic value;
            if (defaultValue is bool)
            {
                value = VA.GetBoolean(variable);
            }
            else if (defaultValue is DateTime)
            {
                value = VA.GetDate(variable);
            }
            else if (defaultValue is decimal)
            {
                value = VA.GetDecimal(variable);
            }
            else if (defaultValue is int)
            {
                value = VA.GetInt(variable);
            }
            else if (defaultValue is short)
            {
                value = VA.GetSmallInt(variable);
            }
            else if (defaultValue is string)
            {
                value = VA.GetText(variable);
            }
            else
            {
                throw new InvalidDataException($"Invalid data type for option '{id}.{name}': '{defaultValue}'");
            }
            return value;
        }
        public void SetConfig(string id, string name, dynamic value)
        {
            string variable = $"{id}.{name}#";
            if (value is bool)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new bool[] { value } }); ;
            }
            else if (value is DateTime)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new DateTime[] { value } });
            }
            else if (value is decimal)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new decimal[] { value } });
            }
            else if (value is int)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new int[] { value } });
            }
            else if (value is short)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new short[] { value } });
            }
            else if (value is string)
            {
                Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}", value } });
            }
            else
            {
                throw new InvalidDataException($"Invalid data type for option '{id}.{name}': '{value}'");
            }
        }

        public void ListConfig()
        {
            foreach (string id in Defaults.Keys)
            {
                ListConfig(id);
            }
        }
        public void ListConfig(string id)
        {
            Log.Notice($"===== {id} settings: =====");
            foreach (string name in Defaults[id].Keys)
            {
                ListConfig(id, name);
            }
        }
        public void ListConfig(string id, string name)
        {
            dynamic option = Defaults[id][name];
            Log.Notice($"“{option.VoiceTrigger}”: {option.Description}");
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

        //private static readonly string LogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "alterNERDtive-logs");
        //private static readonly TextWriterTraceListener LogListener = new TextWriterTraceListener(Path.Combine(LogFolder, "VoiceAttack.log"), "error");
        //private static readonly TextWriterTraceListener DebugListener = new TextWriterTraceListener(Path.Combine(LogFolder, "VoiceAttack-debug.log"), "debug");
        //private readonly TraceSource TraceSource;

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

        //static VoiceAttackLog()
        //{
        //    Directory.CreateDirectory(LogFolder);
        //    string path = Path.Combine(LogFolder, $"VoiceAttack");
        //    foreach (string name in new String[] { "", "-debug" })
        //    {
        //        File.Delete($"{path}{name}-5.log");
        //        for (int i = 4; i > 0; i--)
        //        {
        //            if (File.Exists($"{path}{name}-{i}.log"))
        //            {
        //                File.Move($"{path}{name}-{i}.log", $"{path}{name}-{i + 1}.log");
        //            }
        //        }
        //        File.Move($"{path}{name}.log", $"{path}{name}-1.log");
        //    }

        //    File.Delete(Path.Combine(LogFolder, "VoiceAttack.log"));
        //    File.Delete(Path.Combine(LogFolder, "VoiceAttack-debug.log"));

        //    DebugListener.TraceOutputOptions = LogListener.TraceOutputOptions = TraceOptions.DateTime;
        //    LogListener.Filter = new EventTypeFilter(SourceLevels.Information);
        //    DebugListener.Filter = new EventTypeFilter(SourceLevels.All);
        //}

        public VoiceAttackLog(dynamic vaProxy, string id)
        {
            VA = vaProxy;
            ID = id;

            //TraceSource = new TraceSource(ID);
            //TraceSource.Listeners.Add(LogListener);
            //TraceSource.Listeners.Add(DebugListener);
            //TraceSource.Switch.Level = SourceLevels.All;
        }

        public void Log(string message, LogLevel level = LogLevel.INFO)
        {
            _ = message ?? throw new ArgumentNullException("message");

            if (level <= CurrentLogLevel)
            {
                VA.WriteToLog($"{level} | {ID}: {message}", LogColour[(int)level]);
            }
            //switch(level)
            //{
            //    case LogLevel.ERROR:
            //        TraceSource.TraceEvent(TraceEventType.Error, 0, message);
            //        break;
            //    case LogLevel.WARN:
            //        TraceSource.TraceEvent(TraceEventType.Warning, 1, message);
            //        break;
            //    case LogLevel.NOTICE:
            //    case LogLevel.INFO:
            //        TraceSource.TraceEvent(TraceEventType.Information, 2, message);
            //        break;
            //    case LogLevel.DEBUG:
            //        TraceSource.TraceEvent(TraceEventType.Verbose, 3, message);
            //        break;
            //}
            //TraceSource.Flush();
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
