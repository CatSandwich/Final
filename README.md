# Pong - Final for Networking
This repository stores the assets and code for our networked implementation of the two player game pong.

## Repo Layout
The networking follows a client-server topology, so the code falls into three core components: Client, Server and Common. 
Common stores the majority of the data types and id enums. The game logic is controlled on the server, almost entirely within GameManager.cs.

### Common
This component stores everything needed by both the Client and the Server. This includes Tags (enums to differentiate packet types) for ClientToServer and ServerToClient. 
All of the shared data types are also in Common. This includes basic types such as Vector3, Vector2 and Box, while also including full packet 
data types such as MoveObjectData or PaddlePositionData.

There is a post-build hook set up to copy the library to the Unity assets folder, as well as alongside the DarkRift server executable, 
so changes are automatically reflected to the client and server upon build.

### Server
The server runs the entire game, allowing input (Paddle position) from clients, and sending back information they need to display the game. In this respect, we follow the principle
that clients are just priviliged spectators. The GameController.cs script stores the core game logic, including a thread-safe async core game loop run on a thread pool upon instantiation.

This project has a post-build hook to copy the library to the plugins folder of the DarkRift server. After build, it is set to run the external DarkRift server executable. With
this and the post-build events from the common project, all server development can be done within an IDE of choice.

### Client
The client is just a spectator of the pong game with the ability to send a vertical position of their paddle. On update, the client tells the server their paddle position. Asides from
that, the rest is just allowing the server to move and resize local objects to conform to the state of the game on the server.

## My personal contribution (CatSandwich)
As this was a group project, I didn't do everything, so I will highlight what I did. We split the project into three main components: Server, Client and Communication. These roles
eventually got meddled a bit and I ended up doing the server while the others worked together on the client and receiving calls from the server.

I worked on:
- The server logic
- The server inbound and outbound communication handling
- The shared data definitions

My highlight for this project isn't any specific piece of code, but rather the architecture and structure of the server. As I do more and more networked projects, I get more comfortable
with architecture and setup. The server, where possible, follows MVC architecture principles. The model consists of the data definitions. The view consists of the plugin message
and client handlers which redirect the data to the controller, the game itself. This architecture makes for easy-to-understand code which would be very simple to work with to expand
at a later point.
