#nullable enable

using alterNERDtive.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteAttack
{
    public class EliteAttack {
        private static dynamic? VA { get; set; }

        private static VoiceAttackLog Log
            => log ??= new VoiceAttackLog(VA, "EliteAttack");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands
            => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

        /*================\
        | plugin contexts |
        \================*/

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

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            VA = vaProxy;
            Log.Notice("Finished startup.");
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly Version VERSION = new Version("8.2.2");

        public static Guid VA_Id()
            => new Guid("{5B46321D-2935-4550-BEEA-36C2145547B8}");
        public static string VA_DisplayName()
            => $"EliteAttack {VERSION}";
        public static string VA_DisplayInfo()
            => "EliteAttack: a plugin for doing Elite-y things.";

        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
            VA.SetText("EliteAttack.version", VERSION.ToString());
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
