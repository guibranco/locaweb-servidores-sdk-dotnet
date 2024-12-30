using System;

namespace LocawebServidores
{
    public class Class1
    {
        public async Task MainAsync(string[] args)
        {
            await Console.Out.WriteLineAsync(string.Join(" ", args));
        }
    }
}
