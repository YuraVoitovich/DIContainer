using DIContainer.exception;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DIContainer
{
    public class ApplicationContextConfig
    {
        private IDictionary<Type, IDictionary<ImplementationEnum, (Type, ClassScope)>> container 
            = new Dictionary<Type, IDictionary<ImplementationEnum, (Type, ClassScope)>>();

        internal bool tryGetImplementationsByType(Type type, out IDictionary<ImplementationEnum, (Type, ClassScope)> dict)
        {
            if (this.container.TryGetValue(type, out var d))
            {
                dict = d;
                return true;
            }
            if (type.IsGenericType)
            {
                Type openGenericType = this.container.Keys.ToList().Find(o => o.Name.Equals(type.Name) && o.IsGenericTypeDefinition);
                if (openGenericType != null)
                {
                    if (openGenericType.IsGenericTypeDefinition)
                    {
                        dict = this.container[openGenericType];
                        return true;
                    }
                }
            }
            dict = null;
            return false;
        }

        private bool isTypeCanBeCreated(Type type)
        {
            if (type.IsClass && !type.IsAbstract)
            {
                return true;
            }
            return false;
        }

        public void Register<TDependency, TImplementation>(ImplementationEnum implNumber, ClassScope classScope = ClassScope.Singleton) 
            where TImplementation : TDependency where TDependency : class
        {
            Type dependencyType = typeof(TDependency);
            Type implType = typeof(TImplementation);

            if (!isTypeCanBeCreated(implType))
            {
                throw new ApplicationContextConfigException("TImplementation should be a class");
            }
            
            if (container.TryGetValue(dependencyType, out var t))
            {
                if (t.TryGetValue(implNumber, out var impl))
                {
                    throw new ApplicationContextConfigException("Implementation with number "
                        + implNumber.ToString() + " exists");
                }
                t.Add(implNumber, (implType, classScope)); 
            } else
            {
                IDictionary<ImplementationEnum, (Type, ClassScope)> dict = 
                    new Dictionary<ImplementationEnum, (Type, ClassScope)>();
                dict.Add(implNumber, (implType, classScope));
                container.Add(dependencyType, dict);
            }

        }

        public void Register<TDependency, TImplementation>(ClassScope classScope = ClassScope.Singleton) 
            where TImplementation : TDependency where TDependency : class
        {
            Type dependencyType = typeof(TDependency);
            Type implType = typeof(TImplementation);


            if (!isTypeCanBeCreated(implType))
            {
                throw new ApplicationContextConfigException("TImplementation should be a class");
            }

            if (container.TryGetValue(dependencyType, out var t))
            {
                t.Add((ImplementationEnum)t.Count, (implType, classScope));
            }
            else
            {
                IDictionary<ImplementationEnum, (Type, ClassScope)> dict = 
                    new Dictionary<ImplementationEnum, (Type, ClassScope)>();
                dict.Add(ImplementationEnum.First, (implType, classScope));
                dict.TryGetValue(ImplementationEnum.First, out var e);
                container.Add(dependencyType, dict);
            }
        }

        public void Register(Type openGenericDependency, Type openGenericImpl, ClassScope classScope = ClassScope.Singleton) 
        {
            Type dependencyType = openGenericDependency;
            Type implType = openGenericImpl;

            if (!isTypeCanBeCreated(implType))
            {
                throw new ApplicationContextConfigException("TImplementation should be a class");
            }

            if (container.TryGetValue(dependencyType, out var t))
            {
                t.Add((ImplementationEnum)t.Count, (implType, classScope));
            }
            else
            {
                IDictionary<ImplementationEnum, (Type, ClassScope)> dict =
                    new Dictionary<ImplementationEnum, (Type, ClassScope)>();
                dict.Add(ImplementationEnum.First, (implType, classScope));
                dict.TryGetValue(ImplementationEnum.First, out var e);
                container.Add(dependencyType, dict);
            }
        }

        public void Register(Type openGenericDependency, Type openGenericImpl, ImplementationEnum implNumber, ClassScope classScope = ClassScope.Singleton)
        {
            Type dependencyType = openGenericDependency;
            Type implType = openGenericImpl;

            if (!isTypeCanBeCreated(implType))
            {
                throw new ApplicationContextConfigException("TImplementation should be a class");
            }

            if (container.TryGetValue(dependencyType, out var t))
            {
                if (t.TryGetValue(implNumber, out var impl))
                {
                    throw new ApplicationContextConfigException("Implementation with number "
                        + implNumber.ToString() + " exists");
                }
                t.Add(implNumber, (implType, classScope));
            }
            else
            {
                IDictionary<ImplementationEnum, (Type, ClassScope)> dict =
                    new Dictionary<ImplementationEnum, (Type, ClassScope)>();
                dict.Add(implNumber, (implType, classScope));
                container.Add(dependencyType, dict);
            }
        }
    }
}
