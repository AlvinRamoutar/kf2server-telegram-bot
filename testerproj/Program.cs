using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace testerproj {
    class Program {

        static void Main(string[] args) {

            Init();

            Console.ReadKey();
        }

        private static void Init() {

            ActionFactory af = new ActionFactory();

            Thread t1 = new Thread(() => {
                Console.WriteLine("Starting Thread {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000);
                Console.WriteLine("Complete Thread {0}", Thread.CurrentThread.ManagedThreadId);
            });

            Thread t2 = new Thread(() => {
                Console.WriteLine("Starting Thread {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(4000);
                Console.WriteLine("Complete Thread {0}", Thread.CurrentThread.ManagedThreadId);
            });

            Thread t3 = new Thread(() => {
                Console.WriteLine("Starting Thread {0}", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2900);
                Console.WriteLine("Complete Thread {0}", Thread.CurrentThread.ManagedThreadId);
            });

            //https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c
            //Task<Task<bool>> task = Task.Run<Task<bool>>(async () => await af.Act(t1));
            //task.Wait();

            af.Act(t1);
            af.Act(t2);
            af.Act(t3);
        }

    }


    class ActionFactory {

        public static bool Busy { get; private set; }

        private static Queue<Thread> Pending = new Queue<Thread>();

        private static readonly int TASK_TIMEOUT = 3000;

        public ActionFactory() {
            new Task(() => {
                Watchman();
            });
        }


        public void Act(Thread actualThread) {

            if(!Busy) {

                System.Timers.Timer timer = new System.Timers.Timer(TASK_TIMEOUT);
                timer.Elapsed += (Object source, ElapsedEventArgs e) => {
                    Console.WriteLine("Aborting thread {0}", actualThread.ManagedThreadId);
                    actualThread.Abort();
                };
                timer.AutoReset = false;

                actualThread.Start();
                timer.Start();
                   
                while(actualThread.IsAlive) {
                    Thread.Sleep(10);
                }
                Console.WriteLine("Complete Thread {0}", actualThread.ManagedThreadId);

            } else {

                Pending.Enqueue(actualThread);

            }

        }

        private void Watchman() {

            while(true) {
                if(!Busy && Pending.Count > 0) {
                    Act(Pending.Dequeue());
                }
                Thread.SpinWait(5000);
            }

        }
        

    }
}
