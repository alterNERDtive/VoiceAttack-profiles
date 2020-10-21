#nullable enable

using System.IO;
using System.IO.Pipes;

namespace RatAttack
{
    class RatAttack_cli
    {
        static void Main(string[] args)
        {
            RatAttack.Ratsignal ratsignal = new RatAttack.Ratsignal(args[0], args.Length > 1 && args[1].Equals("true"));

            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "RatAttack", PipeDirection.Out))
            {
                pipeClient.Connect(120); // 120s timeout should really be enough, no? if VA doesn’t respond you probably don’t want the threads to stick around.
                using (StreamWriter writer = new StreamWriter(pipeClient))
                {
                    writer.WriteLine(ratsignal);
                }
            }
        }
    }
}
