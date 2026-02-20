public class AccessControlService
{
    public bool IsAvailableNow(DateTimeOffset? from, DateTimeOffset? until)
    {
        var now = DateTimeOffset.UtcNow;

        if (from.HasValue && now < from.Value)
            return false;

        if (until.HasValue && now > until.Value)
            return false;

        return true;
    }

    public bool IsCourseAvailable(DateTimeOffset? start, DateTimeOffset? end)
    {
        return IsAvailableNow(start, end);
    }
}
