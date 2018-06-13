# Jido Multiplayer Sample App
## Getting Started
- Sign up for a Jido API key at www.jidomaps.com
- This sample app uses the Jido Unity Plugin for ARKit 1.5 and was built using Unity 2017.3.1, XCode Version 9.3, and Unity Networking.
- Clone this repository and open the enclosed Unity project. 
- Open the scene `Jido Multiplayer` and add your Jido API key to the `MapSession` Game Object
- Make sure to register the app with your Unity Multiplayer Networking in Services > Multiplayer
## Building the Sample App
- Build the Unity project  for iOS in File > Build Settings...
- The build will create a folder where your XCode project lives. In Finder, double click on the file called `pods.command`. This will automatically install the Jido pod and open the XCode workspace of your project.
- Set up your Signing for the app in XCode
- Attach an iPhone (6S and up) to XCode and build the project.
## Playing the Game
This sample app uses Unity Networking to provide real time gameplay synchronization and Jido to synchronize the AR components of the game. To play the game, you first start by connecting the devices over Unity Networking, then kick off Jido's AR tracking.
### Set up Networking
- Player 1 "Hosts" the game by pressing `Host Game`
- Player 2 presses `Find a Game` and selects Player 1's server from the dropdown. 
- Player 2 presses `Confirm Server` to join the game.
### Start Jido Tracking
- As prompted, each player taps the box that appears on their opponent
- When each player successfully taps their opponent, their corresponding prompt goes away.
- Once both players have tapped their opponent game immediately starts.
### Gameplay
- Each player can shoot the other player's ship by tapping on their screen. 
- A quick tap sends a slow laser beam, but if you hold your finger down longer the laser charges up and will shoot fast upon release
- Tapping the shield icon will cause a shield to appear for 3 seconds, shooting is disabled when your shield is up.

## Editing the Unity Project
This app was designed as a sample project and can also be used as a good code base to build your own multiplayer apps. Below is some more information about how this sample project works that may help when using this as a code base.
### General Overview
This sample app uses Unity Networking. (Note: If you are not familiar with Unity Networking, or multiplayer networking in general, we suggest to follow a simple tutorial like the one be found [here](https://unity3d.com/learn/tutorials/topics/multiplayer-networking/introduction-simple-multiplayer-example?playlist=29690) to familiarize yourself with the terminology.)

Unity Networking spawns a Player Prefab for each player in the network. Each of these Player Prefabs has a `Networked Transform` that will stay updated with the Players latest transform **in their own reference frame**. Since in AR, each player has their own reference frame, we cannot simply show this Networked Transform. Instead, we have a local `Player Model` that displays the remote player's transform **after it has been converted by Jido to the local reference frame**. The `Player Prefab` of remote players is therefore hidden and simply used as a pass-through to the local `Player Model` that is actually the one you interact with.

### Special Scripts and Game Objects
In order for Jido to keep the AR content synchronized, there are a few scripts and objects that must stay in place.
- Player Prefabs must have `Jido_Transform_Control.cs` and `FollowCamera.cs` attached
- Player Models must have the child Game object `Jido_Update` with a box collider scaled to (2,2,2) in **world coordinates** and the `Jido_Update_Transform.cs` script attached.
- When remote player's joint the game, they must add their Name to the`lookFor` Queue in the `Game Manager`. This let's the local player know that they need to sync up with that player.
- When Jido detects `person` objects in the scene, they must instantiate an `ObjectBoundsPrefab` at the detected position and scale. In this sample app that occurs in the `GameManager`.

### Useful Methods
- The `Jido_Transform_Control.cs` on each Player Prefab has a method called `GetLocalPosition`. This can be used by remote Player Prefab's to convert vectors from their own reference frame to the local one. 
	- One example use case of this would be if one player places a Game Object in the room, their remote versions can use this method to make the Game Object appear in the same physical location on all other devices.

## Notes
- 
