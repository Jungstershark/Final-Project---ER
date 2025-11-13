using Unity.VisualScripting;
using UnityEngine;

public class SpellTableOrbs : MonoBehaviour
{
    public GameObject orbObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OrbSelected()
    {
        orbObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public void OrbDeselected()
    {
        orbObject.GetComponent<Renderer>().material.color = Color.white;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
