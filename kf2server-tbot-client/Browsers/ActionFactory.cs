using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace kf2server_tbot_client.Browsers {

    class ActionFactory {


        #region Singleton Structure
        private static ActionFactory instance = null;
        private static readonly object padlock = new object();

        public static ActionFactory Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ActionFactory();
                    }
                    return instance;
                }
            }
        }

        ActionFactory() { }
        #endregion


        private static readonly int TASK_TIMEOUT = Properties.Settings.Default.DefaultTaskTimeoutSeconds;

        public static List<int> tracker = new List<int>();

        ActionQueue aQ = new ActionQueue();

        Queue<Task> Pending = new Queue<Task>();
        Task lastTask { get; set; }

        object loque = new object();

        public void AddAction(Task<object> task) {

            Task<object> taskWrapper = new Task<object>(() => {

                Task<object> actualTask = task;

                try {
                    actualTask.Start();

                    if (actualTask.Wait(TASK_TIMEOUT * 1000))
                        return actualTask.Result;

                    return 0;

                } catch (Exception e) {

                    //Act on exceptions
                    return null;

                } finally {
                    actualTask.Dispose();
                }



            }, TaskCreationOptions.LongRunning);

            taskWrapper.Start();

        }


        public Tuple<bool, string> Add(Thread t) {

            CancellationTokenSource cts = new CancellationTokenSource();
            Tuple<bool, string> result = new Tuple<bool, string>(false, "Generic Error");

            ActionFactory.Instance.EnhancedAdd(t, cts);

            Thread.Sleep(3000);
            cts.Cancel();
            cts.Dispose();

            return result;
        }


        public void EnhancedAdd(Thread actualThread, CancellationTokenSource cts) {

            aQ.Enqueue(actualThread, cts);

        }


        /// <summary>
        /// Task.ContinueWith busted; execution order wrong
        /// </summary>
        /// <param name="actualThread"></param>
        /// <param name="cts"></param>
        public void EnhancedAdd312(Thread actualThread, CancellationTokenSource cts) {

            Task wrapperTask = new Task(() => {
                // Initially cancelled check
                if (cts.Token.IsCancellationRequested) {

                    Console.WriteLine("  Premature thread termination: {0}", Task.CurrentId);
                    actualThread.Abort();
                   
                }

                actualThread.Start();

                // Continuous cancellation check
                while (true) {
                    if (cts.Token.IsCancellationRequested) {
                        Console.WriteLine("  Thread termination: {0}", Task.CurrentId);
                        actualThread.Abort();

                        break;
                    }
                    if (!actualThread.IsAlive) {
                        Console.WriteLine("  Thread completion: {0}", Task.CurrentId);

                        break;
                    }
                }
            }, cts.Token, TaskCreationOptions.None);

            lock (loque) {
                if (lastTask == null) {

                    Console.WriteLine("  Enqueueing ({0})", wrapperTask.Id);
                    wrapperTask.Start();
                    Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);

                }

                else {
                    Console.WriteLine("  Enqueueing (child {0} of parent {1})", wrapperTask.Id, lastTask.Id);

                    tracker.Add(wrapperTask.Id);

                    lastTask.ContinueWith((pendingTask) => {
                        Console.WriteLine("Starting wrapperTask {0}", wrapperTask.Id);
                        wrapperTask.Start();
                    });
                    Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);
                }

                lastTask = wrapperTask;
            }



        }



        public void EnhancedAdd5(Thread actualThread, CancellationTokenSource cts) {



            Task wrapperTask = new Task(() => {
                // Initially cancelled check
                if (cts.Token.IsCancellationRequested) {
                    Console.WriteLine("Premature thread termination: {0}", Task.CurrentId);

                    lock (loque) {
                        Pending.Dequeue();
                    }

                    actualThread.Abort();
                }

                actualThread.Start();

                // Continuous cancellation check
                while (true) {
                    if (cts.Token.IsCancellationRequested) {
                        Console.WriteLine("Thread termination: {0}", Task.CurrentId);
                        actualThread.Abort();

                        try {
                            lock (loque) {
                                Pending.Dequeue();
                            }
                        }
                        catch (Exception) {
                            Console.WriteLine("Cannot dequeue; queue is emptyadu");
                        }

                        break;
                    }
                    if (!actualThread.IsAlive) {
                        Console.WriteLine("Thread completion: {0}", Task.CurrentId);

                        try {
                            lock (loque) {
                                Pending.Dequeue();
                            }
                        }
                        catch (Exception) {
                            Console.WriteLine("Cannot dequeue; queue is emptyadu");
                        }

                        break;
                    }
                }
            }, cts.Token, TaskCreationOptions.None);

            lock (loque) {
                if (Pending.Count != 0) {

                    Pending.Last<Task>().ContinueWith((pendingTask) => {
                        Console.WriteLine("Starting task {0}", wrapperTask.Id);
                        wrapperTask.Start();
                    });

                    try {
                        Console.WriteLine("  Enqueueing (child {0} of {1})... Remaining: {2}", wrapperTask.Id, Pending.Last<Task>().Id, Pending.Count);
                        Pending.Enqueue(wrapperTask);
                        Console.WriteLine("  Successfully enqueued. Remaining: {0}", Pending.Count);
                    }
                    catch (Exception e) { Console.WriteLine("Eggception when enqueueing: " + e.Message); }

                    //object locker = new object();
                    //lock (locker) { Pending.Enqueue(wrapperTask); }
                }
                else {

                    wrapperTask.Start();

                    try {
                        Console.WriteLine("  Enqueueing... ({0}) Remaining: {1}", wrapperTask.Id, Pending.Count);
                        object locker = new object();
                        lock (locker) {
                            Pending.Enqueue(wrapperTask);
                        }

                        Console.WriteLine("  Successfully enqueued. Remaining: {0}", Pending.Count);
                    }
                    catch (Exception e) { Console.WriteLine("Eggception when enqueueing: " + e.Message); }

                    //object locker = new object();
                    //lock (locker) { Pending.Enqueue(wrapperTask); }

                }
            } 

        }



        public void EnhancedAdd1(Thread actualThread, CancellationTokenSource cts) {

            Task wrapperTask = new Task(() => {
                // Initially cancelled check
                if (cts.Token.IsCancellationRequested) {
                    Console.WriteLine("Premature thread termination: {0}", Task.CurrentId);

                    /*
                    try {
                        Console.WriteLine("  Dequeueing... Remaining: {0}", Pending.Count);
                        Pending.Dequeue();
                        Console.WriteLine("  Successfully dequeued. Remaining: {0}", Pending.Count);
                    }
                    catch (Exception e) { Console.WriteLine("Eggception when dequeing: " + e.Message); }
                    */
                    Pending.Dequeue();

                    actualThread.Abort();
                }

                actualThread.Start();

                // Continuous cancellation check
                while (true) {
                    if (cts.Token.IsCancellationRequested) {
                        Console.WriteLine("Thread termination: {0}", Task.CurrentId);
                        actualThread.Abort();

                        /*
                        try {
                            Console.WriteLine("  Dequeueing... Remaining: {0}", Pending.Count);
                            Pending.Dequeue();
                            Console.WriteLine("  Successfully dequeued. Remaining: {0}", Pending.Count);
                        } catch (Exception e) { Console.WriteLine("Eggception when dequeing: " + e.Message);  }
                        */

                        try {
                            Pending.Dequeue();
                        } catch(Exception) {
                            Console.WriteLine("Cannot dequeue; queue is emptyadu");
                        }


                        //object locker = new object();
                        //lock (locker) { Pending.Dequeue();
                        //    Console.WriteLine("  Successfully dequeued. Remaining: {0}", Pending.Count);
                        //}
                        break;
                    }
                    if(!actualThread.IsAlive) {
                        Console.WriteLine("Thread completion: {0}", Task.CurrentId);

                        /*
                        try {
                            Console.WriteLine("  Dequeueing... Remaining: {0}", Pending.Count);
                            Pending.Dequeue();
                            Console.WriteLine("  Successfully dequeued. Remaining: {0}", Pending.Count);
                        }
                        catch (Exception e) { Console.WriteLine("Eggception when dequeing: " + e.Message); }
                        */

                        try {
                            Pending.Dequeue();
                        }
                        catch (Exception) {
                            Console.WriteLine("Cannot dequeue; queue is emptyadu");
                        }

                        break;
                    }
                }
            }, cts.Token, TaskCreationOptions.None);

            object queueLock = new object();

            lock(queueLock) {
                if (Pending.Count != 0) {

                    Pending.Last<Task>().ContinueWith((pendingTask) => {
                        Console.WriteLine("Starting task {0}", wrapperTask.Id);
                        wrapperTask.Start();
                    });

                    try {
                        Console.WriteLine("  Enqueueing (child {0} of {1})... Remaining: {2}", wrapperTask.Id, Pending.Last<Task>().Id, Pending.Count);
                        Pending.Enqueue(wrapperTask);
                        Console.WriteLine("  Successfully enqueued. Remaining: {0}", Pending.Count);
                    }
                    catch (Exception e) { Console.WriteLine("Eggception when enqueueing: " + e.Message); }

                    //object locker = new object();
                    //lock (locker) { Pending.Enqueue(wrapperTask); }
                }
                else {

                    wrapperTask.Start();

                    try {
                        Console.WriteLine("  Enqueueing... ({0}) Remaining: {1}", wrapperTask.Id, Pending.Count);
                        object locker = new object();
                        lock(locker) {
                            Pending.Enqueue(wrapperTask);
                        }
                    
                        Console.WriteLine("  Successfully enqueued. Remaining: {0}", Pending.Count);
                    }
                    catch (Exception e) { Console.WriteLine("Eggception when enqueueing: " + e.Message); }

                    //object locker = new object();
                    //lock (locker) { Pending.Enqueue(wrapperTask); }

                }
            }

        }



        /*
        public void EnhancedAdd(Task actualTask, CancellationTokenSource cts) {


            Task wrapperTask = new Task(() => {

                Thread thrd = Thread.CurrentThread;

                Task t = new Task(() => {
                    while (true) {
                        if (cts.Token.IsCancellationRequested) {

                            object locker = new object();
                            lock(locker) {
                                Pending.Dequeue();
                            }
                            
                            thrd.Abort();
                        }
                    }
                });
                t.Start();

                actualTask.Start();

                if (actualTask.Wait(TASK_TIMEOUT)) {

                    object locker = new object();
                    lock(locker) {
                        Pending.Dequeue();
                    }
                }

            }, cts.Token, TaskCreationOptions.None);


            if (Pending.Count != 0) {

                Pending.Last<Task>().ContinueWith((pendingTask) => {
                    wrapperTask.Start();
                });

                object locker = new object();
                lock(locker) {
                    Pending.Enqueue(wrapperTask);
                }

            }
            else {

                object locker = new object();
                lock (locker) {
                    Pending.Enqueue(wrapperTask);
                }

                wrapperTask.Start();

            }

        }
        */

    }

}

