#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace alterNERDtive.util
{
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

    public enum LogLevel
    {
        ERROR,
        WARN,
        NOTICE,
        INFO,
        DEBUG
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
                    PassedText: strings == null ? null : $"\"{String.Join<string>("\";\"", strings)}\"",
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

        public VoiceAttackLog(dynamic vaProxy, string id) => (VA, ID) = (vaProxy, id);

        public void Log(string message, LogLevel level = LogLevel.INFO)
        {
            Log(ID, message, level);
        }

        public void Log(string sender, string message, LogLevel level = LogLevel.INFO)
        {
            _ = sender ?? throw new ArgumentNullException("sender");
            _ = message ?? throw new ArgumentNullException("message");

            if (level <= CurrentLogLevel)
                VA.WriteToLog($"{level} | {sender}: {message}", LogColour[(int)level]);
        }

        public void Error(string message) => Log(message, LogLevel.ERROR);
        public void Warn(string message) => Log(message, LogLevel.WARN);
        public void Notice(string message) => Log(message, LogLevel.NOTICE);
        public void Info(string message) => Log(message, LogLevel.INFO);
        public void Debug(string message) => Log(message, LogLevel.DEBUG);
    }

    public interface IPipable
    {
        public void ParseString(string serialization);
        public string ToString();
    }

    public class PipeServer<Thing> where Thing : IPipable, new()
    {
        private readonly string PipeName;
        private readonly SignalHandler Handler;
        private readonly VoiceAttackLog Log;

        private bool Running = false;

        private NamedPipeServerStream? Server;

        public PipeServer(VoiceAttackLog log, string name, SignalHandler handler)
            => (Log, PipeName, Handler) = (log, name, handler);

        public delegate void SignalHandler(Thing thing);

        public PipeServer<Thing> Run()
        {
            Log.Debug("Starting RATSIGNAL pipe …");
            if (!Running)
            {
                Running = true;
                WaitForConnection();
            }
            return this;
        }

        public PipeServer<Thing> Stop()
        {
            Log.Debug("Stopping RATSIGNAL pipe …");
            if (Running)
            {
                Running = false;
                Server!.Close();
            }
            return this;
        }

        private void WaitForConnection()
        {
            try
            {
                Server = new NamedPipeServerStream(PipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                Server.BeginWaitForConnection(OnConnect, Server);
            }
            catch (Exception e)
            {
                Log.Error($"Error setting up pipe: {e.Message}");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            NamedPipeServerStream server = (NamedPipeServerStream)ar.AsyncState;
            try
            {
                server.EndWaitForConnection(ar);
                WaitForConnection();
                using StreamReader reader = new StreamReader(server);
                Thing thing = new Thing();
                thing.ParseString(reader.ReadToEnd());
                Handler(thing);
            }
            catch (ObjectDisposedException)
            {
                Log.Info("Ratsignal pipe has been closed.");
            }
            catch (Exception e)
            {
                Log.Error($"Error reading pipe: {e.Message}");
            }
        }
    }
}
