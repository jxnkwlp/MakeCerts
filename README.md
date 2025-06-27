# MKCERT
 
``` shell
Description:
  A tool for create self signed certificate

Usage:
  mkcert [<subjects>...] [options]

Arguments:
  <subjects>

Options:
  -o, --output <output>       [default: certs]
  -y, --year <year>           [default: 5]
  -i, --install
  --ca
  -root, --root-ca <root-ca>
  --version                   Show version information
  -?, -h, --help              Show help and usage information
```

## New CA cert

``` shell
mkcert --ca -i "myca"
```

## New cert with CA

``` shell
mkcert --root "myca" "localhost"
```

## New cert without CA

``` shell
mkcert "localhost" "myhome.local"
```
