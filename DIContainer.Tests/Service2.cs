using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Tests
{
    internal class Service2 : IService2
    {
        public IService1 service1 { private set; get; }
        public Service2([DependencyKey(ImplementationEnum.Second)] IService1 service1)
        {
            this.service1 = service1;
        }
    }
}
