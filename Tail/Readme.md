# Tail Addin #

Read the tail end of a textfile.

## Usage ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Tail/Tail.csx"

services: 
  logtail: 
    type: tail
    filename: "/project/logs/myfile.log"

pipelines: 
  - 
    when: 
      - logtail
    to:
      - log 
```

## Configuration ##

```filename``` - the file to read from

