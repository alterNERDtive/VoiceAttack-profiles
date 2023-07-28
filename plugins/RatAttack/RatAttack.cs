// <copyright file="RatAttack.cs" company="alterNERDtive">
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

using alterNERDtive.util;

namespace RatAttack
{
    /// <summary>
    /// VoiceAttack plugin for the RatAttack profile.
    /// </summary>
    public class RatAttack
    {
        private static readonly Version VERSION = new ("6.4");

        private static readonly Regex RatsignalRegex = new (
            @"^RATSIGNAL Case #(?<number>\d+) (?<platform>(PC|Xbox|Playstation))( )?(?<mode>LEG|HOR|ODY)?(?<oxygen> \(Code Red\))? – CMDR (?<cmdr>.+) – System: (None|u\u200bnknown system|""(?<system>.+)"" \((?<systemInfo>([a-zA-Z0-9\s\(\)\-/]*(~?[0-9,\.]+ LY (""[a-zA-Z\-]+"" of|from) [a-zA-Z0-9\s\*\-]+)?( \([a-zA-Z\s]+\))?|Not found in galaxy database|Invalid system name))\)(?<permit> \(((?<permitName>.*) )?Permit Required\))?) – Language: (?<language>[a-zA-z0-9\x7f-\xff\-\(\)&,\s\.]+)( – Nick: (?<nick>[a-zA-Z0-9_\[\]\-\^]+))? \((LEG|HOR|ODY|XB|PS)_SIGNAL\)\v*$");

        private static PipeServer<Ratsignal>? ratsignalPipe;
        private static VoiceAttackLog? log;
        private static VoiceAttackCommands? commands;

        private static ConcurrentDictionary<int, RatCase> CaseList { get; } = new ();

        private static dynamic? VA { get; set; }

        private static PipeServer<Ratsignal> RatsignalPipe
            => ratsignalPipe ??= new (
                Log,
                "RatAttack",
                new PipeServer<Ratsignal>.SignalHandler(On_Ratsignal));

        private static VoiceAttackLog Log => log ??= new (VA, "RatAttack");

        private static VoiceAttackCommands Commands => commands ??= new (VA, Log);

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The GUID.</returns>
        public static Guid VA_Id()
            => new ("{F2ADF0AE-4837-4E4A-9C87-8A7E2FA63E5F}");

        /// <summary>
        /// The plugin’s display name, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The display name.</returns>
        public static string VA_DisplayName()
            => $"RatAttack {VERSION}";

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The description.</returns>
        public static string VA_DisplayInfo()
            => "RatAttack: a plugin to handle FuelRats cases.";

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            VA.SetText("RatAttack.version", VERSION.ToString());
            vaProxy.ProfileChanged += new Action<Guid?, Guid?, string, string>(On_ProfileChanged);
            Log.Notice("Init successful.");
        }

        /// <summary>
        /// The Invoke method, as required by the VoiceAttack plugin API.
        /// Runs whenever a plugin context is invoked.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Invoke1(dynamic vaProxy)
        {
            string context = vaProxy.Context.ToLower();
            Log.Debug($"Running context '{context}' …");
            try
            {
                switch (context)
                {
                    case "getcasedata":
                        // plugin methods
                        Context_GetCaseData(vaProxy);
                        break;
                    case "parseratsignal":
                        Context_ParseRatsignal(vaProxy);
                        break;
                    case "startup":
                        Context_Startup(vaProxy);
                        break;
                    case "edsm.getnearestcmdr":
                        // EDSM
                        Context_EDSM_GetNearestCMDR(vaProxy);
                        break;
                    case "log.log":
                        // log
                        Context_Log(vaProxy);
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
            Log.Debug("Starting teardown …");
            Log.Debug("Closing RATSIGNAL pipe …");
            RatsignalPipe.Stop();
            Log.Debug("Teardown finished.");
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
        /// Parses a RATSIGNAL and extracts case data for storage.
        /// </summary>
        /// <param name="ratsignal">The incoming RATSIGNAL.</param>
        /// <returns>The case number.</returns>
        /// <exception cref="ArgumentException">Thrown on invalid RATSIGNAL.</exception>
        private static int ParseRatsignal(string ratsignal)
        {
            if (!RatsignalRegex.IsMatch(ratsignal))
            {
                throw new ArgumentException($"Invalid RATSIGNAL format: '{ratsignal}'.", "ratsignal");
            }

            Match match = RatsignalRegex.Match(ratsignal);

            string cmdr = match.Groups["cmdr"].Value;
            string? language = match.Groups["language"].Value;
            string? system = match.Groups["system"].Value;
            string? systemInfo = match.Groups["systemInfo"].Value;
            bool permitLocked = match.Groups["permit"].Success;
            string? permitName = match.Groups["permitName"].Value;
            string platform = match.Groups["platform"].Value;
            bool codeRed = match.Groups["oxygen"].Success;
            string? mode = match.Groups["mode"].Value;

            int number = int.Parse(match.Groups["number"].Value);

            if (string.IsNullOrEmpty(system))
            {
                system = "None";
            }

            Log.Debug($"New rat case: CMDR “{cmdr}” in “{system}”{(!string.IsNullOrEmpty(systemInfo) ? $" ({systemInfo})" : string.Empty)} on {platform}{(!string.IsNullOrEmpty(mode) ? $" ({mode})" : string.Empty)}, permit locked: {permitLocked}{(permitLocked && !string.IsNullOrEmpty(permitName) ? $" (permit name: {permitName})" : string.Empty)}, code red: {codeRed} (#{number}).");

            CaseList[number] = new RatCase(cmdr, language, system, systemInfo, permitLocked, permitName, platform, mode, codeRed, number);

            return number;
        }

        private static void On_Ratsignal(Ratsignal ratsignal)
        {
            try
            {
                int number = ParseRatsignal(ratsignal.Signal);
                Log.Notice($"New rat case: {CaseList[number]}.");
                Commands.TriggerEvent("RatAttack.incomingCase", parameters: new dynamic[] { new int[] { number }, new bool[] { ratsignal.Announce } });
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message);
                Commands.TriggerEvent("RatAttack.invalidRatsignal", parameters: new dynamic[] { new string[] { ratsignal.Signal } });
            }
            catch (Exception e)
            {
                Log.Error($"Unhandled exception while parsing RATSIGNAL: '{e.Message}'.");
            }
        }

        private static void On_ProfileChanged(Guid? from, Guid? to, string fromName, string toName)
            => VA_Exit1(VA);

        /*================\
        | plugin contexts |
        \================*/

#pragma warning disable IDE0060 // Remove unused parameter
        private static void Context_EDSM_GetNearestCMDR(dynamic vaProxy)
        {
            int caseNo = vaProxy.GetInt("~caseNo") ?? throw new ArgumentNullException("~caseNo");
            string cmdrList = vaProxy.GetText("~cmdrs") ?? throw new ArgumentNullException("~cmdrs");
            string[] cmdrs = cmdrList.Split(';');
            if (cmdrs.Length == 0)
            {
                throw new ArgumentNullException("~cmdrs");
            }

            string system = CaseList[caseNo]?.System ?? throw new ArgumentException($"Case #{caseNo} has no system information", "~caseNo");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\edsm-getnearest.exe";
            string arguments = $@"--short --text --system ""{system}"" ""{string.Join(@""" """, cmdrs)}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();

            string message = stdout;
            string? errorMessage = null;
            bool error;

            switch (p.ExitCode)
            {
                case 0:
                    error = false;
                    Log.Info(message);
                    break;
                case 1: // CMDR not found, Server Error, Api Exception (jeez, what a mess did I make there?)
                    error = true;
                    Log.Error(message);
                    break;
                case 2: // System not found
                    error = true;
                    Log.Warn(message);
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

        private static void Context_GetCaseData(dynamic vaProxy)
        {
            int cn = vaProxy.GetInt("~caseNumber");

            if (CaseList.ContainsKey(cn))
            {
                RatCase rc = CaseList[cn];

                vaProxy.SetInt("~~caseNumber", rc.Number);
                vaProxy.SetText("~~cmdr", rc.Cmdr);
                vaProxy.SetText("~~system", rc?.System?.ToLower());
                vaProxy.SetText("~~systemInfo", rc?.SystemInfo);
                vaProxy.SetBoolean("~~permitLocked", rc?.PermitLocked);
                vaProxy.SetText("~~permitName", rc?.PermitName);
                vaProxy.SetText("~~platform", rc?.Platform);
                vaProxy.SetText("~~mode", rc?.Mode);
                vaProxy.SetBoolean("~~codeRed", rc?.CodeRed);
            }
            else
            {
                Log.Warn($"Case #{cn} not found in the case list");
            }
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

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            _ = RatsignalPipe.Run();
            Log.Notice("Finished startup.");
        }

        private static void Context_ParseRatsignal(dynamic vaProxy)
        {
            Log.Warn("Passing a RATSIGNAL to VoiceAttack through the clipboard or a file is DEPRECATED and will no longer be supported in the future.");
            On_Ratsignal(new Ratsignal(vaProxy.GetText("~ratsignal"), vaProxy.GetBoolean("~announceRatsignal") ?? false));
        }
#pragma warning restore IDE0060 // Remove unused parameter

        /// <summary>
        /// Encapsulates a RATSIGNAL for sending between the CLI helper tool and
        /// the plugin via named pipe.
        /// </summary>
        public class Ratsignal : IPipable
        {
            private readonly char separator = '\x1F';

            /// <summary>
            /// Initializes a new instance of the <see cref="Ratsignal"/> class.
            /// </summary>
            public Ratsignal()
                => (this.Signal, this.Announce) = (string.Empty, false);

            /// <summary>
            /// Initializes a new instance of the <see cref="Ratsignal"/> class.
            /// </summary>
            /// <param name="signal">The RATSIGNAL.</param>
            /// <param name="announce">Whether or not to announce the new case.</param>
            public Ratsignal(string signal, bool announce)
                => (this.Signal, this.Announce) = (signal, announce);

            /// <summary>
            /// Gets or sets the RATSIGNAL.
            /// </summary>
            public string Signal { get; set; }

            /// <summary>
            /// Gets or Sets a value indicating whether to announce the incoming
            /// case.
            /// </summary>
            public bool Announce { get; set; }

            /// <summary>
            /// Initializes the <see cref="Ratsignal"/> instance from a
            /// serialized representation.
            /// FIXXME: should probably make this a static factory method.
            /// </summary>
            /// <param name="serialization">The serialized <see cref="Ratsignal"/>.</param>
            /// <exception cref="ArgumentException">Thrown on receiving an invalid signal.</exception>
            public void ParseString(string serialization)
            {
                try
                {
                    string[] parts = serialization.Split(this.separator);
                    this.Signal = parts[0];
                    this.Announce = bool.Parse(parts[1]);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Invalid serialized RATSIGNAL: '{serialization}'", e);
                }
            }

            /// <inheritdoc/>
            public override string ToString()
                => $"{this.Signal}{this.separator}{this.Announce}";
        }

        private class RatCase
        {
            public RatCase(string cmdr, string? language, string? system, string? systemInfo, bool permitLocked, string? permitName, string platform, string mode, bool codeRed, int number)
                => (this.Cmdr, this.Language, this.System, this.SystemInfo, this.PermitLocked, this.PermitName, this.Platform, this.Mode, this.CodeRed, this.Number)
                = (cmdr, language, system, systemInfo, permitLocked, permitName, platform, mode, codeRed, number);

            public string Cmdr { get; }

            public string? Language { get; }

            public string? System { get; }

            public string? SystemInfo { get; }

            public bool PermitLocked { get; }

            public string? PermitName { get; }

            public string Platform { get; }

            public string? Mode { get; }

            public bool CodeRed { get; }

            public int Number { get; }

            public string ShortInfo
            {
                get => $"#{this.Number}, {this.Platform}{(!string.IsNullOrEmpty(this.Mode) ? $" ({this.Mode})" : string.Empty)}{(this.CodeRed ? ", code red" : string.Empty)}, {this.System ?? "None"}{(!string.IsNullOrEmpty(this.SystemInfo) ? $" ({this.SystemInfo}{(this.PermitLocked ? ", permit required" : string.Empty)})" : string.Empty)}";
            }

            public override string ToString()
                => this.ShortInfo;
        }
    }
}
