namespace EHunter.Controls
{
    internal interface IViewFor<TViewModel> where TViewModel : class
    {
        TViewModel? ViewModel { get; set; }
    }
}
