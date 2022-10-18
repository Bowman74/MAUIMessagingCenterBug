using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MAUIMessagingCenterBug.SafeMessagingCenter;

namespace MAUIMessagingCenterBug
{
    public class SafeMessagingCenter : IMessagingCenter
    {
        internal SafeMessagingCenter() { }

        private static int _waitTimeoutMiliseconds = 1000;
        public static int _currentThreadId = -1;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static IMessagingCenter _instance = new SafeMessagingCenter();

        internal enum LockState
        {
            Failed = 0,
            Success = 1,
            Recursive = 2
        }

        public static IMessagingCenter Instance
        {
            get { return _instance; }
        }


        public void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Send<TSender, TArgs>(sender, message, args);
                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        public void Send<TSender>(TSender sender, string message) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Send<TSender>(sender, message);
                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        public void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Subscribe<TSender, TArgs>(subscriber, message, callback, source);
                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        public void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Subscribe<TSender>(subscriber, message, callback);

                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        public void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Unsubscribe<TSender, TArgs>(subscriber, message);
                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        public void Unsubscribe<TSender>(object subscriber, string message) where TSender : class
        {
            LockState lockState = LockState.Failed;
            try
            {
                lockState = WaitOnSemephore();
                if (lockState != LockState.Failed)
                {
                    MessagingCenter.Unsubscribe<TSender>(subscriber, message);
                }
            }
            finally
            {
                ReleaseSemephore(lockState);
            }
        }

        internal LockState WaitOnSemephore()
        {
            LockState returnValue = LockState.Failed;
            if (_currentThreadId != -1 && _currentThreadId == Thread.CurrentThread.ManagedThreadId)
            {
                returnValue = LockState.Recursive;
            }
            else
            {
                returnValue = _semaphore.Wait(_waitTimeoutMiliseconds) ? LockState.Success : LockState.Failed;
                if (returnValue != LockState.Success)
                {
                    _currentThreadId = Thread.CurrentThread.ManagedThreadId;
                }
            }
            return returnValue;
        }

        internal void ReleaseSemephore(LockState lockState)
        {
            if (lockState == LockState.Success)
            {
                _currentThreadId = -1;
                _semaphore.Release();
            }
        }
    }
}
