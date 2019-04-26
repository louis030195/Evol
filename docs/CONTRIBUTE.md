# TODO
- Think & fill this list of ideas ...
- Optimize (object pooling for ex, use photon instanciate override stuff)
- Prepare steam API (friend play etc)
- Make a google form feedback (English + French)
- Tweak multiplayer (test, make it less laggy ...)
- Improve UI
- More runes
- Bosses
- Lore
- Think / handle player death (respawn or not ... ?)
- Improve custom editor
- Improve AI (sound, animations, code, behaviour, introduce machine learning, ranged ennemies, give AIs abilities, even think about doing custom editor node based AI FSM for heuristics)
- Ability ideas (try to find original abilities): (BTW these abilities could be used by ennemies)
    - Teleportation
    - Anti gravity
    - Traps
    - Push / Grab / Move / Build items, objects etc ...
    - Reflect abilities
    - Wololo
    - Rays
    - Channelled stuff
    - Laser link between players
    - Lightning abilities
    - Dashes
    - Clone or not original ?
    - A utility character more original than a healer
    - Invisiblity ?
    - What else ... ?


# Developers doc
If you want to contribute to this project feel free to contact Louis.
## Unity development
### Best practices
try to follow this:
- [Nice list of good practices](http://www.gamasutra.com/blogs/HermanTulleken/20160812/279100/50_Tips_and_Best_Practices_for_Unity_2016_Edition.php)
### Naming convention
Use JetBrains Rider, it will fix your naming convention automatically (following Unity naming convention)
### Few rules
- Always prefer TextMeshPro over Text
- Keep in mind that ScriptableObject are useful for storing serialized data kind of life a database but you shouldn't change the data during runtime, it's useful for fixed data like parameters
- Try to comment everything
- Be careful with object naming (especially in UI), it should be clear to understand what it represents 
- ...
### Create a new character

Use the custom editor character creator windows (evol/)

Create a CharacterData Scriptable object:
Create/Evol/Character and fill with the appropriate data

Don't forget to put the photon animator view as last component (it seems to be required for trigger parameters)

tweak the parameters (custom editor not finished)

### Create a new AI

Use the custom editor ai creator windows (evol/)

tweak the parameters (custom editor not finished)

### Create a new ability

Add these components to the prefab (can be different depending on the ability u're trying to do)

<img src="images/create_a_new_ability.png" width="300" height="300">

Create a AbilityData Scriptable object:
Create/Evol/Ability and fill with the appropriate data


### Useful links
#### Tutorials
- [Nice Unity tutorials](https://catlikecoding.com/unity/tutorials/)
#### Machine learning
- [Github](https://github.com/Unity-Technologies/ml-agents)
#### Network
##### PlayFab
- [Getting started special Unity](https://api.playfab.com/docs/getting-started/unity-getting-started)
- [Unity tutorials](https://api.playfab.com/tutorials/unity)

#### Photon
- [Custom auth](https://doc.photonengine.com/en-us/realtime/current/connection-and-authentication/authentication/custom-authentication)