using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Version
{
    Normal,
    CloseBuildings,
    RegulateBuildings,
    CloseAndRegulateBuidings
}

public class GameValues : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameValues instance;
    void Start()
    {
        if(instance == null)
		{
            instance = this;
		}
		else
		{
            Destroy(this.gameObject);
		}
    }

    public System.Random random = new System.Random();

    //Will be set in the editor
    public int xhouses;
    public int yhouses;
    public int xstores;
    public int ystores;
    public int infectedMean;
    public int infectedSD;
    public int buildingSize;
    public int immuneTurns;
    public int characterAmount;
    public int dayTurns;
    public int stayAtHome;
    public float infectMultiplier;
    public float totalInfections = 0;

    public Version version;

    public int closePercentage;

    public int maximumAllowence;

    public List<float> previousInfections = new List<float>(); 
    public float RZero;


}
