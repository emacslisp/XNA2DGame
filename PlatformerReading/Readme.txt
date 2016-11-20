1.Game
(1).Read the configuration from "main.ini" file The sample main.ini is illustrated below:
[KeyBoard]
#KeyBoard for Game 
#you can only choose from a-zA-Z up,down,left,right.If you choose other charactors the game will not work.
up=up      
down=down
left=left
right=right
pause=p
quit=q

(2).Main Algorithm

If boundingbox of ball intersects with left blocks and right blocks. pongSpeed.X will be -pongSpeed.X. If boundingbox of ball intersects with up and down, pongSpeed.Y will be -pongSpeed.Y.

If boundingbox of ball intersects with 
