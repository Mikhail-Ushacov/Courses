using System;

public static class AppNavigationService
{
    public static Action<object>? NavigateAction;

    public static void Navigate(object view)
    {
        NavigateAction?.Invoke(view);
    }
}
