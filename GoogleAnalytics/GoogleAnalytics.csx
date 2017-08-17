using Foopipes.Abstractions.Services;

class GoogleAnalyticsService : ServiceBase
{
    public string TrackingId => Config["trackingId"];

    public void VerifyArgs()
    {
        if (TrackingId == null)
            throw new ArgumentException("Google Analytics TrackingID not specified");
    }
}

Service.Register("googleAnalytics", typeof(GoogleAnalyticsService));

PipelineTask("ga.event").Json(async (context, json, ct) =>
{
    var service = await context.GetService<GoogleAnalyticsService>(defaultName: "ga");
    service.VerifyArgs();

    // See https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide#event

    var data = JObject.FromObject(new
    {
        v = 1,
        t = "event",
        tid = service.TrackingId,
        cid = await context.GetExpandedConfigValue("clientId", false),
        ec = await context.GetExpandedConfigValue("eventCategory", true),
        ea = await context.GetExpandedConfigValue("eventAction", true),
        el = await context.GetExpandedConfigValue("eventLabel", false),
        ev = await context.GetExpandedConfigValue("eventValue", false),
        uip = await context.GetExpandedConfigValue("userIP", false),
    });

    var config = new Dictionary<string, string>
        {
            {"url", "https://www.google-analytics.com/collect" },
            {"method", "post"},
            {"body", "formUrlEncoded"}
        };

    var r = await context.RunTask("http").WithData(data).WithArguments(config).Invoke(ct);
    return json;
});

