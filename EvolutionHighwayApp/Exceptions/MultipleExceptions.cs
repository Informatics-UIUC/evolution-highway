using System;
using System.Collections.Generic;
using System.Text;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Exceptions
{
    public class MultipleExceptions : Exception
    {
        private readonly IEnumerable<Exception> _exceptions;
 
        public MultipleExceptions(IEnumerable<Exception> exceptions)
        {
            _exceptions = exceptions;
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                Exceptions.ForEach(e =>
                                    {
                                        if (e.InnerException != null)
                                            sb.AppendFormat("{0} ({1})", e.Message, e.InnerException.Message);
                                        else
                                            sb.Append(e.Message);
                                        sb.AppendLine();
                                    });
                return sb.ToString();
            }
        }

        public IEnumerable<Exception> Exceptions
        {
            get { return _exceptions; }
        }
    }
}
