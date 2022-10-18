using System.Diagnostics;

namespace MAUIMessagingCenterBug;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        bool exitLoop = false;

        try
        {
            Task.Run(() =>
            {
                do
                {
                    try
                    {
                        //Debug.Print($"Running Messaigng center operations on thread: {Thread.CurrentThread.ManagedThreadId}");
                        MessagingCenter.Subscribe<MainPage, string>(this, $"test{0}", (a, b) => { });
                        MessagingCenter.Unsubscribe<MainPage, string>(this, $"test{0}");
                    }
                    catch (Exception ex1)
                    {
                        Debug.Print($"Error Occurred: {ex1}");
                    }
                } while (!exitLoop);
            });

            Task.Run(() =>
            {
                do
                {
                    try
                    {
                        //Debug.Print($"Running Messaigng center operations on thread: {Thread.CurrentThread.ManagedThreadId}");
                        MessagingCenter.Subscribe<MainPage, string>(this, $"test{0}", (a, b) => {
                            MessagingCenter.Unsubscribe<MainPage, string>(this, $"test{0}");
                        });
                        MessagingCenter.Send<MainPage, string>(this, $"test{0}", "foo");
                    }
                    catch (Exception ex2)
                    {
                        Debug.Print($"Error Occurred: {ex2}");
                    }

                } while (!exitLoop);
            });

            await DisplayAlert("Done", $"Waiting for errors", "OK");
        }
        finally
        {
            exitLoop = true;
        }
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        bool exitLoop = false;

        try
        {
            Task.Run(() =>
            {
                do
                {
                    try
                    {
                        //Debug.Print($"Running Messaigng center operations on thread: {Thread.CurrentThread.ManagedThreadId}");
                        SafeMessagingCenter.Instance.Subscribe<MainPage, string>(this, $"test{0}", (a, b) => { });
                        Debug.Print($"About to unsubscribe on thread: {Thread.CurrentThread.ManagedThreadId}");
                        SafeMessagingCenter.Instance.Unsubscribe<MainPage, string>(this, $"test{0}");
                        Debug.Print($"Unsubscribed on thread: {Thread.CurrentThread.ManagedThreadId}");
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"Error Occurred: {ex}");
                    }
                } while (!exitLoop);
            });

            Task.Run(() =>
            {
                do
                {
                    try
                    {
                        //Debug.Print($"Running Messaigng center operations on thread: {Thread.CurrentThread.ManagedThreadId}");
                        SafeMessagingCenter.Instance.Subscribe<MainPage, string>(this, $"test{0}", (a, b) => {
                            Debug.Print($"About to internal unsubscribe on thread: {Thread.CurrentThread.ManagedThreadId}");
                            SafeMessagingCenter.Instance.Unsubscribe<MainPage, string>(this, $"test{0}");
                            Debug.Print($"Internal unsubscribed on thread: {Thread.CurrentThread.ManagedThreadId}");
                        });
                        SafeMessagingCenter.Instance.Send<MainPage, string>(this, $"test{0}", "foo");
                    }
                    catch (Exception ex)
                    {
                        Debug.Print($"Error Occurred: {ex}");
                    }
                } while (!exitLoop);
            });

            await DisplayAlert("Done", $"Waiting for errors", "OK");
        }
        finally
        {
            exitLoop = true;
        }
    }
}

