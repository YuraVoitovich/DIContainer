using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DIContainer
{
    public class ApplicationContextConfig
    {
        private IDictionary<Type, IDictionary<ImplementationEnum, Type>> container = new Dictionary<Type, IDictionary<ImplementationEnum, Type>>();

        public void Register(ImplementationEnum implNumber)
        {

        }
    }
}
