global using System.Reflection;
using ReflectionExamplesDummyIoCContainer;

Console.Title = "Inversion of Control Container";

var iocContainer = new IoCContainer();
iocContainer.Register<IWaterService, TapWaterService>();
var waterService = iocContainer.Resolve<IWaterService>();

//iocContainer.Register<IBeanService<Catimor>, ArabicaBeanService<Catimor>>();
//iocContainer.Register<IBeanService<>, ArabicaBeanService<>>();
//iocContainer.Register<typeof(IBeanService<>), typeof(ArabicaBeanService<>)>();
iocContainer.Register(typeof(IBeanService<>), typeof(ArabicaBeanService<>));

iocContainer.Register<ICoffeeService, CoffeeService>();
var coffeeService = iocContainer.Resolve<ICoffeeService>();

Console.ReadKey();
