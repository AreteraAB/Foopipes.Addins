# Page.REST Addin #

Simple wrapper around [Page.REST](http://page.REST/) service. 

Page.Rest is an HTTP API you can use to extract content from any web page as JSON. 
You can get the title, description, open graph, embed content or any other information available at a given public URL.


## Tasks ##

### page.rest ###

```url``` - The url to scrape (default)

```token``` - Access token for Page.REST

```selectors``` - CSS selectors for what content to extract. Multiple selectors can be separated with ';'.

## Example ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Page.REST/Page.REST.csx"

pipelines:
  - 
    when: 
      - queue: started
    do: 
      - page.rest: "https://foopipes.com/blog"
        selectors: ".row div a"
        token: "secret access token or ${PAGEREST_TOKEN}"

    to:
      - log
```
