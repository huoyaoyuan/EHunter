namespace EHunter.UI.ViewModels.Messages
{
    public sealed class NavigateMessage
    {
        public NavigateMessage(PageVM target) => Target = target;

        public PageVM Target { get; }
    }

    public sealed class NavigateBackMessage
    {
    }

    public sealed class NavigateForwardMessage
    {
    }
}
