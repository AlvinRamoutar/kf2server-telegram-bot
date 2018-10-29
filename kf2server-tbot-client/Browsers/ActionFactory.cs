using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        List<TaskFactory> PendingActions = new List<TaskFactory>();
        Queue<Task> Pending = new Queue<Task>();


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


        public void Add() {

        }


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


    }

}

