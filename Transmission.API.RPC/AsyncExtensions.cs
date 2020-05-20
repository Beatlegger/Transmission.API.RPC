using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.API.RPC
{
    /// <summary>
    /// Async extension
    /// </summary>
    public static class AsyncExtensions
    {
        /// <summary>
        /// Wait and unwrap exception
        /// </summary>
        /// <param name="task"></param>
        public static void WaitAndUnwrapException(this Task task)
        {
            try
            {
                task.Wait();
            } catch(Exception e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw e;
            }
        }
    }
}
