# Mailgun Addin #

Send Google Analytics events

## Usage ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/GoogleAnalytics/GoogleAnalytics.csx"

services:
  analytics:
    type: googleAnalytics
    trackingId: UA-xxxxxx-yy

pipelines:
  - 
    when: 
      - { queue: started }
    from:
      - { http: "https://jsonplaceholder.typicode.com/posts", method: get }
    to:
      - { ga.event, service: analytics, clientId: "555", eventCategory: "post", eventAction: "fetched" }
```

## Service arguments ###

```trackingId``` - Tracking ID / Property ID. Required. (tid) 

## Tasks ###

See https://developers.google.com/analytics/devguides/collection/protocol/v1/devguide#event for explanation.

### ga.event ###

```service``` - Registered googleAnalytics service. Defaults to ```ga```.

```clientId``` - Anonymous Client ID. (cid)

```eventCategory``` - Event Category. Required. (ec)

```eventAction``` - Event Action. Required. (ea)

```eventLabel``` - Event label. (el)

```eventValue``` - Event value. (ev)

```userIP``` - IP address override. (uip)

