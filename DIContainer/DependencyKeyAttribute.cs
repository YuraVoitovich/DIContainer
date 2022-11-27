using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class DependencyKeyAttribute : Attribute
    {
        public ImplementationEnum ImplementationEnum { set; get; }

        public DependencyKeyAttribute(ImplementationEnum implementationEnum)
        {
            ImplementationEnum = implementationEnum;
        }
    }
}
