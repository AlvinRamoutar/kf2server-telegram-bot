using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testerproj {
    class Program {

        ActionFactory af = new ActionFactory();

        static void Main(string[] args) {

            //ActionFactory af = new ActionFactory();
            Program p = new Program();

            Console.WriteLine("=BEGIN=");

            Console.WriteLine(p.Numero());
            Console.WriteLine(p.Numero2());
            Console.WriteLine(p.Numero3());

            Console.WriteLine("=END=");

            Console.ReadLine();

        }

        public int Numero() {

            Console.WriteLine("Enqueueing Task");

            Task<int> t = new Task<int>(() => {
                
                int num = new Random().Next(Int32.MinValue, Int32.MaxValue);
                return new ReturnTest().Ret(num);
            });

            af.AddAction(t);

            return t.Result;
        }

        public int Numero2() {

            Console.WriteLine("Enqueueing Task");

            Task<int> t = new Task<int>(() => {

                int num = new Random().Next(Int32.MinValue, Int32.MaxValue);
                return new ReturnTest().Ret(num);
            });

            af.AddAction(t);

            return t.Result;
        }

        public int Numero3() {

            Console.WriteLine("Enqueueing Task");

            Task<int> t = new Task<int>(() => {

                int num = new Random().Next(Int32.MinValue, Int32.MaxValue);
                return new ReturnTest().Ret(num);
            });

            af.AddAction(t);

            return t.Result;
        }

    }

    class ActionFactory {

        private static readonly int TASK_TIMEOUT = 3;

        static Queue<Task> PendingActions { get; set; }

        public ActionFactory() {
            PendingActions = new Queue<Task>();
        }

        public void AddAction(Task<int> task) {

            Task taskWrapper = new Task(() => {

                Task<int> actualTask = task;

                try {
                    Console.WriteLine("Starting a task! {0}", actualTask.Id);
                    actualTask.Start();

                    if (!actualTask.Wait(TASK_TIMEOUT * 1000)) {
                        Console.WriteLine("Task Timeout! {0}", actualTask.Id);
                        return;
                    }

                    Console.WriteLine("Task ended! {0}", actualTask.Id);
                } catch (Exception e) {

                    //Act on exceptions
                    Console.WriteLine("EGGception! {0}", actualTask.Id);

                }

                Console.WriteLine("Concluded TaskWrapper task");
            });

            if (PendingActions.Count != 0) {

                Console.WriteLine("There is a lineup! Placing this task in queue. Remaining tasks: {0}",
                    PendingActions.Count);


                taskWrapper.ContinueWith((pendingTask) => {
                    Task.WaitAll(PendingActions.First());
                    Console.WriteLine("Task Concluded");
                    PendingActions.Dequeue();
                });
                taskWrapper.Start();
                PendingActions.Enqueue(taskWrapper);

            } else {

                Console.WriteLine("Task ready; no queue!");
                PendingActions.Enqueue(taskWrapper);
                taskWrapper.Start();
            }
        }

    }

    class ReturnTest {

        public int Ret(int squarez) {

            //int sleepTime = new Random().Next(1000, 10000);
            int sleepTime = 5000;

            Console.WriteLine("This task is sleeping for: {0}", sleepTime);
            System.Threading.Thread.Sleep(sleepTime);
            return squarez * squarez;

        }
    }
}
