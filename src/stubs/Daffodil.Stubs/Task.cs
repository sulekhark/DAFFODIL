// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Daffodil.Stubs
{
    public class Task
    {
        internal object m_action;
        internal Task m_parent;
        internal object m_stateObject;
        private static Task s_completedTask;
        internal static Task t_currentTask;
        // The Task object contingentProperties.m_exceptionHolder class has all the exceptions.
        // TaskCancelledException is a separate field and m_faultExceptions is a list that holds all other exceptions.
        // For the present, modeling all the above as just one field.
        internal Exception mdl_exception;
        // There are two choices for modeling continuations:
        //    1) StandardTaskContinuation.Run is stateful. It checks if the antecedent task is completed. If yes, it runs the continuation task.
        //       If this is analyzed in a flow-insensitive way, the continuation function will be called whether or not the antecedent executed.
        //    2) Just record the continuation task. In the Start method of the antecedent task, after m_action is called, synchronously start
        //       the continuation task.
        // Chose to model continuations like option 2 above since it is a bit more precise.
        internal Task mdl_continuationTask;
        internal bool mdl_isContinuationTask;
        internal Task mdl_antecedentTask;

        public static Task CompletedTask
        {
            get
            {
                if (s_completedTask == null)
                    s_completedTask = new Task();
                return s_completedTask;
            }
        }
        public AggregateException Exception
        {
            get
            {
                return GetExceptions(false);
            }
        }

        internal AggregateException GetExceptions(bool includeTaskCanceledExceptions)
        {
            // Commenting the line below as it does not get wrapped into AggregateException because of SSA. SRK 2nd Feb 2020
            // Exception ex = new TaskCanceledException("Task canceled");
            Exception ex = mdl_exception;
            // Removed an unnecessary layer of wrapping below. SRK 2nd Feb 2020
            // return new AggregateException(new Exception[] {ex});
            return new AggregateException("daffodil_stub aggr wrapper", ex);
        }

        // defined just for ease of stubbing
        internal Task()
        {
            m_action = null;
            m_parent = null;
            m_stateObject = null;
            mdl_isContinuationTask = false;
            mdl_continuationTask = null;
            mdl_antecedentTask = null;
        }

        internal Task(Delegate action, object state, Task parent) : this()
        {
            m_action = action;
            m_stateObject = state;
            m_parent = parent;
        }

        public Task(Action action) : this()
        { m_action = action; }
        public Task(Action action, CancellationToken cancellationToken) : this(action) { }
        public Task(Action action, TaskCreationOptions creationOptions) : this(action) { m_parent = t_currentTask; }
        public Task(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : this(action)
        { m_parent = t_currentTask; }

        public Task(Action<object> action, object state) : this()
        {
            m_action = action;
            m_stateObject = state;
        }
        public Task(Action<object> action, object state, CancellationToken cancellationToken) : this(action, state) { }
        public Task(Action<object> action, object state, TaskCreationOptions creationOptions) : this(action, state)
        { m_parent = t_currentTask; }
        public Task(Action<object> action, object state, CancellationToken cancellationToken,
                    TaskCreationOptions creationOptions) : this(action, state)
        { m_parent = t_currentTask; }


       
        public static Task FromCanceled(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException("cancellationToken");
            }
            return new Task();
        }

        public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                throw new ArgumentOutOfRangeException("cancellationToken");
            }
            return new Task<TResult>();
        }

        public static Task FromException(Exception exception)
        {
            return Task.FromException<VoidTaskResult>(exception);
        }

        public static Task<TResult> FromException<TResult>(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            Task<TResult> task = new Task<TResult>();
            task.TrySetException(exception);
            return task;
        }

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            return new Task<TResult>(result);
        }

        public TaskAwaiter GetAwaiter()
        {
            return new TaskAwaiter(this);
        }

        public static Task Run(Action action)
        {
            Task task = new Task(action);
            task.Start();
            return task;
        }
        public static Task Run(Action action, CancellationToken cancellationToken)
        { return Run(action); }


        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            // At present, trying to track only exceptions raised by the application. SRK 2nd Feb 2020
            // if (function == null)
            // {
                // throw new ArgumentNullException("function");
            // }
            Task<TResult> task = new Task<TResult>(function);
            task.Start();
            return task;
        }
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        { return Run<TResult>(function); }

        public void Start()
        {
            t_currentTask = this;
            try
            {
                if (m_action is Action)
                {
                    Action act = m_action as Action;
                    act();
                }
                else if (m_action is Action<object>)
                {
                    Action<object> acto = m_action as Action<object>;
                    acto(m_stateObject);
                }
                else if (m_action is Action<Task> && mdl_isContinuationTask)
                {
                    Action<Task> actt = m_action as Action<Task>;
                    actt(mdl_antecedentTask);
                }
                else if (m_action is Action<Task, object> && mdl_isContinuationTask)
                {
                    Action<Task, object> actto = m_action as Action<Task, object>;
                    actto(mdl_antecedentTask, m_stateObject);
                }
                this.m_parent.mdl_exception = mdl_exception;
                mdl_continuationTask.Start();

                // To model for flow-insensitive pointer analysis, the lines below are not really required. 
                m_action = null;
                mdl_continuationTask = null;
            }
            catch (Exception e)
            {
                // Actual code (see System.Threading.Tasks.Task.InternalRunSynchronously) wraps e in TaskSchedulerException
                // Ignoring the above while modeling.
                mdl_exception = e;
                this.m_parent.mdl_exception = mdl_exception;
                throw e;
            }
            // Not modeled any of the scheduler exceptions
            // At present, trying to track only exceptions raised by the application. SRK 2nd Feb 2020
            // throw new InvalidOperationException("Task_Start_TaskCompleted");
            // throw new InvalidOperationException("Task_Start_Promise");
            // throw new InvalidOperationException("Task_Start_ContinuationTask");
            // throw new InvalidOperationException("Task_Start_AlreadyStarted");
        }

        public void Start(TaskScheduler scheduler)
        { Start(); }

        public Task ContinueWith(Action<Task> continuationAction)
        {
            Task task = new Task(continuationAction, null, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task ContinueWith(Action<Task> continuationAction, CancellationToken cancellationToken)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task> continuationAction, TaskScheduler scheduler)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task> continuationAction, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task> continuationAction, CancellationToken cancellationToken,
            TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationAction); }

        public Task ContinueWith(Action<Task, object> continuationAction, object state)
        {
            Task task = new Task(continuationAction, state, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task ContinueWith(Action<Task, object> continuationAction, object state, CancellationToken cancellationToken)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task, object> continuationAction, object state, TaskScheduler scheduler)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task, object> continuationAction, object state, CancellationToken cancellationToken,
            TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationAction, state); }

        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction)
        {
            Task<TResult> task = new Task<TResult>(continuationFunction, null, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, CancellationToken cancellationToken)
        { return ContinueWith(continuationFunction); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationFunction); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction, CancellationToken cancellationToken,
                                                   TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction); }

        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state)
        {
            Task<TResult> task = new Task<TResult>(continuationFunction, state, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state,
                                                   CancellationToken cancellationToken)
        { return ContinueWith(continuationFunction, state); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction, state); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state,
                                                   TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationFunction, state); }
        public Task<TResult> ContinueWith<TResult>(Func<Task, object, TResult> continuationFunction, object state, 
                             CancellationToken cancellationToken, TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction, state); }


        public void Wait()
        {
            Exception ex = GetExceptions(true);
            throw ex;
            throw new OperationCanceledException("OperationCanceled");
        }
        public void Wait(CancellationToken cancellationToken)
        { Wait(); }
        public bool Wait(int millisecondsTimeout)
        { Wait(); return true; }
        public bool Wait(TimeSpan timeout)
        { Wait(); return true; }
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        { Wait(); return true; }

        public static void WaitAll(Task[] tasks)
        {
            List<Exception> innerExceptions = new List<Exception>();
            for (int j = 0; j < tasks.Length; j++)
            {
                Task t = tasks[j];
                innerExceptions.Add(t.mdl_exception);
            }
            throw new AggregateException(innerExceptions);
            throw new OperationCanceledException("OperationCanceled");
        }
        public static void WaitAll(Task[] tasks, CancellationToken cancellationToken)
        { WaitAll(tasks); }
        public static bool WaitAll(Task[] tasks, TimeSpan timeout)
        { WaitAll(tasks); return true; }
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout)
        { WaitAll(tasks); return true; }
        public static bool WaitAll(Task[] tasks, int millisecondsTimeout, CancellationToken cancellationToken)
        { WaitAll(tasks); return true; }
    }



    public class Task<TResult> : Task
    {
        internal TResult m_result;

        public TResult Result
        {
            get
            {
                Exception ex = GetExceptions(true);
                if (ex != null) throw ex;
                return m_result;
            }
        }

        // defined just for ease of stubbing
        internal Task()
        {
            m_action = null;
            m_parent = null;
            m_stateObject = null;
            mdl_isContinuationTask = false;
            mdl_continuationTask = null;
            mdl_antecedentTask = null;
        }
        internal Task(TResult result) : this() { m_result = result; }

        internal Task(Delegate action, object state, Task parent) : this()
        {
            m_action = action;
            m_stateObject = state;
            m_parent = parent;
        }

        public Task(Func<TResult> function) : this()
        { m_action = function; }
        public Task(Func<TResult> function, CancellationToken cancellationToken) : this(function) { }
        public Task(Func<TResult> function, TaskCreationOptions creationOptions) : this(function) { m_parent = t_currentTask; }
        public Task(Func<TResult> function, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : this(function)
        { m_parent = t_currentTask; }

    
        public Task(Func<object, TResult> function, object state) : this()
        {
            m_action = function;
            m_stateObject = state;
        }
        public Task(Func<object, TResult> function, object state, CancellationToken cancellationToken) : this(function, state) { }
        public Task(Func<object, TResult> function, object state, TaskCreationOptions creationOptions) : this(function, state)
        { m_parent = t_currentTask; }
        public Task(Func<object, TResult> function, object state,
                    CancellationToken cancellationToken, TaskCreationOptions creationOptions) : this(function, state)
        { m_parent = t_currentTask; }


        public new void Start()
        {
            t_currentTask = this;
            try
            {
                if (m_action is Action)
                {
                    Action act = m_action as Action;
                    act();
                }
                else if (m_action is Action<object>)
                {
                    Action<object> acto = m_action as Action<object>;
                    acto(m_stateObject);
                }
                else if (m_action is Action<Task> && mdl_isContinuationTask)
                {
                    Action<Task> actt = m_action as Action<Task>;
                    actt(mdl_antecedentTask);
                }
                else if (m_action is Action<Task, object> && mdl_isContinuationTask)
                {
                    Action<Task, object> actto = m_action as Action<Task, object>;
                    actto(mdl_antecedentTask, m_stateObject);
                }
                else if (m_action is Func<Task, TResult> && mdl_isContinuationTask)
                {
                    Func<Task, TResult> actf = m_action as Func<Task, TResult>;
                    m_result = actf(mdl_antecedentTask);
                }
                else if (m_action is Func<Task, object, TResult> && mdl_isContinuationTask)
                {
                    Func<Task, object, TResult> actfo = m_action as Func<Task, object, TResult>;
                    m_result = actfo(mdl_antecedentTask, m_stateObject);
                }
                /*****
                 * SRK: Not sure how to model continuation tasks whose antecedent result type is the generic type TAntecedentResult
                 * All the four chunks of ContinueWith in this class are like this.
                else if (m_action is Action<Task<TAntecedentResult>>)
                {
                    Action<Task<TAntecedentResult>> actft = m_action as Action<Task<TAntecedentResult>>;
                    Task<TAntecedentResult> mdl_ante = mdl_antecedentTask as Task<TAntecedentResult>;
                    actft(mdl_ante);
                }
                *****/
                this.m_parent.mdl_exception = mdl_exception;
                mdl_continuationTask.Start();

                // To model for flow-insensitive pointer analysis, the lines below are not really required. 
                m_action = null;
                mdl_continuationTask = null;
            }
            catch (Exception e)
            {
                // Actual code (see System.Threading.Tasks.Task.InternalRunSynchronously) wraps e in TaskSchedulerException
                // Ignoring the above while modeling.
                mdl_exception = e;
                this.m_parent.mdl_exception = mdl_exception;
                throw e;
            }
            // Not modeled any of the scheduler exceptions
            // At present, trying to track only exceptions raised by the application. SRK 2nd Feb 2020
            // throw new InvalidOperationException("Task_Start_TaskCompleted");
            // throw new InvalidOperationException("Task_Start_Promise");
            // throw new InvalidOperationException("Task_Start_ContinuationTask");
            // throw new InvalidOperationException("Task_Start_AlreadyStarted");
        }


        public Task ContinueWith(Action<Task<TResult>> continuationAction)
        {
            Task task = new Task(continuationAction, null, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task ContinueWith(Action<Task<TResult>> continuationAction, CancellationToken cancellationToken)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task<TResult>> continuationAction, TaskScheduler scheduler)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task<TResult>> continuationAction, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationAction); }
        public Task ContinueWith(Action<Task<TResult>> continuationAction, CancellationToken cancellationToken,
                                 TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationAction); }

        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state)
        {
            Task task = new Task(continuationAction, state, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, TaskScheduler scheduler)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationAction, state); }
        public Task ContinueWith(Action<Task<TResult>, object> continuationAction, object state, CancellationToken cancellationToken,
                                 TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationAction, state); }

        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction)
        {
            Task<TNewResult> task = new Task<TNewResult>(continuationFunction, null, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken)
        { return ContinueWith(continuationFunction); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationFunction); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction, CancellationToken cancellationToken,
                                                         TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction); }


        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state)
        {
            Task<TNewResult> task = new Task<TNewResult>(continuationFunction, state, t_currentTask);
            task.mdl_isContinuationTask = true;
            task.mdl_antecedentTask = this;
            mdl_continuationTask = task;
            return task;
        }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken)
        { return ContinueWith(continuationFunction, state); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction, state); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, TaskContinuationOptions continuationOptions)
        { return ContinueWith(continuationFunction, state); }
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, object, TNewResult> continuationFunction, object state, CancellationToken cancellationToken,
                                                         TaskContinuationOptions continuationOptions, TaskScheduler scheduler)
        { return ContinueWith(continuationFunction, state); }


        public new TaskAwaiter<TResult> GetAwaiter()
        {
            return new TaskAwaiter<TResult>(this);
        }

        internal bool TrySetException(object exceptionObject)
        {
            mdl_exception = exceptionObject as Exception;
            this.m_parent.mdl_exception = mdl_exception;
            return true;
        }

        internal bool TrySetResult(TResult result)
        {
            m_result = result;
            this.m_parent.mdl_exception = mdl_exception;
            Exception ex = GetExceptions(true);
            return true;
        }
    }
}
