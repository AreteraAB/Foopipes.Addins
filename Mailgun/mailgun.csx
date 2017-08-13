using Foopipes.Abstractions.Services;
using Foopipes.Abstractions.Exceptions;

class MailgunService : ServiceBase
{
	public string ApiKey => Config["apiKey"];
	public string ApiBaseUrl => Config["apiBaseUrl"];
	public string DefaultTo => Config["defaultTo"];
	public string DefaultFrom => Config["defaultFrom"];

	public void VerifyArgs()
	{
		if (ApiKey == null)
			throw new ArgumentException("Mailgun apiKey not specified");

		if (ApiBaseUrl == null)
			throw new ArgumentException("Mailgun apiBaseUrl not specified");
	}
}

Service.Register("mailgun", typeof(MailgunService));

PipelineTask("mailgun.send").Json(async (context, json, ct)=>
	{
		var service = await context.GetService<MailgunService>(defaultName: "mailgun");
		service.VerifyArgs();

		var data = JObject.FromObject(new 
		{
			to = await context.GetExpandedConfigValue("to", false) ?? service.DefaultTo ?? throw new ArgumentException("No from argument"),
			from = await context.GetExpandedConfigValue("from", false) ?? service.DefaultFrom ?? "noreply@nosender.com",
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

		var r = await context.RunTask("http").WithData(data).WithArguments(config).Invoke(ct);
		return json; 
	});

