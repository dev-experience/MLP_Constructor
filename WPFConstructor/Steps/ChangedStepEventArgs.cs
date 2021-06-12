namespace WPFConstructor.Steps
{
    public class ChangedStepEventArgs
    {
        public ChangedStepEventArgs(bool newState)
        {
            Complete = newState;
        }

        public bool Complete { get; }
    }
}