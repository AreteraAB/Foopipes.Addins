# ParseDateTime Addin #

Parse DateTime values

## Tasks ##
 * parseDateTime.parseExact - Parse DateTime based on a pattern
  
  Arguments:
    * path    - JsonPath to the _target_ field (default value)
    * value   - Binding expression to the _source_ value
    * pattern - Date pattern
    * kind    - "Utc" or "Local" (optional)
	* culture - Name of culture to use (optional)
	
	
## Example Usage ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Regexp/Regexp.csx"
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/ParseDateTime/ParseDateTime.csx"

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
      - { parseDateTime.parseExact: "datetime", value: "#{datetime}", pattern: "yyyy-MM-dd HH:mm:ss,fff", kind: Local }
    to:
      - log
```

