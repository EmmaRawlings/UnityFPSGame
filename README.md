# UnityFPSGame
A project I made in Unity to try my hand at FPS game mechanics, including player motion, aiming, AI, and dynamic environment details.

Included are two demos compiled to run on Windows, found inside the "Builds" folder. Feel free to try them out.

**Demos:**  
  *KingOfTheHillTest* - A map with some difficult geometry to aid in testing the platforming mechanics.  
    -It also has a simple turret that will track you as you navigate the terrain, and a camera that renders onto a panel nearby.  
    -Within the map are some physics boxes and bounce pads (indicated by transparent blue boxes).  
  *SinglePlayerTest* - A small map that demonstrates AI pathfinding in an arena.  
    -The AI will find the shortest route via nav meshes and can jump off or on to certain platforms using links between nav meshes.  
    -This AI will try to get to the little round disc on the floor, in the unity scene editor, you can move this disc around to change where the AI moves to.  
    -Also in the stage is a weapon for the player to use, a lifting door that closes behind you, and two linked portals on opposite sides of the map.  

**Controls are as follows:**  
  -move mouse to look around and aim  
  -W,A,S, and D to move forward, left, back, and right  
  -tap space to jump  
  -hold space while moving forward into a ledge to climb it (Space + W + look towards ledge)  
  -left click to fire weapon (if you have one equipped)  
  -middle click while aiming at a weapon to pick it up (little red/blue capsules)  
  -middle click while aiming at a small physics box to pick it up and click again to drop it (KingOfTheHill demo only)  
