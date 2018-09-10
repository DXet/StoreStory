using System;
using System.Collections.Generic;

namespace DecoratorExample.Example5
{
    public interface IUseCaseHandler<in T, out TResult> { TResult Handle(T input); }
    public struct UnitQuery : IQuery { public static UnitQuery Instanse = new UnitQuery(); }

    public interface IQuery { }

    public interface ICommand { }

    public interface IQueryHandler<in TInput, out TOutput> : IUseCaseHandler<TInput, TOutput> where TInput : IQuery { }

    public interface ICommandHandler<in TInput, out TOutput> : IUseCaseHandler<TInput, TOutput> where TInput : ICommand { }

    public interface IApplicationService { }

    public class Validation<TError, TModel> { }

    public interface IError { }

    public class SomeError : IError { }

    public class StoreModel { }

    public class Store { }

    public class StoreCreateCommand : ICommand { }
    public class StoreManyCreateCommand : ICommand { }

    public struct Unit { }

    public interface IStoreApplicationService : IApplicationService
    {
        Validation<IError, StoreModel> Create(StoreCreateCommand command);
        Validation<IError, IEnumerable<StoreModel>> CreateMany(StoreManyCreateCommand command);

        IEnumerable<StoreModel> GetAll(Unit unit);
    }

    public interface IInterrupter<in T>
    {
        Unit Interrupt(T input);
    }

    public interface IInterrupterArguments { }

    //public class InterrupterReason
    //{
    //    public InterrupterReason(string message)
    //    {
    //        Message = message;
    //    }

    //    public string Message { get; }
    //}

    public class CreateArguments : IInterrupterArguments
    {
        public bool IsCanceled { get; private set; }
        public IError Reason { get; private set; }

        public void Cancel(IError reason)
        {
            IsCanceled = true;
            Reason = reason;
        }
    }

    public interface IInterrupterModel
    {
    }

    public class StoreCreatingInterrupterModel : IInterrupterModel
    {
        public StoreCreatingInterrupterModel(Store store, CreateArguments arguments)
        {
            Store = store;
            Arguments = arguments;
        }

        public Store Store { get; }
        public CreateArguments Arguments { get; }
    }

    public interface IStoreCreateInterrupter : IInterrupter<StoreCreatingInterrupterModel> { }

    public static class InterrupterDispatcher
    {
        public static TModel Dispatch<TModel>(
            IEnumerable<IInterrupter<TModel>> interrupters,
            TModel model
        )
            where TModel : IInterrupterModel
        {
            foreach (var interrupter in interrupters)
            {
                interrupter.Interrupt(model);
            }

            return model;
        }
    }

    public class StoreApplicationService : IStoreApplicationService
    {
        private readonly ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>> 
            storeCreateCommandHandler;

        public StoreApplicationService(
            ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>> storeCreateCommandHandler
        )
        {
            this.storeCreateCommandHandler = storeCreateCommandHandler;
        }

        public Validation<IError, StoreModel> Create(StoreCreateCommand command) =>
            storeCreateCommandHandler.Handle(command);

        public Validation<IError, IEnumerable<StoreModel>> CreateMany(StoreManyCreateCommand command) => throw new NotImplementedException();

        public IEnumerable<StoreModel> GetAll(Unit unit) => throw new NotImplementedException();
    }

    public class StoreCreateCommandHandler : ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>>
    {
        private readonly IEnumerable<IStoreCreateInterrupter> storeCreateInterrupters;

        public StoreCreateCommandHandler(IEnumerable<IStoreCreateInterrupter> storeCreateInterrupters)
        {
            this.storeCreateInterrupters = storeCreateInterrupters;
        }

        public Validation<IError, StoreModel> Handle(StoreCreateCommand input)
        {
            // validate
            // create domain model
            var store = new Store();
            // interrupt
            DispatchCreating(store);
            // TODO: use bind next

            // call store service, in order to save it
            // throw async Event
            // use async message bus (persist event ?)
            // return successful result
            return new Validation<IError, StoreModel>();
        }

        private Validation<IError, Unit> DispatchCreating(Store store)
        {
            var args = new CreateArguments();
            var i = new StoreCreatingInterrupterModel(store, args);
            //var g = storeCreateInterrupters.Aggregate(i, (model, interrupter) => InterrupterDispatcher.Dispatch(interrupter, model));
            var iResult = InterrupterDispatcher.Dispatch(storeCreateInterrupters, i);

            if (iResult.Arguments.IsCanceled)
            {
                // TODO: return reason
                //Fail(args.Reason)
                return null;
            }

            // TODO: return unit
            return new Validation<IError, Unit>();
        }
    }

    //public class StoreGetAllQueryHandler 
    //{

    //}
}
