// <copyright file="util.cs" company="alterNERDtive">
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
using System.IO.Pipes;
using System.Linq;

namespace alterNERDtive.util
{
    /// <summary>
    /// Log levels that can be used when writing to the VoiceAttack log.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Log level for error messages. Errors cause execution to abort.
        /// </summary>
        ERROR,

        /// <summary>
        /// Log level for warning messages. Warnings should not cause execution
        /// to abort.
        /// </summary>
        WARN,

        /// <summary>
        /// Log level for notices. Notices should be noteworthy.
        /// </summary>
        NOTICE,

        /// <summary>
        /// Log level for informational messages. These should not be
        /// noteworthy.
        /// </summary>
        INFO,

        /// <summary>
        /// Log level for debug messages. They should be useful only for
        /// debugging.
        /// </summary>
        DEBUG,
    }

    /// <summary>
    /// Interface describing an object that can be send through a Pipe. Needs
    /// serializing to and deserializing from <see cref="string"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "file contains collection of utility classes")]
    public interface IPipable
    {
        /// <summary>
        /// Parses object data from a serialized string.
        /// </summary>
        /// <param name="serialization">The serialized string.</param>
        public void ParseString(string serialization);

        /// <summary>
        /// Serializes the object to a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        public string ToString();
    }

    /// <summary>
    /// Contains the configuration for a plugin/profile and handles
    /// configuration-related VoiceAttack interactions.
    /// </summary>
    public class Configuration
    {
        private static readonly Dictionary<string, OptDict<string, Option>> Defaults = new ()
        {
            {
                "alterNERDtive-base",
                new OptDict<string, Option>
                {
                    /*{ new Option<decimal>("delays.keyPressDuration", (decimal)0.01, voiceTrigger: "key press duration",
                        description: "The time keys will be held down for.") },*/
                    {
                        new Option<decimal>(
                            name: "delays.quitToDesktop",
                            defaultValue: 10.0M,
                            voiceTrigger: "quit to desktop delay",
                            description: "The delay before restarting the game after hitting “Exit to Desktop”, in seconds.\nDefault: 10.0. (Used by the `restart from desktop` command)")
                    },
                    {
                        new Option<bool>(
                            name: "eddi.quietMode",
                            defaultValue: true,
                            voiceTrigger: "eddi quiet mode",
                            description: "Make EDDI shut up. Disables all built-in speech responders.")
                    },
                    {
                        new Option<string>(
                            name: "elite.pasteKey",
                            defaultValue: "v",
                            voiceTrigger: "elite paste key",
                            description: "The key used to paste in conjunction with CTRL. The physical key in your layout that would be 'V' on QWERTY.")
                    },
                    {
                        new Option<bool>(
                            name: "enableAutoUpdateCheck",
                            defaultValue: true,
                            voiceTrigger: "auto update check",
                            description: "Automatically check Github for profiles updates when the profile loads.")
                    },
                    {
                        new Option<string>(
                            name: "log.logLevel",
                            defaultValue: "NOTICE",
                            voiceTrigger: "log level",
                            validValues: new List<string> { "ERROR", "WARN", "NOTICE", "INFO", "DEBUG" },
                            description: "The level of detail for logging to the VoiceAttack log.\nValid levels are \"ERROR\", \"WARN\", \"NOTICE\", \"INFO\" and \"DEBUG\".\nDefault: \"NOTICE\".")
                    },
                }
            },
            {
                "EliteAttack",
                new OptDict<string, Option>
                {
                    {
                        new Option<bool>(
                            name: "announceEdsmSystemStatus",
                            defaultValue: true,
                            voiceTrigger: "edsm system status",
                            description: "Pull system data from EDSM and compare it against your discovery scan.")
                    },
                    {
                        new Option<bool>(
                            name: "announceJumpsInRoute",
                            defaultValue: true,
                            voiceTrigger: "route jump count",
                            description: "Give a jump count on plotting a route.")
                    },
                    {
                        new Option<bool>(
                            name: "announceMappingCandidates",
                            defaultValue: true,
                            voiceTrigger: "mapping candidates",
                            description: "Announce bodies worth mapping when you have finished scanning a system.\n(Terraformables, Water Worlds, Earth-Like Worlds and Ammonia Worlds that have not been mapped yet.)")
                    },
                    {
                        new Option<bool>(
                            name: "announceOutdatedStationData",
                            defaultValue: true,
                            voiceTrigger: "outdated stations",
                            description: "Announce stations with outdated data in the online databases.")
                    },
                    {
                        new Option<int>(
                            name: "outdatedStationThreshold",
                            defaultValue: 365,
                            voiceTrigger: "outdated station threshold",
                            description: "The threshold for station data to count as “outdated”, in days.\nDefault: 365.")
                    },
                    {
                        new Option<bool>(
                            name: "includeOutdatedSettlements",
                            defaultValue: true,
                            voiceTrigger: "include outdated settlements",
                            description: "Include outdated Odyssey settlements in the outdated stations list.")
                    },
                    {
                        new Option<bool>(
                            name: "announceR2RMappingCandidates",
                            defaultValue: false,
                            voiceTrigger: "road to riches",
                            description: "Announce bodies worth scanning if you are looking for some starting cash on the Road to Riches.")
                    },
                    {
                        new Option<bool>(
                            name: "announceRepairs",
                            defaultValue: true,
                            voiceTrigger: "repair reports",
                            description: "Report on AFMU repairs.")
                    },
                    {
                        new Option<bool>(
                            name: "announceSynthesis",
                            defaultValue: true,
                            voiceTrigger: "synthesis reports",
                            description: "Report on synthesis.")
                    },
                    {
                        new Option<bool>(
                            name: "autoHonkNewSystems",
                            defaultValue: true,
                            voiceTrigger: "auto honk new systems",
                            description: "Automatically honk upon entering a system if it is your first visit.")
                    },
                    {
                        new Option<bool>(
                            name: "autoHonkAllSystems",
                            defaultValue: false,
                            voiceTrigger: "auto honk all systems",
                            description: "Automatically honk upon entering a system, each jump, without constraints.")
                    },
                    {
                        new Option<int>(
                            name: "scannerFireGroup",
                            defaultValue: 0,
                            voiceTrigger: "scanner fire group",
                            description: "The fire group your discovery scanner is assigned to.\nDefault: 0 (the first one).")
                    },
                    {
                        new Option<bool>(
                            name: "usePrimaryFireForDiscoveryScan",
                            defaultValue: false,
                            voiceTrigger: "discovery scan on primary fire",
                            description: "Use primary fire for honking instead of secondary.")
                    },
                    {
                        new Option<bool>(
                            name: "autoRefuel",
                            defaultValue: true,
                            voiceTrigger: "auto refuel",
                            description: "Automatically refuel after docking at a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoRepair",
                            defaultValue: true,
                            voiceTrigger: "auto repair",
                            description: "Automatically repair after docking at a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoRestock",
                            defaultValue: true,
                            voiceTrigger: "auto restock",
                            description: "Automatically restock after docking at a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoHangar",
                            defaultValue: true,
                            voiceTrigger: "auto move to hangar",
                            description: "Automatically move the ship to the hangar after docking at a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoStationServices",
                            defaultValue: true,
                            voiceTrigger: "auto enter station services",
                            description: "Automatically enter the Station Services menu after docking at a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoRetractLandingGear",
                            defaultValue: true,
                            voiceTrigger: "auto retract landing gear",
                            description: "Automatically retract landing gear when lifting off a planet / undocking from a station.")
                    },
                    {
                        new Option<bool>(
                            name: "autoDisableSrvLights",
                            defaultValue: true,
                            voiceTrigger: "auto disable s r v lights",
                            description: "Automatically turn SRV lights off when deploying one.")
                    },
                    {
                        new Option<bool>(
                            name: "flightAssistOff",
                            defaultValue: false,
                            voiceTrigger: "flight assist off",
                            description: "Permanent Flight Assist off mode. You should really do that, it’s great.")
                    },
                    {
                        new Option<bool>(
                            name: "hyperspaceDethrottle",
                            defaultValue: true,
                            voiceTrigger: "hyper space dethrottle",
                            description: "Throttle down after a jump and when dropping from SC. Like the SC Assist module does.")
                    },
                    {
                        new Option<bool>(
                            name: "limpetCheck",
                            defaultValue: true,
                            voiceTrigger: "limpet check",
                            description: "Do a limpet check when undocking, reminding you if you forgot to buy some.")
                    },
                }
            },
            {
                "RatAttack",
                new OptDict<string, Option>
                {
                    {
                        new Option<bool>(
                            name: "autoCloseCase",
                            defaultValue: false,
                            voiceTrigger: "auto close fuel rat case",
                            description: "Automatically close a rat case when sending “fuel+” via voice command or ingame chat.")
                    },
                    {
                        new Option<bool>(
                            "autoCopySystem",
                            defaultValue: true,
                            voiceTrigger: "auto copy rat case system",
                            description: "Automatically copy the client’s system to the clipboard when you open a rat case.")
                    },
                    {
                        new Option<bool>(
                            name: "announceNearestCMDR",
                            defaultValue: false,
                            voiceTrigger: "nearest commander to fuel rat case",
                            description: "Announce the nearest commander to incoming rat cases.")
                    },
                    {
                        new Option<string>(
                            name: "CMDRs",
                            defaultValue: string.Empty,
                            voiceTrigger: "fuel rat commanders",
                            description: "All your CMDRs that are ready to take rat cases.\nUse ‘;’ as separator, e.g. “Bud Spencer;Terrence Hill”.")
                    },
                    {
                        new Option<bool>(
                            name: "announcePlatform",
                            defaultValue: false,
                            voiceTrigger: "platform for fuel rat case",
                            description: "Announce the platform for incoming rat cases.")
                    },
                    {
                        new Option<string>(
                            name: "platforms",
                            defaultValue: "PC",
                            voiceTrigger: "fuel rat platforms",
                            validValues: new List<string> { "PC", "Xbox", "Playstation" },
                            description: "The platform(s) you want to get case announcements for (PC, Xbox, Playstation).\nUse ‘;’ as separator, e.g. “PC;Xbox”.")
                    },
                    {
                        new Option<bool>(
                            name: "announceSystemInfo",
                            defaultValue: true,
                            voiceTrigger: "system information for fuel rat case",
                            description: "System information provided by Mecha.")
                    },
                    {
                        new Option<bool>(
                            name: "confirmCalls",
                            defaultValue: true,
                            voiceTrigger: "fuel rat call confirmation",
                            description: "Only make calls in #fuelrats after vocal confirmation to prevent mistakes.")
                    },
                    {
                        new Option<bool>(
                            name: "onDuty",
                            defaultValue: true,
                            voiceTrigger: "fuel rat duty",
                            description: "On duty, receiving case announcements via TTS.")
                    },
                }
            },
            {
                "SpanshAttack",
                new OptDict<string, Option>
                {
                    {
                        new Option<string>(
                            name: "announceJumpsLeft",
                            defaultValue: ";1;3;5;10;15;20;30;50;75;100;",
                            voiceTrigger: "announce jumps left",
                            description: "Estimated jumps left to announce when reached.\nNEEDS to have leading and trailing “;”.")
                    },
                    {
                        new Option<bool>(
                            name: "announceWaypoints",
                            defaultValue: true,
                            voiceTrigger: "waypoint announcements",
                            description: "Announce each waypoint by name.")
                    },
                    {
                        new Option<bool>(
                            name: "autoJumpAfterScooping",
                            defaultValue: true,
                            voiceTrigger: "auto jump after scooping",
                            description: "Automatically jump out when fuel scooping is complete.")
                    },
                    {
                        new Option<bool>(
                            name: "autoPlot",
                            defaultValue: true,
                            voiceTrigger: "auto plot",
                            description: "Automatically plot to the next waypoint after supercharging.")
                    },
                    {
                        new Option<bool>(
                            name: "clearOnShutdown",
                            defaultValue: true,
                            voiceTrigger: "clear neutron route on shutdown",
                            description: "Clear an active neutron route when the game is shut down.")
                    },
                    {
                        new Option<bool>(
                            name: "copyWaypointToClipboard",
                            defaultValue: false,
                            voiceTrigger: "copy neutron waypoints to clipboard",
                            description: "Copy each neutron waypoint into the Windows clipboard.")
                    },
                    {
                        new Option<bool>(
                            "defaultToLadenRange",
                            defaultValue: false,
                            voiceTrigger: "default to laden range",
                            description: "Default to the current ship’s laden range as reported by EDDI instead of prompting for input.")
                    },
                    {
                        new Option<bool>(
                            name: "timeTrip",
                            defaultValue: false,
                            voiceTrigger: "time neutron route",
                            description: "Keep track of how long a neutron route takes you to complete.")
                    },
                }
            },
            {
                "StreamAttack",
                new OptDict<string, Option>
                {
                    {
                        new Option<string>(
                            name: "outputDir",
                            defaultValue: @"%appdata%\StreamAttack\",
                            voiceTrigger: "StreamAttack output directory",
                            description: "The directory the status files are written to.")
                    },
                }
            },
        };

#pragma warning disable SA1306 // Field names should begin with lower-case letter
        private readonly dynamic VA;
        private readonly string ID;
#pragma warning restore SA1306 // Field names should begin with lower-case letter
        private readonly VoiceAttackLog log;
        private readonly VoiceAttackCommands commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        /// <param name="log">The VoiceAttack log.</param>
        /// <param name="commands">The VoiceAttack commands object.</param>
        /// <param name="id">The profile ID.</param>
        public Configuration(dynamic vaProxy, VoiceAttackLog log, VoiceAttackCommands commands, string id)
            => (this.VA, this.log, this.commands, this.ID)
             = (vaProxy, log, commands, id);

        /// <summary>
        /// Gets the default value of an option.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <param name="name">The name of the option.</param>
        /// <returns>The default value of the option.</returns>
        public static dynamic GetDefault(string id, string name)
        {
            return ((Option<dynamic>)Defaults[id][name]).DefaultValue;
        }

        /// <summary>
        /// Gets an Option.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <param name="name">The name of the Option.</param>
        /// <returns>The Option.</returns>
        public static Option GetOption(string id, string name)
        {
            return Defaults[id][name];
        }

        /// <summary>
        /// Gets all Options for a give profile.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <returns>The </returns>
        public static OptDict<string, Option> GetOptions(string id)
        {
            return Defaults[id];
        }

        /// <summary>
        /// Checks if a given option has a default value.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <param name="name">The name of the option.</param>
        /// <returns>Wether the option has a default value.</returns>
        public static bool HasDefault(string id, string name)
        {
            return Defaults[id].ContainsKey(name);
        }

        /// <summary>
        /// Gets the default value of an option by name.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>The value of the option.</returns>
        public dynamic GetDefault(string name)
        {
            return GetDefault(this.ID, name);
        }

        /// <summary>
        /// Gets an Option by name.
        /// </summary>
        /// <param name="name">The name of the Option.</param>
        /// <returns>The Option.</returns>
        public Option GetOption(string name)
        {
            return GetOption(this.ID, name);
        }

        /// <summary>
        /// Sets the voice triggers for the voice commands that set options from
        /// VoiceAttack.
        /// </summary>
        /// <param name="type">The data type to set voice triggers for.</param>
        /// <exception cref="ArgumentException">Thrown when duplicate triggers are found.</exception>
        public void SetVoiceTriggers(Type type)
        {
            List<string> triggers = new ();
            foreach (Dictionary<string, Option> options in Defaults.Values)
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
                this.VA.SetText($"alterNERDtive-base.triggers.{type.Name}", triggerString);
                this.log.Debug($"Voice triggers for {type.Name}: '{triggerString}'");
            }
            else
            {
                // make sure we don’t accidentally have weird things happening with empty config voice triggers
                string triggerString = $"tenuiadafesslejüsljlejutlesuivle{type.Name}";
                this.VA.SetText($"alterNERDtive-base.triggers.{type.Name}", triggerString);
                this.log.Debug($"No voice triggers found for {type.Name}");
            }
        }

        /// <summary>
        /// Sets VoiceAttack variables required for handling the reporting and
        /// setting of a configuration option.
        /// </summary>
        /// <param name="vaProxy">The command’s VoiceAttack proxy object.</param>
        /// <param name="trigger">The voice trigger for the option in question.</param>
        /// <exception cref="ArgumentNullException">Thrown when the voice trigger is missing/null.</exception>
        public void SetVariablesForTrigger(dynamic vaProxy, string trigger)
        {
            _ = trigger ?? throw new ArgumentNullException("trigger");

            foreach (KeyValuePair<string, OptDict<string, Option>> options in Defaults)
            {
                try
                {
                    dynamic option = options.Value.First(item => ((dynamic)item.Value).VoiceTrigger.ToLower() == trigger).Value;
                    vaProxy.SetText("~name", $"{options.Key}.{option.Name}");
                    vaProxy.SetText("~ttsDescription", option.TtsDescription);
                    vaProxy.SetText("~description", option.Description);
                    break;
                }
                catch (InvalidOperationException)
                {
                    // trigger doesn’t exist in this profile, skip
                }
            }
        }

        /// <summary>
        /// Checks if a given option has a default value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <returns>Whether the option has a default value.</returns>
        public bool HasDefault(string name)
        {
            return HasDefault(this.ID, name);
        }

        /// <summary>
        /// Loads stored value of all options from the currently active
        /// VoiceAttack profile.
        /// </summary>
        public void LoadFromProfile()
        {
            foreach (KeyValuePair<string,OptDict<string,Option>> options in Defaults)
            {
                this.LoadFromProfile(options.Key);
            }
        }

        /// <summary>
        /// Loads stored options for a given profile ID from the currently
        /// active VoiceAttack profile.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <exception cref="InvalidDataException">Thrown when encountering an incompatible data type.</exception>
        public void LoadFromProfile(string id)
        {
            foreach (dynamic option in Defaults[id].Values)
            {
                string name = $"{id}.{option.Name}";
                string type = option.TypeString ?? throw new InvalidDataException($"Invalid data type for option '{name}': '{option}'");
                this.log.Debug($"Loading value for option '{name}' from profile …");
                this.commands.Run("alterNERDtive-base.loadVariableFromProfile", wait: true, parameters: new dynamic[] { new string[] { $"{name}#", type } });
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
                log.Warn($"No default configuration value found for '{id}.{name}'");
                return null;
            }

            dynamic option = Defaults[id][name];
            dynamic value = option.DefaultValue;
            log.Debug($"Loading default configuration value, '{id}.{name}': '{value}' …");
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
            log.Notice($"===== {id} configuration: =====");
            foreach (string name in Defaults[id].Keys)
            {
                DumpConfig(id, name);
            }
        }
        public void DumpConfig(string id, string name)
        {
            dynamic defaultValue = ((dynamic)Defaults[id][name]).DefaultValue;
            dynamic value = GetConfig(id, name);
            log.Notice($"{id}.{name}# = {value}{(value == defaultValue ? " (default)" : "")}");
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
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new bool[] { value } }); ;
            }
            else if (value is DateTime)
            {
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new DateTime[] { value } });
            }
            else if (value is decimal)
            {
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new decimal[] { value } });
            }
            else if (value is int)
            {
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new int[] { value } });
            }
            else if (value is short)
            {
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}" }, new short[] { value } });
            }
            else if (value is string)
            {
                commands.Run("alterNERDtive-base.saveVariableToProfile", wait: true, parameters: new dynamic[] { new string[] { $"{variable}", value } });
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
            log.Notice($"===== {id} settings: =====");
            foreach (string name in Defaults[id].Keys)
            {
                ListConfig(id, name);
            }
        }
        public void ListConfig(string id, string name)
        {
            dynamic option = Defaults[id][name];
            log.Notice($"“{option.VoiceTrigger}”: {option.Description}");
        }

        /// <summary>
        /// A plugin/profile option. Abstract base class for typed Options.
        /// </summary>
        public abstract class Option
        {
            /// <summary>
            /// Gets the name of the Option.
            /// </summary>
            public abstract string Name { get; }

            /// <summary>
            /// Gets the voice trigger phrase for the Option.
            /// </summary>
            public abstract string VoiceTrigger { get; }

            /// <summary>
            /// Gets the (optional) text to speech description for the Option.
            /// Usually the voice trigger is used in confirmation text to
            /// speech, this can be used to override it.
            /// </summary>
            public abstract string TtsDescription { get; }

            /// <summary>
            /// Gets the description of the Option.
            /// </summary>
            public abstract string Description { get; }

            /// <summary>
            /// Gets the type string for the option as used by VoiceAttack.
            /// </summary>
            public abstract string? TypeString { get; }
        }

        /// <summary>
        /// A typed plugin/profile option.
        /// </summary>
        /// <typeparam name="T">The data type of the option.</typeparam>
        public class Option<T> : Option
        {
            private readonly string? ttsDescription;
            private readonly string? description;

            /// <summary>
            /// Initializes a new instance of the <see cref="Option{T}"/> class.
            /// </summary>
            /// <param name="name">The name of the option.</param>
            /// <param name="defaultValue">The default value for the option.</param>
            /// <param name="voiceTrigger">The voice trigger for the option.</param>
            /// <param name="validValues">The (optional) list of valid values for the otpion.</param>
            /// <param name="ttsDescription">The (optional) TTS description of the option.</param>
            /// <param name="description">The descrption of the option.</param>
            public Option(string name, T defaultValue, string voiceTrigger, List<T>? validValues = null, string? ttsDescription = null, string? description = null)
                => (this.Name, this.DefaultValue, this.VoiceTrigger, this.ValidValues, this.ttsDescription, this.description)
                 = (name, defaultValue, voiceTrigger, validValues, ttsDescription, description);

            /// <inheritdoc/>
            public override string Name { get; }

            /// <summary>
            /// Gets the default value for the Option.
            /// </summary>
            public T DefaultValue { get; }

            /// <summary>
            /// Gets the (optional) list of valid values for the Option.
            /// </summary>
            public List<T>? ValidValues { get; }

            /// <inheritdoc/>
            public override string VoiceTrigger { get; }

            /// <inheritdoc/>
            public override string TtsDescription { get => this.ttsDescription ?? this.VoiceTrigger; }

            /// <inheritdoc/>
            public override string Description { get => this.description ?? "No description available."; }

            /// <summary>
            /// Gets the data type of the Option.
            /// </summary>
            public Type Type { get => typeof(T); }

            /// <inheritdoc/>
            public override string? TypeString
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

            /// <summary>
            /// Converts an <see cref="Option{T}"/> to a <see cref="Tuple{T1,
            /// T2}"/> of <see cref="string"/> and <see cref="Option{T}"/>.
            /// </summary>
            /// <param name="o">The Option to convert.</param>
            public static implicit operator (string, Option)(Option<T> o) => (o.Name, o);

            /// <summary>
            /// Converts an <see cref="Option{T}"/> to the contained <see
            /// cref="T"/> default value.
            /// </summary>
            /// <param name="o">The option to convert.</param>
            public static explicit operator T(Option<T> o) => o.DefaultValue;

            /// <inheritdoc/>
            public override string ToString() => this.DefaultValue!.ToString();
        }

        /// <summary>
        /// A Dictionary containing <see cref="Option{T}"/>s. Used in
        /// conjunction with <see cref="Option{T}"/>’s implicit conversion to a
        /// tuple to make adding Options less painful.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        public class OptDict<TKey, TValue> : Dictionary<TKey, TValue>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="OptDict{TKey, TValue}"/> class.
            /// </summary>
            public OptDict()
                : base()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="OptDict{TKey, TValue}"/> class.
            /// </summary>
            /// <param name="capacity">The initial capacity.</param>
            public OptDict(int capacity)
                : base(capacity)
            {
            }

            /// <summary>
            /// Adds a <see cref="Tuple{TKey,TValue}"/> to the list.
            /// </summary>
            /// <param name="tuple">The Tuple to be added.</param>
            public void Add((TKey, TValue) tuple)
            {
                this.Add(tuple.Item1, tuple.Item2);
            }
        }
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

    public class PipeServer<Thing>
        where Thing : IPipable, new()
    {
        private readonly string pipeName;
        private readonly SignalHandler handler;
        private readonly VoiceAttackLog log;

        private bool running = false;

        private NamedPipeServerStream? server;

        public PipeServer(VoiceAttackLog log, string name, SignalHandler handler)
            => (this.log, this.pipeName, this.handler) = (log, name, handler);

        public delegate void SignalHandler(Thing thing);

        public PipeServer<Thing> Run()
        {
            this.log.Debug($"Starting '{this.pipeName}' pipe …");
            if (!this.running)
            {
                this.running = true;
                this.WaitForConnection();
            }

            return this;
        }

        public PipeServer<Thing> Stop()
        {
            this.log.Debug($"Stopping '{this.pipeName}' pipe …");
            if (this.running)
            {
                this.running = false;
                this.server!.Close();
            }

            return this;
        }

        private void WaitForConnection()
        {
            try
            {
                this.server = new NamedPipeServerStream(
                    this.pipeName,
                    PipeDirection.In,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous);
                this.server.BeginWaitForConnection(this.OnConnect, this.server);
            }
            catch (Exception e)
            {
                this.log.Error($"Error setting up pipe: {e.Message}");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            NamedPipeServerStream server = (NamedPipeServerStream)ar.AsyncState;
            try
            {
                server.EndWaitForConnection(ar);
                this.WaitForConnection();
                using StreamReader reader = new (server);
                Thing thing = new ();
                thing.ParseString(reader.ReadToEnd());
                this.handler(thing);
            }
            catch (ObjectDisposedException)
            {
                this.log.Debug($"'{this.pipeName}' pipe has been closed.");
            }
            catch (Exception e)
            {
                this.log.Error($"Error reading pipe: {e.Message}");
            }
        }
    }
}
