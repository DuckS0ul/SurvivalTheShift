using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint
{
    public string itemName;

    public string Req1;
    public string Req2;

    public int Req1amount;
    public int Req2amount;

    public int numOfRequirements;

    public int numberOfItemsProduce;

    public Blueprint(string name, int producedItems, int reqNUM, string R1, int R1num, string R2, int R2num)
    {
        itemName = name;

        numberOfItemsProduce = producedItems;

        numOfRequirements = reqNUM;

        Req1 = R1; 
        Req2 = R2;

        Req1amount = R1num;
        Req2amount = R2num;
    }
}
