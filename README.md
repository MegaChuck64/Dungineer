# Dungineer
## TODO
decouple graphics from game logic.
Lets just have a map, not worrying about sprite sizes or anything. 
Then we can easily swap out how the graphics are done. 
Can do all tile based collisions this way, etc. 

---

- do character select scene
- think of a better way to handle control of map objects,
- loading and stuff
- we don't want to hardcode every tile
- enhance terminal, selecting history lines, commands, 
- do movemen
#
#
# Lets actually use components, stop doing oop over inheritance
# lets create gameobjects with json files 
---

### Lets just make the goal simple

- fixed size map with no camera movement, all fits on screen
- swarms of enemies come in from the edge of the screen
- Approaching infinity style movement
	- click tile on map, as many ticks happen as it takes for player to get to target
- Left click is movement, right click is action. Switch actions with numbers