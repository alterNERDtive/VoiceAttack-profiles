#nullable enable

using System;
using System.Text.RegularExpressions;
using alterNERDtive.util;

namespace RatAttack
{
    public class RatAttack
    {
        private static RatCase[] CaseList { get; } = new RatCase[20];
        private static dynamic? VA { get; set; }
        private static alterNERDtive.util.PipeServer<Ratsignal> RatsignalPipe
            => ratsignalPipe ??= new alterNERDtive.util.PipeServer<Ratsignal>(Log, "RatAttack",
                new alterNERDtive.util.PipeServer<Ratsignal>.SignalHandler(On_Ratsignal));
        private static alterNERDtive.util.PipeServer<Ratsignal>? ratsignalPipe;

        private static Regex RatsignalRegex = new Regex(
            @"^RATSIGNAL - CMDR (?<cmdr>.+) - Reported System: (?<system>.+) \(([0-9,\.]+ LY from .*|not in galaxy database|landmark)\)(?<permit> \x034\(((?<permitName>.*) )?Permit Required\)\x03)? - Platform: \x03(6|3|12)(?<platform>(PC|Xbox|PS4))\x03 - O2: (\x034)?(?<oxygen>(NOT )?OK)\x03? Language: .+ \(Case #(?<number>\d+)\) \((PC|XB|PS)_SIGNAL\)\v*$"
            );

        private static VoiceAttackLog Log
            => log ??= new VoiceAttackLog(VA, "RatAttack");
        private static VoiceAttackLog? log;

        private static VoiceAttackCommands Commands
            => commands ??= new VoiceAttackCommands(VA, Log);
        private static VoiceAttackCommands? commands;

        private class RatCase
        {
            public string Cmdr;
            public string System;
            public bool PermitLocked;
            public string Platform;
            public bool CodeRed;
            public int Number;

            public RatCase(string cmdr, string system, bool permitLocked, string platform, bool codeRed, int number)
                => (Cmdr, System, PermitLocked, Platform, CodeRed, Number) = (cmdr, system, permitLocked, platform, codeRed, number);

            public string ShortInfo
            {
                get => $"#{Number}, {Platform}, {System}{(PermitLocked ? " (permit required)" : "")}{(CodeRed ? ", code red" : "")}";
            }

            public override string ToString()
                => ShortInfo;
        }

        public class Ratsignal : IPipable
        {
            public string Signal { get; set; }
            public bool Announce { get; set; }

            public Ratsignal()
                => (Signal, Announce) = ("", false);

            public Ratsignal(string signal, bool announce)
                => (Signal, Announce) = (signal, announce);

            public void ParseString(string serialization)
            {
                try
                {
                    string[] parts = serialization.Split('|');
                    Signal = parts[0];
                    Announce = Boolean.Parse(parts[1]);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Invalid serialized RATSIGNAL: '{serialization}'", e);
                }
            }

            public override string ToString()
                => $"{Signal}|{Announce}";
        }

        private static int ParseRatsignal(string ratsignal)
        {
            if (!RatsignalRegex.IsMatch(ratsignal))
                throw new ArgumentException($"Invalid RATSIGNAL format: '{ratsignal}'.", "ratsignal");

            Match match = RatsignalRegex.Match(ratsignal);

            string cmdr = match.Groups["cmdr"].Value;
            string system = match.Groups["system"].Value;
            bool permitLocked = match.Groups["permit"].Success;
            string permitName = match.Groups["permitName"].Value ?? "";
            string platform = match.Groups["platform"].Value;
            bool codeRed = match.Groups["oxygen"].Value.Equals("NOT OK");

            int number = int.Parse(match.Groups["number"].Value);

            Log.Debug($"New rat case: CMDR “{cmdr}” in “{system}” on {platform}, permit locked: {permitLocked}{(permitLocked && !permitName.Equals("") ? $" (permit name: {permitName})" : "")}, code red: {codeRed} (#{number}).");

            CaseList[number] = new RatCase(cmdr, system, permitLocked, platform, codeRed, number);

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

        /*================\
        | plugin contexts |
        \================*/

        private static void Context_GetCaseData(dynamic vaProxy)
        {
            int cn = vaProxy.GetInt("~caseNumber");
            RatCase rc = CaseList[cn];

            vaProxy.SetInt("~~caseNumber", rc?.Number);
            vaProxy.SetText("~~cmdr", rc?.Cmdr);
            vaProxy.SetText("~~system", rc?.System.ToLower());
            vaProxy.SetBoolean("~~permitLocked", rc?.PermitLocked);
            vaProxy.SetText("~~platform", rc?.Platform);
            vaProxy.SetBoolean("~~codeRed", rc?.CodeRed);
        }

        private static void Context_Startup(dynamic vaProxy)
        {
            Log.Notice("Starting up …");
            VA = vaProxy;
            _ = RatsignalPipe.Run();
            Log.Notice("Finished startup.");
        }

        private static void Context_ParseRatsignal(dynamic vaProxy)
        {
            Log.Warn("Passing a RATSIGNAL from VoiceAttack (through the clipboard or a file) is DEPRECATED and will no longer be supported in the future.");
            On_Ratsignal(new Ratsignal(vaProxy.GetText("~ratsignal"), vaProxy.GetBoolean("~announceRatsignal")));
        }

        /*========================================\
        | required VoiceAttack plugin shenanigans |
        \========================================*/

        static readonly string VERSION = "0.0.1";

        public static Guid VA_Id()
            => new Guid("{F2ADF0AE-4837-4E4A-9C87-8A7E2FA63E5F}");
        public static string VA_DisplayName()
            => $"RatAttack {VERSION}";
        public static string VA_DisplayInfo()
            => "RatAttack: a plugin to handle FuelRats cases.";

        public static void VA_Init1(dynamic vaProxy)
        {
            VA = vaProxy;
            Log.Notice("Initializing …");
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
                    case "getcasedata":
                        Context_GetCaseData(vaProxy);
                        break;
                    case "parseratsignal":
                        Context_ParseRatsignal(vaProxy);
                        break;
                    case "startup":
                        Context_Startup(vaProxy);
                        break;
                    // plugin settings
                    // NYI
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

        public static void VA_Exit1(dynamic vaProxy)
        {
            Log.Debug("Starting teardown …");
            Log.Debug("Closing RATSIGNAL pipe …");
            RatsignalPipe.Stop();
            Log.Debug("Teardown finished.");
        }

        public static void VA_StopCommand() { }
    }
}
