UniMediator
=======
[![BCH compliance](https://bettercodehub.com/edge/badge/tharinga/UniMediator?branch=master)](https://bettercodehub.com/)

UniMediator is a Unity package that enables you to easily use the Mediator pattern in Unity. This library is inspired by MediatR and works in a very similar way. If you've used MediatR before you already know how to use UniMediator.

## What problem does UniMediator solve?

UniMediator allows your MonoBehaviours to invoke methods on each other without having a dependency on or a reference to the objects it is invoking those methods on. Target methods are resolved based on their method signature and do not need to explicitly subscribe or unsubscribe.

## Installation

This repository follows Unity's package format. To add UniMediator to your Unity project, locate your Project manifest (called ```manifest.json```, in the ```Packages``` folder under the root folder of your Unity Project) and add this repository as a dependency as below:

```json
{
  "dependencies": {
    "com.tharinga.unimediator": "https://github.com/tharinga/UniMediator.git"
  }
}
```
The package should automatically be installed and be visible in the Package Manager window.

## Usage

Add UniMediator to your scene by clicking ```Window > UniMEdiator > Install```

You only need to add UniMediator to the scene that is the entry point to your game, however adding it to multiple scenes will not cause any problems.

UniMediator supports two types of messages:

1. **IMulticastMessage** targets 0 or many handler methods with a void return type. Listeners for this type of message would typically be objects with a transient life cycle.

2. **ISingleMessage\<T\>** targets 1 and exactly 1 handler method with a return type of T. Listeners for this type of message would typically be services with a Singleton life cycle. 

### Using IMulticastMessage

An ```IMulticastMessage``` is created as follows:
```csharp
public class ExampleMessage : IMulticastMessage  
{  
    public int Value { get; set; }  
  
    public ExampleMessage(int value)  
    {  
        Value = value;  
    }  
}
```
A handler for this message is created as follows:

```csharp
public class ExampleHandler : MonoBehaviour,   
    IMulticastMessageHandler<ExampleMessage>  
{  
    public void Handle(ExampleMessage message)  
    {  
        Debug.Log(message.Value);  
    }  
}
```

You do not need to register the ```Handle``` method with UniMediator, it is resolved automatically.

You can now dispatch messages from anywhere in your project by creating a message object and dispatching it through the mediator as follows:

```csharp
var message = new ExampleMessage(123);
Mediator.Publish(message);
``` 
That's it, no further configuration is required. UniMediator will automatically unsubscribe the handler if the GameObject it is attached to is destroyed.
 


### Using ISingleMessage\<T\>

An ```ISingleMessage<T>``` is created as follows:
```csharp
public class ExampleMessage : ISingleMessage<string>  
{  
    public string Message { get; set; }  
  
    public ExampleMessage(string message)  
    {  
        Message = message;  
    }  
}
```
A handler for this message is created as follows:

```csharp
public class ExampleMessageHandler : MonoBehaviour,   
    ISingleMessageHandler<SingleMessage, string>  
{  
    public string Handle(ExampleMessage message)  
    {  
        return message.Message + "Pong";  
    }  
}
```
You can now dispatch messages and get a return value from anywhere in your project by creating a message object and dispatching it through the mediator as follows:

```csharp
var message = new ExampleMessage("Ping");
var result = Mediator.Send(message);

Debug.Log(result); // prints "PingPong"
``` 
Note that there must be 1 and only 1 handler for each ```ISingleMessage<T>```. Because returning value types is supported, there is no sane default value to return when no listener exists, so a ```UniMediatorException``` will be thrown if there is no handler in the scene. 

### Keeping your objects humble

The examples above use a static convenience method. If you want to keep your objects humble you can instead inject the ```IMediator``` interface. 

For example if you use Zenject you can bind UniMediator as follows:

```csharp
Container.Bind<IMediator>()
    .To<MediatorImpl>()
    .FromComponentInHierarchy()
    .AsSingle();
```
And you would then inject UniMediator as follows:

```csharp
private IMediator _mediator;

[Inject]
public void Construct(IMediator mediator)
{
    _mediator = mediator;    
}
```
