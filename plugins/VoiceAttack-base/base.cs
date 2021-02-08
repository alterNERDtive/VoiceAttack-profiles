#nullable enable

using alterNERDtive.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace alterNERDtive
{
    public class BasePlugin
    {
        private static dynamic? VA { get; set; }
        private static readonly Dictionary<Guid, string> Profiles = new Dictionary<Guid, string> {
            { new Guid("{F7F59CFD-1AE2-4A7E-8F62-C62372418BAC}"), "alterNERDtive-base" },
            { new Guid("{f31b575b-6ce4-44eb-91fc-7459e55013cf}"), "EliteAttack" },
            { new Guid("{87276668-2a6e-4d80-af77-80651daa58b7}"), "RatAttack" },
            { new Guid("{e722b29d-898e-47dd-a843-a409c87e0bd8}"), "SpanshAttack" },
            { new Guid("{05580e6c-442c-46cd-b36f-f5a1f967ec59}"), "StreamAttack" }
        };
        private static readonly List<string> ActiveProfiles = new List<string>();
        private static readonly List<string> InstalledProfiles = new List<string>();

        private static readonly Regex ConfigurationVariableRegex = new Regex(@$"(?<id>({String.Join("|", Profiles.Values)}))\.(?<name>.+)#");

        private static VoiceAttackLog Log => log ??= new VoiceAttackLog(VA, "alterNERDtive-base");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

        private static Configuration Config => config ??= new Configuration(VA, Log, Commands, "alterNERDtive-base");
        private static Configuration? config;

        private static void CheckProfiles(dynamic vaProxy)
        {
            ActiveProfiles.Clear();
            InstalledProfiles.Clear();

            foreach (KeyValuePair<Guid, string> profile in Profiles)
            {
                if (vaProxy.Command.Exists($"(({profile.Value}.startup))"))
                // Sadly there is no way to find _active_ profiles, so we have to check the one command that always is in them.
                {
                    ActiveProfiles.Add(profile.Value);
                }
                if (vaProxy.Profile.Exists(profile.Key))
                {
                    InstalledProfiles.Add(profile.Value);
                }
            }
            Log.Debug($"Profiles found: {string.Join<string>(", ", ActiveProfiles)}");
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

                    dynamic o = Config.GetOption(id, name);
                    // When loaded from profile but not explicitly set, will be null.
                    // Then load default.
                    // Same applies to resetting a saved option (= saving null to the profile).
                    if (to == null)
                    {
                        _ = to ?? Config.ApplyDefault(id, name);
                    }
                    else
                    {
                        // When not null, check if there’s a constraint on valid values.
                        if (o.ValidValues != null)
                        {
                            if (!o.ValidValues.Contains(to))
                            {
                                // Handle “arrays” of values
                                bool valid = false;
                                if (to is string && ((string)to).Contains(";"))
                                {
                                    valid = true;
                                    foreach (string value in ((string)to).Split(';'))
                                    {
                                        if (!o.ValidValues.Contains(value))
                                        {
                                            valid = false;
                                        }
                                    }
                                }

                                if (!valid)
                                {
                                    Log.Error($@"Invalid value ""{to}"" for option ""{id}.{option}"", reverting to default …");
                                    Config.ApplyDefault(id, name);
                                }
                            }
                        }

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

        private static void UpdateCheck()
        {
            Version latestVersion;
            try
            {
                latestVersion = new Version(new WebClient().DownloadString("https://raw.githubusercontent.com/alterNERDtive/VoiceAttack-profiles/release/VERSION"));
            }
            catch (Exception)
            {
                throw new Exception("Error fetching latest profiles version from Github.");
            }

            Log.Notice($"Local version: {VERSION}, latest release: {latestVersion}.");

            Commands.TriggerEvent("alterNERDtive-base.updateCheck",
                parameters: new dynamic[] { new string[] { VERSION.ToString(), latestVersion.ToString() }, new bool[] { VERSION.CompareTo(latestVersion) < 0 } });
        }

        /*================\
        | plugin contexts |
        \================*/

        private static void Context_Config_Dump(dynamic vaProxy)
        {
            Config.DumpConfig();
        }

        private static void Context_Config_List(dynamic vaProxy)
        {
            Config.ListConfig();
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
            string trigger = vaProxy.GetText("~trigger") ?? throw new ArgumentNullException("~trigger");
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

        private static void Context_Eddi_Event(dynamic vaProxy)
        {
            string eddiEvent = vaProxy.Command.Name();
            string command = eddiEvent.Substring(2, eddiEvent.Length - 4);
            Log.Debug($"Running EDDI event '{command}' …");
            Commands.RunAll(ActiveProfiles, command, logMissing: false, subcommand: true); // FIXXME: a) triggerAll or something, b) change all profiles to use "((<name>.<event>))" over "<name>.<event>"
        }

        private static void Context_EDSM_BodyCount(dynamic vaProxy)
        {
            string system = vaProxy.GetText("~system") ?? throw new ArgumentNullException("~system");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\explorationtools.exe";
            string arguments = $@"bodycount ""{system}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            int bodyCount = 0;
            bool error = false;
            string errorMessage = "";

            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            switch (p.ExitCode)
            {
                case 0:
                    bodyCount = int.Parse(stdout);
                    Log.Info($"EDSM body count for {system}: {bodyCount}");
                    break;
                case 1:
                    error = true;
                    Log.Error(stdout);
                    errorMessage = stdout;
                    break;
                case 2:
                    error = true;
                    Log.Notice($@"System ""{system}"" not found on EDSM");
                    errorMessage = stdout;
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;
            }

            vaProxy.SetInt("~bodyCount", bodyCount);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_EDSM_DistanceBetween(dynamic vaProxy)
        {
            string fromSystem = vaProxy.GetText("~fromSystem") ?? throw new ArgumentNullException("~fromSystem");
            string toSystem = vaProxy.GetText("~toSystem") ?? throw new ArgumentNullException("~toSystem");
            int roundTo = vaProxy.GetInt("~roundTo") ?? 2;

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\explorationtools.exe";
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
                    Log.Info($"{fromSystem} → {toSystem}: {distance} ly");
                    break;
                case 1:
                case 2:
                    error = true;
                    Log.Error(stdout);
                    errorMessage = stdout;
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;

            }

            vaProxy.SetDecimal("~distance", distance);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_Log(dynamic vaProxy)
        {
            string message = vaProxy.GetText("~message");
            string level = vaProxy.GetText("~level");

            if (level == null)
            {
                Log.Log(message);
            }
            else
            {
                try
                {
                    Log.Log(message, (LogLevel)Enum.Parse(typeof(LogLevel), level.ToUpper()));
                }
                catch (ArgumentNullException) { throw; }
                catch (ArgumentException)
                {
                    Log.Error($"Invalid log level '{level}'.");
                }
            }
        }

        private static void Context_Spansh_OutdatedStations(dynamic vaProxy)
        {
            string system = vaProxy.GetText("~system") ?? throw new ArgumentNullException("~system");
            int minage = vaProxy.GetInt("~minage") ?? throw new ArgumentNullException("~minage");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\spansh.exe";
            string arguments = $@"oldstations --system ""{system}"" --minage {minage}";

            Process p = PythonProxy.SetupPythonScript(path, arguments);


            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();

            string message = stdout;
            string? errorMessage = null;
            bool error = true;

            switch (p.ExitCode)
            {
                case 0:
                    error = false;
                    Log.Notice($"Outdated stations for {system}: {message}");
                    break;
                case 1:
                    error = true;
                    Log.Error(message);
                    break;
                case 3:
                    error = true;
                    Log.Info($@"No outdated stations found for ""{system}""");
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;

            }

            vaProxy.SetText("~message", message);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            CheckProfiles(VA);
            Log.Notice($"Active profiles: {string.Join(", ", ActiveProfiles)}");
            Commands.TriggerEventAll(ActiveProfiles, "startup", logMissing: false);
            Log.Notice("Finished startup.");
        }

        private static void Context_Update_Check(dynamic vaProxy)
        {
            UpdateCheck();
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly Version VERSION = new Version("4.0.3");

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
            VA.SetText("alterNERDtive-base.version", VERSION.ToString());
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
                    case "config.list":
                        Context_Config_List(vaProxy);
                        break;
                    case "config.setup":
                        Context_Config_Setup(vaProxy);
                        break;
                    case "config.versionmigration":
                        Context_Config_VersionMigration(vaProxy);
                        break;
                    // EDSM
                    case "edsm.bodycount":
                        Context_EDSM_BodyCount(vaProxy);
                        break;
                    case "edsm.distancebetween":
                        Context_EDSM_DistanceBetween(vaProxy);
                        break;
                    // EDDI
                    case "eddi.event":
                        Context_Eddi_Event(vaProxy);
                        break;
                    // Spansh
                    case "spansh.outdatedstations":
                        Context_Spansh_OutdatedStations(vaProxy);
                        break;
                    // log
                    case "log.log":
                        Context_Log(vaProxy);
                        break;
                    // update
                    case "update.check":
                        Context_Update_Check(vaProxy);
                        break;
                    // invalid
                    default:
                        Log.Error($"Invalid plugin context '{vaProxy.Context}'.");
                        break;
                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error($"Missing parameter '{e.ParamName}' for context '{context}'");
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