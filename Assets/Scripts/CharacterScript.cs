using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    public Animator anim; // Animator component reference for handling animations
    public bool isTalking; // Flag to indicate if the character is currently talking

    void Start()
    {
        anim = GetComponent<Animator>(); // Get the Animator component attached to the GameObject
        isTalking = false; // Initialize the talking state to false
    }

    public void PlayAnimation(string _name)
    {
        // Play the specified animation based on the provided name
        switch (_name)
        {
            case "idle":
                anim.SetTrigger("toIdle"); // Set trigger for idle animation
                break;
            case "talk":
                isTalking = true; // Set the character as talking
                anim.SetTrigger("toTalk"); // Set trigger for talking animation
                break;
            case "think":
                anim.SetTrigger("toThink"); // Set trigger for thinking animation
                break;
        }
    }
}
