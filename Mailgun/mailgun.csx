using Foopipes.Abstractions.Services;
using Foopipes.Abstractions.Exceptions;

class MailgunService : ServiceBase
{
	public string ApiKey => Config["apiKey"];
	public string ApiBaseUrl => Config["apiBaseUrl"];
}

Service.Register("mailgun", typeof(MailgunService));

Task("mailgun.send").Json(async (context, json, ct)=>
	{
		var service = (MailgunService)context.GetService(await context.GetExpandedConfigValue("service", false) ?? "mailgun") 
			?? throw new FoopipesException("No Mailgun service found");
		var data = JObject.FromObject(new 
		{
			to = await context.GetExpandedConfigValue("to", true),
			from = await context.GetExpandedConfigValue("from", false) ?? "noreply@nosender.com",
			subject = await context.GetExpandedConfigValue("subject", false) ?? "(empty)",
			text = await context.GetExpandedConfigValue("text", false) ?? "(empty)"
		});

		var config = new Dictionary<string, string>
		{
			{"url", service.ApiBaseUrl + "/messages" },
			{"method", "post"},
			{"body", "multipartFormUrlEncoded"},
			{"user", "api:" + service.ApiKey}
		};

		var r = await context.RunJsonTaskAsync("http", new JsonData(data), config, ct);
		return json.Data; 
	});

