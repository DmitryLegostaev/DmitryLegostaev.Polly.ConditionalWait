[![Nuget](https://img.shields.io/nuget/v/DmitryLegostaev.Polly.ConditionalWait.svg)](https://www.nuget.org/packages/DmitryLegostaev.Polly.ConditionalWait)
[![Build](https://github.com/DmitryLegostaev/DmitryLegostaev.Polly.ConditionalWait/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/DmitryLegostaev/DmitryLegostaev.Polly.ConditionalWait/actions/workflows/build.yml)
![GitHub repo size](https://img.shields.io/github/repo-size/DmitryLegostaev/DmitryLegostaev.Polly.ConditionalWait)

## DmitryLegostaev.Polly.ConditionalWait

A small class library to provide Conditional Wait functionality using Polly v7 library.

### Usage
#### Obtaining ConditionalWait object instance
Explicitly create ConditionalWait object
```csharp
var conditionalWait = new ConditionalWait();
```
or use a DI to obtain ConditionalWait object.


#### (optional) Configuration of default Timeout and BackOffDelay
You can configure default timeout and backoffdelay for ConditionalWait object through its constructor. Defaults are 30s/300ms
```csharp
var conditionalWait = new ConditionalWait(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
```
or set environment variables
```csharp
Environment.SetEnvironmentVariable("ConditionalWait__defaultTimeout", TimeSpan.FromSeconds(10).ToString());
Environment.SetEnvironmentVariable("ConditionalWait__defaultBackOffDelay", TimeSpan.FromSeconds(3).ToString());
```
Configuration priority is: Constructor parameters -> Environment variables -> Pre-defined defaults (30s/300ms)

#### Actual usage
Each method require Func to execute within its body. WaitForPredicateAndGetResult requires a predicate to be passed as argument in addition to Func.
```csharp
// Wait for true to be returned from Func execution.
conditionalWait.WaitForTrue(() => 2 + 2 == 4);

// Wait for not null object to be returned from Func execution. Returns Func execution result to the calling method.
var equationResult = conditionalWait.WaitForAndGetResult(() => 2 + 2);

// Executing Func, obtaining its result, passing the result to the predicate, waiting for true to be returned from predicate. Returns Func execution result to the calling method.
var equationResult = conditionalWait.WaitForPredicateAndGetResult(() => 2 + 2, eq => eq == 4);
```

Each method could consume IConditionalWaitConfiguration object or Timeout/BackOffDelay. 
If 2nd option is chosen but Timeout or BackOffDelay is missing, then it will be obtained from ConditionalWait defaults defined during ConditionalWait object instantiation.
```csharp
conditionalWait.WaitForTrue(() => 2 + 2 == 4,
    new ConditionalWaitConfiguration(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3)));

conditionalWait.WaitForTrue(() => 2 + 2 == 4,
    TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3));
    
conditionalWait.WaitForTrue(() => 2 + 2 == 4,
    timeout: TimeSpan.FromSeconds(10));
    
conditionalWait.WaitForTrue(() => 2 + 2 == 4,
    backoffDelay: TimeSpan.FromSeconds(3));
```

Also ConditionalWait methods consume some optional arguments:

| Argument name      | Type          | Purpose                                                                                                  |
|--------------------|---------------|----------------------------------------------------------------------------------------------------------|
| exceptionsToIgnore | IList\<Type\> | List with Exception types (derived from System.Exception) to be ignored during ConditionalWait execution |
| failReason         | string        | String to be added to ConditionalWait timed out exception message                                        |
| codePurpose        | string        | String to be added to each retry attempt message (doesn't work without logger)                           |
| logger             | ILogger       | Microsoft.Extensions.Logging object to add debug outputs during ConditionalWait execution                |

#### ConditionalWaitConfiguration
To create ConditionalWaitConfiguration object you should pass TimeSpan Timeout and TimeSpan BackOffDelay to its constructor.
Also you can set Factor and BackoffType to customize main ConditionalWait behaviour based on WaitAndRetry policy.

ConditionalWaitConfiguration could be mapped from .NET Configuration by Microsoft.Extensions.Configuration.ConfigurationBinder.

To understand more about ConditionalWaitConfiguration capabilities visit https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry

