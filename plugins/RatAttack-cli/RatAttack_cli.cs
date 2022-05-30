// <copyright file="RatAttack_cli.cs" company="alterNERDtive">
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
using System.IO;
using System.IO.Pipes;
using System.Text.RegularExpressions;

namespace RatAttack
{
    /// <summary>
    /// CLI helper tool for the RatAttack VoiceAttack plugin. Accepts RATSIGNALs
    /// e.g. from an IRC client and passes them to the plugin via named pipe.
    /// </summary>
    public class RatAttack_cli
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            RatAttack.Ratsignal ratsignal = new (StripIrcCodes(args[0]), args.Length > 1 && args[1].ToLower() == "true");

            using (NamedPipeClientStream pipeClient = new (".", "RatAttack", PipeDirection.Out))
            {
                try
                {
                    // try connecting for up to 2 minutes; then we’ll assume VoiceAttack just isn’t up and won’t come up
                    pipeClient.Connect(120000);
                    using StreamWriter writer = new (pipeClient);
                    writer.WriteLine(ratsignal);
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

        private static string StripIrcCodes(string message)
        {
            return Regex.Replace(message, @"[\x02\x11\x0F\x1D\x1E\x1F\x16]|\x03(\d\d?(,\d\d?)?)?", string.Empty);
        }
    }
}
