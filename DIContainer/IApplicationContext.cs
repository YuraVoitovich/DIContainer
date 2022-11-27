using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public interface IApplicationContext
    {
        TDependency Resolve<TDependency>();

        TDependency Resolve<TDependency>(ImplementationEnum implNumber);

        IEnumerable<TDependency> GetAllImplementations<TDependency>();
    }
}
