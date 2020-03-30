using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MyMailBox.Utils
{
    public class ThreadInvoker
    {
        #region Singleton

        private ThreadInvoker()
        {
        }

        public static ThreadInvoker Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
            internal static readonly ThreadInvoker instance = new ThreadInvoker();
        }

        #endregion

        static readonly object padlock = new object();

        #region New Thread

        public void RunByNewThread(Action action)
        {
            lock (padlock)
            {
                action.BeginInvoke(ar => ActionCompleted(ar, res => action.EndInvoke(res)), null);
            }
        }

        public void RunByNewThread<TResult>(Func<TResult> func, Action<TResult> callbackAction)
        {
            lock (padlock)
            {
                func.BeginInvoke(ar =>
                FuncCompleted<TResult>(ar, res => func.EndInvoke(res), callbackAction), null);
            }
        }

        private static void ActionCompleted(IAsyncResult asyncResult,
                                            Action<IAsyncResult> endInvoke)
        {
            if (asyncResult.IsCompleted)
            {
                endInvoke(asyncResult);
            }
        }

        private static void FuncCompleted<TResult>(IAsyncResult asyncResult,
                                                   Func<IAsyncResult, TResult> endInvoke,
                                                   Action<TResult> callbackAction)
        {
            if (asyncResult.IsCompleted)
            {
                TResult response = endInvoke(asyncResult);
                if (callbackAction != null)
                {
                    callbackAction(response);
                }
            }
        }

        #endregion

        #region UI Thread

        private Dispatcher m_Dispatcher = null;

        //You have to Init the Dispatcher in the UI thread! 
        // Init once per application (if there is only one Dispatcher).
        public void InitDispacter(Dispatcher dispatcher = null)
        {
            m_Dispatcher = dispatcher == null ? (new UserControl()).Dispatcher : dispatcher;
        }

        public void RunByUiThread(Action action)
        {
            #region UI Thread Safety

            //handle by UI Thread.
            if (m_Dispatcher.Thread != Thread.CurrentThread)
            {
                m_Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                return;
            }

            action();

            #endregion
        }

        public T RunByUiThread<T>(Func<T> function)
        {
            #region UI Thread Safety
            //handle by UI Thread.
            if (m_Dispatcher.Thread != Thread.CurrentThread)
            {
                return (T)m_Dispatcher.Invoke(DispatcherPriority.Normal, function);
            }
            return function();
            #endregion
        }

        #endregion
    }
}
