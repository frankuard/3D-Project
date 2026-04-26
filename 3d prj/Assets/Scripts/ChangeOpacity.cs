using UnityEngine;

public class ChangeOpacity : MonoBehaviour
{
    void Start()
    {
        // Get the renderer of the sphere
        Renderer renderer = GetComponent<Renderer>();
        
        // Get the current color
        Color color = renderer.material.color;
        
        // Set alpha value (0.0 is fully transparent, 1.0 is fully opaque)
        color.a = 0.5f; 
        
        // Apply the new color back to the material
        renderer.material.color = color;
    }
}