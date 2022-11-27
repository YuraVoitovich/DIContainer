using DIContainer.exception;
using System.Collections;

namespace DIContainer.Tests
{
    [TestClass]
    public class ApplicationContextConfigTest
    {

        private static IApplicationContext applicationContext;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ApplicationContextConfig config = new ApplicationContextConfig();
            config.Register<IService1, Service1>(ClassScope.Prototype);
            config.Register<IService1, Service1_2>(ClassScope.Prototype);
            config.Register<IService2, Service2>(ClassScope.Prototype);
            config.Register<AbstractService, AbstractServiceImpl>(ClassScope.Prototype);
            config.Register<Service1, Service1>();
            config.Register<Service2, Service2>(); 
            config.Register<IGenericService<int>, IntGenericService>();
            config.Register(typeof(IGenericService<>), typeof(GenericService<>));
            applicationContext = new ApplicationContext(config);
        }

        [TestMethod]
        public void TestAbstractService()
        {
            AbstractService service = applicationContext.Resolve<AbstractService>();
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(AbstractServiceImpl), service.GetType());
        }

        [TestMethod]
        public void TestService1WithoutInterface()
        {
            Service1 service = applicationContext.Resolve<Service1>();
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(Service1), service.GetType());
        }

        [TestMethod]
        public void TestPrototypeScope()
        {
            AbstractService service1 = applicationContext.Resolve<AbstractService>();
            AbstractService service2 = applicationContext.Resolve<AbstractService>();
            Assert.AreNotEqual(service1, service2);
        }

        [TestMethod]
        public void TestSingletonScope()
        {
            Service1 service1 = applicationContext.Resolve<Service1>();
            Service1 service2 = applicationContext.Resolve<Service1>();
            Assert.AreEqual(service1, service2);
        }

        [TestMethod]
        public void TestService1()
        {
            IService1 service = applicationContext.Resolve<IService1>(ImplementationEnum.First);
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(Service1), service.GetType());
        }

        [TestMethod]
        public void TestService1_2()
        {
            IService1 service = applicationContext.Resolve<IService1>(ImplementationEnum.Second);
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(Service1_2), service.GetType());
        }

        [TestMethod]
        public void TestEnumerableOfService1Impls()
        {
            IEnumerable<IService1> impls = applicationContext.GetAllImplementations<IService1>();
            Assert.AreEqual(2, impls.Count());
        }

        [TestMethod]
        public void TestGenericDoubleService()
        {
            IGenericService<double> service = applicationContext.Resolve<IGenericService<double>>();
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(GenericService<double>), service.GetType());
        }

        [TestMethod]
        public void TestEnumerableInConstructor()
        {
            Service2 service = applicationContext.Resolve<Service2>();
            
        }


        [TestMethod]
        public void TestIntGenericService()
        {
            IGenericService<int> service = applicationContext.Resolve<IGenericService<int>>();
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(IntGenericService), service.GetType());
        }

        [TestMethod]
        public void TestGenericIService2Service()
        {
            IGenericService<IService2> service = applicationContext.Resolve<IGenericService<IService2>>();
            Assert.IsNotNull(service);
            Assert.AreEqual(typeof(GenericService<IService2>), service.GetType());
        }

    }
}