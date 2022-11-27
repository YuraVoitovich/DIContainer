using DIContainer.exception;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    public class ApplicationContext : IApplicationContext
    {
        private ApplicationContextConfig _config;
        private IDictionary<Type, IDictionary<ImplementationEnum, object>> singletons 
            = new Dictionary<Type, IDictionary<ImplementationEnum, object>>();
        public ApplicationContext(ApplicationContextConfig config)
        {
            _config = config;
        }

        private object create(Type type)
        {
            List<ConstructorInfo> constructors = type.GetConstructors().ToList();
            constructors.Sort((o1, o2) => o1.GetParameters().Length
            .CompareTo(o2.GetParameters().Length));
            ConstructorInfo constructor = constructors.First();
            var parameterInfos = constructor.GetParameters().ToList();
            object[] parameters = new object[parameterInfos.Count];
            for (int i = 0; i < parameters.Length; i++)
            {
                List<Attribute> attributes = parameterInfos[i].GetCustomAttributes().ToList();
                bool isAttributeFound = false;
                if (parameterInfos[i].ParameterType.Name.Equals(typeof(IEnumerable<>).Name))
                {
                    try
                    {
                        parameters[i] = GetAllImplementations(parameterInfos[i].ParameterType.GetGenericArguments()[0]);
                    }
                    catch (ApplicationContextException e)
                    {
                        parameters[i] = null;
                    }
                }
                else
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute is DependencyKeyAttribute d)
                        {
                            try
                            {
                                parameters[i] = resolve(parameterInfos[i].ParameterType, d.ImplementationEnum);
                            }
                            catch (ApplicationContextException e)
                            {
                                parameters[i] = null;
                            }
                            isAttributeFound = true;
                            break;
                        }
                    }
                    if (!isAttributeFound)
                    {
                        try
                        {
                            parameters[i] = resolve(parameterInfos[i].ParameterType);
                        }
                        catch (ApplicationContextException e)
                        {
                            parameters[i] = null;
                        }
                    }
                }
            }
            return constructor.Invoke(parameters);
        }

        private object resolveImpl(Type type, ClassScope scope, ImplementationEnum implNumber)
        {
            if (scope == ClassScope.Prototype)
            {
                return create(type);
            }
            if (scope == ClassScope.Singleton)
            {
                if (singletons.TryGetValue(type, out var impl))
                {
                    if (impl.TryGetValue(implNumber, out var obj))
                    {
                        return obj; 
                    } else
                    {
                        object o = create(type);
                        impl.Add(implNumber, o);
                        return o;
                    }
                } else
                {
                    IDictionary<ImplementationEnum, Object> dict = new Dictionary<ImplementationEnum, Object>();
                    object o = create(type);
                    dict.Add(implNumber, o);
                    singletons.Add(type, dict);
                    return o;
                }

            }
            return null;
        }

        public TDependency Resolve<TDependency>(ImplementationEnum implnumber)
        {
            Type dependencyType = typeof(TDependency);
            return (TDependency)resolve(dependencyType, implnumber);
        }

        private object resolve(Type dependencyType)
        {
            if (_config.tryGetImplementationsByType(dependencyType, out var impls))
            { 
                if (impls.Count > 1)
                {
                    throw new ApplicationContextException("Cannot resolve dependency for type "
                        + dependencyType.FullName + ": exists more than one implementation");
                }
                List<(Type, ClassScope)> implsList = impls.Values.ToList();
                (Type, ClassScope) implsTuple = implsList[0];
                Type implType = implsTuple.Item1;
                ClassScope scope = implsTuple.Item2;
                if (implType.IsGenericTypeDefinition)
                {
                    _config.tryGetImplementationsByType(dependencyType.GetGenericArguments()[0], out var genericImpls);
                    implType = implType.MakeGenericType(dependencyType.GetGenericArguments()[0]);
                }
                return resolveImpl(implType, scope, impls.Keys.ToList()[0]);
            }
            else
            {
                throw new ApplicationContextException("Cannot resolve dependency for type "
                        + dependencyType.FullName + ": cannot find implementations");
            }
        }

        private object resolve(Type dependencyType, ImplementationEnum implNumber)
        {
            if (_config.tryGetImplementationsByType(dependencyType, out var impls))
            {
                if (impls.TryGetValue(implNumber, out var impl))
                {
                    return resolveImpl(impl.Item1, impl.Item2, implNumber);
                }
                else
                {
                    throw new ApplicationContextException("Cannot resolve dependency for type "
                        + dependencyType.FullName + ": cannot find implementation with number " + implNumber.ToString());
                }
            }
            else
            {
                throw new ApplicationContextException("Cannot resolve dependency for type "
                        + dependencyType.FullName + ": cannot find implementations");
            }
        }

        public TDependency Resolve<TDependency>()
        {
            Type dependencyType = typeof(TDependency);
            return (TDependency)resolve(dependencyType);
        }

        private object GetAllImplementations(Type type)
        {
            if (_config.tryGetImplementationsByType(type, out var impls))
            {
                Type collectionType = typeof(List<>).MakeGenericType(type);
                object generatedCollection = collectionType.GetConstructor(new Type[0]).Invoke(null);
                var genericArguments = collectionType.GetGenericArguments();
                MethodInfo addMethod = typeof(List<>).MakeGenericType(collectionType.GetGenericArguments()).GetMethod("Add");
                foreach (var it in impls.Keys.ToList())
                {
                    addMethod.Invoke(generatedCollection, new object[] { resolve(type, it) });
                }
                return generatedCollection;
            }
            else
            {
                throw new ApplicationContextException("Cannot resolve dependency for type "
                        + type.FullName + ": cannot find implementations");
            }
        }

        public IEnumerable<TDependency> GetAllImplementations<TDependency>()
        { 
            return (IEnumerable<TDependency>)GetAllImplementations(typeof(TDependency));
        }
    }
}
