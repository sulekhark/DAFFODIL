// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;

namespace Daffodil.Stubs
{
    internal class VoidTaskResult
    {
    }

    public struct AsyncTaskMethodBuilder
    {
        AsyncTaskMethodBuilder<VoidTaskResult> m_builder;
        public Task Task
        {
            get { return m_builder.Task; }
        }

        public static AsyncTaskMethodBuilder Create()
        {
            AsyncTaskMethodBuilder atmb;
            atmb.m_builder.m_task = new Task<VoidTaskResult>();
            return atmb;
        }

        public void SetException(Exception exception)
        {
            m_builder.SetException(exception);
        }

        public void SetResult()
        {
            m_builder.SetResult(null);
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }
    }

    
    public struct AsyncTaskMethodBuilder<TResult>
    {
        public Task<TResult> m_task;
        static Task<TResult> s_defaultResultTask;

        public Task<TResult> Task
        {
            get
            {
                Task<TResult> task = m_task;
                return task;
            }
        }

        public AsyncTaskMethodBuilder<TResult> Create()
        {
            AsyncTaskMethodBuilder<TResult> atmb;
            atmb.m_task = new Task<TResult>();
            return atmb;
        }

        static AsyncTaskMethodBuilder()
        {
            s_defaultResultTask = new Task<TResult>();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetException(Exception exception)
        {
            m_task.mdl_exception = exception;
        }

        public void SetResult(TResult result)
        {
            m_task.m_result = result;
        }
    }
}
