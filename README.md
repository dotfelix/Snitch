Snitch
=======

Real-time exception alerting for .NET Core apps


### Installing Snitch

You can install [Snitch with NuGet](https://www.nuget.org/packages/Snitch):

    Install-Package Snitched
    
Or via the .NET Core command line interface:

    dotnet add package Snitched

### Using Snitch

You can use Snitch as

```csharp
// service collection
services.AddSnitch(appName: "Snitch.Example")

// app build
app.UseSnitch()
```

### Usage
```csharp
try 
{
    // your code here
}
catch (Exception ex)
{
    await ex.Snitched();
}
```

### Examples
See Example Project
See Test
