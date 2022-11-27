using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.exception
{
    public class ApplicationContextConfigException : Exception
    {
        public ApplicationContextConfigException()
        {
        }

        public ApplicationContextConfigException(string message) : base(message)
        {
        }

        public ApplicationContextConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
