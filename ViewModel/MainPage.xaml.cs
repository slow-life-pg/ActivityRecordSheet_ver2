namespace ActivityRecordSheet_ver2;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		RecordDatePicker.Date = DateTime.Today;
	}

	private void OnMoodValueChanged(object sender, ValueChangedEventArgs e)
	{
		var slider = (Slider)sender;
		var snappedValue = Math.Round(e.NewValue * 2, MidpointRounding.AwayFromZero) / 2;
		if (slider.Value != snappedValue)
			slider.Value = snappedValue;
	}

	private void OnMoodSliderHandlerChanged(object sender, EventArgs e)
	{
		OnControlHandlerChanged(sender, e);
#if WINDOWS
		if (((Slider)sender).Handler?.PlatformView is Microsoft.UI.Xaml.Controls.Slider slider)
		{
			slider.SnapsTo = Microsoft.UI.Xaml.Controls.Primitives.SliderSnapsTo.StepValues;
			slider.StepFrequency = 0.5;
			slider.SmallChange = 0.5;
			slider.LargeChange = 0.5;
		}
#endif
	}

	private void OnControlHandlerChanged(object sender, EventArgs e)
	{
#if WINDOWS
		if (((View)sender).Handler?.PlatformView is Microsoft.UI.Xaml.FrameworkElement element)
		{
			element.UseSystemFocusVisuals = true;
			element.FocusVisualPrimaryThickness = new Microsoft.UI.Xaml.Thickness(0);
			element.FocusVisualSecondaryThickness = new Microsoft.UI.Xaml.Thickness(0);
		}
#endif
	}

	private void OnRatingSliderLoaded(object sender, EventArgs e)
	{
		// Apply after MAUI's initial property mappings, which set the native step size.
		OnMoodSliderHandlerChanged(sender, e);
	}

	private void OnMoodSliderLoaded(object sender, EventArgs e)
	{
		OnRatingSliderLoaded(sender, e);
		// Initial window activation can replace the focus assigned during layout.
		if (Window is not null)
			Window.Activated += OnInitialWindowActivated;
		MoodSlider.Dispatcher.Dispatch(FocusMoodSlider);
	}

	private void OnInitialWindowActivated(object sender, EventArgs e)
	{
		((Window)sender).Activated -= OnInitialWindowActivated;
		MoodSlider.Dispatcher.Dispatch(FocusMoodSlider);
	}

	private void FocusMoodSlider()
	{
#if WINDOWS
		if (MoodSlider.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.Slider slider)
			slider.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
#else
		MoodSlider.Focus();
#endif
	}
}

