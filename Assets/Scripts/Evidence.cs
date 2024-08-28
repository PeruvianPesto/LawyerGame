using UnityEngine;

public class Evidence 
{
    public string name; // Name of the evidence
    public Sprite image; // Image of the evidence
    [TextArea]
    public string description; // Description of the evidence

    public Evidence(string name, Sprite image, string description)
    {
        this.name = name;
        this.image = image;
        this.description = description;
    }
}
