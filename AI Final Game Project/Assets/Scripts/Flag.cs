using UnityEngine;

public class Flag : MonoBehaviour
{
    public MeshRenderer flagRenderer;
    Material flagMaterial;

    private void Awake()
    {
        flagMaterial = new Material(flagRenderer.material);
        flagRenderer.material = flagMaterial;
    }

    public void UpdateColor(Color color)
    {
        flagMaterial.color = color;
    }
}
