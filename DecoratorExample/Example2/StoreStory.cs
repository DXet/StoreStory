using System.Collections.Generic;

namespace DecoratorExample.Example2
{
    public interface IApplicationService { }

    public interface IUseCaseHandler<in T, out TResult> { TResult Handle(T input); }

    public class Validation<TError, TModel> { }

    public interface IError { }

    public class StoreModel { }

    public class StoreCreateCommand : ICommand { }
    public class StoreManyCreateCommand : ICommand { }

    public struct Unit { }

    public struct UnitQuery : IQuery { public static UnitQuery Instanse = new UnitQuery(); }

    public interface IQuery { }

    public interface ICommand { }

    public interface IQueryHandler<in TInput, out TOutput> : IUseCaseHandler<TInput, TOutput> where TInput : IQuery { }

    public interface ICommandHandler<in TInput, out TOutput> : IUseCaseHandler<TInput, TOutput> where TInput : ICommand { }

    public interface IStoreCreateCommandHandler : ICommandHandler<StoreCreateCommand, Validation<IError, StoreModel>> { }

    public class StoreCreateCommandHandler : IStoreCreateCommandHandler
    {
        public Validation<IError, StoreModel> Handle(StoreCreateCommand input)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IStoreCreateManyCommandHandler : ICommandHandler<StoreManyCreateCommand, Validation<IError, IEnumerable<StoreModel>>> { }

    public class StoreCreateManyCommandHandler : IStoreCreateManyCommandHandler
    {
        public Validation<IError, IEnumerable<StoreModel>> Handle(StoreManyCreateCommand input)
        {
            throw new System.NotImplementedException();
        }
    }

    //public interface IStoreGetAllQueryHandler : IQueryHandler<UnitQuery, IEnumerable<StoreModel>> { }

    public class StoreGetAllQueryHandler :
    //IStoreGetAllQueryHandler
        IQueryHandler<UnitQuery, IEnumerable<StoreModel>>
    {
        public IEnumerable<StoreModel> Handle(UnitQuery unit)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StoreGetAllQueryHandlerDecorator : IQueryHandler<UnitQuery, IEnumerable<StoreModel>>
    {
        private readonly IQueryHandler<UnitQuery, IEnumerable<StoreModel>> decoratee;

        public StoreGetAllQueryHandlerDecorator(IQueryHandler<UnitQuery, IEnumerable<StoreModel>> decoratee)
        {
            this.decoratee = decoratee;
        }

        public IEnumerable<StoreModel> Handle(UnitQuery unit)
        {
            return decoratee.Handle(unit);
        }
    }

    public interface IStoreApplicationService : IApplicationService
    {
        Validation<IError, StoreModel> Create(StoreCreateCommand command);
        Validation<IError, IEnumerable<StoreModel>> CreateMany(StoreManyCreateCommand command);

        IEnumerable<StoreModel> GetAll(UnitQuery unit);
    }

    public class StoreApplicationService : IStoreApplicationService
    {
        private readonly IStoreCreateCommandHandler storeCreateCommandHandler;
        private readonly IStoreCreateManyCommandHandler storeCreateManyCommandHandler;
        //private readonly IStoreGetAllQueryHandler storeGetAllQueryHandler;
        private readonly IQueryHandler<UnitQuery, IEnumerable<StoreModel>> storeGetAllQueryHandler;

        public StoreApplicationService(
            IStoreCreateCommandHandler storeCreateCommandHandler,
            IStoreCreateManyCommandHandler storeCreateManyCommandHandler,
            //IStoreGetAllQueryHandler storeGetAllQueryHandler
            IQueryHandler<UnitQuery, IEnumerable<StoreModel>> storeGetAllQueryHandler
        )
        {
            this.storeCreateCommandHandler = storeCreateCommandHandler;
            this.storeCreateManyCommandHandler = storeCreateManyCommandHandler;
            this.storeGetAllQueryHandler = storeGetAllQueryHandler;
        }

        public Validation<IError, StoreModel> Create(StoreCreateCommand command) => storeCreateCommandHandler.Handle(command);

        public Validation<IError, IEnumerable<StoreModel>> CreateMany(StoreManyCreateCommand command) => storeCreateManyCommandHandler.Handle(command);

        public IEnumerable<StoreModel> GetAll(UnitQuery unit) => storeGetAllQueryHandler.Handle(unit);
    }
}
