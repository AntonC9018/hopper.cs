<!-- TOC -->

- [Overview](#overview)
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

<!-- /TOC -->


# Overview

This is the C# rework of [my previous project](https://github.com/AntonC9018/Dungeon-Hopper), which was programmed with lua.

This particular repo is just the `Model` part of the project.

The documentation does not exist, although some general concepts like Chains, Tinkers, Retouchers and Decorators (called Behaviors in this code) as well as some others, which all have been described in the [docs for the prior version](https://antonc9018.github.io/Dungeon-Hopper-Docs/), are present in this version too, although in a somewhat different form.

The file `main.cs` contains some demo code. You may look through it to assess the capabilities of the system.

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
