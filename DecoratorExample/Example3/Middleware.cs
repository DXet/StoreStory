using System;
using System.Collections.Generic;
using System.Linq;

namespace DecoratorExample.Example3
{
    public abstract class Middleware<T, TResult> : IMiddleware<T, TResult>
    {
        private readonly IEnumerable<IMiddleware<T, TResult>> middlewareItems;

        protected Middleware(IEnumerable<IMiddleware<T, TResult>> middlewareItems)
        {
            this.middlewareItems = middlewareItems;
        }

        public abstract T BeforeNext(T input);
        public abstract TResult AfterNext(TResult prevResult);

        private Func<T, TResult> GetNext(IMiddleware<T, TResult> currentMiddleware, IList<IMiddleware<T, TResult>> mItems)
        {
            if (currentMiddleware == null)
            {
                return null;
            }

            var currentMiddlewareIndex = mItems.IndexOf(currentMiddleware);
            var nextMiddleware = mItems.ElementAtOrDefault(currentMiddlewareIndex + 1);
            if (nextMiddleware == null)
            {
                return input => currentMiddleware.Next(null, input);
            }

            return input => currentMiddleware.Next(nextMiddleware, input);
        }

        public TResult Handle(T input)
        {
            var mItems = middlewareItems.ToList();
            var bInput = BeforeNext(input);

            var next = GetNext(mItems.FirstOrDefault(), mItems);
            var nextResult = next(bInput);

            var aResult = AfterNext(nextResult);
            return aResult;
        }

        public TResult Next(IMiddleware<T, TResult> next, T input)
        {
            throw new NotImplementedException();
        }
    }

    public interface IMiddleware<T, TResult>
    {
        TResult Next(IMiddleware<T, TResult> next, T input);
    }


    //public interface IMiddlewareWorkflow<in T, out TResult>
    //{
    //    TResult Handle(T input);
    //}

    //public class MiddlewareWorkflow<T, TResult> : IMiddlewareWorkflow<T, TResult>
    //{
    //    private readonly IEnumerable<IMiddleware<T, TResult>> middlewareItems;

    //    public MiddlewareWorkflow(IEnumerable<IMiddleware<T, TResult>> middlewareItems)
    //    {
    //        this.middlewareItems = middlewareItems;
    //    }

    //    public TResult Handle(T input)
    //    {
    //        var mItems = middlewareItems.ToList();
    //        if (mItems.Count == 0)
    //        {
    //            throw new ArgumentException("bla bla bla");
    //        }

    //        var next = GetNext(mItems.First(), mItems);
    //        var result = next(input);

    //        return result;
    //    }

    //    private Func<T, TResult> GetNext(IMiddleware<T, TResult> currentMiddleware, List<IMiddleware<T, TResult>> mItems)
    //    {
    //        var currentMiddlewareIndex = mItems.IndexOf(currentMiddleware);
    //        var nextMiddleware = mItems.ElementAtOrDefault(currentMiddlewareIndex + 1);
    //        if (nextMiddleware == null)
    //        {
    //            return input => currentMiddleware.Next(null, input);
    //        }

    //        //GetNext(nextMiddleware, mItems)

    //        return input => currentMiddleware.Next(nextMiddleware, input);
    //    }
    //}
}
