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
->DONE

=== Not_Ready
#anim talk
#color white
Not yet, Your Honor!

#color blue
(Let me see...)
->Begin