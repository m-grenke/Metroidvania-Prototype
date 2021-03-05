using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour
{
    public string name;
    
    public int maxHP;
    public int currentHP;

    public int maxMP;
    public float currentMP; //fractional MP values possible

    public 

    // Use this for initialization
    bool freshSpawn = true;
    void Start ()
    {
        if(freshSpawn)
        {
            currentHP = maxHP;
            currentMP = maxMP;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    bool trueDamage = true;
    public void Damage(int damage)
    {
        if(trueDamage)
        {
            currentHP -= damage;
        }
    }


}
