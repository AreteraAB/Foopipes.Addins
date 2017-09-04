using System.Globalization;
using System;

PipelineTask("parseDateTime.parseExact")
    .Json(async (context, json, ct) =>
    {
        var path = context.GetConfigValue("path", true);
        var val = await context.GetExpandedConfigValue("value", true);
        var pattern = context.GetConfigValue("pattern", true);

        var culture = context.GetConfigValue("culture", false);
        var kind = context.GetAndConvertConfigValue<DateTimeKind?>("kind");
        var offset = context.GetAndConvertConfigValue<TimeSpan?>("offset");

        var ci = culture != null ? new CultureInfo(culture) : CultureInfo.InvariantCulture;
        var parsed = DateTime.ParseExact(val, pattern, ci);
        if (kind.HasValue)
            parsed = DateTime.SpecifyKind(parsed, kind.Value);

        if (offset.HasValue)
        {
            await context.SetValue(path, new DateTimeOffset(parsed, offset.Value));
        }
        else
        {
            await context.SetValue(path, parsed);
        }
        return json;
    })
    .WithDefaultConfigKey("path");

