
# ViewStateless

Encrypted ViewState generator / viewer.

## Usage

```
docker build -t viewstateless .
```

```
$ docker run --rm -it viewstateless encrypt \
  --ek 03c77ee00ec5cc17f81d1ab76a22e5edd98fea8ebba89c2b \
  --vk d35f9976db1a9858d9487b17ddbf22c29fe602125bf5dbe3 \
  --msg "$(echo -n hello world | base64)"
QvKL3yz1SpchBeCylzZEEC5iofUCmcF/lKPDASR3T8hoOOAoym3TaHplbX8=

$ docker run --rm -it viewstateless decrypt \
  --ek 03c77ee00ec5cc17f81d1ab76a22e5edd98fea8ebba89c2b \
  --vk d35f9976db1a9858d9487b17ddbf22c29fe602125bf5dbe3 \
  --msg QvKL3yz1SpchBeCylzZEEC5iofUCmcF/lKPDASR3T8hoOOAoym3TaHplbX8=
hello world
```
