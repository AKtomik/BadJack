# Bad Back

## Release

thoses commands are here to make the smallest and most compact executable.
The runner do not needs dotnet to be installed. If you want so, change `--self-contained` to true.

### linux

```sh
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
```

### window

```sh
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true
```