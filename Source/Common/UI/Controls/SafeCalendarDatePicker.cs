using Microsoft.UI.Xaml.Controls;

namespace EHunter.Controls
{
    [DependencyProperty("SafeDate", typeof(DateTimeOffset), DefaultValue = "default(DateTimeOffset)", InstanceChangedCallback = true)]
    public partial class SafeCalendarDatePicker : CalendarDatePicker
    {
        public SafeCalendarDatePicker()
        {
            DateChanged += (s, e) =>
            {
                if (e.NewDate == null)
                    s.Date = e.OldDate;
                else
                    SafeDate = e.NewDate.Value;
            };
        }

        partial void OnSafeDateChanged(DateTimeOffset oldValue, DateTimeOffset newValue) => Date = newValue;
    }
}
