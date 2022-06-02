// <copyright file="base.cs" company="alterNERDtive">
// Copyright 2019–2022 alterNERDtive.
//
// This file is part of alterNERDtive VoiceAttack profiles for Elite Dangerous.
//
// alterNERDtive VoiceAttack profiles for Elite Dangerous is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// alterNERDtive VoiceAttack profiles for Elite Dangerous is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with alterNERDtive VoiceAttack profiles for Elite Dangerous.  If not, see &lt;https://www.gnu.org/licenses/&gt;.
// </copyright>

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

using alterNERDtive.util;

namespace alterNERDtive
{
    /// <summary>
    /// This is the base plugin orchestrating all the profile-specific plugins
    /// to work together properly. It handles things like configuration or
    /// subscribing to VoiceAttack-triggered events.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "F off :)")]
    public class BasePlugin
    {
        private static readonly Version VERSION = new ("4.4");

        private static readonly Dictionary<Guid, string> Profiles = new ()
        {
            { new Guid("{F7F59CFD-1AE2-4A7E-8F62-C62372418BAC}"), "alterNERDtive-base" },
            { new Guid("{f31b575b-6ce4-44eb-91fc-7459e55013cf}"), "EliteAttack" },
            { new Guid("{87276668-2a6e-4d80-af77-80651daa58b7}"), "RatAttack" },
            { new Guid("{e722b29d-898e-47dd-a843-a409c87e0bd8}"), "SpanshAttack" },
            { new Guid("{05580e6c-442c-46cd-b36f-f5a1f967ec59}"), "StreamAttack" },
        };

        private static readonly List<string> ActiveProfiles = new ();
        private static readonly List<string> InstalledProfiles = new ();

        private static readonly Regex ConfigurationVariableRegex = new (@$"(?<id>({string.Join("|", Profiles.Values)}))\.(?<name>.+)#");

        private static VoiceAttackCommands? commands;
        private static Configuration? config;
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands => commands ??= new (VA, Log);

        private static Configuration Config => config ??= new (VA, Log, Commands, "alterNERDtive-base");

        private static VoiceAttackLog Log => log ??= new (VA, "alterNERDtive-base");

        private static dynamic? VA { get; set; }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The GUID.</returns>
        public static Guid VA_Id()
            => new ("{F7F59CFD-1AE2-4A7E-8F62-C62372418BAC}");

        /// <summary>
        /// The plugin’s display name, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The display name.</returns>
        public static string VA_DisplayName()
            => $"alterNERDtive-base {VERSION}";

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The description.</returns>
        public static string VA_DisplayInfo()
            => "The alterNERDtive plugin to manage all the alterNERDtive profiles!";

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            VA.SetText("alterNERDtive-base.version", VERSION.ToString());
            vaProxy.BooleanVariableChanged += new Action<string, bool?, bool?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.DateVariableChanged += new Action<string, DateTime?, DateTime?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.DecimalVariableChanged += new Action<string, decimal?, decimal?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.IntegerVariableChanged += new Action<string, int?, int?, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            vaProxy.TextVariableChanged += new Action<string, string, string, Guid?>((name, from, to, id) => { ConfigurationChanged(name, from, to, id); });
            VA.SetBoolean("alterNERDtive-base.initialized", true);
            Commands.TriggerEvent("alterNERDtive-base.initialized", wait: false, logMissing: false);
            Log.Notice("Init successful.");
        }

        /// <summary>
        /// The Invoke method, as required by the VoiceAttack plugin API.
        /// Runs whenever a plugin context is invoked.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
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
                        Context_Startup();
                        break;
                    case "config.dialog":
                        // config
                        Context_Config_Dialog();
                        break;
                    case "config.dump":
                        Context_Config_Dump();
                        break;
                    case "config.getvariables":
                        Context_Config_SetVariables();
                        break;
                    case "config.list":
                        Context_Config_List();
                        break;
                    case "config.setup":
                        Context_Config_Setup();
                        break;
                    case "config.versionmigration":
                        Context_Config_VersionMigration();
                        break;
                    case "edsm.bodycount":
                        // EDSM
                        Context_EDSM_BodyCount();
                        break;
                    case "edsm.distancebetween":
                        Context_EDSM_DistanceBetween();
                        break;
                    case "eddi.event":
                        // EDDI
                        Context_Eddi_Event();
                        break;
                    case "spansh.outdatedstations":
                        // Spansh
                        Context_Spansh_OutdatedStations();
                        break;
                    case "log.log":
                        // log
                        Context_Log();
                        break;
                    case "update.check":
                        // update
                        Context_Update_Check();
                        break;
                    default:
                        // invalid
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

        /// <summary>
        /// The Exit method, as required by the VoiceAttack plugin API.
        /// Runs when VoiceAttack is shut down.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "required by VoiceAttack plugin API")]
        public static void VA_Exit1(dynamic vaProxy)
        {
        }

        /// <summary>
        /// The StopCommand method, as required by the VoiceAttack plugin API.
        /// Runs whenever all commands are stopped using the “Stop All Commands”
        /// button or action.
        /// </summary>
        public static void VA_StopCommand()
        {
        }

        /// <summary>
        /// Returns whether a given profile is currently active.
        /// </summary>
        /// <param name="profileName">The name of the profile in question.</param>
        /// <returns>The state of the profile in question.</returns>
        public static bool IsProfileActive(string profileName) => ActiveProfiles.Contains(profileName);

        private static void CheckProfiles(dynamic vaProxy)
        {
            ActiveProfiles.Clear();
            InstalledProfiles.Clear();

            foreach (KeyValuePair<Guid, string> profile in Profiles)
            {
                if (vaProxy.Command.Exists($"(({profile.Value}.startup))"))
                {
                    // Sadly there is no way to find _active_ profiles, so we have to check the one command that always is in them.
                    ActiveProfiles.Add(profile.Value);
                }

                if (vaProxy.Profile.Exists(profile.Key))
                {
                    InstalledProfiles.Add(profile.Value);
                }
            }

            Log.Debug($"Profiles found: {string.Join<string>(", ", ActiveProfiles)}");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "required by VoiceAttack plugin API")]
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

                        // if null, EDDI isn’t up yet
                        if (option == "alterNERDtive-base.eddi.quietMode#" && VA!.GetText("EDDI version") != null)
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

            Commands.TriggerEvent(
                "alterNERDtive-base.updateCheck",
                parameters: new dynamic[] { new string[] { VERSION.ToString(), latestVersion.ToString() }, new bool[] { VERSION.CompareTo(latestVersion) < 0 } });
        }

        /*================\
        | plugin contexts |
        \================*/

#pragma warning disable IDE0060 // Remove unused parameter
        private static void Context_Config_Dialog(dynamic vaProxy)
        {
            Thread dialogThread = new Thread(new ThreadStart(() =>
            {
                System.Windows.Window options = new ()
                {
                    Title = "alterNERDtive Profile Options",
                    Content = new SettingsDialog(Config, Log),
                    SizeToContent = System.Windows.SizeToContent.WidthAndHeight,
                    ResizeMode = System.Windows.ResizeMode.NoResize,
                    WindowStyle = System.Windows.WindowStyle.ToolWindow,
                };
                options.ShowDialog();
                options.Activate();
            }));
            dialogThread.SetApartmentState(ApartmentState.STA);
            dialogThread.IsBackground = true;
            dialogThread.Start();
        }

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
            // =============
            // === 4.3.1 ===
            // =============

            // EliteAttack
            foreach (string option in new string[] { "autoStationService" })
            {
                string name = $"EliteAttack.{option}s#";
                string oldName = $"EliteAttack.{option}#";
                Commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{oldName}", "boolean" } });
                bool? value = vaProxy.GetBoolean(oldName);
                if (value != null)
                {
                    Log.Info($"Migrating option {oldName} …");
                    Commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}" }, new bool[] { (bool)value } });
                    Commands.Run("alterNERDtive-base.unsetVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{oldName}", "boolean" } });
                }
            }

            // ===========
            // === 4.2 ===
            // ===========

            // SpanshAttack
            string edtsPath = $@"{vaProxy.SessionState["VA_SOUNDS"]}\scripts\edts.exe";
            if (File.Exists(edtsPath))
            {
                File.Delete(edtsPath);
            }

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
                bool? value = vaProxy.GetBoolean(oldName);
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
                bool? value = vaProxy.GetBoolean(name);
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
                string value = vaProxy.GetText(name);
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
                bool? value = vaProxy.GetBoolean(name);
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
                string value = vaProxy.GetText(name);
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
                string value = vaProxy.GetText(name);
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

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\scripts\explorationtools.exe";
            string arguments = $@"bodycount ""{system}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            int bodyCount = 0;
            bool error = false;
            string errorMessage = string.Empty;

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
            string errorMessage = string.Empty;

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
                catch (ArgumentNullException)
                {
                    throw;
                }
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
            CheckProfiles(vaProxy);
            Log.Notice($"Active profiles: {string.Join(", ", ActiveProfiles)}");
            Commands.TriggerEventAll(ActiveProfiles, "startup", logMissing: false);
            Log.Notice("Finished startup.");
        }

        private static void Context_Update_Check(dynamic vaProxy)
        {
            UpdateCheck();
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
