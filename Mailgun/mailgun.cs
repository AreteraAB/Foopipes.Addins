using Foopipes.Abstractions.Services;
using Foopipes.Abstractions.Exceptions;

class MailgunService : ServiceBase
{
	public string ApiKey => Config["apiKey"];
	public string ApiBaseUrl => Config["apiBaseUrl"];
}

Service.Register("mailgun", typeof(MailgunService));

Task("mailgun.send").JsonAsync(async (context, json, ct)=>
	{
		var service = (MailgunService)context.GetService(context.GetExpandedConfigValue("service", false) ?? "mailgun") 
			?? throw new FoopipesException("No Mailgun service found");
		var data = JObject.FromObject(new 
		{
			to = context.GetExpandedConfigValue("to", true),
			from = context.GetExpandedConfigValue("from", false) ?? "noreply@nosender.com",
			subject = context.GetExpandedConfigValue("subject", false) ?? "(empty)",
			text = context.GetExpandedConfigValue("text", false) ?? "(empty)"
		});

		var config = new Dictionary<string, string>
		{
			{"url", service.ApiBaseUrl + "/messages" },
			{"method", "post"},
			{"body", "multipartFormUrlEncoded"},
			{"user", "api:" + service.ApiKey}
		};

		await context.RunJsonTaskAsync("http", new JsonData(data), config, ct);
		return new ProcessJsonResult(json); 
	});

