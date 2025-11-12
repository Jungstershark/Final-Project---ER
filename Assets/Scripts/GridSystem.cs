using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/* 
Ini
*/

class GridSystem
{
    private Dictionary<List<(int, int)>, string> spellBook;
    private bool activated;
    public List<List<(int, int)>> grid;
    public List<List<GameObject>> objectGrid;
    public (int, int) startPoint;

    public GridSystem(int rows = 3, int cols = 3, List<List<GameObject>>)
    {
        this.spellBook = new Dictionary<List<(int, int)>, string>
        {
            { new List<(int, int)> { (0, 0), (0, 1), (0, 2) }, "Test 1" }
        };

        this.activated = false;
        this.startPoint = (-1, -1);
        this.grid = new List<List<(int, int)>>();
        for (int i = 0; i < rows; i++)
        {
            List<(int, int)> sublist = new List<(int, int)>();
            List<GameObject> sublist_object = new List<GameObject>();
            for (int j = 0; j < cols; j++)
            {
                sublist.Add((-1, -1));
                sublist_object.Add(GameObject);
            }
            this.grid.Add(sublist);
            this.objectGrid.Add(sublist_object);
        }
    }

    public void Test()
    {
        Debug.Log("Grid System working");
    }

    public bool isActivated()
    {
        return this.activated;
    }

    public void toggleActivation()
    {
        if (this.activated)
        {
            this.activated = false;
            Debug.Log("Grid System deactivated");
        }
        else
        {
            this.activated = true;
            Debug.Log("Grid System activated");
        }
    }

    public void drawLine((int, int) from, (int, int) to)
    {
        // add a line to the combination
        this.grid[from.Item1][from.Item2] = to;
        if (this.startPoint == (-1, -1))
        {
            this.startPoint = from;
        }
        Debug.Log($"Line drawn from {from} to {to}");
    }

    public void removeLine((int, int) from, (int, int) to)
    {
        // remove a line in the combination
        if (from == (this.startPoint))
        {
            this.startPoint = (-1, -1);
        }
        this.grid[from.Item1][from.Item2] = (-1, -1);
    }

    public List<(int, int)> currentCombination()
    {
        // returns current combination of points as a List
        List<(int, int)> output = new List<(int, int)>();
        (int, int) currentPoint = this.startPoint;

        while (currentPoint != (-1, -1))
        {
            output.Add(currentPoint);
            currentPoint = this.grid[currentPoint.Item1][currentPoint.Item2];
        }
        return output;
    }

    public string checkSpell()
    {
        // check if current combination matches any spells in spellBook. 
        // If yes, return spell name. Else, return null
        List<(int, int)> current = this.currentCombination();
        foreach (KeyValuePair<List<(int, int)>, string> spell in this.spellBook)
        {
            if (spell.Key.SequenceEqual(current))
            {
                return this.spellBook[spell.Key];
            }
        }
        return null;
    }
}