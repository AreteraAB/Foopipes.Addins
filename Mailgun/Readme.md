# Mailgun Addin #

Send email with Mailgun

## Usage ##

```
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/Mailgun/mailgun.cs"

services:
  mailgun:
    type: mailgun
    apiBaseUrl: https://api.mailgun.net/v3/sandbox5ded26xxxxxxxxxxxxb8.mailgun.org
    apiKey: key-3a56bxxxxxxxxxxxxxxx5c

pipelines:
  - 
    when: 
      - { queue: started }
    from:
      - { http: "https://jsonplaceholder.typicode.com/posts", method: get }
    to:
      - { log }
    error:
      - {mailgun.send, to: me@mydomain.com, from: me@mydomain.com, subject: Error, text: An error occured }
```
