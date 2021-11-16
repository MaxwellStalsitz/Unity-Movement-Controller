# Unity-Movement-Controller
Simple character controller based movement controller, with support for jumping, animation, and all around movement (along with slope detection)

Implementation is as simple as moving the script to an object (preferably a capsule/player) that contains a character controller component.
From there, you can tweak and change the preset settings as much as you would like. Thanks.

Explanation of variables:

Player and Orientation transform variables are meant for the player object, along with an "orientation" object, which is just a parent object
for the camera, like such:

(the orientation object should be empty and have default transform)

>Player
 >Orientation 
  >Camera

Walk Speed and Run Speed are self-explanitory, being used to controll the players movement speed.

Run build up is used to determine the speed/acceleration in which the player changes from walk to run

Move Smooth Time adjusts the smoothing when walking, the higher the value, the slower it appears for you to come to a halt.

Run Key determines which key is pressed in order to run

Jump Fall Off is an animation curve that determines how the player jumps/falls, I would suggest tweaking that a considerable amount.

Jump Multiplier decides how high the jump is being multiplied, the higher the value, the higher the jump.

Jump Key determines which key is pressed in order to jump

Slope Force is the force that is being added whenever the character is on a slope and moving

Slope Force Ray Length determines the distance from the ground that player has to be in order to add the slope force, which is used in the raycast
that determines whether or not to add the force

Lock Cursor, well, locks the cursor

Gravity Multiplier simply multiplies the gravity being added to the player.

And Finally, use animation determines whether or not to add animation to the player, which calls a function that can be customized depending on your 
specific needs.
