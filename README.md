<!-- TOC -->

- [Overview](#overview)
  - [WARNING](#warning)
  - [View](#view)
- [Introduction](#introduction)
  - [Style](#style)
  - [The architectural structure](#the-architectural-structure)
  - [Entities](#entities)
  - [Layers](#layers)
  - [World events](#world-events)
  - [Game loop](#game-loop)
  - [Communication between `Model` and `Controller`](#communication-between-model-and-controller)
  - [Chains](#chains)
  - [Behaviors](#behaviors)
  - [Entity factory](#entity-factory)
  - [Retouchers and Tinkers](#retouchers-and-tinkers)
- [Documentation](#documentation)
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

The file `main.cs` contains some demo code. You may look through it to assess the capabilities of the system.

## WARNING

Most of the stuff below is **out-of-date**! 
The project has been massively refactored lately, and most of the things described below have become irrelevant.
Likewise, all of the new features are yet to be documented.

## View

There is one WIP `View` I'm currently working on, see [this](https://github.com/AntonC9018/hopper-godot).

There also an implementation based on unity (currently broken), see [this repo](https://github.com/AntonC9018/hopper-unity).

# Introduction

So, if you wish to contibute to the project, there are a couple of things I need to explain before you can start. 

1. The overall architectural structure of the program (`Model` - `Controller` - `View`). 

2. How does the logical game loop work? What are world events? What are `Entities` and `Layers`?

3. How `Model` and `Controller` communicate between each other.

4. How are Entities created? What are `Chains`, `Behaviors` and `Retouchers`? How to set up a custom Entity Factory?

5. What are `Tinkers` and how are they used in code.

6. What are `Stats` and stat files? How do stat paths work?

7. What is `Kind`, `Instance` and `Patch`? What are `Registry`, `ModRegistry`, `KindRegistry`, `PatchArea`,`PatchSubArea` and `InstanceRegistry`?

I will try to describe the points from above as concise as possible. For more details and examples, look into the actual code or into tests.

## Style

Before we start, I would like to quickly mention some points concerning the style guide. 

1. I'm using 4 spaces for indentation. 

2. Local variables are typically `camelCase`, although sometimes I would use `snake_case`, especially for long blocks of intense linear code and especially if that code is for an algorithm or a more complex data structure.

3. All static fields, especially readonly ones, should be in `PascalCase`, although I'm not sure about whether this convention is good or not so good. The motivation is that it makes them more visible among local variables in code. For example:
```cs
// I would do this
public static readonly SomeField = something;
// instead of this
public static readonly someField = something;
// or instead of this
public static readonly SOME_FIELD = something;
```

4. Member fields are prefixed with `m_` and are in `camelCase`, e.g. `m_someVariable`. If you're coding a new class and aren't sure about whether a particular field should be hidden behind a property and made private, don't worry about it too much. Make them public until you have enough information and understanding to decide what exactly they should be. You might need to factor them in an interface, in which case you will need a property, or you may need them to be public to be able to test things. In any case, if you're not sure, just let them stay public.

5. Avoid properties on structs and so-called data classes. If it's a struct or a data class, all members should be public. Also, the convention is to *not* have an `m_` before field names of such a class. You should name them simply in `camelCase` (or `snake_case`, if you use this class in cases described in bullet 2). For example, an option class would have all field public and in `camelCase`.

6. Methods, classes and properties should be in `PascalCase`. 

7. Folders should be named with capital letters, according to namespaces, classes or the category of items they contain. However, if there are multiple words in a name of a namespace or folder, consider inserting an underscore in between the words for readability. For example, `Test_Namespace` instead of `TestNamespace` and same for folders. Also, avoid having a class `Foo` inside a namespace with the same name, since if you try to reference `Foo` having included the namespace `Foo`, the compiler would think that you mean the namespace `Foo`, not the class.

## The architectural structure

The project follows the MVC or MVVM pattern, which means that it consists of 3 independent components. 

1. The `Model` (`Logic`, `Core` or `Engine`) is responsible for logic and the actual state of the world, knowing nothing about graphics.

2. The `Controller` (`View_Controller` or `View_Model`) is responsible for monitoring the `Model` though events and registering and processing the changes in the game state. It also links every `Logical Entity` (`Logent`) from the `Model` to a `Scene Entity` (`Scent`). A `Scene Entity` represents an actual object on the screen. The `Controller` instantiates `Scents` for new `Logents` using, perhaps confusingly called so, `Models` for this. `Models` are objects that can create new `Scents`. It then uses the `Animator` that uses a timer to correctly synchronize animations of `Logents` to phases (discussed later). Which animations to play is determines by `Sieves`, which do this by looking at which events happened during a game loop and perhaps in what sequence. 

3. The `View`, which provides a concrete implementation for `Models`, `Scents` and `Sieves`. 


## Entities

`Entities` are objects that occupy some location in the world and affect the game logic. For example, the *player* is an entity, *enemies* are entities, as well as *walls, traps, items, environmental objects* and *special floor tiles*. 

Entities that are not controlled by player are called `Non-player Entities`.

There is also a special type of entities, `Reals`, which I cannot define rigorously. Informally, they are like *tangible* entities. For example, *entities, players and environmental objects* (like chests or crates) would be considered reals. *Traps* and *items* are not considered reals. Note, that *walls* technically follow this informal definition, but they have a special type of wall.  

In contrast, e.g. *simple floor tiles* that do not affect the logic, are not considered an entity, even though they have a particular position in the world.

There are also *particles*, which are objects created by specific events in the world that do not affect the logic. For example, the *smoke* after an explosion. *Simple floor tiles* from the above example are also particles from the perspective of the logic code. 

## Layers

`Layers` are attributed to entities. Layers identify, informally, which level of a cell the entity is at. For example, there is the `REAL` layer, which is designed for reals, the `WALL` layer for walls and so on (see Core/World/Cell). 

Specific actions may target specific layers. For example, by default, the `sliding` action targets only the `REAL` layer.

## World events

World events are C# events that get invoked once some event happens.

There are basic events defined by world, which have to do with *phase change*, *start and end of loop* and *spawning of entities*. 

However, other code may create new events using the `WorldEvent` class. There events will not be invoked by the world itself, but by the component that initialized it. However, they are still stored in the world. For example, the aforementioned *explosion event* does not get invoked by the world, it is managed by the *Explosion* class.

## Game loop

The `World` keeps track of all `Entities` (`WorldStateManager`) and also keeps them inside a 2d `Grid` (`GridManager`). 

The updates in the game state happen by turns. They are initiated e.g. by user input. The logic that happens during such turn is called **game loop** (the term turn is actually never used; we use the term game loop to mean that). During the game loop, the following things happen:

1. `Players` **execute** their action. To be clear, player actions are set beforehand, the world is not responsible for that. The `View` should provide an engine-specific input manager that sets up the action based on user input.

2. `Non-player Entities` **calculate** their next action.

3. `Non-player Entities` **execute** their action in an order based on the layer they're part of. The exact order follows the values layers have in the `Layer` enum (ascending). Each layer corresponds to exactly one `Phase`. The variable that indicates the current phase is incremented together with the layer being processed. This information is used mainly by the `View_Controller`.

4. All entities are **ticked**.

5. **Dead** entities are removed.

The world does not run the game loop by itself at any specific intervals, neither would it run all by itself if the player provided an input. The game loop is started on demand by calling the `Loop()` method. In this sense, the `Core` code is a library, not an executable.

## Communication between `Model` and `Controller`

Entities use History objects to track individual changes that they record during the game loop. For example, there are `attacking_do`, `attacked_do`, `move_do` etc. update codes (names subject to change).

The controller subscribes to the end of loop world event beforehand. When the game loop ends, the end of loop event is invoked as well, whereby the controller gets notified about the updates in the world. It gathers the individual histories of all entities, inspects and processes them.

There are also specific world events that are of interest to the controller. Specifically those that result in *particles* being created. Since the e.g. *explosion event* does not have either an associated update or an associated entity, it does not get passed to the controller with the history objects. So for this reason the cotroller has to subscribe to such an event separately. The controller makes use of special objects that *watch* over a particular world event, thereby these objects got the name `Watchers`.


## Chains

`Chains` is a term used for a modified form of a responsability chain. The chains in my code are objects that hold a list of handlers that you can *pass*, given an event (an object with data that you pass to the handlers). See tests for examples.

Chains are mainly used in dynamic contexts here. Note that the handlers may be added or removed at any time. Also, each handler is associated a *priority*.

However, barebone chains are rarely used in the code. There are helper constructs that help you work with chains a little bit more rigorously. There are `Chain Templates` that help you build and instantiate a chain (they are basically chain factories).

`Behaviors` use chains very extensively.

## Behaviors

`Behaviors` are components of an entity that grant them an associated behavior, ability or property. For example, by default, no entity can be attacked. For it to be able to take hits, it must have an `Attackable` behavior. There are behaviors for taking damage — `Damageable`, being able to attack — `Attacking`, being able to be displaced — `Displaceable`, being able to voluntarily move — `Moving` and so on. These are among the basic ones, specified by Core. You may specify your own.

Behaviors are cool because they are very dynamic, making use of chains. Let's look at the `Attacking` behavior. It provides two chains: `Attacking.Check` and `Attacking.Do`. The first one gets traversed before the second one. The handlers of this chain figure out whether an attack should succeed or fail. They find and set potential targets, without trying to attack them, and get the needed attack stat, saving it on the chain event. Then the `Attacking.Do` chain is traversed. It actually applies the attack to the targets. The attack is considered to be successful if `Attacking.Do` chain was reached. 

The above procedure is called a `CheckDoCycle`. What does this have to do with being dynamic? The thing is, some other code may insert a handler to the chains of any behavior of this entity, that would, for example, substitute different targets, or make the attack fail. This way, the default behavior, described above, is not set in stone, which is really useful.

## Entity factory

Although technically possible, adding behaviors to an already existent entity is not allowed.

Entity `Kinds` with specific behavior are defined before their initialization by the use of an `EntityFactory`. The Entity Factory provides a fluent interface for adding behaviors to an entity kind and after that getting an instance of that entity.

## Retouchers and Tinkers

`Retouchers` are objects that help you *retouch* an entity behavior before its initialization. *Retouching* means adding one or more handlers to one or more chains of a specific behavior. For example, by default, the attack would be considered successful even if the list of targets was empty. By applying the `Skip.EmptyAttack` retoucher to the entity factory, this is prevented for all future instances created by this factory — an attack with no targets will fail.

In contrast, `Tinkers` *tink* the chains of an already instantiated entity. *Tinking* means the same as *retouching*, but with an entity instance, rather than factory. Tinkers are usually employed by items and status effects to temporarily grant a specific ability to the player.


# Documentation

This section describes some of the features the information on which cannot be found outside of my head currently.

## Exporting stuff 

Exporting stuff means adding certain content from both static and non-static classes.

This content is going to be automatically initialized and saved in the registry when your mod is loaded.

There are 2 types of thing you can export:
- `Chains` (events) for the `MoreChains` component or `global` chains.
- `Handlers` for any chains, including behavioral chains, more chains and global chains.


### How to export to `MoreChains`

In order to export chains, you need to define an `Index` with the type of your chain, and mark it with the `Export` attribute.

Be careful to mark it with `Hopper.Shared.Attributes.ExportAttribute` in case you're using a game engine where an attribute with this name already exists (e.g. `Godot.Export`).
The code generator matches the name semantically, not textually.

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

You may export global chains from components, tags or behaviors.


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
In the code, handlers have an explicit `identifier`, or a `priority` number, in case of `priority chains` (linear chains currently cannot be exported, which implies that all chains are priority chains, so all handlers will end up having priority as well).

You can only export static methods from static classes or classes marked with `[InstanceExport]`, or any methods from component, behaviors or tags.

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
The names and types of the parameters must match the names and parameters of the corresponding fields in the context class.
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
- a static constructor, possibly referencing any exported chain or handler wrappers.

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
    // Gets the chain via the path and adds the entity.
    // This assumes the entity has `MoreChains`.
    // This also assumes the chain does not already have the given handler.
    DoStuffHandlerWrapper.HookTo(entity);

    // Gets the chain via the path and adds the entity, 
    // if the entity has the given component 
    // and the chain does not already the handler
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
[AutoActivation] // provides chains
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

A generalization of this feature is that you may take the components as arguments to your handler functions, doesn't matter if it's static or instance (they don't have to be in component classes).

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