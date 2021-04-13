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