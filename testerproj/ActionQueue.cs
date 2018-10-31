using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace testerproj {
    class ActionQueue {

        #region Singleton Structure
        private static ActionQueue instance = null;
        private static readonly object padlock = new object();

        public static ActionQueue Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ActionQueue();
                    }
                    return instance;
                }
            }
        }
        ActionQueue() { }
        #endregion

        private readonly Queue<Task> queue = new Queue<Task>();


        public void Act(Thread actualThread) {

            CancellationTokenSource cts = new CancellationTokenSource();

            Task wrapperTask = new Task(() => {
                // Initially cancelled check
                if (cts.Token.IsCancellationRequested) {

                    Console.WriteLine("  Premature thread termination: {0}", Task.CurrentId);

                    this.Dequeue();

                    actualThread.Abort();

                }

                actualThread.Start();

                // Continuous cancellation check
                while (true) {
                    if (cts.Token.IsCancellationRequested) {
                        Console.WriteLine("  Thread termination: {0}", Task.CurrentId);
                        actualThread.Abort();

                        this.Dequeue();

                        return;
                    }
                    if (!actualThread.IsAlive) {
                        Console.WriteLine("  Thread completion: {0}", Task.CurrentId);

                        this.Dequeue();

                        return;
                    }
                }

            }, cts.Token, TaskCreationOptions.None);

            Enqueue(wrapperTask, cts);
        }


        public void Enqueue(Task wrapperTask, CancellationTokenSource cts) {

            System.Timers.Timer timeoutTimer = new System.Timers.Timer(5000);
            timeoutTimer.AutoReset = false;
            timeoutTimer.Elapsed += (Object source, ElapsedEventArgs e) => {
                if(!wrapperTask.IsCompleted) {
                    Console.WriteLine("Timeout. Aborting.");
                    cts.Cancel();
                    cts.Dispose();
                }
            };


            lock (queue) {
                if (queue.Count == 0) {

                    Console.WriteLine("  Enqueueing ({0})", wrapperTask.Id);
                    queue.Enqueue(wrapperTask);
                    wrapperTask.Start();
                    timeoutTimer.Start();
                    Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);

                }

                else {
                    Console.WriteLine("  Enqueueing (child {0} of parent {1})", wrapperTask.Id, queue.Last<Task>().Id);

                    queue.Last<Task>().ContinueWith((pendingTask) => {
                        Console.WriteLine("Starting wrapperTask {0}", wrapperTask.Id);
                        wrapperTask.Start();
                        timeoutTimer.Start();
                    });

                    queue.Enqueue(wrapperTask);
                    Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);
                }
            }

        }



        public Task Dequeue() {
            lock (queue) {

                try {
                    Task item = queue.Dequeue();

                    return item;

                } catch(Exception) {

                    return null;
                }
            }
        }




    }
}
