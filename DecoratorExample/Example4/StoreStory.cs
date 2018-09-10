using System;
using System.Collections.Generic;

namespace DecoratorExample.Example4
{
    public interface IApplicationService { }

    public class Validation<TError, TModel> { }

    public interface IError { }

    public class SomeError : IError { }

    public class StoreModel { }

    public class Store { }

    public class StoreCreateCommand : ICommand { }
    public class StoreManyCreateCommand : ICommand { }

    public struct Unit { }

    public interface ICommand { }

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
        private readonly IEnumerable<IStoreCreateInterrupter> storeCreateInterrupters;

        public StoreApplicationService(IEnumerable<IStoreCreateInterrupter> storeCreateInterrupters)
        {
            this.storeCreateInterrupters = storeCreateInterrupters;
        }

        public Validation<IError, StoreModel> Create(StoreCreateCommand command)
        {
            // validate
            // create domain model
            var store = new Store();
            // interrupt
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

            // call store service, in order to save it
            // throw async Event

            // return successful result
            return new Validation<IError, StoreModel>();
        }

        public Validation<IError, IEnumerable<StoreModel>> CreateMany(StoreManyCreateCommand command) => throw new NotImplementedException();

        public IEnumerable<StoreModel> GetAll(Unit unit) => throw new NotImplementedException();
    }
}
