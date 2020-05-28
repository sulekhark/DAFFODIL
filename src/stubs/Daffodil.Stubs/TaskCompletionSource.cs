// Author: Sulekha Kulkarni
// Date: Nov 2019


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daffodil.Stubs
{
    class TaskCompletionSource<TResult>
    {
        private readonly Task<TResult> m_task;

        public TaskCompletionSource()
        {
            this.m_task = new Task<TResult>();
        }
        public TaskCompletionSource(object state, TaskCreationOptions creationOptions)
        {
            this.m_task = new Task<TResult>(null, state, creationOptions);
        }
        public TaskCompletionSource(object state) : this(state, TaskCreationOptions.None) { }
        public TaskCompletionSource(TaskCreationOptions creationOptions) : this(null, creationOptions) { }


        public void SetException(Exception exception) { this.TrySetException(exception); }
        public bool TrySetException(Exception exception) { return this.m_task.TrySetException(exception); }
        public void SetResult(TResult result) { this.TrySetResult(result); }
        public bool TrySetResult(TResult result) { return this.m_task.TrySetResult(result); }
        public void SetException(IEnumerable<Exception> exceptions) { this.TrySetException(exceptions); }
        public void TrySetException(IEnumerable<Exception> exceptions)
        {
            foreach (Exception exception in exceptions) this.m_task.TrySetException(exception);
        }
    }
}
