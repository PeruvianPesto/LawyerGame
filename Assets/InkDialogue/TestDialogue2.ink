-> Begin

=== Begin
#anim think
#color blue
#speaker Lawyer
#addingevidence "Badge" "Sprites/Test" "A badge"
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

#color white
#speaker Judge
Let the witness speak!

->CrossExaminationStart

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

=== CrossExaminationStart
#speaker Witness
#color white
I have never touched that knife ever!

#speaker Witness
#color white
#cross_examination Knife
I am lying here; please contradict me with the knife.

#speaker Witness
#color white
So it could not be me.

#speaker Witness
#anim think
#color blue
Is he really telling the truth?

-> CrossExaminationStart

=== IncorrectAnswer
#speaker Judge
#color white
Lawyer! Make sure not to present wrong evidence or I will penalize you!

->CrossExaminationStart

===CorrectAnswer
#speaker Lawyer
#color white
Objection! This contradicts your statement!

-> END











