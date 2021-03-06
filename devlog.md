<!-- TOC -->

    - [Some stuff](#some-stuff)
- [Registry](#registry)
  - [Update](#update)
  - [Solution](#solution)
  - [New stuff, 2021](#new-stuff-2021)
  - [Newer stuff, April 2021](#newer-stuff-april-2021)
    - [Component Copying](#component-copying)
    - [What to do next](#what-to-do-next)
    - [Registry](#registry-1)
    - [World](#world)
    - [Entity type — registry interaction](#entity-type--registry-interaction)
    - [Stats](#stats)
    - [Targeting](#targeting)
    - [Items](#items)
    - [Statuses](#statuses)
    - [Fixes](#fixes)
    - [Shield](#shield)
    - [Problem with Items](#problem-with-items)
    - [Handler update](#handler-update)
    - [Items update](#items-update)
    - [Entity modifiers](#entity-modifiers)
    - [Bouncing](#bouncing)
    - [Managing modifiers](#managing-modifiers)
    - [Changing the context](#changing-the-context)
    - [Bug with stats](#bug-with-stats)
    - [More Chains (aka events)](#more-chains-aka-events)
      - [The Plan](#the-plan)
- [Proper project setup](#proper-project-setup)
    - [My current setup](#my-current-setup)
    - [What I want to achieve](#what-i-want-to-achieve)
    - [Building into one directory?](#building-into-one-directory)
    - [Why not run tests through your IDE?](#why-not-run-tests-through-your-ide)
    - [How can you help me](#how-can-you-help-me)
    - [Actual code of the current set up](#actual-code-of-the-current-set-up)
- [More ideas](#more-ideas)
- [Entity Type IR (sort of)](#entity-type-ir-sort-of)

<!-- /TOC -->

## Some stuff

1. (+) Events like bombs should be on world, accessible via paths like I have done before with stats and behaviors.
2. I don't like the difference in the way we handle e.g. bomb explosions vs normal entity actions, i.e. history.
3. (+) The target logic is overgeneralized. It shall be simplified to a less generic thing, since the current code is too hard to understand, let alone reuse.
4. (+-) The registering of game content is global. It should somehow be scoped for the ease of testing. My idea is to at least use a **default registry**. (done). There is still a problem with the sort of way we register things in the registry. It feels bad, because we have direct addressing to a global component in e.g. Tinker and Status constructors, which makes them less stable and you wouldn't expect the global state to change just because a class has been instantiated. I will need to figure out a way to separate this. Either using a mediator or registering **kinds** (this is what I decided to call instances of classes registered at setup phase, e.g. tinkers and statuses)outside. The second option feels alright, but will require a bunch more boilerplate, which can be potentially eliminated by use of global helper functions contained in a class. 
5. (+-) Clearly define the function of the ViewModel and the Model. 
6. Move the logic of testing whether a block is in the way into the displaceable behavior, and parametrize the targeted layers (e.g. potentially let the person go through walls).
7. Think about what needs to be done to allow sizeful entities.
    - The displacement logic is contained within the function `Entity.ResetPosInGrid()` which takes the person out of the grid, then puts it back and finally updates the position. The problem with that is the *enter/leave listeners* on cells. They either should not react to sizeful entities, or be frozen until the entity fully moves and then fired, one of each type (it becomes much more complicated).
    - The other displacement logic will also be affected. This means either a separate chain stack for default displacement logic for `Displaceable` behavior, or abstracting the vector type into some *vector group*, which will be required for testing whether there is no block for all of the spaces of the entity.
    - The weapon logic / targeting logic will also need an update, providing default handling for them. I mean if you have a dagger on a 2x2 entity, it should attack 2 things at once. (or one thing, but targeting 2 cells)
    - ...  
8. Separate positive and negative effects / modifiers for stats / certain stats into corresponding groups to allow bulk tinkering / removal / updating. Especially useful for stats of resistance e.g. to **magic** or e.g. a composite **flight** stat.
9. Not pushing traps if flying (one mechanism of push source resistance is not enough).
10. Better HP system. HP pickups. It could even be decentralized, since serialization is easy.
11. Finally do serialization + saves + server. This has lowest priority.
12. Aggro system + Lighting system.
13. *Content* (release on death) vs *State change*. I reckon sometimes a change in behavior will require completely different stats and chain stack as well as a different sequence object. Two different state machines are always easier to manage. 
14. (+)Change direction inputs to one Vector input. 

# Registry

So I have had this idea lately, that would be kind of nice.

Assume you want to load a specific combination of mods. Each mod defines some new content (assume a consistent order in which the content is initialized). Also assume that no circular dependencies exist for mods. Following my terminology, the *content* of a mod is defined as a collection of *kinds*, which are sort of like types, that each have a unique id among their category. E.g. tinker instances are kinds, because each individual tinker has an associated id and they are all of the common type `Tinker`.

In the current code, once a kind has been created, e.g. a new instance of tinker or some other type of a kind has been instantiated, it will get a unique global id, which would be saved in the *default global registry*. Now if we assume that the content of a mod is statically constructed, i.e. the kinds are saved as static fields on some classes defined by the mod, the problem becomes that it cannot be reinitialized later. Once a mod has been loaded, the ids cannot be reassigned and the kinds cannot be recreated. This means that mod unloading and reloading is inconsistent.

For example, you had a mod `A` that defined 4 tinkers, which received id's from 0 thru 3. Now you load a new mod, named `B`, which itself defines 4 tinkers. They get id's from 4 thru 7. Now you wish to unload the mod `A`. Since there is no mechanism for neither reloading nor id patching, the tinkers from the mod `B` will still have their previous id's. This is bad, because, although we have just one mod, mod `B`, if we have had only `B` in the first place, the id's would have been different, because the tinkers would have had the id's from 0 thru 3 instead. This is called an inconsistency.   

So the simplest way to remedy this issue is for every mod to define the *list of content* it creates and an *init function*, which doesn't touch the global state in any way, so that it may be run again. This function would receive a registry which is to take the kinds defined by the mod. Then assume we had mods `A` and `B`. Now, if we wish to unload `A`, we would clear the registry and then redefine `B` using its init function.

This would also eliminate the problems with serialization (saves and server). Assume you had a save that was played with mods `A` and `B`. Now, if the order in which `A` and `B` were defined were inconsistent, or we had another mod besides these two being ignored (don't know what it would look like, actually), the id's might get messed up in that after the reload of the game the content from the two mods would get assigned other id's from the ones stored in the save file and so it would load incorrectly. Now the story with the server is that assume the player has either additional mods enabled than the server, or has the id's of the kinds not match the id's of the corresponding kinds at the server. In this case, if we wanted to send info that concerns a kind from server to client, we would have to keep track of the mapping server <-> client, which, I think, is complicated. Instead, when connecting to a server, check if the mods match, and if they don't, unload all the mods and reload just the ones that the server requires. Of course, this shouldn't affect graphics only mods, e.g. those that change the default textures.

This approach would require the mods to explicitly define all their content and list their dependencies.

## Update

In some sense the mapping client <-> server was in a way easier for the actual content implementation since it would eliminate a level of indirectness at the client level. There is however a problem with that aproach too. A valid solution, though, would be to delegate this problem to the serializer/server/whatever implementation (in a way, because there are caveats).

The things that we need to understand is that the content can be of different types.

1. *Static Immutable Content*. This content is not dependent on other non-static content in any way. The logic part. These are *item* 'singletons' (not necessarily one class), most *retouchers* and *tinkers*. The thing to understand here is that **their logic does not require a relation to the registry to function properly**, disregarding for a moment the fact that, for example, for tinkers to be identified, it is necessary for them to have a unique per-registry id.

2. *(Kind) Registry Content*. These are those same tinkers and retouchers, in that their logic does not depend on registry, the difference being that they reference kinds (singletons) that exist in per-registry fashion in that they are instantiated anew per registry. They are to be described below. There are 4 ways to try to solve this reference problem. Some of them involve modifying the registry in a way:

    - First, give the *registry content* a unique global id. Now, when the patching phase of the registry begins, save a reference to the needed object in a map at that registry. **The downside** is that it would need a global id and this patching where a reference or the actual id is saved should be done explicitly. **I do not like this approach**. It is probably the buggiest solution of all.

    - Actually have this *registry content* type of content be a kind, that is, have it be registered together with the kind it requires. I'm currently doing this in some places. But this requires creating closures which is just mind-bending at times. The code is just harder to understand in this case.

    - For the mod, explicitly list all of its content, as described above. Then this *registry content* would use the registry at runtime to get to the needed mod and then get the needed kind from that content definition. I'm currently doing this in most cases. The problem with this approach is that, for first, the remapping has to be done at runtime and, for second, it is **long**. Think `entity.World.Registry.GetMod<ModName>().Stuff.Kind` instead of simply `Stuff.Kind` or something. It is easier to reference content around if you have a static object on some static class in some namespace - you just `using` that namespace. Moreover, this qualification basically repeats the namespace qualification, and it is just annoying to type out every time. 

    - First, the *kind* in this case is assigned a static global id. This global id is then remapped to get the registry id, which is then remapped to get an actual *kind*. It is similar to the client <-> server remapping scheme in the sense that a global id is mapped to another, but here we are doing in at runtime in the logic code, which is just mixing up different abstraction levels. It is just hard to follow.

    I WOULD JUST LIKE TO BE ABLE TO REFERENCE STATIC OBJECTS AND EVERYTHING ELSE BE FIGURED OUT UNDER THE HOOD, with as little remapping as possible.

    In any case, the constraint that any mod can define any content, and that content must get a unique id within an application, feels very depressing. 

    Another approach would be to have each mod assigned globally (in the entire world sense) unique offset numbers. This way no map is necessary. the global ids are the same global id's everywhere. The downside is that these id's have to be somehow assigned. The opside is that it will run faster everywhere since no remapping will be necessary. I think, actually, that most games do it exactly this way, since it is the simplest solution of all. I LOVE THIS ONE MOST.

    However, in this case, array file stats will have to become dictionary files again. Another problem will be with e.g. slots in an inventory: if slots are defined globally the inventory will have to have a per-registry list with the actual slots to use. It also affects `InputMappings`, come to think of it. But everything refreshingly becomes SO MUCH MORE USABLE this way. Although in the current code the problems with slots, input mappings and attack and push sources has not yet been adequately addressed either.

3. Speaking of *sources + array file stats*, *input mappings*, *item slots*, and *default stats* in general. They fall into another category: *extending, mod-dependent, but static content*. (let's call these *Extendents*. like extending + they are acting).

Take *sources*: They can be extended, that is, one may define more sources, which have to have their id's so that we are able to index into the *array files stats*, but these sources themselves are a static type of content, which fit in the rules of the first category. Which stats there are in the *array file stats* is dependent on the mod, in which sense they are mod-dependent. 

Same goes for e.g. *item slots*: they are static themself, but they do affect which slot the inventory gets and they do depend on mods, since new mods can define new slots, even as their static content.

*Default stats* are kind of like a container, a playground for these *Extendents*. So let's call them *Playgrounds*

4. And lastly, there is another, final category: *extendible, mod-dependent, (non-static) registry content*. I think this category might as well not be allowed. One example I can come up with is the entity factories, which may be modifiable by other mods. I think, we should ban this, though. ..........

More future content will fall into the third category and act similar to default stats. For example, another thing would be the *pools*. I can't speculate much, but I think that the graphical content and stuff will have to fall into this 4th category. 


## Solution

So my solution is going to be: 

1. Have the first 2 categories be defined globally (statically).
2. Define an explicit load procedure that would ensure that both all content has been loaded and in the correct order. On this step, assign them id's their global id's, based on the id offset of the mod that they are defined in. Also, ensure on this step, that all of their depedencies have been loaded correctly. 
3. Define the *sources*, *input mappings* and so on globally, and have them get their id's explicitly.
4. Reregister the *sources*, *input mappings* and so on per registry, so that e.g. *array file stats* know which *sources* they have to take into account.
5. For the last category, do nothing for now (make the factories be statically defined).

I think for this solution it would make sense to make a separate class for each subproblem. 

1. First point doesn't need a new concept: namespaces and static classes / singletons (kinds) already do this.
2. Say this will be called a `Registry`. It assigns the initial global id's. The actual order is ensured by the `ModLoader`, which calls the corresponding `Setup Procedures` on the corresponding `Mods`. It is going to have that mapping, which I have recently made scoped. Now it will be more global again (in the sense that there will be just one registry now).
3. `Registry`, as well. Currently sort of doing that already.
4. Now this one, I'm realizing, is similar to patching in the current code. In fact, these extendents are, by definition, patches. Patches, though, affected the registry. These will affect the `Repository` instead. In fact, the registry usually shall not be touched by the game logic code.
5. Do nothing, for now. In the future, either reuse the `Repository`, or define some other structure.



## New stuff, 2021

1. Stat definitions depend on sources having id's. We need to think of a good way of patching this content.

2. I want to have static content without centralized loading procedure (I have no clue how to do this).

## Newer stuff, April 2021

We want to define the entity types in json files since:
1. This will provide easier integration with external tools.

Suppose we wanted to target Attackable behavior. We wanted to add it to the entity types and specify an inject parameter, kinda like this: { name: "Attackable", parameterName: "something" }. 
In order to validate it / provide possibility for selection of this from a dropdown menu, we need to either *export it from the code source files* or *specify separate json files that would mention them*. 
Thing is, these will really have to be smart, that is they are going to have to know the type of the parameter, what are the possible options for it and in where the list of applicable options could be found (it may be shared across similar items). 
The second thing is that the json files will have to be written manually for each of the new types, which is annoying and error-prone, just like header files in C. 
All of this metadata to me should be specified directly in the source files. 
Then, if one needed the json representation, it could in theory be easily derived from the source files by code analysis.

Now to the question of how to actually define these entity types. Things I want:
1. Lighter syntax.
2. Compiler errors if a dependency has not been fulfilled.
3. Some easily understood concept of inheritance. Possibility to remove / add new stuff to specialize the inherited type.
4. Possibility to generate types from a context-aware external tool.

So, first let's understand in what way types are going to be created. The possibilities:
1. Run functions on entities that would add components, retouchers and initialize them in the correct ways.
2. Create some intermediate representation of the types and then instantiate new entities using that. This has miserably failed for me since with this you basically have to create two copies of your logic: one for the actual entity and one for the "factory". With this, you get lots of maintenance issues.
3. A compromise between the two methods: have a factory, that would augment an entity and on instantiation just create copies of that entity. That is, we'll have functions that would set up the factory, adding the appropriate components, hooking up handlers as well as metadata for initializing components. 

This last part is something I'm not totally sure about. 
When I mention metadata, I am talking about the chosen "preset" used to instantiate the given behavior, which just means the set of handlers that will be available on the chains at the start. 
Most of the behaviors will have the default preset, but for some there will either be more, or there will be the ability to create custom presets. 
Also, other *components* (not necessarily behaviors), may define handlers. 
Some components may want to add handlers to chains of another behavior, e.g. it could be useful for the history component that would react to events of the different behaviors by hooking up handlers onto their chains at instantiation, or we could entirely prohibit this and have the individual components be responsible for this history stuff, by checking if the given entity has the history component in their initialization procedure and hooking up the respective handlers to the respective chains on themselves.
This will also set up possible future support for certain behaviors being added dynamically at runtime.

However, there is the idea of hooking up a handler at the tick's do chains, so, in fact, both ways have to be used to some degree.

Now to the components. I have not created a complete mental model of how this is going to work yet, but here are my thoughts and observations:
1. The components are relatively easy: the user will have to provide the injection (dependencies) as parameters to an autogenerated constructor.
2. The behaviors work the same way, the difference being that they also have to intialize their chains at start.
3. As has been discussed in the prevoius paragraph, it will be useful to run an optional init procedure for both components and behaviors. They may affect their own or the chains of others in this procedure. There is an added complication that this procedure has to be somehow marked in order for the meta program to become aware of these, which is a necessary step in adding them into the json graphical editor future program.

So:
1. Add all components / behaviors.
2. Select and run their init functions.
3. Add more handlers (retouchers).

Now since Stats, the Inventory, the History are all components, they integrate nicely into the above scheme. 
Take Stats:
1. They are first added without initialization, just with the injections.
2. The rest is specialied in the init function.

Perhaps init is not a great name.
The "Adding" part involves, as I see it, calling the appropriate constructor with the required injections. It should not involve any more logic than plainly assigning the injected values to the specified fields / properties.
Also, the autogenerated contructors will initialize chains to empty.
The init function will happen after all such contructors have run.
The init function is going to be run directly by the user, so it is their right to decide which one exactly to run or whether to run one at all.
The init function may take additional arguments and may even be defined as an extension method for the given behavior.

As long as we do not have autogenerated code for these entity type specification procedure, this will remain error-prone, but when that is automated, no errors will slip through.

The other thing we have not covered is the inheritance part. With these entity initialization procedure it is a little tricky:
1. In simple cases, inheritance is as easy as calling another already defined function before adding your own stuff. "In simple cases" being when:
    - there is no need to remove existing components;
    - the components already added function on initialization in the same way both with the new components you are going to add and without them.
2. In cases when the above two do happen, a usual three-step procedure is not going to work.

I have a solution:
1. Specify a procedure for each of the steps.
2. Run them in succession.

This way, if a component is to be removed, it is going to be done before it has affected the initialization of other components, and if a new component is going to be added, it is going to be done before any other components had the chance to get initialized differently without that new component in mind. This way, these foreseable problems have been solved.

### Component Copying

Next to the question of component copying.

I've decided that every component should have a copy constructor either specified manually or autogenerated. 

Such a constructor is essential for the entity factory to even function. Since it works by assembling an entity and then creating copies of that entity on initialization, the fastest way to create entities of the same kind with the same set of handlers would be to just copy it.

Since all of the components are statically known in advance, there is no need for copying via reflection.
The code generation tool may be of use for providing default copy strategy for most components (probably).

Another idea is to run the same set functions on a fresh entity every time to spawn it, however that would negate the possibility of optimizing chains so that they are copied lazily. That's the least I can think of. Anyway, copying via functions I feel like will be of use later too.

So to copy any object one has a couple of options, some of which have already been mentioned:
1. Copy via reflection (kind of like automatic serialization);
2. Copy via code generation.

There are also shallow copies, that should work for most objects, and deep copies.

My idea is to introduce a new interface, copyable, and have things that require deep copying implement that interface. Although this one is not really required of Chains and other objects the type of which is going to be known in advance, it is required of behaviors. Also I would rather not get into generic interfaces, that gets messy fast. Implementing the copyable interface is definitely a requirement for components, though. The entity should also have a copy method.

Copying via reflection could also be beneficial and I kind of want to be using that instead, but let's implement it half manually for now and return to this point later. I feel like the syntax is going to be more natural if we wrote normal functions first.

Ok, the thing I need to understand now is what is actually going to be copied each time:
1. Values of injected fields will definitely be copied. It is not yet clear whether they will be mutable or not, but I feel like making them immutable would be a good idea.
2. Chains are always copied (I will implement this first). This one is easy. Since no instance closures are allowed for handlers, this literally means copying the chains datastructure. It may also be enhanced in the future to an immutable sorted set, so that it is only copied when a change to the handlers of the new object is made, but that is in the realm of speculation as of now.

I've looked through the code and I feel like I will do the following:
1. If the component has defined a copy constructor, do nothing.
2. If it didn't, generate a copy constructor. In that costructor:
    - Copy all values of the injected fields, leaving non-injected fields at default.
    - If the value implements ICopyable, copy it by using the copy method. Otherwise, copy it by assignment.
    - Copy all of the chains by the Copy method.

The terminology "copy constructor" is actually slightly wrong. The point is not to exactly copy the component, but to copy the chain handlers and the injected fields, that is, those parts of the component that are related to the type of the entity, while disregarding the values that would change at runtime such as the position.


### What to do next

Things left to restore:
1. Movement (grid, world, transform, moving, displaceable, move stat)
2. World cycle and grid logic
3. Rework items and inventory
4. Rework weapons
5. Rework stats
6. Rework registry
7. Rework status effects (they should use temporary components as storage)

Now the order is not clear and probably does not matter too much.

I guess, I will not be able to even launch the game without the *registry* working properly so I assume we should focus on that first.

I am genuinely curious as to how the items will turn out.


### Registry

Ok, so the previous ideas mostly stay there. The updates are the following:
1. There are no tinkers and retouchers per-sey (these have been combined into HandlerGroups) although the registry is still going to be responsible for giving them ids, but now in form of priorities.
2. The factories, items and components now probably do not need id's as those can be generated by the metacompiler. (Or do they? I'm not sure how I want to do this. What I'm thinking is just to generate a central procedure to initialize them at moment of loading up of content.)
3. The registry is going to become global again. It just makes sense for it to be global since id's are statically assigned.
4. I'm going to change how the id looks. It will lose the offset, but instead have a second number that would show the mod id it came from. So, in its simplest form, and the one I'm going to go for, at least initially, 2 ints.

Let's indentify the things that I'm sure are going to be needing id's:
1. Handlers need their priorities (sort of like id's, mapping not required).
2. Entity Factories need id's (in order to identify entity types in other code, mapping not required??).
3. Component indices (mapping not required).


### World

World back to global again! At least this is what I think I'm going to go for. For now, it makes things easier. Later, if we wanted to add subworlds, we'll figure something out. 


### Entity type — registry interaction

As has been established, entity factories will be initialized via a 3-step procedure:
1. Add components.
2. Initialize components.
3. Add more stuff (e.g. retouchers).

I do not see how the last two could intersect (affect each other) so at least for now it is in fact a 3 step procedure (the last two steps may be take in any order).

I plan to eventually migrate such logic of defining these procedures for any entity type to json light syntax, but for now let's keep them in code (it is going to take time and a better understanding of the system to implement the json part right).

Both json types (static) and (static) manual factory specifications must be generated some code for. In particular, json types will be converted into the latter, with eventual inclusion into the init function of the mod. 
So if both the json types and the code types are to exist at the same time in the codebase (for now we have just the latter) both of them will need to be taken into account when generating this init procedure.
I'm going to introduce an attribute to mark the classes that correspond to definitions of the entity types.
This attribute is also going to include a flag to indicate whether it was autogenerated from json or specified manually. 
This attribute will allow for easier search of such types in the code.
I called it `EntityTypeAttribute` and it has a `IsGenerated` field.

The next thing to address is inheritance. I will add the `Abstract` field to that entity type to indicate that no init function needs to be generated.

The same entity type with a little bit of difference (e.g. slightly different stats) can be achieved via inheritance: create a class marked with `EntityType` and `Abstract` set to true, then create one more class for each of the different types and 'inherit' from the base type.

The process of inheritance consists in calling the methods of the base type in the corresponding subtypes.

```C#
[EntityType(Abstract = true)]
public static class BaseType
{
    public static void AddComponents(Entity subject) {/*something*/}
    public static void InitComponents(Entity subject) {/*something*/}
    public static void Retouch(Entity subject) {/*something*/}
}

[EntityType(Abstract = false)]
public static class DerivedType
{
    public static void AddComponents(Entity subject) { 
        Base.AddComponents(subject); 
        /*something else*/ 
    }
    public static void InitComponents(Entity subject) {
        Base.InitComponents(subject); 
        /*something else*/
    }
    public static void Retouch(Entity subject) {
        Base.Retouch(subject); 
        /*something else*/
    }
}
```

In the future it will be beneficial to add something nicer to this, e.g. pass the names of the inherited types to the attribute.
The reason normal C# inheritance is not used is because:
1. There is no multiple inheritance;
2. There is no partial inheritance (e.g. init from one, retouch from another);
3. There is no reason for these functions to be virtual (they are always called explicitly, no need for polymorphism).

The 3-step init procedure for all these types is just an imaginary one. What is going to actually happed is these 3 functions are just going to be called in succession in the autogenerated code. 
*I could make these types normal types and implement some common interface and then have one init function for this common interface but I'm not sure if I want to do that*. 

What I'll need to eventually implement, though, is dynamic type creation (read a json file, establish what functions must be called and in which sequence, if any, add the new specified components, if any, modify all the necessary components as needed, all dynamically figured out). 
So, there will be a common intermediate representation for these jsons, and then the two implementations. One will generate source files, the other will generate factories at runtime.

So let's understand the terminology a bit better:
1. The static class that has the three methods that describe the entity type is called the **entity type**.
2. The baked object that creates entity instances of that type and that is stored in the registry is the **entity factory**.
3. The intermediate representation is called the **entity type wrapper**.

### Stats

Speaking of stats, there are at least two important perpendicular decisions for them:
1. The way they are actually stored in memory (I'm currently using nested dictionaries, which I do not like);
2. How they are accessed in code and how easy it is to create new ones.

Now the second point was partially good in the previous code. I was pretty happy with the interface that they provided for accessing stats. Pretty, but not totally. E.g., to get the Attack stat lazily, one would do:
```C#
player.Stats.GetRawLazy(Push.Source.Resistance.Path)
```

With the power of alias methods, this can be shortened to:
```C#
player.GetStat(Push.Source.Resistance.Path)
```

The thing is that:
1. It takes a ridiculous amount of boilerplate to write out these stats;
2. Stats use reflection internally to eliminate some of the boilerplate.

Really, the only way to lower it is to generate all the code from json files.
I think I'd like to tackle stats next.

So for every stat the following needs to be specified:
1. Each of the fields;
2. The global default values for each of the fields;
3. The `Index` for each of the stats (like indices of components, they hold the id of the stat as well as the type as the generic type argument).
4. Optionally, specify nested types (Attack **.** Resistance **.** Source)
5. Optionally, specify static fields.
6. Generate a `InitIndex()` procedure for the stat and all of the nested stats.

Let me make a point. E.g. the attack defalut stat value depends on the basic source. 
The source itself does not have a static index, since the source is NOT a stat that is going to be stored directly in the stats dictionary.  
Instead, the source must have an instance field, and that instance will be considered the actual stat.

OK now I'm starting to understand.

We have exported component TYPES by **indices**, while the handlers (which are fields) were exported by **priorities**.
Here, the classes (like `Attack` or `Attack.Resistance`) are being exported by **indices**, AS WELL AS source instances! So instances in this sense become like handlers.
So, each SOURCE is going to have an associated WRAPPER, just like the handlers, which will simply sort of turn it into a type. The wrapper will have a field (or function) for the default value, a field for the index. 

Let's allow this sort of machination not only for the sources but also for any types with this sort of syntax:
```C#
public struct StatWrapper<T>
{
    Index<T> Index;
    System.Action<T> Default;
}
```

And then stipulate that if stat classes are to be treated as such instances too, they must provide static version of these items (or have them autogenerated).

Ok, I'm being a little bit wrong here. SOURCE is just the index for a given attack source, while SOURCE_RESISTANCE is what actually has the data. So creating a source will involve also creating the associated resistance to that source.
Let's call that source stat the "identifier" stat and the associated resistance stat the "identified" so that the stat identifies that stat.

I'm going to go for the following syntax:
1. upper and lower case letters indicate fields of the type;
2. underscore before the name indicates a nested class (nested type);
3. @ indicates metadata about the type;
4. $ indicates a static field.

A sample conversion that I'm going for:
```json
{
    "damage": 1,
    "pierce": 1,
    "power": 1,
    "source": { "@type": "Index<Source.Resistance>", "@default": "Source.Basic.Index" },

    "_Resistance":
    {
        "armor": 0,
        "minDamage": 1,
        "maxDamage": 10,
        "pierce": 1
    },

    "_Source":
    {
        "@identifies": "Resistance",
        "_Resistance": { "amount": 1 },
        "$Basic": { "@type": "Source", "@default": { "amount": 2 } },
        "$SomeOther": { "@type": "Source" }
    }
}
```

```C#
public struct Attack : IStat
{
    public Index<Source.Resistance> source;
    public int power;
    public int damage;
    public int pierce;

    public static Index<Attack> Index;
    public static Attack Default() => new Attack
    {
        source = BasicSource,
        power = 1,
        damage = 1,
        pierce = 1
    };

    public Attack AddWith(Attack other) => new Attack
    {
        source = source,
        power = power + other.power,
        damage = damage + other.damage,
        pierce = pierce + other.pierce
    };


    public struct Resistance : IStat
    {
        public int armor;
        public int minDamage;
        public int maxDamage;
        public int pierce;

        public static Index<Resistance> Index;
        public static Resistance Default() => new Resistance
        {
            armor = 0,
            minDamage = 1,
            maxDamage = 10,
            pierce = 1
        };

        public Resistance AddWith(Resistance other) => new Resistance
        {
            armor = armor + other.armor,
            minDamage = minDamage + other.minDamage,
            maxDamage = maxDamage + other.maxDamage,
            pierce = pierce + other.pierce
        };
    }

    public struct Source
    {
        public Index<Resistance> Index;
        public System.Func<Resistance> Default;

        public struct Resistance : IStat
        {
            public int amount;
            public Source.Resistance Default() => new Resistance
            {
                amount = 1   
            };

            public Resistance AddWith(Resistance other) => new Resistance
            {
                amount = amount + other.amount
            };
        }

        public static Source Basic = new Source
        {
            Default = () => new Resistance { amount = 2 }
        };

        public static Source SomeOther = new Source
        {
            Default = Resistance.Default
        };
    }
}
```

And also some init functions. 
Summing up what we need for this:
1. Read json files.
2. Identify special symbols @ $ and _ and react differently depending on that.
3. Types and the default values may be copied as text for now (not verified), since doing errors for this is whole other problem.
4. Build up structs either with Roslyn syntax factories or with templates. I'm going to go for syntax factories on this one, just to try it out.

Ended up not using syntax factories but building it all up with T4 and I'm glad I did.
The code with syntax factories turns out too verbose.
Since the stats are now structs, had to make some changes in the functions that set up these stats on the context.
Also, found out that one cannot return a reference (`ref T`) of a struct inside a dictionary (stored by the interface, i.e. the boxed version. 
The solution is to store them in a *user-defined* box, that is, a class that has the stat as a field.
Actually, I'm going to try that next since I really want to be able to return them by ref.


### Targeting

Seriously, show to me the idiot who wrote this! (it was me)
```C#
public interface IWorldSpot
{
    IntVector2 Pos { get; }
    World World { get; }
}
```

Like, what is the point of this? Why not just have it be a struct and cast the Entity to this struct implicitly? 

There are different targeting options employed currently in the code:
1. Buffered
2. Unbuffered

The buffered version uses Pattern objects to get relevant entities from the relevant positions, then runs a sequence of handlers to select just one or just a few entities from the list of targeted entities.

I guess this is fine, but the problem is that there is no benefit in using chains over simply functions.
Why not just call a bunch of functions in succession, or compose them with + operators?

Ended up splitting the two implementations: the buffered one and the unbuffered one completely. The buffered implementation just works with attacking (highly parameterizable one-class pattern through piece objects) while the unbuffered one works with the pattern interface and is more generic. To be noted, it disallows passing extra data with the targeting context since it lacks the context altogether (the individual targets are processed independently). 


### Items

Items will be entities. When they are dropped and exist in the world, they will be added to the grid. When they are picked up, they will be removed from it, but the entity itself will not die.

It may be a good idea to remove the transform component together with taking the item out of the grid.

It will have the following components:
1. `ItemComponent` holds the logic associated with a particular item. In fact, this is going to be a parametrizeable behavior with chains.
2. `Transform` to indicate position in the world.
3. If the items will become susceptible to explosions (I have not decided yet) they should also be granted the attackable and the damageable behaviors.
4. Shovels will have unbuffered target providers, while melee weapons will have weapon target providers. (These are both components).
5. Ranged weapons will also have a unbuffered target provider, which they will make use of upon activation. 

Now, it is not clear how the activation should work. There are a couple of possibilities:

1. As I'm doing it now: the item manually attaches a handler onto the chain for one of the action types on the controllable behavior. 
This way, when the player presses a corresponding button, the item's handler will pick up on that and force the item's selected action to be activated. 
This scales just fine to entities without controllable behavior: the e.g. sequence may just reference the action defined by the item and so the entity will e.g. shoot no problem, although at the same time the entity will have to have an inventory and the item will have to be stored in there.

2. When a key is pressed, go to the inventory and look for the items that mention this action type. 
This way, the item's activation condition will be centralized.
I feel that this is way more complicated for these reasons:
    - Cases when one item references multiple action slots have to be accounted for;
    - Cases when the item does not actually need to be activated have to be detected and then the next item in sequence will have to try to activate;
    - It is less efficient (probably) because one will have to iterate through all the items;
    - Hard to introduce priority;
    - Some more complicated cases just cannot be covered by this approach.

There is a problem though. The logical thing is to associate each (activateable) slot in the inventory to a button slot in the controllable behavior. This way, the item's side effects will be separated from the activations:
- First, traverse some check action chain with the selected action code;
- Second, if that succeded, go to the inventory, look at that slot and set the action to activating that item;
- Set the actions direction to player's orientation;
- When time comes to act, just execute the action.

The only problem with this approach is that check will not allow for action substitution, but that can be remedied by trying to get an action from handlers at that stage. 
But here is a problem again: those handlers will have to check the actual action slot selected. If they were in the inventory, that would have been easier. 
So, there should be a before chain and a after chain and in between those the actual getting the action of the particular item, all stored in the inventory. 
But then the problem is that the inventory now is tighly coupled with action slots, which is probably undesirable if we wanted ordinary entities to have an inventory.
Although actually, this is totally fine. 
To get the action the entities may just ask the slot in the inventory to give them an action for the specified direction instead of setting them directly.
The inventory is a component and must not be inherited!

Ok, so, summing it all up:
1. There are three types of (input) action slots:
    - Vector action slot (direction input);
    - Item action slot (reach out to the inventory and retrieve the action from there);
    - Other (direct chains on the controllable behavior. This may actually be just totally omitted and is probably never going to prove useful).
2. The items will define an activation for the ItemComponent in form of chain handlers.
3. The items will also define a "get action" strategy for the ItemComponent (again, through chains).
4. The slots in the inventory will contain a before and an after chains (probably, I'm not sure if this is useful).
5. Actually, I think bullet 4 is not useful. 
There should be one such chain, in the controllable behavior. It will be responsible for completely overwriting an action gained from the item.
Although a similar algorithm will have to be written the second time for the sequential entities.
I feel like this is not the solution also. 
What I'm certain of is that the actions will be retrieved from slots, whether or not the chains for before or after should exist for all the actions or just the one particular action slot we'll see later when it becomes more clear what is actually needed and what is not.
So let's first implement the part with the slots.
6. The slots in the inventory will contain either:
- a single activateable item;
- a single non-activateable item; (not associated an input)
- a countable activateable item; (associated a slot)
- a countable non-activateable item; (not associated an input)
7. The slots in the inventory may or may not be mapped to inputs; the same is true for slots, although this idea is going to be neglected for now.

For now, I'm not going to worry about mutiple possible slots for an item.

What now is clear is that the items will have to be handled differently, like, completely differently depending on the category:
1. Unique activateable items will be as described above: they will have the transform component already in place and they will not stack.
2. Countable items WILL STACK, but their effect will not. So e.g. bombs need a counter. The big difference is that when such item is picked up, it must be destroyed and coalesced with the one already in the inventory. 
This is hard, because how do you code it then? 
Should any item just have a Pickuppable component with the pickup handlers and a separate active item component with the activate handler? 
Should all items' transform be taken away when they are picked up?

Actually yes, that makes sense. This split (Pickuppable and Active) is not enough though. It does not account for passive effects: should they be on the pickuppable or on the active? 
If they were to be on the pickuppable, then how do you check if this item has already been picked up to not activate the handler?

So the answer is probably more separation:
1. ~~Pickuppable~~ `Equippable` responsible for whether an item can be picked up.
2. ~~PassiveItem is activated once the item has been picked up~~.
3. ~~ActionItem is queried for getting the action.~~
4. ActiveItem is responsible for the action execution.
5. ItemCounter.

Now, the inventory can check whether an item can or cannot be activated by checking if the ActionItem is present on the item entity.

Pickuppable can be used for not letting the player get the item by walking over it (together with a sort of cost component, through the check chain). 
Pickuppable will also remove the item off the grid and remove its transform components. 
If the item is countable, it will have to be counter toward the ItemCounter and destroyed. 
If it is not countable, it should be just appended to the Inventory, given as input. 

~~Action item has an explicit input mapping attached to it. ~~
~~It will spit out an action as the result of its activation.~~

Active item has an activation method and can be activated. 
This is the place where one would add the particular handlers e.g. handling the exact shooting mechanism.
If an item is active, it will be activated automatically when the input mapped to it is provided. 


There may be items that overwrite other items' active abilities. 
This can be achieved by changing the handlers of that item with your own ones, from the Active, or overwriting the action returned by Action.

Ok, that was straightforward enough. Now, how do you allocate slots?
Well, slots are needed for controllable for mapping inputs to actual items in the inventory, but also they will be useful for getting certain item categories: shovel, ranged weapon, melee weapon.
So the slot may be mapped and may not be mapped.

```C#
struct Slot { bool isActionMapped; Identifier id; }
```

And then these slots will have to be assigner id's by the registry, which means more code generation!
At this point, though, I could make it a bit more general maybe? 
Nah, for now let's do it this way. 
I thought that for such structs, that just need a category and do not have to be stored anywhere, a more general generator can be used (in order to not make one for slots, one for whatever else there is going to be, etc.
I may add entity wrappers in the future, to make certain guarantees about the entity type.
The wrappers would restrict which components can be accessed directly by component query methods.
These slots would then wrap the items that are returned by them in the wrapper struct for that entity type.

So, I'm going to add another attribute for slots and add code generation for fields marked with that.

Also, one probably would like to store them somewhere, especially because of the metadata associated with them, like the name of the thing, although this is not clear to me right now.

### Statuses

Requirements:
1. The effect should be completely custom (achieved through chain handlers).
2. The effect should tick down, if needed. When the tick value is zero, the effect should be removed.
3. The effect can have storage (achieved through temporary components).
4. Some effects may choose to not be affected by ticking and remove themselves only when they deem necessary.

Why not implement all of statuses as temporary components? 
The components will be responsible for storage, they may add chains if they really needed to. 
The effect that they apply will be the first function that is called when they are applied.
That may be done either with subclassing (interfaces).

No, I'm thinking these will just be handled with normal inheritance.
There will be Status instances, instantiated from StatusIndices.
Status indices will contain the status index.

Currently, only static handlers are exported.
I have renamed statuses that are treated as temporary components into EntityModifiers.
So, there will be wrappers, who will be given functions of instantiation + hooking up process of the entity modifier.
Some parts of this code will be very similar. 
For example, hooking up the component is just adding the component to the entity by its identifier.
Thing is, in order for a single function to do this, for the sake of code reusability, closures have to be created.
NO! like that does not need any closures. There can just be a function that istantiates the modifier, and the other function would be a member function and would just append it to the entity using the member field of index.
The problem is with the removing bit.
It can be done generically, by getting the component by its index, removing it and then calling the unbind function (through an interface).
So, it would be nice to allow closures to become handlers too (closures on wrappers).
(So now we need to allow instance functions to become handlers).

Implemented code generation for instance fields and instance handlers.
This was the solution, since instance handlers can capture any data.


### Fixes

Changed around the pool implementation (it used to work with decks, now it is a simple less than pool).

Fixed some more stuff.

Now code generation needs to be adjusted to work dynamically with mods.
1. Take a project and a root namespace name. In case of Core, it should take the project path, 


### Shield

The shield item defines handlers that look at the current shield.



### Problem with Items

Items sometimes need to add or remove handlers on equipping / unequipping, and those handlers should be able to also remove the item that added them.

With items, the workflow has been the following:
1. The item defines a slot, which it is going to bind to.
2. The item specifies a component (added to the item) to hold their data.
3. Subtypes of that item (e.g. tweaking stats) are created as normal entity types.
4. The subtypes add that new component, reconfigured as needed.
5. The base type adds to equippable component handlers that add those other handlers onto entity chains when the item is equipped.
6. When removing the item from the specific item slot of this item from handlers, hooked onto entity chains, the item is found and removed by its item slot.

Problems:

1. So, the items of that base type end up sort of being grouped together in that slot. This is intentional.
If that was not the case, the handlers will have to have captured the id of the item. 
I guess this can be done with the InstanceExport attribute, it's just really annoying, because you'd need to (optionally) create the base Entity type, then the particular Entity type, already parametrized, then a closure for that type that references that type, which also, by the way, has to be referenced in the entity type, then that closure has to be inited, handlers have to be manually given id's, handler groups need to be manually initialized too. 
The id of the entity type has to be referenced in that closure, which is not even possible yet.
2. 




1. Code generation for slots (inventory Get<Slot>, TryGet<Slot>, entity Get<Slot>, TryGet<Slot>).
2. Code generation for autogenerating handlers for adding (removing) items to (from) slot, with adding (removing) of their handlers too (automatic generation of handlers added onto the equippable component which would add (remove) the specified group of handlers).
3. Code generation for groups of handlers with multiple target chains.
4. Allow instance entity types. E.g. the factory of the item, that has tweaked damage, should be generated from a parametrizeable entity type static class (perhaps allow non-static entity types? that is, the same I've done with InstanceExport).

Now, these has a lot in common with the problems encountered with entity modifiers. 
Now perhaps this is not a problem to be solved with entity modifiers but with a different strategy, but anyways.
Assume we had a freeze stat that applied different amounts of freeze effect onto targets.
We do not need to create different closures for each of these "submodifiers", because they should add the same entity modifier, that is, their entity modifier is of the same class AND is indexed in the entities component dictionary by the same id. 
However, as it stands currently, the only way to apply this exact amount of modifier, or even pass any data to the entity modifier component, is by referencing this amount through the closure. 
But, creating a closure also means creating a new index for the component, as well as new handlers and a new bind function.
So we want the bind function to reference the same exact entity modifier component in the component dictionary on the entity, while also having the ability to pass to it parameters via the bind function.
This can be done by either by:
1. Having the bind function take another parameter with type object and casting it to the expected type and then passing the data. This is terrible.
2. Having the bind function be generic, as well as the index, that encapsulates this function, and pass to it the data of the required type. This involves generics and so is messy. 
Also, it would mean creating another structure just to pass some data to the component. 
The exact data that it needs is already known based on the component (the injects).
3. Take an already correctly instantiated component as input. 
This is viable actually. The closure could also define a function that would take as arguments the data that needs to be passed to the index, which then would use the bind function to bind the necessary things to the entity.

There is a boilerplate element to this, though.
The thing to notice is that these entity modifiers are basically, or actually just like components and presets:
1. Components get passed injected data.
2. Components are added onto the entity.
3. Components are intialized by calling one of their presets.

Entity modifiers, though, might have additional logic. 
They kind of work like entity types, but without a factory.

Now comes the idea of dynamic components: components that only make sense at runtime. 
They can be both added and removed. 

1. Add components by invoking a static method, which combines the constructor with the injected fields, the AddTo() method, and calls a preset.
2. Remove components by calling a "unset", corresponding to the given preset and removing the component.
3. Generate a wrapper for this that would define a status stat for this. It would hide the "component" part of the entity modifier.
4. The interface for this modifier should look about the same as for the future status effects.

### Handler update

Handlers should be classes! that will make easier dependency handling.
Thing is, it is not possible to generate code for handler groups in the current version.
A handler group may have been initialized (handlers with their priority assigned copied over) before a handler it references has been assigned priority.
This means I would have to track dependencies between handlers from different classes.
(If one could store references as fields or in an array, this would not be a problem, but C# does not allow that).
With them being classes, it is also possible to create a generic attach-detach component for items that would attach its handlers on equipping and detach them on unequipping.
This type of thing would be SO annoying to type out manually each time.
Actually, this sort of thing would be totally possible with interfaces + boxing and dependency tracking, but again, with classes, it's just easier (and less copying, but more dereferencing).


### Items update

Decided to reconsider how I handle the item logic. 
Instead of having an ultra customizble chain, decided to switch it up for a general function that would handle the logic of all possible cases of item types:
1. with countable component
2. with slot component

The handlers that are hooked onto the entity as the result of equipping the item are customizable.
Made an interface for them called IHookable that would hook/unhook the handlers to/from the entity.

This looks good and I like that currently.

Maybe it is going to be worth going back to chains at some point, but not right now.


### Entity modifiers

Now to the topic of entity modifiers.

They should work kind of similar to the way that items ended up working.

There should be a Hookable object that should be hooked when the entity modifier is applied and unhooked afterwards. (kinda like a tinker/stat modifier).

I guess the freezing stat is an example:
1. It defines an EntityModifier called FreezingEntityModifier, that store data for the freezing stat.
It has methods for working with that data. In this case, the data is the outer ice cube entity.
2. It defines a static class Freeze for applying and removing the entity modifier.
3. It defines a stat and a source for integration with the stat system.

The reason a generic index is not used is because you want to pass in parameters, e.g. the hp of the ice cube or the timeout (amount), which is not currenlty exploited by the icecube.

What I'm going to do, though, is I'm going to decrease the cube's health every tick and once it reaches 0, I'm going to kill it off.


### Bouncing

The way bouncing was implemented before is sort of impossible currently:
1. Each turn, every single trap is activated. If there is an entity on top of the trap, it is pushed. 
If there is no entity, the trap starts listening for entities incoming over it.
When an entity comes on top, push it. 
This is achieved by listening for enter cell events until the end of the loop (currently possible). 
2. The problem is that the trap has to keep track of whether the entity that remained on top has moved.
Thing is, if the entity from top ended up on top of the trap, it should not be pushed on the next activation.
So the idea was to check whether or not the entity has moved off of the trap, and reset the variable that indicates whether the trap should push or not.

Ok, another idea:
1. Have a permanent enter listener.
2. Once an enter is triggered, renew the entity that is considered to be on top to the entity that's actually on top.
3. Do the same thing for the leave event, renewing the entity on top to that entity.
4. In the enter listener, if it is our turn to move (checked by comparing acting's order with the current order), push the entity instantly.
5. Do not push the same entity twice (this means a dictionary refreshed at ticking).
6. At ticking, set the variable that indicates whether there are entities on top to false/true.
7. At leave, only set that variable to false if there are no entities.

This is kind of complicated but it is the simplest solution to me.
This, however, requires being able to both add and remove listeners to cell movement triggers.
I currenlty only have the ability to add those though.
I think it could be beneficial to keep/remove handlers according to some condition (e.g. remove if the entity is dead or does not have a specific components anymore).
Let me try and implement that.



~Ok, one more idea. Do a sort of a system for this:~
1. ~The system keeps track of all of the bouncing components currently in game.~
2. ~The system keeps track of a list of entities~


### Managing modifiers

So it's cool and all: you can add modifiers by applying components dynamically and calling the preset method.

Now, say we have the sliding modifier. It works on its own. Now we wanted to add a callback for queuing the right animation once the entity slides / the effect of sliding is applied. These are all events that have to be somehow exposed, they depend on the type of entity and why not make it changeable.

So, there is some options.

1. Define a `Slideable` or something component, where you would store that. This is bad because say a mod added new effects, then it would have to add the component of that effect to all of the entity types.
2. Store the event handlers in a separate component. 
Say, `MoreChains`. 
This component would lazy load the chains when they are required, it would allow chain mutation for instances and it would be setuppable, just like stats, on the factory. 
So to add your custom animation, one would get the chain by its id on the factory and add the handler to it.
The id's will be available as `Indices<Chain>` as static fields and initialized by the registry.

### Changing the context

I hate the fact that the context is based on a base class. What I would like to do is have a `ShouldPropagate()` method or a `Propagate` property that would contain the logic for whether the propagation should be stopped. This will be optional for a chain, though. Propagation of event with propagation checking enabled will be defined as an extension method over the Chain. This will also be available for other types of chains, like the Linear chain. 

So the plan is to remove `ContextBase` altogether, which shouldn't be that hard.


### Bug with stats

Just noticed a potential bug with stats.

Since mods may add new effects which may apply even to entities from other mods which don't know anything about them, they should use default stats associated with that mod.

However, since the stats are initialized 



### More Chains (aka events)

To make the new more chains component intergrate well with the rest of the system, 
1. We need a way of identifying those chains in the Export attribute.
2. Allow linear chains




```C#
public static class Sliding
{
    // This chain should be automatically given a unique identifier
    // We should be able to address it in the [Export] with e.g. the following syntax:
    // [Export(Chain = Sliding.Applied)]
    public static Index<Chain<int>> Applied = new Index<Chain<int>>();

    // ------- The autogenerated code will contain --------

    public void InitFunc()
    {
        Applied.id = Registry.Global.NextChainId(); // or something like this
    }

    public static ChainPath<Chain<int>> AppliedPath = new ChainPath<Chain<int>>(
        entity => entity.GetMoreChains().Get(Applied));

    // or another possibility
    public struct ChainDescription<T>
    {
        public Index<T> Index;
        public ChainPath<T> Path;
        
    }
}
```

Actually, I want to change how chains are registered.

One would want some chains to be e.g. linear, whereas some of them should work with priorities.

So, just the Chains attribute is not enough.

So instead of using the chains attribute, I propose we write the fields manually and indicate that these are indeed Chains for simplicity.

```C#
// How it used to be
[Chains("Hello"/*, ... etc */)]
public class SomeBehavior
{
    // A context used to be required
    public class Context {}

    /* No more code. The things that are autogened follow */
    public readonly Chain<Context> _HelloChain;
    public static readonly BehaviorChainPath<Chain<Context>> HelloPath = 
        new BehaviorChainPath<Chain<Context>>((entity) => /*whatever*/._HelloChain);
    
    // Also, the chains were inited in the constructor.
}

// Now there is two ways to proceed
// 1. Shove all events (chains) in the MoreChains component.
// 2. Split the logic into MoreChains chains and normal chains.
//
// I opt for the second one, at least for now, but it's close.
// It's pretty simple to change later, but not completely trivial.

// Note that AutoActivation does not go away.
// It's still useful for getting up and running for simpler behaviors and for prototyping.
// It will work just like before.
public class SomeBehavior
{
    public class Context {} // this one is not required now, unless AutoActivation is used.

    // Define readonly chains manually
    // Not necessarily public also, since they are accesible only via the path from the outside code.
    // Also a plus is that the name could be anything.
    [Chain] /*public*/ private readonly Chain<Context> _HelloChain;

    // Another plus is that you may use something other than context.
    // Also, note the usage of linear chain.
    [Chain] /*public*/ private readonly LinearChain<int> _WorldChain;

    // Probably also worth adding a name to the chain attribute
    // [Chain("World")]

    /* The generated code follows */
    public static readonly BehaviorChainPath<Chain<Context>> HelloPath /* = whatever ... */;
    public static readonly BehaviorChainPath<LinearChain<int>> WorldPath /* = whatever ... */;

    // The initialization code too, obviously
}

// The first option with sticking all of these on MoreChains is not explored, although
// adding chains to that component is definitely required.
public static class Demo
{
    // If this happens for the MoreChains component, the attribute is applied to indices
    // This one is inited in an autogened static constructor to avoid boilerplate.
    [Chain] public static readonly Index<Chain<int>> MyChain;

    // A path is going to be generated automatically
    // It's not going to be a BehaviorPath anymore, because it is more generic.
    public static readonly MoreChainsPath<Chain<int>> MyChainPath
        // Probably do this in the static constructor?
        = new MoreChainsPath<Chain<int>>(MyChain);

    // It will be given an id in the main init function automatically

    // Now, if one wanted to add handlers to this chain, either query it manually
    public void Thing() => Demo.MyChainPath(entity).Add(SomeHandler);

    // Or use the export attribute, e.g. like this
    // + signifies MoreChains
    // Since we now allow something different than just objects as contexts
    // this will be kind of tough to implement.
    // Probably we should allow just objects.
    [Export(Chain = "+Demo.MyChain", Dynamic = true)]
    public static void SomeStuff(int context)
    {
        // Do some stuff with the context
    }

    // And then add it like this
    public void Thing()
    {
        // This adds the handler from above onto the 
        /*ClassWhereSomeStuffIsDefined.*/SomeStuffHandlerWrapper.HookTo(entity);
    }
}
```


#### The Plan

1. Add the `[Chain]` attribute with Name property
2. Remove `[Chains]`.
3. Get the fields decorated with `[Chain]`. Ignore non-index chains on non-behavior classes. For index field.
4. Add a new snippet for generating code from indices.
5. Give identifiers to these indices by the registry.
6. Recognize the + syntax. In this case, get the correspondind static class instead of behavior.
7. Chains now store their context themselves. They also store their type (linear or nor).
8. Let's say the context stays as it is for now (actor + other stuff on an object).
9. Export attribute disallow priorities for linear chains (emit a warning).


# Proper project setup

### My current setup

I have a game (library) project already in development which already has a lot of features. 

It contains a bunch of subprojects:
- `Utils` with utility code, like `IEnumerable` extensions, some data structures etc.
- `Core` with the base library of the game. Contains essential game logic code.
- `Meta` for code analysis and generation for the main project (`Core`) and any mods.
- `Shared` contains some stuff available for both `Core` and `Meta`.
- `TestContent` is a test mod. 
- `Tests` with tests for the `Core` code and the code from `TestContent`. I'm using `nunit-3` for tests.
- `Mine` as a playground for quickly running and debugging certain tests.

References between projects are achieved via `ProjectReference` in the `.csproj` files.

My `Utils`, `Core`, `Shared`, `TestContent` and `Mine` subprojects do not reference any external projects, just each other (and no circular dependencies). All of these also have their `AssemblyName` property in `.csproj` files set to their name. For example, here is the `Hopper.Core.csproj` file (others are similar): 

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Library</OutputType>
    <AssemblyName>Hopper.Core</AssemblyName>
    <Name>Core</Name>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Utils\Hopper.Utils.csproj" />
    <ProjectReference Include="..\Shared\Hopper.Shared.csproj" />
  </ItemGroup>

</Project>
```

Another problem with this is that I do not really care which version it exports for. I'm using only features from C# 7.X, so no default interface implementations, no nullable types and no a couple more things. C# 7.X is supported by basically any dotnet runtime (I guess?) there is, be it `Core`, `Framework`, `Standart`, whatever. (I am still confused by these, even though I have read tons of documentation on this).

The base folder *does not* have a solution file. I personally do not understand the meaning behind solutions. I'd appreciate if a knowlegeable person explained it to me.


### What I want to achieve

I'd like the particulars of the build system to be defined from an outer project, but with a default fallback. So, if I import this code as a `git submodule` in another project, say a Godot project, I'd have my build system pick up on Godot's build system while all of the subprojects kept working: I want to be able to generate code through `Meta`, I want to be able to run my tests and I want to even be able to run some test code on just the included code, all while being a subproject of the main project in Godot.

Also, I want to somehow clearly state to the outer project that `Tests` are for tests and should not be in the build of the game, `Meta` is a tool, and should not be in the game build as well. Maybe, these should even be built in separate folders, to aboid confusion.

I also want the runtime version of the outer project (e.g. `DotNet Framework 4.8` to propagate to the subprojects). I also want this version to be distinct from the version of the `Meta` tool, which should be built with the runtime needed for `Roslyn` to work. Ideally, this second "tool version" should be selectable by the outer project, but should have a default value too.

I want my projects to build individual dlls, so we don't get all code compiled into a single dll.

I would like to avoid splitting it up into even more github projects. I want the Core projects to have both the code generation tool and the tests code.


### Building into one directory?

I have only half-figured out how to force `msbuild` to write output to a single folder. So, I have a build folder in the root of the project, where dll's of all subprojects end up. I have been able to achieve this via a `Directory.Build.props` like this:

```xml
<Project>
 <PropertyGroup>
    <BaseOutputPath>..\build\bin</BaseOutputPath>
 </PropertyGroup>
</Project>
```

There are a couple of problems with this solution:
1. All of the subprojects, apparently, if they reference another subproject, even within the same project, seem to be recompiling these projects. E.g. `Tests` reference `Core` and `Utils`, `Core` references `Utils` as well. When `Tests` are built, it builds `Core`, which builds `Utils`, but then `Tests` builds `Utils` the second time. This is annoying.  
2. All of the binaries end up mangled in that one folder, which is also annoying. I'd like them to be stored in separate folders, e.g. by the name of the project, in that build folder, but I could not find a way to get a reference to the project in `Directory.Build.props`. I must set this path in `.props`, because if I set it in corresponding `.csproj` files, where the name is known, `Nuget` then messes it up by writing to the default path anyway (that is, a nested to the project `bin`).

I have also not been able to figure out how to write the assembly info, I guess, generated in nested `obj` folders, to somewhere else. Whichever setting/property I tried in `msbuild`, it didn't end up working out.

I also have a couple of helper `bat` scripts. One launches `Meta`, the code generator, on the `Core` subproject and on the `TestContent` subproject, generating code for both, under their project folders. The other runs either all tests or a selection of tests. I would like to either convert these to tasks which I could run in an IDE or via `dotnet whatever` commands, or make them decide what exactly to do depending on whether the project is included as a submodule or not. 

For example, my `test.bat` literally looks like this:

```bat
@echo off
cd .Tests
call dotnet build
cd ..

if %1==all (
    nunit3-console build\bin\Debug\net4.8\Hopper.Tests.dll
) else (
    nunit3-console build\bin\Debug\net4.8\Hopper.Tests.dll --test=Hopper.Tests.%1
)
```

So it literally builds the `Tests` project and then goes ahead and runs the compiled dll by path. This is obviously no good if the dll ends up in a different folder.


### Why not run tests through your IDE?

Oh, I was not able to run tests via VS Code. I have struggled for like 2 days, then ditched it. So, basically, if you write code for tests, VS Code automatically pick up on it. It displays `Run Test`, `Debug Test` and other buttons above your tests.

![Run Tests, Debug Tests](https://imgur.com/ddzTsUu)

However, if you click on it, it always runs in the integrated terminal, which is really slow, annoying and doesn't color code correcly. I have searched for hours, but have not been able to find a sloution to this. (I'm using `ConEmu` as my console, which I like a lot more than the janky integrated terminal).

If you try debugging the test, the error `Error processing 'configurationDone' request. Only 64-bit processes can be debugged.` pops up. I tried to find what exact command and in what way `VS Code` actually tries debugging those tests with, but could not find any info whatsoever on this. This is why I have another project for debugging and quick tests: I am unable to debug tests. This is, too, extremely annoying.


### How can you help me

I would like to get an advice from a more experienced developer on how to set up such projects in the most flexible, but at the same time least troublesome and annoying way possible. Partially breaking the existing setup is fine. 

The most important thing is for this nested project to be totally independent of the parent-project, but at the same time to be customizable from the outside. Also, keep in mind that this project must be copied as a git submodule into the outer project.

I would appreciate examples of real projects, maybe blogposts that explain this in depth, any tricks or concepts I will need to learn to set this up, anything. I'm totally new to all this and have no idea where to start. I am too overwhelmed by the problems that have come up that I have no idea how to solve.

I would also appreciate useful information on Godot build system and examples of any complex Godot projects that make use of dotnet, or, likewise, any articles on how to set it up properly or how to customize it.

Ideally, I would like to avoid reading 1000 pages of random documentation to hopefully fish some useful ideas. I would like to get this sorted in, ideally, a few days.


### Actual code of the current set up

The Godot code is currently broken: the nested repo for first is not used in any way (it's just included as a submodule) and, for second, there is essentially no game yet. See [1][the state of the godot outer project as of the day of writing]. 

[2][Here is the state of the library project as of the day of writing]. It is the repo described above, with the code generator and the `Core` code. 

[1]: https://github.com/AntonC9018/hopper-godot/tree/e2656e219fdee4375e3b4c86594446086a115c2f
[2]: https://github.com/AntonC9018/hopper.cs/tree/37afc578ba174285403ae74269d9e7b514d61174




# More ideas

1. Moving should do Check and then Do, separately, on the displaceable, or work differently than a check do (like check and before or something). +
2. Movs should use the faction to figure out what faction to target.
3. Add DB "indices" to the registry, that is, accelerate common queries. +
4. Flag and enum registry types.


# Entity Type IR (sort of)

The goal is to be able to generate typesafe code for entity type creation (entity types should be called *archetypes* btw).
Currently you have to write all of this code manually (the 3-step procedure of setting up an entity factory).

Ok, for first perhaps there needs to be a separate step for initializing entity factories, in order to avoid double lazy loading of e.g. stats.

Now to injections. In order to call the constructor with plausible arguments, they must at least match the types of the injection. This is easy for manually written code. 
There is a sort of a catch though. 
Even though two different injections may be of the same type, it does not necessarily mean that the values provided to one of them are going to be meaningful for the other one too.

My idea is to have *categories*. By default, concrete types like enums, form a category. Other types form custom categories.
So e.g. `Attacking` is exporting `SomeFunc` of type `Action<int>`. 
This injection will automatically be assigned the category `Attacking.SomeFunc`, so that the only functions that can be injected into this field have been specifically designed for this field. 


```C#
class Attacking : IComponent 
{
    [Inject] Action<int> SomeFunc;
}

static class Stuff
{
    // It will be possible to reference this function in the json entity type spec
    // By saying the full qualification (Stuff.Func1) (or ModName.Stuff.Func1 idk yet)
    [Category("Attacking.SomeFunc")]
    // Since having raw strings like this is unreliable, the following is preffered
    // [Category(nameof(Attacking) + "." + nameof(Attacking.SomeFunc))]
    void Func1(int thing)
    {
        // do stuff
    }

    // This one is not marked with category, so it will not be visible.
    void Func2(int thing) {}
}
```

The fields will have to be type-checked by the analyzer, which is fine.
The data collected by analyzing the code can be stored in a dictionary of categories and used in order to verify json files with entity types.


```Json
// my_entity.json => MyEntity entity type (name of static class too)
{
    "components":
    {
        "Attacking":
        {
            // Allowed, works fine
            "SomeFunc": "Stuff.Func1",

            // Allowed, but tweakable
            "SomeFunc": null,

            // Compile-time error (while analyzing code):
            // SomeRandomFunc is not within the required category
            "SomeFunc": "SomeRandomFunc"
        }
    }
}
```

Now, optionality. Add named arguments to `[Inject]`:

```C#
class InjectAttribute
{
    // Whether the category allows null as a value 
    bool Nullable = true;
    
    // I'm not sure about this one and how to implement this
    // This probably should be a string representing an item from the category pool
    object DefaultValue;

    // By default, create a new category
    // Otherwise, things cannot reference this field to get exported
    bool CreateCategory = true;

    // Category name to override the full qualification
    string CategoryName = null;

    // Make the given injection accept an already existing category
    // Since this cannot coexist with CreateCategory, 
    // this should be a property setting the same bool 
    bool ShareCategory = false;
}
```

We'll be able to apply `[Category]` to static fields and functions.

The problem with such power is that the code generation will become even slower, which means we really need something more powerful, like an lsp, which I have no clue how to do.
But, when we have an lsp, it will be possible to provide diagnostics at the time of writing the code, instead of erroring out at the time of code generation.  