# RxProperty

RxProperty provides a Observable and Getable / Setable property using ReactiveUI as a base.

Target frameworks are .net 6.0, .net 7.0, .net 8.0, and .NET Standard 2.0.

## Installation

Install-Package RxProperty

## Usage


### RxProperty

```csharp
var property = new RxProperty<int>(0);
property.Subscribe(x => Console.WriteLine(x));
property.Value = 1;
property.Value = 2;
```


