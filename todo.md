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