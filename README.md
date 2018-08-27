# Jido Multiplayer Sample App
## Getting Started
- Sign up for a Jido API key at www.jidomaps.com
- This sample app uses the Jido Unity Plugin for ARKit 1.5 and was built using Unity 2017.3.1, XCode Version 9.3, and Unity Networking.
- Clone this repository and open the root directory in Unity  
- Open the scene `Jido Multiplayer` and add your Jido API key to the `MapSession` Game Object as seen below
![](https://github.com/jidomaps/unity_multiplayer/blob/master/Assets/Api_Key.png)
- Make sure to register the app with your Unity Multiplayer Networking in Services > Multiplayer as seen below
![](https://github.com/jidomaps/unity_multiplayer/blob/master/Assets/RegisterMultiplayer.png)
## Building the Sample App
- Navigate to File > Build Settings... 
- Select platform as iOS and build the project
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

## Components
### JidoMultiplayer
The Unity Scene for this game is called JidoMultiplayer.

### MapSession
MapSession, nested under the JidoMapsComponent of the JidoMultiplayer has a public Developer Key property. Setting this property with a valid developer key is necessary authenticating with our API and running the app.

### GameManager
The Game Manager Game Object, linked to the GameManager.cs script, encapsulates the high-level state logic for the app. This component configures the app's main UI components and managing the current state of the app. This Game Manager script also controls the ARKit video and camera components that are activated and deactivated depending on whether or not the game is in play or paused. 

### Jido_Manager
The Game Manager Game Object is also linked to the Jido_Manager script which controls the logic for tracking and synchronizing the position of virtual objects and the two players. 

Jido_Manager has public variables for setting two prefabs: Scanned Object Bounds Prefab and Player Model Prefab. 

### ScannedObjectBounds
The Scanned Object Bounds Prefab is the rectangle prefab used for visualizing the position of the other player during the pre-game calibration step.

### PlayerModel
The PlayerModel Prefab defines a player’s avatar for the game. In this app, this Player Model Prefab is set to the spaceship prefab. 

The Player Model Prefab has a ModelController script component with public properties for a Shield prefab, Projectile Spawn Point (transform) and a World Space Health Bar. The projectile spawn point determines an offset, relative to a spaceship avatar, from which shot projectiles are spawned. This offset is helpful for making sure that a player's projectile doesn't collide with their own spaceship or shield. Among the Player Model's sub-components is a Jido_Model component with a Jido_Model script. The Jido_Model script is responsible for tracking the Player Model prefab and keeping the player-to-player transform accurate throughout the game. 

### Player Prefab
In addition to the Player Model Prefab which encapsulates visual characteristics and is necessary for visualizing each opposing player, a Player Prefab handles tracking, scoring and shooting logic for each player.

