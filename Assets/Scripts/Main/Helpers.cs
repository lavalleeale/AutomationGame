using System;
using UnityEngine;

public class Helpers
{
    static public string FormatNumber(int number)
    {
        if (number > 1000000)
            return Mathf.Floor(number / 1000000).ToString() + "M";
        else if (number > 1000)
            return Mathf.Floor(number / 1000).ToString() + "K";
        else
            return number.ToString();
    }
}
