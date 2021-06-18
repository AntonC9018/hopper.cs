<!-- TOC -->

- [Overview](#overview)
  - [View](#view)
- [Introduction](#introduction)
- [Documentation](#documentation)
  - [Components](#components)
    - [How to register components](#how-to-register-components)
    - [How are components stored](#how-are-components-stored)
    - [Injected fields](#injected-fields)
    - [`InitInWorld`](#initinworld)
  - [Exporting stuff](#exporting-stuff)
    - [How to export to `MoreChains`](#how-to-export-to-morechains)
    - [How to export `global` chains](#how-to-export-global-chains)
    - [Defining chains in behaviors](#defining-chains-in-behaviors)
    - [How to export handlers](#how-to-export-handlers)
    - [Exporting handlers in components](#exporting-handlers-in-components)

<!-- /TOC -->


# Overview

This is the C# rework of [my previous project](https://github.com/AntonC9018/Dungeon-Hopper), which was programmed with lua.

This particular repo is just the `Model` part of the project.

The documentation does not exist, although some general concepts like Chains, Tinkers, Retouchers and Decorators (called Behaviors in this code) as well as some others, which all have been described in the [docs for the prior version](https://antonc9018.github.io/Dungeon-Hopper-Docs/), are present in this version too, although in a somewhat different form.

## View

There is one WIP `View` I'm currently working on, see [this](https://github.com/AntonC9018/hopper-godot).

There also an implementation based on unity (currently broken), see [this repo](https://github.com/AntonC9018/hopper-unity).

# Introduction

Uh-oh...

# Documentation

This section describes some of the features the information on which cannot be found outside of my head currently.


## Components

`Components` are pieces of data stored in `Entity`.
They are what define the properites of entities.


### How to register components

Make a class that *directly* implements `IComponent`.

```C#
public partial class MyComponent : IComponent
{
    public int myField;
}
```

You will see a bunch of errors pop up if you try to just compile the code.
Also, there is currently no way to add `MyComponent` to the `Entity`.
They appear because components need code to be generated for them.

Running the code generator should yield:
- An `Index` static field with the id for your component;
- An `InitIndex()` method, which registers the index in the registry;
- `GetMyComponent()` and `TryGetMyComponent()` extension methods for `Entity`;
- A parameterless constructor, initializing injected fields;
- A copy constructor, copying injected fields;
- A `Copy()` method, explicitly implementing `IComponent.Copy()`;
- Another `Copy()` method;
- An `AddTo` method taking in the injected fields, generated to simplify adding the component to an entity (or entity factory).

You have not specified any injected fields yet, so the constructors are empty.

> Note: if you have not declared at least one field, the code generator will not pick up on your component class.
> This is most likely a bug in Roslyn.

An example follows.

```C#
void Usage(Entity entity)
{
    // Instantiates a new MyComponent instance and adds it to the given entity
    var component = MyComponent.AddTo(entity);

    // Retrieves MyComponent from the entity.
    // The entity must contain the component, otherwise this results in an assertion error.
    var component = entity.GetMyComponent();
    // Or using the index.
    var component = entity.GetComponent(MyComponent.Index);

    // Returns the component if it exists on entity
    if (entity.TryGetMyComponent(out var component)) // ...
    // Or using index
    if (entity.TryGetComponent(MyComponent.Index, out var component)) // ...

    // Note that the component variable is statically typed
    // so this is valid and is statically checked.
    component.myField = 5;
}
```

### How are components stored

The components are stored in a dictionary in `Entity` instances. 
They are indexed by their `Identifiers`, known statically for each component class, and allocated by the registry.
In fact, the identifier can be retrieved after your mod has loaded via the index.

```C#
void Example()
{
    // This is the key by which the component is stored.
    Identifier id = MyComponent.Index.Id;
}
```

Because of the fact that this id is assigned to the component *class*, there may never be 2 components of the same class at once in the `Entity`, assuming you have been using `AddTo()` to add components.


### Injected fields

The idea behind having components is that they are the same both in the entity factory on the subject entity and on entity instances. 
When the factory instantiates a new entity, it copies all components from the subject.
So *creating a copy* in this case does not imply creating a runtime copy, but rather capturing the *type* of the component.
The exact definition of the *type* is controlled by *injections*.

If you need a field to be copied when the component is copied when instantiating an entity, you need to mark that field with `[Inject]`.
The field can be of any type.

```C#
using Hopper.Shared.Attributes;

public partial class MyComponent : IComponent
{
    // This field will be copied when the component is copied.
    [Inject] public int injection;

    // This field will get the default value of 5 when the component is copied.
    public int five = 5;

    // This field will be 0 when the component is copied.
    public int zero;
}
```

The generated constructor in this will require to pass in a value for `injection`:

```C#
void Example(Entity entity)
{
    // Initialize with injection = 5
    var component = new MyComponent(injection: 5);

    // Initialize and add MyComponent to the entity, passing 5 to the constructor
    var component = MyComponent.AddTo(entity, 5);

    // You can add components to the subject in factories as well
    var factory = new EntityFactory();
    var component = MyComponent.AddTo(factory, 5);
    // Equivalent to
    var component = MyComponent.AddTo(factory.subject, 5);
}
```

You may override the autogenerated constructor if you specify your own one, in which case `AddTo()` will also call to it:

```C#
public partial class MyComponent : IComponent
{
    // This field will be copied when the component is copied.
    [Inject] public int injection;

    // Must take all injected fields
    public MyComponent(int injection_)
    {
        this.injection = injection_ + 5;
    }
}
```

You may also override the copy constructor:

```C#
public partial class MyComponent : IComponent
{
    // This field will be copied when the component is copied.
    [Inject] public int injection;

    // Must take all injected fields
    public MyComponent(int injection_)
    {
        this.injection = injection_ + 5;
    }

    public MyComponent(MyComponent other)
    {
        this.injection = other.injection + 5;
    }
}
```

If the injected field implements `ICopyable`, it will be copied by calling `Copy()` instead of a direct assignment.


```C#
public class Thing : ICopyable
{
    ICopyable.Copy() => new Thing();
}

public partial class MyComponent : IComponent
{
    [Inject] public Thing thing;
}

void Test()
{
    var thing         = new Thing();
    var component     = new MyComponent(thing);
    var componentCopy = new MyComponent(component);
    
    // It was passed to component by reference
    Assert.AreSame(thing, component.thing);
    // It was copied in the copy constructor
    Assert.AreNotSame(thing, componentCopy.thing);
}
```

### `InitInWorld`

If you feel like you need an extra initialization step when the entity your component is in gets placed in the world, you may define an `InitInWorld()` member function like this:

```C#
public partial class MyComponent : IComponent
{
    // May even be private
    public void InitInWorld(Tranform transform)
    {
        // You may access the entity via the transform.
        var entity = transform.entity;
        
        // The transform contains the position and orientation of the entity.
        transform.position;
        transform.orientation;
    }
}
```

Running the code generator additionally generated an adapter for this function and an `AddInitTo()` method.

```C#
void Usage(EntityFactory factory)
{
    // AddInitTo can only be used on a factory.
    MyComponent.AddTo(factory);
    MyComponent.AddInitTo(factory);

    // Creating the entity via `SpawnEntity()` 
    // runs your `InitInWorld()` on the copied instance of the component.
    var entity = World.Global.SpawnEntity(factory, IntVector2.Zero);
}
```


## Exporting stuff 

Exporting stuff means adding certain content from both static and non-static classes.

This content is going to be automatically initialized and saved in the registry when your mod is loaded.

There are 2 types of thing you can export:
- `Chains` (events) for the `MoreChains` component or `global` chains.
- `Handlers` for any chains, including behavioral chains, more chains and global chains.


### How to export to `MoreChains`

In order to export chains, you need to define an `Index` with the type of your chain, and mark it with the `Chain` attribute.

```C#
using Hopper.Core;
using Hopper.Shared.Attributes;

public static partial class Thing
{
    // Export a MoreChains chain.
    // Note the `+` in front.
    [Chain("+ChainName")] public static readonly Index<Chain<Context>> ChainNameIndex
        // Currently, you also must immediately initialize it.
        // It is pretty tough to make it be done automatically by the code generator. 
        = new Index<Chain<Context>>();
}
```

The context of a `MoreChains` chain can be any valid context, which means it must have a field or property with name `actor` of type `Entity`. Otherwise you'll get a code generation error.

The chain name can be anything you like and it does not have to coincide with the prefix before the `Index` part. 
You may name the chain index the same name as the chain.

The name given to the chain passed in the `Chain` attribute constructor must start with a `+` at the front, instructing the chain to be exported for the `MoreChains` component. 
The name of the chain must be a valid `C#` token name. 
In case it is not, you will get errors in the generated code, as opposed to warnings at generation time. 

Generating code for the example code above yields:
- a static field `ChainNamePath` of type `MoreChainsPath<Chain<Context>>`;
- a static field `ChainNameDefault` of type `Chain<Context>`;
- a static constructor for the two fields;
- some init methods called in the main init method.

If more than one chain is exported, static fields will be created for each of the exported chains and the static constructor and the init method will reference these as well.

Since the generated code includes a static constructor, you must not include one yourself.
Also, you may not reference any of the generated fields while initializing other static fields of your class on declaration, because the static constructor is run after declaration initializers and so they will end up referencing nulls.
If you need to reference the generated things in a static context, you will need to do it in a separate static class in order to work around this problem.

```C#
using Hopper.Core;
using Hopper.Shared.Attributes;

public static partial class Thing
{
    [Chain("+ChainName")] public static readonly Index<Chain<Context>> ChainNameIndex = new Index<Chain<Context>>();

    public static readonly Something AnotherThing 
        = new Something(ChainNamePath); // ChainNamePath is null, don't do that!

    static Thing() // a static constructor already defined in the generated code, error!
    {
    }

    public static void DoSomeStuff()
    {
        var a = ChainNamePath; // Ok, static functions run after complete initialization.
    } 
}

// Instead do
public static partial class ThingData
{
    public static readonly Something AnotherThing 
        = new Something(Thing.ChainNamePath); // Ok, thing will be initialized completely first.
}
```

With this in place, you can get the chain you have defined from any entity at runtime.

```C#
public void GetChainAndDoStuff(Entity entity)
{
    // Returns the chain from `MoreChains` component of the entity.
    // Returns `null` if the entity does not have `MoreChains`.
    var chain = Thing.ChainNamePath.Get(entity);

    // Returns the chain from `MoreChains` component, 
    // but only if it has been lazy loaded or will have handlers.
    // Otherwise, it will returns null.
    Thing.ChainNamePath.GetIfExists(entity)
        // A common use-case is to `Pass` the chain at this point.
        ?.Pass(context);

    // Same as ChainNamePath.Get()
    var chain = entity.GetMoreChains().GetLazy(Thing.ChainNameIndex);
}
```

No 2 classes with the same name from distinct namespaces/mods can export things. 
The code generator will report an error if it encounters one.  
This is done this way, because referencing chains in exported handlers is done with the syntax `ClassName.ExportedChainName` (with a `+` or a `g` in front depending on the contribution type). 

If you decide to export code from a non-static class, you must mark it with `[InstanceExport]`, otherwise the code generator will ignore it:
```C#
[InstanceExport]
public partial class Thing
{
    // export as usual.
}
```

You may export to `MoreChains` from components, tags or behaviors.


### How to export `global` chains

The idea is pretty similar to the `MoreChains` version, except a `g` is used instead of the `+`.

```C#
using Hopper.Core;
using Hopper.Shared.Attributes;

public static partial class Thing
{
    // Export a global chain.
    // Note the `g` in front.
    [Chain("gChainName")] public static readonly Index<Chain<Context>> ChainNameIndex
        // Currently, you also must immediately initialize it.
        = new Index<Chain<Context>>();
}
```

All things from the section on `MoreChains` also apply. 
The only difference is that the chain may take any type, e.g. a vector.
That is, `Chain<IntVector2>` is valid for global chains, but not for `MoreChains`.

```C#
public void Usage()
{
    // Get the chain from the global world.
    var chain = Thing.ChainNamePath.Get();

    // Currently, the global world is always the one referenced.
    // So, currently, the above is equivalent to the following.
    var chain = Thing.ChainNamePath.Get(World.Global);

    // If you have a reference to a particular world, though,
    // you may use it like this:
    var chain = Thing.ChainNamePath.Get(world);

    // This is equivalent to the following.
    // `world.Chains` in this case is of type `MoreChains`.
    // Yep, they are implemented via the same code.
    var chain = world.Chains.GetLazy(Thing.ChainNameIndex);
}
```

You may export global chains from components, tags or behaviors, in which case you do not need `[InstanceExport]`.


### Defining chains in behaviors

If your component depends on certain chains from `MoreChains` defined by you, you may consider turning it into a behavior and having those chains be instance fields of the behavior, instead of being lazy loaded from the template in `MoreChains`.
This is especially true if your component uses a couple of chains at once, so there is no point in lazy loading them.

```C#
// Note the interface `IBehavior`.
// The compiler errors will go away once the code is generated, so ignore them.
public partial class MyBehavior : IBehavior 
{
    // Define the chain itself, without initialization.
    [Chain("Before")] public readonly Chain<Context> _BeforeChain; 

    // The name of the variable may be anything valid.
    [Chain("After")]  public readonly Chain<OtherContext> _NameIsDifferent; 
}
```

Note how the string passed into the attribute constructor do not have a prefix.
The context must also be a valid context, with an `actor` property or field.

The generated code will include:
- a static readonly field `ChainNamePath` of type `BehaviorChainPath<Chain<Context>>`;
- initialization of chains to empty chains in the autogenerated implicit constructor;
- correct copying of chains in the autogenerated copy constructor.

Defining chains in behaviors has a number of benefits, allowing for a number of syntactical shortcuts.
You'll see these in the next section.

```C#
public void Usage(Entity entity)
{
    // Returns the chain from MyBehavior if entity has MyBehavior.
    // Otherwise, returns null.
    var chain = MyBehavior.ChainNamePath.Get(entity);

    // This is equivalent to saying:
    if (entity.TryGetComponent(MyBehavior.Index, out var beh))
    {
        var chain = beh._ChainName;
    }

    // Since the chain will always be present on the behavior, there is no
    // `IfExists` version of `Get`.
}
```


### How to export handlers

`Handlers` are functions which can be put in chains.
In the code, handlers have an explicit `identifier`, or a `priority` number, in case of `priority chains` (linear chains currently cannot be exported, which implies that all chains are priority chains, so all handlers will end up having a priority as well).


In order to export handler functions, mark them with `[Export]`.
Be careful to mark it with `Hopper.Shared.Attributes.ExportAttribute` in case you're using a game engine where an attribute with this name already exists (e.g. `Godot.Export`).
The code generator matches the name semantically, not textually.

You can only export static methods from static classes or classes marked with `[InstanceExport]`, or any methods from components, behaviors or tags.

```C#
public static partial class Test
{
    // Export the static method for `Displaceable.Check` chain,
    // Use the default priority rank. 
    // The method may be private or public.
    [Export(Chain = "Displaceable.Check")]
    private static void DoStuff(Displaceable.Context context)
    {
        // do stuff with the context
    }

    // Export with a `high` priority
    [Export(Chain = "Displaceable.Check", Priority = PriorityRank.High)]
    private static void DoStuff2(Displaceable.Context context)
    {
        // do stuff with the context
    }
} 
```

Instead of taking in the entire context, you may take any of its fields as parameters instead.
The names and types of the parameters must match the names and types of the corresponding fields in the context class.
The method will be exported correctly via an autogenerated adapter method.

```C#
public static partial class ChainExporter
{
    // Define a context class
    public class Context
    {
        public Entity actor;
        public int hello;
        public int world;
    }

    // Export a chain for a test
    [Chain("+Example")] public static readonly Index<Chain<Context>> ExampleIndex = new Index<Chain<Context>>();
}

public static partial class HandlerExporter
{
    // Export the static method for the `Example` MoreChains chain defined above.
    // Take the whole context as the only argument.
    [Export(Chain = "+ChainExporter.Example")]
    public static void DoStuff(ChainExporter.Context context)
    {
        // do stuff with the context
    }

    // The AUTOGENERATED adapter is identical to the function in this case
    public static void DoStuffAdapter(ChainExporter.Context context)
    {
        DoStuff(context);
    } 


    // Take just hello and world
    [Export(Chain = "+ChainExporter.Example")]
    public static void DoStuff(int hello, int world)
    {
        // do stuff with the numbers
    }

    // The AUTOGENERATED adapter will access those fields.
    public static void DoStuffAdapter(ChainExporter.Context context)
    {
        var _hello = context.hello;
        var _world = context.world;
        DoStuff(_hello, _world);
    } 


    // Take hello by ref and world as out.
    [Export(Chain = "+ChainExporter.Example")]
    public static void DoStuff(ref int hello, out int world)
    {
        // change the numbers in the context
        hello = 5;
        world = 6;
    }
    
    // The AUTOGENERATED adapter will pass them with correct modifiers.
    public static void DoStuffAdapter(ChainExporter.Context context)
    {
        DoStuff(ref context.hello, out context.world);
    } 


    // Taking the context and some of the values is also possible.
    // But what's the point?
    [Export(Chain = "+ChainExporter.Example")]
    public static void DoStuff(ChainExporter.Context context, ref int hello, out int world)
    {
        // change the numbers in the context
        hello = 5;
        world = 6;

        // context.hello == 5;
        // context.world == 6;
    }
}
```

The generated code will include:
- a static function `HandlerNameAdapter` actually used in the handler;
- a static field `HandlerNameHandler` of type `Handler<Context>`;
- an init function that initializes all handlers in the registry;
- an optional `HandlerNameHandlerWrapper` (not for global chains);
- a static constructor, possibly referencing the exported chains and the handler wrappers.

In order to simplify attaching individual handlers, the `Dynamic` option may be used to generate a handler wrapper. 
It encapsulates the handler and the chain path.

```C#
// without `Dynamic`
[Export(Chain = "+ChainExporter.Example")]
public static void DoStuff(ChainExporter.Context context)
{
    // do stuff with the context
}

public void AddDoStuffExample(Entity entity)
{
    // Error prone, since the component + chain must match.
    // Although you will get a compile-time error if the context does not match.
    ChainExporter.ExampleChainPath.Get(entity).Add(DoStuffHandler);
}


// with `Dynamic = true`
[Export(Chain = "+ChainExporter.Example", Dynamic = true)]
public static void DoStuff(ChainExporter.Context context)
{
    // do stuff with the context
}

public void AddDoStuffExample(Entity entity)
{
    // Gets the chain via the path and adds the handler.
    // This assumes the entity has `MoreChains`.
    // This also assumes the chain does not already have the given handler.
    DoStuffHandlerWrapper.HookTo(entity);

    // Gets the chain via the path and adds the handler, 
    // if the entity has the given component 
    // and the chain does not already have the handler
    DoStuffHandlerWrapper.TryHookTo(entity);

    // Similarly, removing.
    // This assumes entity has both the chain and the handler.
    DoStuffHandlerWrapper.UnhookFrom(entity);

    // This guarantees the handler will be removed if it exists.
    DoStuffHandlerWrapper.TryUnhookFrom(entity);
}
```

Counted handlers are not currently supported, but they might be in the future.

If the context has a setter for the `Propagate` property, returning bool from a handler will set that property:

```C#
public class MyContext 
{ 
    public bool Propagate { get; set; }
}

public static class Thing
{
    [Export(Chain = "Some chain which uses MyContext")]
    public static bool DoStuff()
    {
        // Stop propagation.
        // The propagation will be stopped if the chain 
        // is being passed with `PassWithPropagationChecking()`. 
        return false;
    }

    // AUTOGENERATED adapter
    public static void DoStuffAdapter(MyContext context)
    {
        context.Propagate = DoStuff();
    }
}
```

### Exporting handlers in components

For behaviors which make use of autoactivation, there is a shorthand for specifying the name of the chain to which to export the handler, while the use of `Dynamic` is disallowed since it would be ambiguous.

```C#
[AutoActivation] // provides chains `Check` and `Do`
public partial class Thing : IBehavior
{
    public class Context
    {
        public Entity actor;
    }

    // Export for the locally created chain (either Check or Do)
    [Export] public static void DoStuff(Context actor)
    {
        // Do stuff with the context
    }

    // Setting the priority is allowed
    [Export(Priority = PriorityRank.High)] 
    public static void DoStuff(Context actor)
    {
        // Do stuff with the context
    }
}
```

Another useful feature that can be used for components, is that the handlers can be instance methods. 
The generated adapters would retrieve the component, which defined the method, using the actor instance from the context, get the component, and call the method.
All other features, like taking arguments by names, generating a wrapper with `Dynamic` or specifying the priority still work with this.

```C#
public partial class MyComponent : IComponent
{
    private int localData;

    [Export(Chain = "some chain")]
    public void DoStuff()
    {
        localData = 5;
    }

    // AUTOGENERATED adapter
    public static void DoStuffAdapter(Context context)
    {
        context.actor.GetMyComponent().DoStuff();
    }
}
```

A generalization of this feature is that you may take components as arguments to your handler functions, doesn't matter if the method is static or instance (they don't have to be in component classes).

```C#
public static partial class Test
{
    // Export with a `high` priority
    [Export(Chain = "some chain")]
    private static void DoStuff(Displaceable displaceable)
    {
        displaceable.Activate(/*...*/);
    }

    // AUTOGENERATED adapter:
    public static void DoStuffAdapter(Context context)
    {
        // Actor must contain the referenced behavior, it's a crash otherwise.
        var _displaceable = context.actor.GetDisplaceable();
        DoStuff(_displaceable);
    }
} 
```

An extra generalization of this feature is that you can take components from any other entity fields by naming the parameters `entityNameInContext_ComponentName`. 
I don't actually use this anywhere in the code, since you need more control most of the time and end up using `TryGetComponent()` instead.

```C#
public class MyContext
{
    public Entity actor;

    // context has another entity.
    public Entity anotherEntity;
}

public static partial class Test
{
    // Export with a `high` priority
    [Export(Chain = "some chain")]
    private static void DoStuff(Displaceable displaceable, Displaceable anotherEntity_Displaceable)
    {
        // refers to actor.Displaceable
        Displaceable.Activate(/*...*/);
        // referers to anotherEntity.Displaceable
        anotherEntity_Displaceable.Activate(/*...*/);
    }

    // AUTOGENERATED adapter:
    public static void DoStuffAdapter(Context context)
    {
        // Actor must contain the referenced behavior, it's a crash otherwise.
        var _displaceable = context.actor.GetDisplaceable();
        var _anotherEntity_Displaceable = context._anotherEntity.GetDisplaceable();
        DoStuff(_displaceable, _anotherEntity);
    }
} 
```
