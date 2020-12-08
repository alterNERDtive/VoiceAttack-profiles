#nullable enable

using alterNERDtive.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace alterNERDtive
{
    public class BasePlugin
    {
        private static dynamic? VA { get; set; }
        private static readonly Dictionary<Guid, string> Profiles = new Dictionary<Guid, string> {
            { new Guid("{f31b575b-6ce4-44eb-91fc-7459e55013cf}"), "EliteAttack" },
            { new Guid("{87276668-2a6e-4d80-af77-80651daa58b7}"), "RatAttack" },
            { new Guid("{e722b29d-898e-47dd-a843-a409c87e0bd8}"), "SpanshAttack" },
            { new Guid("{05580e6c-442c-46cd-b36f-f5a1f967ec59}"), "StreamAttack" }
        };
        private static readonly List<string> ActiveProfiles = new List<string>();

        private static readonly Regex ConfigurationVariableRegex = new Regex(@$"(?<id>(alterNERDtive-base|{String.Join("|", Profiles.Values)}))\.(?<name>.+)#");

        private static VoiceAttackLog Log => log ??= new VoiceAttackLog(VA, "alterNERDtive-base");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

        private static Configuration Config => config ??= new Configuration(VA, Log, Commands, "alterNERDtive-base");
        private static Configuration? config;

        private static void CheckProfiles(dynamic vaProxy)
        {
            ActiveProfiles.Clear();

            foreach (KeyValuePair<Guid, string> entry in Profiles)
            {
                if (vaProxy.Profile.Exists(entry.Key))
                    ActiveProfiles.Add(entry.Value);
            }
            Log.Debug($"Profiles found: {string.Join<string>(", ", ActiveProfiles)}");
        }

        /*================\
        | plugin contexts |
        \================*/

        private static void Context_Config_Dump(dynamic vaProxy)
        {
            Config.DumpConfig();
        }

        private static void Context_Config_Setup(dynamic vaProxy)
        {
            Log.Debug("Loading default configuration …");
            Config.ApplyAllDefaults();
            foreach (System.Type type in new List<System.Type> { typeof(bool), typeof(DateTime), typeof(decimal), typeof(int), typeof(short), typeof(string) })
            {
                Config.SetVoiceTriggers(type);
            }
            Config.LoadFromProfile();
            Log.Debug("Finished loading configuration.");
        }

        private static void Context_Config_SetVariables(dynamic vaProxy)
        {
            string trigger = vaProxy.GetText("~trigger");
            Log.Debug($"Loading variables for trigger '{trigger}' …");
            Config.SetVariablesForTrigger(vaProxy, trigger);
        }

        private static void Context_Config_VersionMigration(dynamic vaProxy)
        {
            // ===========
            // === 4.0 ===
            // ===========

            // EliteAttack
            string prefix = "EliteAttack";
            string oldPrefix = "EliteDangerous";
            foreach (string option in new string[] { "announceEdsmSystemStatus", "announceMappingCandidates", "announceOutdatedStationData", "announceR2RMappingCandidates", "autoRestock", "flightAssistOff", "hyperspaceDethrottle" })
            {
                string name = $"{prefix}.{option}";
                string oldName = $"{oldPrefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{oldName}", "boolean" } });
                bool? value = VA!.GetBoolean(oldName);
                if (value != null)
                {
                    Log.Info($"Migrating option {oldName} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#" }, new bool[] { (bool)value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{oldName}", "boolean" } });
                }
            }

            // RatAttack
            prefix = "RatAttack";
            foreach (string option in new string[] { "autoCloseCase", "announceNearestCMDR", "announcePlatform", "confirmCalls", "onDuty" })
            {
                string name = $"{prefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "boolean" } });
                bool? value = VA!.GetBoolean(name);
                if (value != null)
                {
                    Log.Info($"Migrating option {name} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#" }, new bool[] { (bool)value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "boolean" } });
                }
            }
            foreach (string option in new string[] { "CMDRs", "platforms" })
            {
                string name = $"{prefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                string value = VA!.GetText(name);
                if (!string.IsNullOrEmpty(value))
                {
                    Log.Info($"Migrating option {name} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#", value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                }
            }

            // SpanshAttack
            prefix = "SpanshAttack";
            foreach (string option in new string[] { "announceWaypoints", "autoJumpAfterScooping", "autoPlot", "clearOnShutdown", "copyWaypointToClipboard", "defaultToLadenRange", "timeTrip" })
            {
                string name = $"{prefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "boolean" } });
                bool? value = VA!.GetBoolean(name);
                if (value != null)
                {
                    Log.Info($"Migrating option {name} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#" }, new bool[] { (bool)value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "boolean" } });
                }
            }
            foreach (string option in new string[] { "announceJumpsLeft" })
            {
                string name = $"{prefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                string value = VA!.GetText(name);
                if (!string.IsNullOrEmpty(value))
                {
                    Log.Info($"Migrating option {name} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#", value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                }
            }

            // StreamAttack
            prefix = "StreamAttack";
            foreach (string option in new string[] { "outputDir" })
            {
                string name = $"{prefix}.{option}";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                string value = VA!.GetText(name);
                if (!string.IsNullOrEmpty(value))
                {
                    Log.Info($"Migrating option {name} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#", value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}", "text" } });
                }
            }
        }

        private static void Context_DistanceBetween(dynamic vaProxy)
        {
            string fromSystem = vaProxy.GetText("~fromSystem");
            string toSystem = vaProxy.GetText("~toSystem");
            int roundTo = vaProxy.GetInt("~roundTo") ?? 2;

            string path = $"{vaProxy.GetText("Python.ScriptPath")}\\explorationtools.exe";
            string arguments = $@"distancebetween --roundto {roundTo} ""{fromSystem}"" ""{toSystem}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            decimal distance = 0;
            bool error = false;
            string errorMessage = "";

            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            switch (p.ExitCode)
            {
                case 0:
                    distance = decimal.Parse(stdout);
                    break;
                case 1:
                case 2:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = stderr;
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;

            }
            Log.Info($"{fromSystem} → {toSystem}: {distance} ly");

            vaProxy.SetDecimal("~distance", distance);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_EddiEvent(dynamic vaProxy)
        {
            string eddiEvent = vaProxy.Command.Name();
            string command = eddiEvent.Substring(2, eddiEvent.Length - 4);
            Log.Debug($"Running EDDI event '{command}' …");
            Commands.RunAll(ActiveProfiles, command, logMissing: false, subcommand: true); // FIXXME: a) triggerAll or something, b) change all profiles to use "((<name>.<event>))" over "<name>.<event>"
        }

        private static void Context_Log(dynamic vaProxy)
        {
            string sender = vaProxy.GetText("~sender");
            string message = vaProxy.GetText("~message");
            string level = vaProxy.GetText("~level");

            if (level == null)
            {
                Log.Log(sender, message);
            }
            else
            {
                try
                {
                    Log.Log(sender, message, (LogLevel)Enum.Parse(typeof(LogLevel), level.ToUpper()));
                }
                catch (ArgumentNullException) { throw; }
                catch (ArgumentException)
                {
                    Log.Error($"Invalid log level '{level}'.");
                }
            }
        }

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            CheckProfiles(VA);
            Log.Notice($"Active profiles: {string.Join(", ", ActiveProfiles)}");
            Commands.TriggerEventAll(ActiveProfiles, "startup", logMissing: true);
            Log.Notice("Finished startup.");
        }

        private static void ConfigurationChanged(string option, dynamic? from, dynamic? to, Guid? guid = null)
        {
            try
            {
                Match match = ConfigurationVariableRegex.Match(option);
                if (match.Success)
                {
                    string id = match.Groups["id"].Value;
                    string name = match.Groups["name"].Value;
                    Log.Debug($"Configuration has changed, '{id}.{name}': '{from}' → '{to}'");

                    Configuration.Option o = Config.GetOption(id, name);
                    // When loaded from profile but not explicitly set, will be null.
                    // Then load default.
                    // Same applies to resetting a saved option (= saving null to the profile).
                    if (to == null)
                    {
                        _ = to ?? Config.ApplyDefault(id, name);
                    }
                    // When not null, check if there’s a constraint on valid values.
                    else if (o.ValidValues != null && !o.ValidValues.Contains(to))
                    {
                        Log.Error($@"Invalid value ""{to}"" for option ""{id}.{option}"", reverting to default …");
                        Config.ApplyDefault(id, name);
                    }
                    else
                    {
                        if (option == "alterNERDtive-base.eddi.quietMode#" && VA!.GetText("EDDI version") != null) // if null, EDDI isn’t up yet
                        {
                            Log.Debug($"Resetting speech responder ({(to ?? false ? "off" : "on")}) …");
                            Commands.Run("alterNERDtive-base.setEDDISpeechResponder");
                        }
                        else if (option == "alterNERDtive-base.log.logLevel#")
                        {
                            Log.SetCurrentLogLevel(to);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unhandled exception while handling changed variable '{option}'. ({e.Message})");
            }
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly string VERSION = "4.0.0";

        public static Guid VA_Id()
            => new Guid("{F7F59CFD-1AE2-4A7E-8F62-C62372418BAC}");
        public static string VA_DisplayName()
            => $"alterNERDtive-base {VERSION}";
        public static string VA_DisplayInfo()
            => "The alterNERDtive plugin to manage all the alterNERDtive profiles!";

        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            vaProxy.BooleanVariableChanged += new Action<String, Boolean?, Boolean?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.DateVariableChanged += new Action<String, DateTime?, DateTime?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.DecimalVariableChanged += new Action<String, decimal?, decimal?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.IntegerVariableChanged += new Action<String, int?, int?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.TextVariableChanged += new Action<String, String, String, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            VA.SetBoolean("alterNERDtive-base.initialized", true);
            Commands.TriggerEvent("alterNERDtive-base.initialized", wait: false);
            Log.Notice("Init successful.");
        }

        public static void VA_Invoke1(dynamic vaProxy)
        {
            VA = vaProxy;

            string context = vaProxy.Context.ToLower();
            Log.Debug($"Running context '{context}' …");
            try
            {
                switch (context)
                {
                    case "startup":
                        Context_Startup(vaProxy);
                        break;
                    // config
                    case "config.dump":
                        Context_Config_Dump(vaProxy);
                        break;
                    case "config.getvariables":
                        Context_Config_SetVariables(vaProxy);
                        break;
                    case "config.setup":
                        Context_Config_Setup(vaProxy);
                        break;
                    case "config.versionmigration":
                        Context_Config_VersionMigration(vaProxy);
                        break;
                    // EDSM
                    case "edsm.distancebetween":
                        Context_DistanceBetween(vaProxy);
                        break;
                    // EDDI
                    case "eddi.event":
                        Context_EddiEvent(vaProxy);
                        break;
                    // log
                    case "log.log":
                        Context_Log(vaProxy);
                        break;
                    // invalid
                    default:
                        Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error($"Missing parameter '~{e.ParamName}' for context '{context}'");
            }
            catch (Exception e)
            {
                Log.Error($"Unhandled exception while executing plugin context '{context}'. ({e.Message})");
            }
        }

        public static void VA_Exit1(dynamic vaProxy) { }

        public static void VA_StopCommand() { }
    }
}