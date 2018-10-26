using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kf2server_tbot_client.Browsers {

    class ActionFactory {

        private static readonly int TASK_TIMEOUT = Properties.Settings.Default.DefaultTaskTimeoutSeconds;

        //List<Task> PendingActions { get; set; }
        List<TaskFactory> PendingActions { get; set; }

        public ActionFactory() {

            //PendingActions = new List<Task>();
            PendingActions = new List<TaskFactory>();
        }

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

    }

}

