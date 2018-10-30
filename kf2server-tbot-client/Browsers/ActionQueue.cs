using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kf2server_tbot_client.Browsers {
    class ActionQueue {

        private readonly Queue<Task> queue = new Queue<Task>();

        public void Enqueue(Thread actualThread, CancellationTokenSource cts) {
            lock (queue) {

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

                if (queue.Count == 0) {

                    Console.WriteLine("  Enqueueing ({0})", wrapperTask.Id);
                    queue.Enqueue(wrapperTask);
                    wrapperTask.Start();
                    Console.WriteLine("  Successfully enqueued {0}.", wrapperTask.Id);

                }

                else {
                    Console.WriteLine("  Enqueueing (child {0} of parent {1})", wrapperTask.Id, queue.Last<Task>().Id);

                    queue.Last<Task>().ContinueWith((pendingTask) => {
                        Console.WriteLine("Starting wrapperTask {0}", wrapperTask.Id);
                        wrapperTask.Start();
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
