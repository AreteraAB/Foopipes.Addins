using Foopipes.Abstractions;
using System.Net;

PipelineTask("page.rest").Json(async (context, json, ct) =>
{
    var token = await context.GetExpandedConfigValue("token", true);
    var url = await context.GetExpandedConfigValue("url", true);
    var selectors = await context.GetExpandedConfigValue("selectors", true);

    var queryString = "?token=" + WebUtility.UrlEncode(token);
    queryString += "&url=" + WebUtility.UrlEncode(url);

    foreach(var selector in selectors.Split(';'))
    {
        queryString += "&selector=" + WebUtility.UrlEncode(selector);
    }

    var config = new Dictionary<string, string>
    {
        {"url", "https://page.rest/fetch" + queryString  },
        {"method", "get"},
        {"format", "json"}
    };

    var r = await context.RunTask("http").WithData(json).WithArguments(config).Invoke(ct);
    return r;
}).WithDefaultConfigKey("url");