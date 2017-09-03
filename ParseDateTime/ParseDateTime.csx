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

        var ci = culture != null ? CultureInfo.GetCultureInfo(culture) : CultureInfo.InvariantCulture;
        var parsed = DateTime.ParseExact(val, pattern, ci);
        if (kind.HasValue)
            parsed = DateTime.SpecifyKind(parsed, kind.Value);

        await context.SetValue(path, parsed);
        return json;
    })
    .WithDefaultConfigKey("path");

