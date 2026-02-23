using System;

public static class AppNavigationService
{
    public static Action<object>? NavigateAction;
    public static Action? GoBackAction;

    public static void Navigate(object view)
    {
        NavigateAction?.Invoke(view);
    }

    public static void GoBack()
    {
        GoBackAction?.Invoke();
    }
}