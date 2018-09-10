using DecoratorExample.Example5;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DecoratorExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var services = new ServiceCollection();

            //services.AddSingleton<IStoreCreateCommandHandler, StoreCreateCommandHandler>();
            //services.AddSingleton<IStoreCreateManyCommandHandler, StoreCreateManyCommandHandler>();
            //services.AddSingleton<IQueryHandler<UnitQuery, IEnumerable<StoreModel>>, StoreGetAllQueryHandler>();


            services.AddSingleton<IStoreApplicationService, StoreApplicationService>();
            services.AddSingleton<IStoreCreateInterrupter, MyStoreCreateInterrupterFirst>();
            services.AddSingleton<IStoreCreateInterrupter, MyStoreCreateInterrupterSecond>();

            services.AddSingleton<ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>>,
                StoreCreateCommandHandler>();

            services.Decorate(typeof(ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>>),
                typeof(MyDecorator));

            services.Decorate(typeof(ICommandHandler<,>), typeof(MyOpedGenericDecorator<,>));

            var serviceProvider = services.BuildServiceProvider();
            var s = serviceProvider.GetService<IStoreApplicationService>();
            s.Create(new StoreCreateCommand());
        }
    }

    public class MyStoreCreateInterrupterFirst : IStoreCreateInterrupter
    {
        public Unit Interrupt(StoreCreatingInterrupterModel input)
        {
            return new Unit();
        }
    }

    public class MyStoreCreateInterrupterSecond : IStoreCreateInterrupter
    {
        public Unit Interrupt(StoreCreatingInterrupterModel input)
        {
            input.Arguments.Cancel(new SomeError());

            return new Unit();
        }
    }

    public class MyDecorator : ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>>
    {
        private readonly ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>> decoratee;

        public MyDecorator(ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>> decoratee)
        {
            this.decoratee = decoratee;
        }

        public Validation<IError, StoreModel> Handle(StoreCreateCommand input)
        {
            return decoratee.Handle(input);
        }
    }

    public class MyOpedGenericDecorator<T, TResult> : ICommandHandler<T, TResult> where T : ICommand
    {
        private readonly ICommandHandler<T, TResult> decoratee;

        public MyOpedGenericDecorator(ICommandHandler<T, TResult> decoratee)
        {
            this.decoratee = decoratee;
        }

        public TResult Handle(T input)
        {
            return decoratee.Handle(input);
        }
    }
}
