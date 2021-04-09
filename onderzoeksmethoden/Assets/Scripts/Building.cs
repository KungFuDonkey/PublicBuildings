using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures.Generic.FastLists;

public class Building : MonoBehaviour
{
    UnsortedDistinctList<Character> people;
    public bool house = false;
    public bool open = true;
    public int maximumPeople = int.MaxValue;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        people = new UnsortedDistinctList<Character>();
        mat = this.GetComponent<Renderer>().material;
    }

    public void UnRegisterPerson(Character character)
	{
        people.Remove(character);
	}
    public void RegisterPerson(Character character)
	{
        people.Add(character);
	}

    public void InfectPeople()
	{
        float infectionChance = InfectionChance() * GameValues.instance.infectMultiplier;
        List<Character> infected = infectedPeople();
        for(int i = 0; i < people.Count; i++)
		{
            if(GameValues.instance.random.NextDouble() < infectionChance)
			{
                people[i].GetInfected();
                infected[GameValues.instance.random.Next(infected.Count)].InfectPerson();
			}
		}
	}

    public float InfectionChance()
    {
        float infectedPeople = 0;
        for(int i = 0; i < people.Count; i++)
		{
            infectedPeople += people[i].state == CharacterState.infected ? 1 : 0;
		}
        if (house) infectedPeople *= 2;
        return infectedPeople / GameValues.instance.buildingSize;
    }

    List<Character> infectedPeople()
	{
        List<Character> infected = new List<Character>();
        for(int i = 0; i < people.Count; i++)
		{
            if (people[i].state == CharacterState.infected) infected.Add(people[i]);
		}
        return infected;
	}

    public void CloseBuilding()
	{
        open = false;
    }

    public void RegulateBuilding()
	{
        maximumPeople = GameValues.instance.maximumAllowence;
	}

	public void Restart()
	{
        people.Clear();
	}

    public bool CheckIfAllowed()
	{
        return people.Count < maximumPeople;
	}
}
