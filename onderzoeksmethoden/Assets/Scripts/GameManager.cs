using System;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.Generic.FastLists;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Building> buildings;
    public List<Building> houses;
    GraphManager graphManager;
    List<Character> characters;
    int turn = 0;
    bool started = false;
    int moveTurn = 0;
    float RValue = 0;
    // Start is called before the first frame update


    void Start()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        GameObject gm = GameObject.Find("GraphManager");
        graphManager = gm.GetComponent<GraphManager>();
        gm.SetActive(false);

        buildings = new List<Building>();
        CreateStores();

        houses = new List<Building>();
        CreateHouses();

        characters = new List<Character>();
        SetupCharacters();

        if(GameValues.instance.version == Version.CloseAndRegulateBuidings || GameValues.instance.version == Version.CloseBuildings)
		{
			CloseBuildings();
		}

        if(GameValues.instance.version == Version.RegulateBuildings || GameValues.instance.version == Version.CloseAndRegulateBuidings)
		{
			RegulateBuildings();
		}
	}

	private void RegulateBuildings()
	{
		for (int i = 0; i < buildings.Count; i++)
		{
			buildings[i].RegulateBuilding();
		}
	}

	private void CloseBuildings()
	{
		for (int i = 0; i < buildings.Count; i++)
		{
			if (GameValues.instance.random.Next(100) < GameValues.instance.closePercentage)
			{
				buildings[i].CloseBuilding();
			}
		}
	}

	float Movestep = 0, InfectStep = 2, STEP = 4;
    // Update is called once per frame
    void Update()
    {
        Movestep -= Time.deltaTime;
        if(Movestep < 0)
		{
            Move();
            Movestep = STEP;
		}
        InfectStep -= Time.deltaTime;

        if (InfectStep < 0)
		{
			UpdateRValue();
			UpdateGraphManager();
			Infect();
			Heal();
			InfectStep = STEP;

		}
	}

	void UpdateGraphManager()
	{
		graphManager.AddState(characters, RValue);
	}

	private void UpdateRValue()
	{
		if (started)
		{
			RValue = GetRvalue();
			Debug.Log(string.Format("R: {0}", RValue));
		}
	}

	private float GetRvalue()
	{
		int patients = 0;
		int rvals = 0;
		for (int i = 0; i < characters.Count; i++)
		{
			if (characters[i].state == CharacterState.infected)
			{
				rvals += characters[i].rval;
				patients++;
			}
		}
		return (float)rvals / (float)patients;
	}

	void SetupCharacters()
	{
        GameObject CharacterObject = Resources.Load<GameObject>("Prefabs/Character");
        for (int i = 0; i < GameValues.instance.characterAmount; i++)
		{
            GameObject p = Instantiate(CharacterObject);
            Character c = p.GetComponent<Character>();
            c.Home = houses[GameValues.instance.random.Next(houses.Count)];
            characters.Add(c);
		}
	}

    public void RegisterBuilding(Building building)
	{
        buildings.Add(building);
	}

    public void BeginSpread()
	{
        started = true;
        characters[GameValues.instance.random.Next(characters.Count)].GetInfected();
        Debug.Log("Start");
	}

    //not used yet
    public void CreateHouses()
	{
        GameObject house = Resources.Load<GameObject>("Prefabs/House");
        for(int x = 0; x < GameValues.instance.xhouses; x++)
		{
            for(int y = 0; y < GameValues.instance.yhouses; y++)
			{
                GameObject h = Instantiate(house, new Vector3(x * 15, 0, -(y + 1) * 15), Quaternion.identity, null);
                Building b = h.GetComponent<Building>();
                houses.Add(b);
			}
		}
	}
    public void CreateStores()
	{
        GameObject store = Resources.Load<GameObject>("Prefabs/Store");
        for(int x = 0; x < GameValues.instance.xstores; x++)
		{
            for(int y = 0; y < GameValues.instance.ystores; y++)
			{
                GameObject s = Instantiate(store, new Vector3(x * 15, 0, y * 15), Quaternion.identity, null);
                Building b = s.GetComponent<Building>();
                buildings.Add(b);
			}
		}
	}

    void Move()
	{
        if(moveTurn > GameValues.instance.dayTurns)
		{
			Debug.Log("HOME");
			moveTurn = 0;
			MoveCharactersHome();
			return;
		}
		Debug.Log("MOVE");
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].ChooseBuilding(buildings[GameValues.instance.random.Next(buildings.Count)]);
        }
        moveTurn++;

    }

	private void MoveCharactersHome()
	{
		for (int i = 0; i < characters.Count; i++)
		{
			characters[i].MoveHome();
		}
	}

	void Infect()
	{
        Debug.Log("INFECT");
        for (int i = 0; i < buildings.Count; i++)
        {
            buildings[i].InfectPeople();
        }
        for(int i = 0; i < houses.Count; i++)
		{
            houses[i].InfectPeople();
		}
    }

	public void Stop()
	{
        Debug.Log("STOP");
        graphManager.Stop();
        started = false;
        graphManager.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

	void Heal()
	{
        Debug.Log("HEAL");
        for(int i = 0; i < characters.Count; i++)
		{
            characters[i].Heal();
		}
	}

    public void Restart()
	{
        for(int i = 0; i < characters.Count; i++)
		{
            Destroy(characters[i].gameObject);
		}
        for(int i = 0; i < buildings.Count; i++)
		{
            buildings[i].Restart();
		}
        for(int i = 0; i < houses.Count; i++)
		{
            houses[i].Restart();
		}
        graphManager.Restart();
        graphManager.gameObject.SetActive(false);
        characters = new List<Character>();
        SetupCharacters();
        Movestep = 0;
        moveTurn = 0;
        InfectStep = 2;
	}
}
