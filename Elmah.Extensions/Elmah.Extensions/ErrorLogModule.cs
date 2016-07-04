using Elmah.Extensions.Exceptions;
using System;
using System.Threading;
using System.Web;

namespace Elmah.Extensions
{
    public class ErrorLogModule : Elmah.ErrorLogModule, IHttpModule
    {
        static int _unhandledExceptionCount = 0;

        static object _initLock = new object();
        static bool _initialized = false;

        public void Init(HttpApplication app)
        {

            // Do this one time for each AppDomain.
            if (!_initialized)
            {
                lock (_initLock)
                {
                    if (!_initialized)
                    {
                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

                        _initialized = true;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        void OnUnhandledException(object o, UnhandledExceptionEventArgs e)
        {
            // Let this occur one time for each AppDomain.
            if (Interlocked.Exchange(ref _unhandledExceptionCount, 1) != 0)
                return;

            //Logar o erro para o elmah, criando uma UnhandledException com a Inner exception com a excepção que ocorreu, para que seja fácil identificar que esta excepção não foi tratada
            this.LogException(new UnhandledException((Exception)e.ExceptionObject), HttpContext.Current);
        }

    }
}