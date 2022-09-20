// <copyright file="SpanshAttack.cs" company="alterNERDtive">
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

using alterNERDtive.edts;
using alterNERDtive.util;

namespace SpanshAttack
{
    /// <summary>
    /// VoiceAttack plugin for the SpanshAttack profile.
    /// </summary>
    public class SpanshAttack
    {
        private static readonly Version VERSION = new ("7.2.3");

        private static VoiceAttackLog? log;
        private static VoiceAttackCommands? commands;

        private static dynamic? VA { get; set; }

        private static VoiceAttackLog Log => log ??= new (VA, "SpanshAttack");

        private static VoiceAttackCommands Commands => commands ??= new (VA, Log);

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        /// <summary>
        /// The plugin’s GUID, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The GUID.</returns>
        public static Guid VA_Id()
            => new ("{e722b29d-898e-47dd-a843-a409c87e0bd8}");

        /// <summary>
        /// The plugin’s display name, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The display name.</returns>
        public static string VA_DisplayName()
            => $"SpanshAttack {VERSION}";

        /// <summary>
        /// The plugin’s description, as required by the VoiceAttack plugin API.
        /// </summary>
        /// <returns>The description.</returns>
        public static string VA_DisplayInfo()
            => "SpanshAttack: a plugin for doing routing with spansh.co.uk for Elite: Dangerous.";

        /// <summary>
        /// The Init method, as required by the VoiceAttack plugin API.
        /// Runs when the plugin is initially loaded.
        /// </summary>
        /// <param name="vaProxy">The VoiceAttack proxy object.</param>
        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            VA.SetText("SpanshAttack.version", VERSION.ToString());
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
                    case "startup":
                        Context_Startup(vaProxy);
                        break;
                    case "edts.getcoordinates":
                        // EDTS
                        Context_EDTS_GetCoordinates(vaProxy);
                        break;
                    case "log.log":
                        // log
                        Context_Log(vaProxy);
                        break;
                    case "spansh.systemexists":
                        // Spansh
                        Context_Spansh_SytemExists(vaProxy);
                        break;
                    case "spansh.nearestsystem":
                        Context_Spansh_Nearestsystem(vaProxy);
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

        /*================\
        | plugin contexts |
        \================*/

#pragma warning disable IDE0060 // Remove unused parameter
        private static void Context_EDTS_GetCoordinates(dynamic vaProxy)
        {
            string name = vaProxy.GetText("~system") ?? throw new ArgumentNullException("~system");

            bool success = false;
            string? errorType = null;
            string? errorMessage = null;

            try
            {
                StarSystem system = EdtsApi.GetCoordinates(name);

                if (system.Coords.Precision < 100)
                {
                    Log.Info($@"Coordinates for ""{name}"": ({system.Coords.X}, {system.Coords.Y}, {system.Coords.Z}), precision: {system.Coords.Precision} ly");
                }
                else
                {
                    Log.Warn($@"Coordinates with low precision for ""{name}"": ({system.Coords.X}, {system.Coords.Y}, {system.Coords.Z}), precision: {system.Coords.Precision} ly");
                }

                vaProxy.SetInt("~x", system.Coords.X);
                vaProxy.SetInt("~y", system.Coords.Y);
                vaProxy.SetInt("~z", system.Coords.Z);
                vaProxy.SetInt("~precision", system.Coords.Precision);

                success = true;
            }
            catch (ArgumentException e)
            {
                errorType = "invalid name";
                errorMessage = e.Message;
            }
            catch (Exception e)
            {
                errorType = "connection error";
                errorMessage = e.Message;
            }

            vaProxy.SetBoolean("~success", success);
            if (!string.IsNullOrWhiteSpace(errorType))
            {
                Log.Error(errorMessage!);
                vaProxy.SetText("~errorType", errorType);
                vaProxy.SetText("~errorMessage", errorMessage);
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

        private static void Context_Spansh_Nearestsystem(dynamic vaProxy)
        {
            int x = vaProxy.GetInt("~x") ?? throw new ArgumentNullException("~x");
            int y = vaProxy.GetInt("~y") ?? throw new ArgumentNullException("~y");
            int z = vaProxy.GetInt("~z") ?? throw new ArgumentNullException("~z");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\spansh.exe";
            string arguments = $@"nearestsystem --parsable {x} {y} {z}";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            Dictionary<char, decimal> coords = new () { { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };
            string system = string.Empty;
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
                    string[] stdoutExploded = stdout.Split('|');
                    system = stdoutExploded[0];
                    distance = decimal.Parse(stdoutExploded[2]);
                    string[] stdoutCoords = stdoutExploded[1].Split(',');
                    coords['x'] = decimal.Parse(stdoutCoords[0]);
                    coords['y'] = decimal.Parse(stdoutCoords[1]);
                    coords['z'] = decimal.Parse(stdoutCoords[2]);
                    Log.Info($"Nearest system to ({x}, {y}, {z}): {system} ({coords['x']}, {coords['y']}, {coords['z']}), distance: {distance} ly");
                    break;
                case 1:
                    error = true;
                    errorMessage = stdout;
                    Log.Error(errorMessage);
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;
            }

            vaProxy.SetText("~system", system);
            vaProxy.SetDecimal("~x", coords['x']);
            vaProxy.SetDecimal("~y", coords['y']);
            vaProxy.SetDecimal("~z", coords['z']);
            vaProxy.SetDecimal("~distance", distance);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_Spansh_SytemExists(dynamic vaProxy)
        {
            string system = vaProxy.GetText("~system") ?? throw new ArgumentNullException("~system");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\spansh.exe";
            string arguments = $@"systemexists ""{system}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            bool exists = true;
            bool error = false;
            string errorMessage = string.Empty;

            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            switch (p.ExitCode)
            {
                case 0:
                    Log.Info($@"System ""{system}"" found in Spansh’s DB");
                    break;
                case 1:
                    error = true;
                    errorMessage = stdout;
                    Log.Error(errorMessage);
                    break;
                case 3:
                    exists = false;
                    Log.Info($@"System ""{system}"" not found in Spansh’s DB");
                    break;
                default:
                    error = true;
                    Log.Error(stderr);
                    errorMessage = "Unrecoverable error in plugin.";
                    break;
            }

            vaProxy.SetBoolean("~systemExists", exists);
            vaProxy.SetBoolean("~error", error);
            vaProxy.SetText("~errorMessage", errorMessage);
            vaProxy.SetInt("~exitCode", p.ExitCode);
        }

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            Log.Notice("Finished startup.");
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }
}
