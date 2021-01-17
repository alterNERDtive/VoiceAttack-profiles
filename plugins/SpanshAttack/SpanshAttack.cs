#nullable enable

using alterNERDtive.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SpanshAttack
{
    public class SpanshAttack
    {
        private static dynamic? VA { get; set; }

        private static VoiceAttackLog Log
            => log ??= new VoiceAttackLog(VA, "SpanshAttack");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands
            => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

        /*================\
        | plugin contexts |
        \================*/

        private static void Context_EDTS_GetCoordinates(dynamic vaProxy)
        {
            string system = vaProxy.GetText("~system") ?? throw new ArgumentNullException("~system");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\edts.exe";
            string arguments = $@"coords ""{system}""";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            Dictionary<char, int> coords = new Dictionary<char, int> { { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };
            int precision = 0;
            bool error = false;
            string errorMessage = "";

            p.Start();
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            switch (p.ExitCode)
            {
                case 0:
                    string[] stdoutExploded = stdout.Split('|');
                    precision = int.Parse(stdoutExploded[1]);
                    string[] stdoutCoords = stdoutExploded[0].Split(',');
                    coords['x'] = int.Parse(stdoutCoords[0]);
                    coords['y'] = int.Parse(stdoutCoords[1]);
                    coords['z'] = int.Parse(stdoutCoords[2]);
                    if (precision < 100)
                    {
                        Log.Info($"Coordinates for {system}: ({coords['x']}, {coords['y']}, {coords['z']}), precision: {precision} ly");
                    }
                    else
                    {
                        Log.Warn($"Coordinates with low precision for {system}: ({coords['x']}, {coords['y']}, {coords['z']}), precision: {precision} ly");
                    }
                    break;
                case 3:
                    errorMessage = $@"No coordinates could be determined for ""{system}"" with acceptable precision";
                    Log.Error(errorMessage);
                    break;
                case 4:
                    errorMessage = $@"""{system}"" is not a valied proc gen system name";
                    Log.Error(errorMessage);
                    break;
                default:
                    break;

            }

            vaProxy.SetInt("~x", coords['x']);
            vaProxy.SetInt("~y", coords['y']);
            vaProxy.SetInt("~z", coords['z']);
            vaProxy.SetInt("~precision", precision);
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

        private static void Context_Spansh_Nearestsystem(dynamic vaProxy)
        {
            int x = vaProxy.GetInt("~x") ?? throw new ArgumentNullException("~x");
            int y = vaProxy.GetInt("~y") ?? throw new ArgumentNullException("~y");
            int z = vaProxy.GetInt("~z") ?? throw new ArgumentNullException("~z");

            string path = $@"{vaProxy.SessionState["VA_SOUNDS"]}\Scripts\spansh.exe";
            string arguments = $@"nearestsystem --parsable {x} {y} {z}";

            Process p = PythonProxy.SetupPythonScript(path, arguments);

            Dictionary<char, decimal> coords = new Dictionary<char, decimal> { { 'x', 0 }, { 'y', 0 }, { 'z', 0 } };
            string system = "";
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
            string errorMessage = "";

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
            VA = vaProxy;
            Log.Notice("Finished startup.");
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly Version VERSION = new Version("7.0.0");

        public static Guid VA_Id()
            => new Guid("{e722b29d-898e-47dd-a843-a409c87e0bd8}");
        public static string VA_DisplayName()
            => $"SpanshAttack {VERSION}";
        public static string VA_DisplayInfo()
            => "SpanshAttack: a plugin for doing routing with spansh.co.uk for Elite: Dangerous.";

        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            VA.SetText("SpanshAttack.version", VERSION.ToString());
            Log.Notice("Init successful.");
        }

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
                    // EDTS
                    case "edts.getcoordinates":
                        Context_EDTS_GetCoordinates(vaProxy);
                        break;
                    // log
                    case "log.log":
                        Context_Log(vaProxy);
                        break;
                    // Spansh
                    case "spansh.systemexists":
                        Context_Spansh_SytemExists(vaProxy);
                        break;
                    case "spansh.nearestsystem":
                        Context_Spansh_Nearestsystem(vaProxy);
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
