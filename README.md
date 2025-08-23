# MKCERT

Make a self signed certificate

## Quick start

Generate a certificate and install it on the system

``` shell
mkcert g -i "mysite.com" "10.2.3.4"
```

## Examples

### New CA cert and install to system

``` shell
mkcert ca "mylab" -i 
```

### New cert

``` shell
mkcert g "mysite.com"
mkcert g "mysite.com" "10.2.3.4"
```

### New cert with custom CA

``` shell
mkcert g --ca "mylab" "localhost"
```

### Other options

Use `-h` to show more options

``` shell
mkcert g -h
mkcert ca -h
mkcert i -h
```
