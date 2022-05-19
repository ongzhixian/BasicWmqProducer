# BasicWmqProducer

A basic .NET Core Websphere MQ producer console application use for simple deployments in Kubernetes.

## dotnet CLI

dotnet CLI used to create this project:

```ps1: In C:\src\github.com\ongzhixian\BasicWmqProducer
dotnet new sln -n BasicWmqProducer
dotnet new console -n BasicWmqProducer.ConsoleApp
dotnet sln .\BasicWmqProducer.sln add .\BasicWmqProducer.ConsoleApp\

dotnet add .\BasicWmqProducer.ConsoleApp\ package Microsoft.Extensions.Configuration
dotnet add .\BasicWmqProducer.ConsoleApp\ package Microsoft.Extensions.Configuration.Json

dotnet add .\BasicWmqProducer.ConsoleApp\ package IBMXMSDotnetClient
```

Other packages that we may want to include to expand on configuration options:
Microsoft.Extensions.Configuration.CommandLine
Microsoft.Extensions.Configuration.Binder
Microsoft.Extensions.Configuration.EnvironmentVariables 

dotnet user-secrets --project .\BasicWmqProducer.ConsoleApp\ init
dotnet user-secrets --project .\BasicWmqProducer.ConsoleApp\ set "ibm_mq:userId" "<user-id>"
dotnet user-secrets --project .\BasicWmqProducer.ConsoleApp\ set "ibm_mq:password" "<user-password>"
