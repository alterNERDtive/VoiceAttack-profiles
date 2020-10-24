#nullable enable

using alterNERDtive.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Timers;

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

        private static VoiceAttackLog Log => log ??= new VoiceAttackLog(VA, "alterNERDtive-base");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

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

        private static void Context_DistanceBetween(dynamic vaProxy)
        {
            string fromSystem = vaProxy.GetText("~fromSystem");
            string toSystem = vaProxy.GetText("~toSystem");
            int roundTo = vaProxy.GetInt("~roundTo") ?? 2;

            string path = $"{vaProxy.GetText("Python.ScriptPath")}\\explorationtools.exe";
            string arguments = $"distancebetween --roundto {roundTo} \"{fromSystem}\" \"{toSystem}\"";

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
                catch (ArgumentException)
                {
                    Log.Error($"Invalid log level '${level}'.");
                }
            }
        }

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            VA = vaProxy;
            CheckProfiles(VA);
            Commands.RunAll(ActiveProfiles, "startup", logMissing: true);
            Log.Notice("Finished startup.");
        }

        private static void Context_SetLogLevel(dynamic vaProxy)
        {
            string level = vaProxy.GetText("~level");
            try
            {
                Log.SetCurrentLogLevel(level);
            }
            catch (ArgumentException)
            {
                Log.Error($"Invalid LogLevel '{level}'.");
            }
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly string VERSION = "0.0.1";

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
            Commands.TriggerEvent("alterNERDtive-base.initialized", wait: false);
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
                    // plugin methods
                    case "distancebetween":
                        Context_DistanceBetween(vaProxy);
                        break;
                    case "eddievent":
                        Context_EddiEvent(vaProxy);
                        break;
                    case "log":
                        Context_Log(vaProxy);
                        break;
                    case "startup":
                        Context_Startup(vaProxy);
                        break;
                    // plugin settings
                    case "setloglevel":
                        Context_SetLogLevel(vaProxy);
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