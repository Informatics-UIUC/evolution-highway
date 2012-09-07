using System;
using System.Diagnostics;

namespace EvolutionHighwayApp.Infrastructure.MVVM
{
    public abstract class ViewModelBase : ModelBase, IDisposable
    {
        protected ViewModelBase()
        {
            //Debug.WriteLine("{0} instantiated", GetType().Name);                        
        }
        
        public virtual void Dispose() { }
    }
}
