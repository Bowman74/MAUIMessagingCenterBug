using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIMessagingCenterBug
{
    public class SafeMessagingCenter : IMessagingCenter
    {
        internal SafeMessagingCenter() { }

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private static IMessagingCenter instance = new SafeMessagingCenter();

        public static IMessagingCenter Instance
        {
            get { return instance; }
        }


        public void Send<TSender, TArgs>(TSender sender, string message, TArgs args) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Send<TSender, TArgs>(sender, message, args);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Send<TSender>(TSender sender, string message) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Send<TSender>(sender, message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Subscribe<TSender, TArgs>(object subscriber, string message, Action<TSender, TArgs> callback, TSender source = null) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Subscribe<TSender, TArgs>(subscriber, message, callback, source);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Subscribe<TSender>(object subscriber, string message, Action<TSender> callback, TSender source = null) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Subscribe<TSender>(subscriber, message, callback);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Unsubscribe<TSender, TArgs>(object subscriber, string message) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Unsubscribe<TSender, TArgs>(subscriber, message);
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Unsubscribe<TSender>(object subscriber, string message) where TSender : class
        {
            try
            {
                semaphore.Wait();
                MessagingCenter.Unsubscribe<TSender>(subscriber, message);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
