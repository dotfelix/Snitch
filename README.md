Snitch
=======

Real-time exception alerting for .NET Core apps


### Installing Snitch

You can install [Snitch with NuGet](https://www.nuget.org/packages/Snitch):

    Install-Package Snitch
    
Or via the .NET Core command line interface:

    dotnet add package Snitch

### Using Snitch

You can use Snitch as

```csharp
// service collection
services.AddSnitch(appName: "Snitch.Example")

// app build
app.UseSnitch()
```

### Examples
See Test
