# Aes Addin #

AES is a subset of the Rijndael cipher. 

## Tasks ##
 * aes.decryptstring
 * aes.decryptjson
 * aes.decryptbinary
 
## Example Usage ##

```
plugins: 
addins:
  - url: "https://raw.githubusercontent.com/AreteraAB/Foopipes.Addins/master/Aes/aes.cs"
services: 
  udp: 
    type: udplistener
    port: 5555

pipelines:
  - 
    when: 
      - udp
    do:
      - { aes.decryptbinary, key: "${key}", iv: "${iv}" }
    to: 
      - log
```
