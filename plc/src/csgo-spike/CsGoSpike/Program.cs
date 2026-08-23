using System;
using System.Threading.Tasks;
using Go;

namespace CsGoSpike
{
    /// <summary>
    /// Minimal producer-consumer over CsGo's CSP channel (chan&lt;T&gt;),
    /// driven by the library's cooperative strand scheduler (work_service).
    /// </summary>
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== CsGo spike: producer -> consumer over chan<int> ===");

            var work = new work_service();
            shared_strand strand = new work_strand(work);
            // bounded channel, buffer = 4 (len > 0 => limit_chan)
            chan<int> ch = chan<int>.make(strand, 4);

            // Root generator completes -> stop the scheduler so work.run() returns.
            generator.go(strand, () => MainWorker(strand, ch), completedHandler: () => work.stop());
            work.run();

            Console.WriteLine("=== work_service stopped; spike finished ===");
        }

        static async Task MainWorker(shared_strand strand, chan<int> ch)
        {
            var children = new generator.children();
            children.go(strand, () => Producer(ch, 5));
            children.go(strand, () => Consumer(ch));
            await children.wait_all();
        }

        static async Task Producer(chan<int> ch, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                await ch.send(i);
                Console.WriteLine($"[producer] sent {i}");
            }
            ch.close(); // sender closes: canonical Go pattern unblocks the consumer
            Console.WriteLine("[producer] closed channel");
        }

        static async Task Consumer(chan<int> ch)
        {
            int total = 0;
            while (true)
            {
                chan_recv_wrap<int> r = await ch.receive();
                if (r.state != chan_state.ok)
                {
                    Console.WriteLine($"[consumer] channel {r.state}, exiting");
                    break;
                }
                total++;
                Console.WriteLine($"[consumer] recv {r.msg} (total {total})");
            }
        }
    }
}
