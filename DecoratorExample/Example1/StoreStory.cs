using System.Collections.Generic;

namespace DecoratorExample.Example1
{
    public interface IApplicationService { }

    public interface IUseCaseHandler<in T, out TResult> { TResult Handle(T input); }

    public class Validation<TError, TModel> { }

    public interface IError { }

    public class StoreModel { }

    public class StoreCreateCommand { }
    public class StoreManyCreateCommand { }

    public struct Unit { }

    public interface IStoreApplicationService : IApplicationService,
        IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>,
        IUseCaseHandler<StoreCreateCommand, Validation<IError, IEnumerable<StoreModel>>>,
        IUseCaseHandler<Unit, IEnumerable<StoreModel>>
    {
        Validation<IError, StoreModel> Create(StoreCreateCommand command);
        Validation<IError, IEnumerable<StoreModel>> Create(StoreManyCreateCommand command);

        IEnumerable<StoreModel> GetAll(Unit unit);
    }

    public class StoreApplicationService : IStoreApplicationService
    {
        private readonly IEnumerable<IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>> createUseCaseHandlers;
        private readonly IUseCaseHandler<StoreCreateCommand, Validation<IError, IEnumerable<StoreModel>>> createManyUseCaseHandler;
        private readonly IUseCaseHandler<Unit, IEnumerable<StoreModel>> getAllUseCaseHandler;

        public StoreApplicationService(
            IEnumerable<IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>> createUseCaseHandlers,
            IUseCaseHandler<StoreCreateCommand, Validation<IError, IEnumerable<StoreModel>>> createManyUseCaseHandler,
            IUseCaseHandler<Unit, IEnumerable<StoreModel>> getAllUseCaseHandler
        )
        {
            this.createUseCaseHandlers = createUseCaseHandlers;
            this.createManyUseCaseHandler = createManyUseCaseHandler;
            this.getAllUseCaseHandler = getAllUseCaseHandler;
        }

        // proxy methods
        public Validation<IError, StoreModel> Create(StoreCreateCommand command)
        {
            // TODO: call first createUseCaseHandlers put command
            // TODO: call next createUseCaseHandlers put command
            // TODO: call real method
            //var current = IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>(this);
            //var g = IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>.Handle(command);

            //createUseCaseHandlers.Aggregate((command, (Validation<IError, StoreModel>)null), (arguments, handler) => (arguments.Item1, handler.Handle(arguments.Item1)));

            //var stepCommand = command;
            //Validation<IError, StoreModel> result;
            //foreach (var useCaseHandler in createUseCaseHandlers)
            //{
            //    result = useCaseHandler.Handle(stepCommand);
            //}

            //return result;

            throw new System.NotImplementedException();
        }

        public Validation<IError, IEnumerable<StoreModel>> Create(StoreManyCreateCommand command)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<StoreModel> GetAll(Unit unit)
        {
            throw new System.NotImplementedException();
        }

        // logic methods
        Validation<IError, StoreModel> IUseCaseHandler<StoreCreateCommand, Validation<IError, StoreModel>>.Handle(StoreCreateCommand input)
        {
            throw new System.NotImplementedException();
        }

        Validation<IError, IEnumerable<StoreModel>> IUseCaseHandler<StoreCreateCommand, Validation<IError, IEnumerable<StoreModel>>>.Handle(StoreCreateCommand input)
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<StoreModel> IUseCaseHandler<Unit, IEnumerable<StoreModel>>.Handle(Unit unit)
        {
            throw new System.NotImplementedException();
        }
    }
}
