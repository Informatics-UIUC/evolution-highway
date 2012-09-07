namespace EvolutionHighwayApp.Events
{
    public class ActionCompletedEventArgs<T>
    {
        public T Result { get; private set; }

        public ActionCompletedEventArgs(T result)
        {
            Result = result;
        }
    }
}
