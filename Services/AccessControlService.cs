using System;

public class AccessControlService
{
    public bool IsAvailableNow(DateTime? from, DateTime? until)
    {
        var now = DateTime.Now;

        if (from.HasValue && now < from.Value)
            return false;

        if (until.HasValue && now > until.Value)
            return false;

        return true;
    }

    public bool IsCourseAvailable(DateTime? start, DateTime? end)
    {
        return IsAvailableNow(start, end);
    }
}
