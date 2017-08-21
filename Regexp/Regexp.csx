using System;
using System.Text;
using System.Text.RegularExpressions;

JObject GroupsToJson(Match match)
{
    var jobj = new JObject();

    if (match.Groups.Count==0)
    {
        jobj["value"] = match.ToString();
        return jobj;
    }

    foreach(var group in match.Groups.Cast<Group>().Skip(match.Groups.Count>1 ? 1:0))
    {
        jobj[group.Name] = group.ToString();
    }

    return jobj;
}

PipelineTask("regexp.matches")
    .Binary(async (context, binary, ct) =>
        {
            var str = binary.ConvertToString();

            var pattern = await context.GetExpandedConfigValue("pattern", true);
            var regexp = new Regex(pattern);
            var matches = regexp.Matches(str);

            var jsonMatches = matches.Cast<Match>().Select(match=>BinaryData.FromString(match.ToString()));
            return new ProcessBinaryResult(jsonMatches);
        })
    .WithDefaultConfigKey("pattern");

PipelineTask("regexp.matches_groups")
    .Binary(async (context, binary, ct) =>
        {
            var str = binary.ConvertToString();

            var pattern = await context.GetExpandedConfigValue("pattern", true);
            var regexp = new Regex(pattern);
            var matches = regexp.Matches(str);

            var jsonMatches = matches.Cast<Match>().Select(match=>new JsonData(GroupsToJson(match)));
            return new ProcessJsonResult(jsonMatches);
        })
    .WithDefaultConfigKey("pattern");


