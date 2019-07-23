using System.Collections.Generic;
using System.Linq;
namespace SRTF
{
    class process
    {
        public static int count;
        public string name { get; set; }
        public int burst { get; set; }
        public int arrival { get; set; }
        public int start { get; set; }
        public int finish { get; set; }
        public int remaining { get; set; }
        public int wait { get; set; }
        public int TA { get; set; }
        public bool isClone { get; set; }
        public static Stack<process> Terminated { get; set; }
        public process()
        {
            count++;
            this.start = -1;
            isClone = false;
        }
        public process(process Cp)
        {
            name = Cp.name;
            arrival = Cp.arrival;
            start = Cp.start;
            finish = Cp.finish;
            remaining = Cp.remaining;
        }

        public static void scheduleSJF(List<process> P, bool preemptive)
        {
            Terminated = new Stack<process>();

            int elapsedTime = 0; //clock
            while (P.Count != 0)
            {
                new_come:
                process running = whoCame(P, elapsedTime, preemptive);
                int enterTime = elapsedTime; // useful if the running process will be prempeted 

                if (running == null)
                {
                    elapsedTime++;
                    continue;
                }

                if (running.start == -1)
                    running.start = enterTime;

                while (running.remaining > 0)
                {
                    running.remaining--;
                    elapsedTime++;

                    if (preemptive && running != whoCame(P, elapsedTime, preemptive))
                    {
                        // store for Gantt Chart
                        process preempted = new process(running)
                        {
                            start = enterTime,
                            finish = elapsedTime,
                            isClone = true
                        };
                        Terminated.Push(preempted);

                        goto new_come;
                    }

                }
                running.finish = elapsedTime;
                Terminated.Push(running); // terminate the running process
                P.Remove(running); // remove it from ready queue
            }
        }

        private static process whoCame(List<process> P, int elapsedTime, bool preemptive)
        {
            return P.Where(p => p.arrival <= elapsedTime).OrderBy(p => preemptive ? p.remaining : p.burst).FirstOrDefault();
        }
    }
}
