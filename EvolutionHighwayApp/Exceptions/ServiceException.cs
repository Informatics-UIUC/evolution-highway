using System;

namespace EvolutionHighwayApp.Exceptions
{
    public class ServiceException: Exception
    {
        public object Context { get; private set; }

        public ServiceException(String message, Exception innerException, object context = null): base(message, innerException)
        {
            Context = context;
        }
    }
}
