using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace kf2server_tbot.Utils {

    /// <summary>
    /// Handles execution of actions by enclosing in thread, and queueing.
    /// <para>Multiple requests can be made, but Selenium must retain focus on a single window at a time.</para>
    /// <para>To overcome this, a thread is created for each requested operation. 
    ///  Each thread is added to a global queue which executes threads as they come.</para>
    /// <para>ActionQueue also handles timing out of threads (operations) via thread abort, 
    ///  which is okay in this situation (since 'browsing' a page cannot be left in an inconsistent state)</para>
    /// </summary>
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

        #region Properties and Fields

        /// Using ConcurrentQueue for thread safety
        private readonly ConcurrentQueue<Task> queue = new ConcurrentQueue<Task>();

        private static readonly int TIMEOUT = Properties.Settings.Default.DefaultTaskTimeoutSeconds;
        #endregion


        /// <summary>
        /// Adds a thread encasing operation to the global ActionQueue.
        /// </summary>
        /// <param name="actualThread">Thread encasing requested operation</param>
        public void Act(Thread actualThread) {

            /// Used for cancellation token. Thread abort is called when the task receives a cancellation request.
            CancellationTokenSource cts = new CancellationTokenSource();

            /// Wraps thread in a task.
            /// Task handles timeout and queue operations.
            Task wrapperTask = new Task(() => {

                /// Initially cancelled check
                if (cts.Token.IsCancellationRequested) {
                    //Console.WriteLine("  Premature thread termination: {0}", Task.CurrentId);
                    this.Dequeue();
                    actualThread.Abort();
                }


                actualThread.Start();


                /// Continuous cancellation check
                while (true) {

                    /// Prevent processor starvation
                    Thread.SpinWait(10);

                    if (cts.Token.IsCancellationRequested) { /// Timed out
                        //Console.WriteLine("  Thread termination: {0}", Task.CurrentId);
                        actualThread.Abort();
                        this.Dequeue();
                        return;
                    }

                    if (!actualThread.IsAlive) { /// Completed Successfully
                        //Console.WriteLine("  Thread completion: {0}", Task.CurrentId);
                        this.Dequeue();
                        return;
                    }
                }

            }, cts.Token, TaskCreationOptions.None);

            /// Add to global queue
            Enqueue(wrapperTask, cts);
        }


        /// <summary>
        /// Adds a task to the global queue
        /// </summary>
        /// <param name="wrapperTask">Thread encased in a Task</param>
        /// <param name="cts">Cancellation Token</param>
        private void Enqueue(Task wrapperTask, CancellationTokenSource cts) {

            /// Timer to handle task timeout
            System.Timers.Timer timeoutTimer = new System.Timers.Timer(TIMEOUT * 1000);

            timeoutTimer.AutoReset = false;

            timeoutTimer.Elapsed += (Object source, ElapsedEventArgs e) => {
                if(!wrapperTask.IsCompleted) {
                    cts.Cancel();
                    cts.Dispose();
                }
            };


            /// If queue is empty, then this element will be the first.
            if (queue.Count == 0) {
                ///Console.WriteLine("  Enqueueing ({0})", wrapperTask.Id);
                queue.Enqueue(wrapperTask);
                wrapperTask.Start();
                timeoutTimer.Start();
                ///Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);
            }

            /// Otherwise, next wrapper task being added to queue will start as a continuation of the previous
            ///  (task chaining)
            else {
                //Console.WriteLine("  Enqueueing (child {0} of parent {1})", wrapperTask.Id, queue.Last<Task>().Id);

                queue.Last<Task>().ContinueWith((pendingTask) => {
                    //Console.WriteLine("Starting wrapperTask {0}", wrapperTask.Id);
                    wrapperTask.Start();
                    timeoutTimer.Start();
                });

                queue.Enqueue(wrapperTask);
                //Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);
            }

        }


        /// <summary>
        /// Dequeue task from global queue. 
        /// Performed on thread completion.
        /// </summary>
        /// <returns>Completed wrapper task</returns>
        private Task Dequeue() {

            try {

                Task item = null;
                queue.TryDequeue(out item);
                
                return item;

            } catch(Exception) {

                return null;

            }

        }


        //Testing, to be removed later.
        public void Test() {

            ActionQueue.Instance.Act(new Thread(() => {
                Console.WriteLine("...");
                for (int i = 0; i < new Random().Next(10, 20); i++) {
                    Console.Write("{0}.", Thread.CurrentThread.ManagedThreadId);
                    System.Threading.Thread.SpinWait(9000000);
                }
                Console.WriteLine("...done");
            }));
            ActionQueue.Instance.Act(new Thread(() => {
                Console.WriteLine("...");
                for (int i = 0; i < new Random().Next(10, 20); i++) {
                    Console.Write("{0}.", Thread.CurrentThread.ManagedThreadId);
                    System.Threading.Thread.SpinWait(9000000);
                }
                Console.WriteLine("...done");
            }));
            ActionQueue.Instance.Act(new Thread(() => {
                Console.WriteLine("...");
                for (int i = 0; i < new Random().Next(10, 20); i++) {
                    Console.Write("{0}.", Thread.CurrentThread.ManagedThreadId);
                    System.Threading.Thread.SpinWait(9000000);
                }
                Console.WriteLine("...done");
            }));
            ActionQueue.Instance.Act(new Thread(() => {
                Console.WriteLine("...");
                for (int i = 0; i < new Random().Next(10, 20); i++) {
                    Console.Write("{0}.", Thread.CurrentThread.ManagedThreadId);
                    System.Threading.Thread.SpinWait(9000000);
                }
                Console.WriteLine("...done");
            }));
        }


    }
}
