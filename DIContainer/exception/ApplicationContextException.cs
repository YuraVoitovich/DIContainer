using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.exception
{
    public class ApplicationContextException : Exception
    {
        public ApplicationContextException()
        {
        }

        public ApplicationContextException(string message) : base(message)
        {
        }

        public ApplicationContextException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
