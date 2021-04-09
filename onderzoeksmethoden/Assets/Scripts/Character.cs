using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Building Home;
    public CharacterState state = CharacterState.healthy;
    int infectedTurns = 0;
    int immuneTurns = 0;
    Building currentBuilding;
    public int rval;
    Material mat;


    // Start is called before the first frame update
    void Start()
    {
        mat = this.GetComponent<Renderer>().material;
        state = CharacterState.healthy;
        mat.color = Color.cyan;
    }


    Vector3 offsetVec = Vector3.up;
    public void ChooseBuilding(Building nextBuilding)
    {
        int random = GameValues.instance.random.Next(100);
        if (random < GameValues.instance.stayAtHome || !nextBuilding.open || !nextBuilding.CheckIfAllowed())
		{
            MoveHome();
		}
		else
		{
            GotoBuilding(nextBuilding);
		}


    }

    public void MoveHome()
	{
        GotoBuilding(Home);
	}

    void GotoBuilding(Building building)
	{
        if (currentBuilding != null) currentBuilding.UnRegisterPerson(this);
        currentBuilding = building;
        currentBuilding.RegisterPerson(this);

        float xoffset = (float)(GameValues.instance.random.NextDouble() - 0.5) * 10;
        float yoffset = (float)(GameValues.instance.random.NextDouble() - 0.5) * 10;
        offsetVec.x = xoffset;
        offsetVec.z = yoffset;


        this.transform.position = currentBuilding.transform.position + offsetVec;
    }


    public void GetInfected()
    {

		if (state == CharacterState.healthy)
		{
            state = CharacterState.infected;
            mat.color = Color.red;

            double u1 = GameValues.instance.random.NextDouble();
            double u2 = GameValues.instance.random.NextDouble();

            double rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

            double turns = GameValues.instance.infectedMean + GameValues.instance.infectedSD * rand_std_normal;

            infectedTurns = (int)turns;
            GameValues.instance.totalInfections++;
        }

    }

    public void Heal()
	{
		if (state == CharacterState.infected)
		{
            infectedTurns--;
            if(infectedTurns <= 0)
			{
                state = CharacterState.immune;
                mat.color = Color.green;
                immuneTurns = GameValues.instance.immuneTurns;
                GameValues.instance.totalInfections--;
			}
		}
		else if (state == CharacterState.immune)
		{
            immuneTurns--;
            if(immuneTurns <= 0)
			{
                state = CharacterState.healthy;
                mat.color = Color.cyan;
			}

		}
	}

    public void InfectPerson()
	{
        rval++;
	}
}
