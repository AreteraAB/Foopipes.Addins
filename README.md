# Foopipes.Addins #

Addins preview for next version of Foopipes.

## How to load an Addin ##
Addins are loaded in the ```addins``` section in ```foopipes.yml```.

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Mailgun/mailgun.csx"
  - filename: "/project/addins/myaddin.csx"
  - script: |
      PipelineTask("mytask").Json(async (context, json, ct)=>
          { 
             // do something
             return json;
          });
```

## Contributing ##
Pull requests are welcome!
