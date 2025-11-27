using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridOrb
{
    public GameObject ob;
    public bool activated;

    public GridOrb(GameObject ob_input)
    {
        this.ob = ob_input;
        this.activated = false;
    }

    public void OrbSelected()
    {
        this.ob.GetComponent<Renderer>().material.color = Color.red;
        this.activated = true;
    }

    public void OrbDeselected()
    {
        this.ob.GetComponent<Renderer>().material.color = Color.white;
        this.activated = false;
    }
}