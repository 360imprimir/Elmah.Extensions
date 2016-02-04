using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Elmah.Extensions
{
    public class FallbackErrorLog : ErrorLog
    {
        public override string Name
        {
            get
            {
                return "Fallback Error Log";
            }
        }

        protected IEnumerable<ErrorLog> ErrorLogs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallbackErrorLog"/> class
        /// using a dictionary of configured settings.
        /// </summary>
        public FallbackErrorLog(IDictionary config)
        {
            if (ReferenceEquals(config, null))
            {
                throw new ArgumentNullException("config");
            }

            if (!config.Contains("childrenLogsConfiguration"))
            {
                throw new ApplicationException("Children logs configuration is missing for the Fallback error log.");
            }

            IList<IDictionary> childrenLogConfigs = config["childrenLogsConfiguration"] as IList<IDictionary>;

            List<ErrorLog> errorLogs = new List<ErrorLog>();
            foreach(IDictionary childConfig in childrenLogConfigs)
            {
                //
                // We modify the settings by removing items as we consume 
                // them so make a copy here.
                //

                var childConfigClone = (IDictionary)((ICloneable)childConfig).Clone();

                //
                // Get the type specification of the service provider.
                //

                var typeSpec = childConfigClone.Find("type", string.Empty);

                if (typeSpec.Length == 0)
                {
                    throw new ApplicationException("Child log configuration is missing the type of the error log.");
                }

                childConfigClone.Remove("type");

                //
                // Locate, create and add the service provider object to the error logs list.
                //

                var type = Type.GetType(typeSpec, true);
                try
                {
                    errorLogs.Add(Activator.CreateInstance(type, new object[] { childConfigClone }) as ErrorLog);
                }
                catch { }
            }

            ErrorLogs = errorLogs;
        }

        /// <summary>
        /// Get the <see cref="ErrorLogEntry"/> with the <paramref name="id"/> from the main <see cref="ErrorLog"/>.
        /// </summary>
        /// <param name="id">Identifier of the <see cref="ErrorLogEntry"/>.</param>
        /// <returns>The <see cref="ErrorLogEntry"/> with the <paramref name="id"/> from the main <see cref="ErrorLog"/></returns>
        public override ErrorLogEntry GetError(string id)
        {
            ErrorLog errorLog = ErrorLogs.FirstOrDefault();
            
            if(ReferenceEquals(errorLog, null))
            {
                throw new ApplicationException("No logger found.");
            }
            
            return errorLog.GetError(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="errorEntryList"></param>
        /// <returns></returns>
        public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);
            }

            if (pageSize < 0)
            {
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);
            }

            ErrorLog errorLog = ErrorLogs.FirstOrDefault();

            if (ReferenceEquals(errorLog, null))
            {
                throw new ApplicationException("No logger found.");
            }

            return errorLog.GetErrors(pageIndex, pageSize, errorEntryList);
        }

        /// <summary>
        /// Logs the <paramref name="error"/> only to the first <see cref="ErrorLog"/> that is currently working.
        /// </summary>
        /// <param name="error">Error to log.</param>
        /// <returns>Identifier of the logged error.</returns>
        public override string Log(Error error)
        {
            if(ReferenceEquals(error, null))
            {
                throw new ArgumentNullException("error");
            }

            foreach (var errorLog in ErrorLogs)
            {
                try
                {
                    return errorLog.Log(error);
                }
                catch { }
            }

            throw new ApplicationException("No logger currently working.");
        }
    }
}
