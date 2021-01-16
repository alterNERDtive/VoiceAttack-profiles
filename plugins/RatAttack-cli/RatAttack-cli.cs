#nullable enable

using System;
using System.IO;
using System.IO.Pipes;

namespace RatAttack
{
    class RatAttack_cli
    {
        static void Main(string[] args)
        {
            RatAttack.Ratsignal ratsignal = new RatAttack.Ratsignal(args[0], args.Length > 1 && args[1] == "true");

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
            }
        }
    }
}
