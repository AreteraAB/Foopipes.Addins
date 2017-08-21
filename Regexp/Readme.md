# Regexp Addin #

Perform Regex expressions on data

## Tasks ##
 * regexp.matches - Returns one binary result per match
 * regexp.matches_groups - Returns one json result per match, with groups spread as properties
 
## Example Usage ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Regexp/Regexp.csx"

services: 
  tail: 
    type: tail
    filename: "c:\\temp\\log4net.log"

pipelines: 
  - 
    when: 
      - tail
    do:
      - log 
      - regexp.matches_groups: (?'datetime'\S+ \S+) \[(?'threadId'\S+)\] (?'level'\S+) (?'logger'\S+) \[(?'context'\S+)\] - (?'message'.*)
    to:
      - log
```

## Examples ##

If log4net is configured:
```
2017-08-19 13:53:01,938 [1] DEBUG Log4NetTest.Program [(null)] - Hello nasty! 2136220501
2017-08-19 13:53:02,439 [1] DEBUG Log4NetTest.Program [(null)] - Hello nasty! 297437929
```

And the regexp pattern is:
```
    do:
      - regexp.matches_groups: (?'datetime'\S+ \S+) \[(?'threadId'\S+)\] (?'level'\S+) (?'logger'\S+) \[(?'context'\S+)\] - (?'message'.*)``
```

The resulting json becomes: (one single match, named groups are spread as properties)
```
{
	"datetime": "2017-08-19 13:53:01,938",
	"threadId": "1",
	"level": "DEBUG",
	"logger": "Log4NetTest.Program",
	"context": "(null)",
	"message": "Hello nasty! 2136220501"
}
```

If the regexp pattern is:
```
    do:
      - regexp.matches: \S+
```

Multiple binary results are returned: (multiple matches, complete match as binary)
```
"2017-08-19"
"13:53:01,938"
"[1]"
"DEBUG"

etc

```
