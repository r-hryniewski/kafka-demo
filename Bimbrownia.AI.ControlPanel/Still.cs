using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bimbrownia.AI.ControlPanel
{
    internal class Still
    {
        public Guid Id { get; }
        private CancellationTokenSource Cts { get; set; }
        private Task Task { get; set; }

        public Still()
        {
            Id = Guid.NewGuid();
        }

        internal void Run()
        {
            Cts = new CancellationTokenSource();

            Task = Task.Run(
                action: async () =>
                {
                    while (!Cts.Token.IsCancellationRequested)
                    {
                        await Console.Out.WriteAsync($"Still runing: {Id}");
                        await Task.Delay(500);
                    }
                },
                cancellationToken: Cts.Token);
        }

        internal async Task DisableAsync()
        {
            Cts.Cancel();
            await Task.FromCanceled(Cts.Token);
            Task.Dispose();
            Cts.Dispose();
        }
    }
}