using Android.App;
using Android.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace ActivityRecordSheet_ver2.Platforms.Android;

public class ImmediateDatePickerHandler : DatePickerHandler
{
    private static readonly IPropertyMapper<IDatePicker, ImmediateDatePickerHandler> CalendarMapper =
        new PropertyMapper<IDatePicker, ImmediateDatePickerHandler>(DatePickerHandler.Mapper)
        {
            [nameof(IDatePicker.IsOpen)] = (handler, picker) =>
            {
                if (picker.IsOpen)
                    handler.ShowCalendar();
                else
                    handler.HideCalendar();
            }
        };

    private AlertDialog calendarDialog;
    private CalendarView calendar;

    public ImmediateDatePickerHandler() : base(CalendarMapper)
    {
    }

    protected override void ConnectHandler(MauiDatePicker platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ShowPicker = ShowCalendar;
        platformView.HidePicker = HideCalendar;
    }

    private void ShowCalendar()
    {
        if (calendarDialog?.IsShowing == true)
            return;

        DisposeCalendar();
        calendar = new CalendarView(Context);
        if (VirtualView.MinimumDate is DateTime minimum)
            calendar.MinDate = ToMilliseconds(minimum);
        if (VirtualView.MaximumDate is DateTime maximum)
            calendar.MaxDate = ToMilliseconds(maximum);
        calendar.Date = ToMilliseconds(VirtualView.Date ?? DateTime.Today);

        calendarDialog = new AlertDialog.Builder(Context)
            .SetTitle("日付")
            .SetView(calendar)
            .SetNegativeButton("キャンセル", (_, _) => { })
            .Create();
        calendar.DateChange += OnDateChange;
        calendarDialog.DismissEvent += OnDismiss;
        calendarDialog.Show();
        VirtualView.IsOpen = true;
    }

    private void OnDateChange(object sender, CalendarView.DateChangeEventArgs e)
    {
        VirtualView.Date = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);
        HideCalendar();
    }

    private void HideCalendar() => calendarDialog?.Dismiss();

    private void OnDismiss(object sender, EventArgs e)
    {
        if (VirtualView is not null)
            VirtualView.IsOpen = false;
    }

    private static long ToMilliseconds(DateTime date) =>
        new DateTimeOffset(date.Date).ToUnixTimeMilliseconds();

    private void DisposeCalendar()
    {
        if (calendarDialog is not null)
        {
            calendarDialog.Dismiss();
            calendarDialog.DismissEvent -= OnDismiss;
            calendarDialog.Dispose();
            calendarDialog = null;
        }
        if (calendar is not null)
        {
            calendar.DateChange -= OnDateChange;
            calendar.Dispose();
            calendar = null;
        }
    }

    protected override void DisconnectHandler(MauiDatePicker platformView)
    {
        DisposeCalendar();
        base.DisconnectHandler(platformView);
    }
}
