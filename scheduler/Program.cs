using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace ConsoleApplication1
{
    enum scheduler_type { FCFS, RR, SJF, P };
    
      
        class Process
        {
            public int PID, dur, arrival, priority;
            public long waitingtime=0, starttime, endtime,temp;
            public Thread t;
            long ref_time;
            public void Run(object o)
            {               
                Thread.Sleep((Int32)o);
            }
            public Process(long reftime)
            {
                ref_time=reftime;
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
            public void setwaitingtime(long w){waitingtime=w;}
            public long getwaitingtime(){return waitingtime;}
            public void start()
            {
                Console.WriteLine("Starting process :{0} at time : {1} ", PID, (DateTime.Now.ToBinary() - ref_time));
                t = new Thread(new ParameterizedThreadStart(Run));
                t.Start(dur);
                starttime = DateTime.Now.ToBinary();
                waitingtime += (starttime - arrival - ref_time);
            }
            public  void suspend()
            {
                temp = DateTime.Now.ToBinary();
                t.Suspend();
            }
            public void resume()
            {
                t.Resume();
                long restime = DateTime.Now.ToBinary();
                waitingtime +=( restime - temp);
            }
        }
        class Scheduler
        {
            private int Quantum;
            private int n;
            long ref_time;
            private Process[] parr;
            private scheduler_type sch_t;
            public Scheduler(long reftime)
            {
                ref_time = reftime;
                 Console.WriteLine("\nenter the number of processes : ");
                 parr = new Process[10];
                 string s = Console.ReadLine();
                 n = Convert.ToInt32(s);
                 for(int j=0;j<n;j++)
                     parr[j]=new Process(ref_time);
                 Console.WriteLine("\nenter the scheduler type :(FCFS/SJF/RR/P) ");
                 s = Console.ReadLine();
                 if (s == "FCFS") sch_t = scheduler_type.FCFS;
                 if (s == "RR") sch_t = scheduler_type.RR;
                 if (s == "SJF") sch_t = scheduler_type.SJF;
                 if (s == "P") sch_t = scheduler_type.P;
            }
            public void Run()
            {
                if (sch_t == scheduler_type.FCFS) FirstComeFirstServe();
                else if (sch_t == scheduler_type.RR) RoundRobin();
                else if (sch_t == scheduler_type.SJF) ShortestJobFirst();
                else if (sch_t == scheduler_type.P) Priority();
            }
            public void FirstComeFirstServe() 
            {
                for (int k = 0; k < n - 1;k++)
                    for(int l=k+1;l<n;l++)
                        if (parr[k].arrival > parr[l].arrival)
                        {
                            Process temp;
                            temp = parr[k];
                            parr[k] = parr[l];
                            parr[l] = temp;
                        }
                    for (int j = 0; j < n; j++)
                    {
                        parr[j].start();
                        parr[j].t.Join(int.MaxValue);
                        Console.WriteLine("Finish process :{0} at time : {1}  ...     Waiting Time :  {2}", parr[j].PID, (DateTime.Now.ToBinary()-ref_time),parr[j].getwaitingtime());
                    }
                    
            }
            public void RoundRobin()
            {
                for (int k = 0; k < n - 1; k++)
                    for (int l = k + 1; l < n; l++)
                        if (parr[k].arrival > parr[l].arrival)
                        {
                            Process temp;
                            temp = parr[k];
                            parr[k] = parr[l];
                            parr[l] = temp;
                        }
                int cont;
                bool first=true;
                long[] susptime = new long[10];
                for (int k = 0; k < n; k++)
                    susptime[k] = 0;
                while(true)
                {
                    for (int j = 0; j < n; j++)
                        if (parr[j].t.ThreadState != ThreadState.Aborted)
                            Console.WriteLine("Finish process :{0} at time : {1}  ...     Waiting Time :  {2}", parr[j].PID, (DateTime.Now.ToBinary()-ref_time), parr[j].getwaitingtime());
                    cont=0;
                    for (int j = 0; j < n; j++)
                        if (parr[j].t.ThreadState!=ThreadState.Aborted) cont = 1;
                    if (cont == 0) break;
                    long t1, t2;
                        if (first)
                        {
                            for (int j = 0; j < n; j++)
                            {
                                t1 = DateTime.Now.ToBinary();
                                parr[j].start();
                                parr[j].t.Join(Quantum);
                                t2 = DateTime.Now.ToBinary();
                                parr[j].suspend();
                                susptime[j] += (t2 - t1);
                                Console.WriteLine("Suspend process :{0} at time : {1} ", parr[j].PID, (DateTime.Now.ToBinary()-ref_time));
                                first = false;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < n; j++)
                            {
                                t1 = DateTime.Now.ToBinary();
                                Console.WriteLine("Resume process :{0} at time : {1} ", parr[j].PID, DateTime.Now.ToBinary());
                                parr[j].resume();
                                parr[j].t.Join(Quantum);
                                t2 = DateTime.Now.ToBinary();
                                parr[j].suspend();
                                susptime[j] += (t2 - t1);
                            }
                        }
                   }
                   for (int k = 0; k < 10; k++)
                      parr[k].setwaitingtime(susptime[k]);
            }
            public void ShortestJobFirst() 
            {
                for (int k = 0; k < n - 1; k++)
                    for (int l = k + 1; l < n; l++)
                        if (parr[k].dur > parr[l].dur)
                        {
                            Process temp;
                            temp = parr[k];
                            parr[k] = parr[l];
                            parr[l] = temp;
                        }
                for (int j = 0; j < n; j++)
                {
                    parr[j].start();
                    parr[j].t.Join(int.MaxValue);
                    Console.WriteLine("Finish process :{0} at time : {1}  ...     Waiting Time : {2}", parr[j].PID, (DateTime.Now.ToBinary()-ref_time),parr[j].getwaitingtime());
                }
                    
              
            }
            public void Priority() 
            {
                for (int k = 0; k < n - 1; k++)
                    for (int l = k + 1; l < n; l++)
                        if (parr[k].priority > parr[l].priority)
                        {
                            Process temp;
                            temp = parr[k];
                            parr[k] = parr[l];
                            parr[l] = temp;
                        }
                for (int j = 0; j < n; j++)
                {
                    parr[j].start();
                    parr[j].t.Join(int.MaxValue);
                    Console.WriteLine("Finish process :{0} at time : {1}  ...     Waiting Time :  {2}", parr[j].PID, (DateTime.Now.ToBinary()-ref_time), parr[j].getwaitingtime());
                }
                    
            }


        }

    class Program
    {
        static long ref_time= DateTime.Now.ToBinary();
        static void Main(string[] args)
        {
            Scheduler sch=new Scheduler(ref_time);
            sch.Run();
        }
    }
}
