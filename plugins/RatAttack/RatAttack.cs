#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using alterNERDtive.util;

namespace RatAttack
{
    public class RatAttack
    {
        private static Dictionary<int,RatCase> CaseList { get; } = new Dictionary<int, RatCase>();
        private static dynamic? VA { get; set; }
        private static alterNERDtive.util.PipeServer<Ratsignal> RatsignalPipe
            => ratsignalPipe ??= new alterNERDtive.util.PipeServer<Ratsignal>(Log, "RatAttack",
                new alterNERDtive.util.PipeServer<Ratsignal>.SignalHandler(On_Ratsignal));
        private static alterNERDtive.util.PipeServer<Ratsignal>? ratsignalPipe;

        private static readonly Regex RatsignalRegex = new Regex(
            @"^RATSIGNAL Case #(?<number>\d+) (?<platform>(PC|Xbox|Playstation))(?<oxygen> \(Code Red\))?(?<odyssey> \(Odyssey\))? – CMDR (?<cmdr>.+) – System: (None|u\u200bnknown system|""(?<system>.+)"" \((?<systemInfo>([a-zA-Z0-9\s\(\)\-/]*(~?[0-9,\.]+ LY (""[a-zA-Z\-]+"" of|from) [a-zA-Z0-9\s\*\-]+)?( \([a-zA-Z\s]+\))?|Not found in galaxy database|Invalid system name))\)(?<permit> \(((?<permitName>.*) )?Permit Required\))?) – Language: (?<language>[a-zA-z0-9\x7f-\xff\-\(\)&,\s\.]+)( – Nick: (?<nick>[a-zA-Z0-9_\[\]\-]+))? \((PC|XB|PS)_SIGNAL\)\v*$"
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
            public string? Language;
            public string? System;
            public string? SystemInfo;
            public bool PermitLocked;
            public string? PermitName;
            public string Platform;
            public bool Odyssey;
            public bool CodeRed;
            public int Number;

            public RatCase(string cmdr, string? language, string? system, string? systemInfo, bool permitLocked, string? permitName, string platform, bool odyssey, bool codeRed, int number)
                => (Cmdr, Language, System, SystemInfo, PermitLocked, PermitName, Platform, Odyssey, CodeRed, Number) = (cmdr, language, system, systemInfo, permitLocked, permitName, platform, odyssey, codeRed, number);

            public string ShortInfo
            {
                get => $"#{Number}, {Platform}{(Odyssey ? " (Odyssey)" : "")}{(CodeRed ? ", code red" : "")}, {System ?? "None"}{(SystemInfo != null ? $" ({SystemInfo}{(PermitLocked ? ", permit required" : "")})" : "")}";
            }

            public override string ToString()
                => ShortInfo;
        }

        public class Ratsignal : IPipable
        {
            public string Signal { get; set; }
            public bool Announce { get; set; }
            private readonly char separator = '\x1F';

            public Ratsignal()
                => (Signal, Announce) = ("", false);

            public Ratsignal(string signal, bool announce)
                => (Signal, Announce) = (signal, announce);

            public void ParseString(string serialization)
            {
                try
                {
                    string[] parts = serialization.Split(separator);
                    Signal = parts[0];
                    Announce = Boolean.Parse(parts[1]);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Invalid serialized RATSIGNAL: '{serialization}'", e);
                }
            }

            public override string ToString()
                => $"{Signal}{separator}{Announce}";
        }

        private static int ParseRatsignal(string ratsignal)
        {
            if (!RatsignalRegex.IsMatch(ratsignal))
                throw new ArgumentException($"Invalid RATSIGNAL format: '{ratsignal}'.", "ratsignal");

            Match match = RatsignalRegex.Match(ratsignal);

            string cmdr = match.Groups["cmdr"].Value;
            string? language = match.Groups["language"].Value;
            string? system = match.Groups["system"].Value;
            string? systemInfo = match.Groups["systemInfo"].Value;
            bool permitLocked = match.Groups["permit"].Success;
            string? permitName = match.Groups["permitName"].Value;
            string platform = match.Groups["platform"].Value;
            bool codeRed = match.Groups["oxygen"].Success;
            bool odyssey = match.Groups["odyssey"].Success;

            int number = int.Parse(match.Groups["number"].Value);

            if (String.IsNullOrEmpty(system))
            {
                system = "None";
            }

            Log.Debug($"New rat case: CMDR “{cmdr}” in “{system}”{(systemInfo != null ? $" ({systemInfo})" : "")} on {platform}{(odyssey ? " (Odyssey)" : "")}, permit locked: {permitLocked}{(permitLocked && permitName != null ? $" (permit name: {permitName})" : "")}, code red: {codeRed} (#{number}).");

            CaseList[number] = new RatCase(cmdr, language, system, systemInfo, permitLocked, permitName, platform, odyssey, codeRed, number);

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
            bool error = true;

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
                vaProxy.SetBoolean("~~odyssey", rc?.Odyssey);
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

        static readonly Version VERSION = new Version("6.3");

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
            VA.SetText("RatAttack.version", VERSION.ToString());
            vaProxy.ProfileChanged += new Action<Guid?, Guid?, String, String>(On_ProfileChanged);
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
                    // EDSM
                    case "edsm.getnearestcmdr":
                        Context_EDSM_GetNearestCMDR(vaProxy);
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
