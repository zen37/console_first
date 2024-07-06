public static class Utilities
{
    public static string GetFormattedDateTime(string timeZoneId)
    {
        string timeZoneId = config["timeZoneId"];
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
        string formattedDateTime = dateToRetrieve.ToString("yyyyMMdd_HHmm");
        return formattedDateTime;
    }

    public static DateTime GetCurrentDateTime(IConfiguration config)
    {
        string timeZoneId = config["timeZoneId"];

        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
        return dateToRetrieve;
    }

    // Other static utility methods related to DateTime can be added here
}




/*
public static class Utilities
{
    private static readonly IConfiguration _config;

    // Static constructor to initialize static fields
    static Utilities()
    {
        string environment = Config.ParseEnvironment(args); // If needed
        _config = Config.LoadConfiguration(environment);
    }

    public static string GetFormattedDateTime()
    {
        string timeZoneId = _config["timeZoneId"];
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
        string formattedDateTime = dateToRetrieve.ToString("yyyyMMdd_HHmm");
        return formattedDateTime;
    }

    // Other static utility methods related to DateTime can be added here
}
*/