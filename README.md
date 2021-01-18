EGO v0.2.0

Changes from v0.1.1 to v0.2.0
-Rewrote most of the code base to current knowledge of Unity.
--General syntax clean up and naming standards.
--Avoided unnecessary GetComponent calls.
-Added enemies to fire the 'bullets'. These enemies can be extended by using coroutines to create more varied firing behaviours.

====Original Description====
--Introduction---
EGO is a simple shooter game project made to illustrate my current understanding of Unity. Use the arrow keys to move and the space key to fire. Your score, and ship size, grows with each asteroid hit.
Releasing the fire button with a full shield will fire a super shot to help clear some space to move if needed. Be careful though as the shield could save your bacon. Taking a hit without a shield up will end
the game.

--Relevant Illustrations--
This project illustrates my understanding of the following;
	* Object Orientated programming.
	* Encapsulation.
	* General understanding of C# syntax.
	* Use of delegates and events.
	* Exposing code elements to the Unity development environment.
	* Appropriately organising code.
	
--Notes---
All code files are located in the 'Scripts' folder of the 'Assets' within the project. Classes should be appropriately commented.

--Improvements--
This project represents a relatively small amount of hours spent working with Unity; here are a few things I would like to acknowledge in order to improve the project;
	* General polish - The games visuals are a little rough around the edges without many bells and whistles. I think UI improvements, such as a proper shield meter, would be my first port of call here.
	* Code collections - Since working with Unity I have become aware that having simple scripts that do a single tasks are beneficial to Unity's workflow. I think the project in its current format illustrates
									how I am used to working with classes that control and entire objects behaviour. Modulating this out would fit more with Unity; such as having a script solely for player movement
									attached to the player object.