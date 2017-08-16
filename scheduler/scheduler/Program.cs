using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace ConsoleApplication1
{
    class Program
    {
        static DateTime ref_time;
        static void Run(object o)
        {
            Thread.Sleep((Int32)o);
        }
        class Process
        {
            private int PID, dur, arrival, priority;
            private long waitingtime=0, starttime, endtime,temp;
            Thread t;
            Process()
            {
                Console.Write("Enter PID  :");
                string s = Console.ReadLine();
                PID = Convert.ToInt32(s);
                Console.Write("Enter process duration  :");
                s = Console.ReadLine();
                dur = Convert.ToInt32(s);
                Console.Write("Enter arrival time  :");
                s = Console.ReadLine();
                arrival = Convert.ToInt32(s);
                Console.Write("Enter process priority  :");
                s = Console.ReadLine();
                priority = Convert.ToInt32(s);
            }
            void start()
            {
                t = new Thread(new ParameterizedThreadStart(Run));
                t.Start(dur);
                starttime = DateTime.Now.ToBinary();
                waitingtime += (starttime - arrival);
            }
            void suspend()
            {
                temp = DateTime.Now.ToBinary();
                t.Suspend();
            }
            void resume()
            {
                t.Resume();
                long restime = DateTime.Now.ToBinary();
                waitingtime +=( restime - temp);
            }
        }
        class Scheduler
        {
            private Process[] parr;
            Scheduler()
            {

               // parr = new Process[10];
            }
        }

        static void Main(string[] args)
        {
        }
    }
}
