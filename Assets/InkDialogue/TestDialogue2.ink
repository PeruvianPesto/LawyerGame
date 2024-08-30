-> Begin

=== Begin
#anim think
#color blue
(Am I really ready?)
+ [Yes] -> Ready
+ [No] -> Not_Ready

=== Ready
#anim idle
#color blue
(I will find out the truth today)

#anim talk
#color white
The defense is ready, Your Honor!

I would like to present new evidence to the court!

#addingevidence "Knife" "Sprites/Test" "This is a knife"
Added Knife to court Record 
->DONE

=== Not_Ready
#anim talk
#color white
Not yet, Your Honor!

#color blue
(Let me see...)
->Begin