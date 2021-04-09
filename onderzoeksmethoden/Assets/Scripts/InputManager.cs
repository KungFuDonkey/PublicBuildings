using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) SpeedUp();
        if (Input.GetKeyDown(KeyCode.Q)) SlowDown();
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
            this.transform.position += 100 * this.transform.up * Time.deltaTime / Time.timeScale;
		}
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
            this.transform.position -= 100 * this.transform.up * Time.deltaTime / Time.timeScale;
		}
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
		{
            this.transform.position += 100 * this.transform.right * Time.deltaTime / Time.timeScale;
		}
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
            this.transform.position -= 100 * this.transform.right * Time.deltaTime / Time.timeScale;
		}
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameManager.instance.BeginSpread();
        }
        if (Input.GetKeyDown(KeyCode.R))
		{
            GameManager.instance.Restart();
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			Time.timeScale = 100;
		}
		if (Input.GetKey(KeyCode.Z))
		{
            GameManager.instance.Stop();
		}
    }

    void SpeedUp()
	{
        Time.timeScale = Time.timeScale + 1 > 100 ? 100 : Time.timeScale + 1;
        Debug.Log(Time.timeScale);
	}

    void SlowDown()
	{
        Time.timeScale = Time.timeScale - 1 < 0 ? 0 : Time.timeScale - 1;
	}
}
