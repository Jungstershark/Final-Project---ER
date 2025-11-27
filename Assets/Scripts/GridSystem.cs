using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

/* 
Ini
*/

public class GridSystem
{
    public Dictionary<List<(int, int)>, string> spellBook;
    public bool activated;
    public List<List<(int, int)>> grid;
    public List<List<GridOrb>> objectGrid;
    public (int, int) startPoint;
    public int totalRows;
    public int totalCols;

    public GridSystem(List<List<GridOrb>> objectGridInput, int rows = 3, int cols = 3)
    {
        this.spellBook = new Dictionary<List<(int, int)>, string>
        {
            { new List<(int, int)> { (0, 1), (1, 0), (1, 1), (1, 2), (2, 1) }, "Shatter" },
            { new List<(int, int)> { (0, 0), (0, 1), (0, 2), (1, 1), (2, 0), (2, 1), (2, 2), (1, 1), (0, 0)}, "Restart" },
            { new List<(int, int)> { (2, 0), (1, 0), (0, 0), (0, 1), (0, 2), (1, 2), (2, 2) }, "Magnet" }
        };

        this.activated = false;
        this.startPoint = (-1, -1);
        this.grid = new List<List<(int, int)>>();
        this.objectGrid = objectGridInput;
        this.totalRows = rows;
        this.totalCols = cols;

        for (int i = 0; i < rows; i++)
        {
            List<(int, int)> sublist = new List<(int, int)>();
            for (int j = 0; j < cols; j++)
            {
                sublist.Add((-1, -1));
            }
            this.grid.Add(sublist);
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

    private bool validatePointBounds(int row, int col)
    {
        if (row > this.totalRows - 1 || col > this.totalCols - 1)
        {
            return false;
        }
        return true;
    }

    public void drawLine((int, int) from, (int, int) to)
    {
        // add a line to the combination

        if (this.validatePointBounds(to.Item1, to.Item2) && this.validatePointBounds(from.Item1, from.Item2)) {
            if (!from.Equals(to)) {
                this.grid[from.Item1][from.Item2] = to;
                if (this.startPoint == (-1, -1))
                {
                    this.startPoint = from;
                    this.objectGrid[from.Item1][from.Item2].OrbSelected();
                }
                this.objectGrid[to.Item1][to.Item2].OrbSelected();
                Debug.Log($"Line drawn from {from} to {to}");
            }
        }
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