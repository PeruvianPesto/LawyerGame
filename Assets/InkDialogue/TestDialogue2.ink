-> Begin

=== Begin
#anim think
#color blue
#speaker Lawyer
(Am I really ready?)
+ [Yes] -> Ready
+ [No] -> Not_Ready

=== Ready
#anim idle
#color blue
#speaker Lawyer
(I will find out the truth today)

#anim talk
#color white
#speaker Lawyer
The defense is ready, Your Honor!

#color white
#speaker Prosecutor
The prosecution is ready, your Honor!

#color white
#speaker Prosecutor
I would also like to add evidence to the court.

#addingevidence "Knife" "Sprites/Test" "This is a knife"
#speaker 
Added Knife to court Record 
->DONE

=== Not_Ready
#anim talk
#color white
#speaker Lawyer
Not yet, Your Honor!

#color blue
#anim think
#speaker Lawyer
(Let me see...)
->Begin