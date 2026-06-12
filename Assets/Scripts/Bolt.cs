using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Bolt : MonoBehaviour
{
    private Renderer rendererComponent;
    private Material mat;
    private Color originalEmission;
    [SerializeField]
    private GameObject nut;
    private Animator nutAnimator;
    private void Start()
    {
        rendererComponent = GetComponent<Renderer>();
        mat = rendererComponent.material;
        originalEmission = mat.GetColor("_EmissionColor");

        nutAnimator = nut.GetComponent<Animator>();
    }
    public void Highlight(Color color)
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * 2f);

        //rendererComponent.material.color = color;
    }
    public void ResetHighlight()
    {
        mat.SetColor("_EmissionColor", originalEmission);

        //rendererComponent.material.color = Color.white;
    }
    public void Reset()
    {
        nut.SetActive(false);
    }
    public void PlaceNut()
    {
        nut.SetActive(true);
    }
    public void Screw()
    {
        nutAnimator.SetTrigger("screw");
    }
}