#nullable enable

using System;
using System.IO;
using System.IO.Pipes;
using System.Text.RegularExpressions;

namespace RatAttack
{
    class RatAttack_cli
    {
        static string stripIrcCodes(string message)
        {
            return Regex.Replace(message, @"[\x02\x11\x0F\x1D\x1E\x1F\x16]|\x03(\d\d?(,\d\d?)?)?", String.Empty);
        }

        static void Main(string[] args)
        {
            RatAttack.Ratsignal ratsignal = new RatAttack.Ratsignal(stripIrcCodes(args[0]), args.Length > 1 && args[1].ToLower() == "true");

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "RatAttack", PipeDirection.Out))
            {
                try
                {
                    // try connecting for up to 2 minutes; then we’ll assume VoiceAttack just isn’t up and won’t come up
                    pipeClient.Connect(120000);
                    using (StreamWriter writer = new StreamWriter(pipeClient))
                    {
                        writer.WriteLine(ratsignal);
                    }
                }
                catch (TimeoutException)
                {
                    Console.Error.WriteLine("Connection to RatAttack pipe has timed out.");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.Error.WriteLine("Cannot connect to RatAttack pipe. Are you running VoiceAttack as Admin?");
                }
            }
        }
    }
}
