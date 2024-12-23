namespace RingSoft.ChurchLogix.Library.ViewModels
{
    public interface IStatsView
    {
        void RefreshChart();
    }

    public class StatsnGraphsViewModel
    {
        public IStatsView View { get; private set; }

        public void Initialize(IStatsView view)
        {
            View = view;
            View.RefreshChart();
        }
    }
}
