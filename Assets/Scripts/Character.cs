using UnityEngine;
using System.Collections;

public enum Team
{
    NA, PLAYER, DEMON, ENVIRONMENT
}

public abstract class Character : MonoBehaviour {

    public InputData inputData;
    protected Humanoid humanoid;
    protected float maxHealth;
    protected float health;
    protected Team team;

	// Use this for initialization
	protected void Start () {
        this.humanoid = this.GetComponent<Humanoid>();
        this.maxHealth = 100.0f;
        this.health = 100.0f;
        this.team = Team.NA;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void damage(float hitPoints, Team team, Character attacker)
    {
        hitPoints = Mathf.Abs(hitPoints);
        this.health -= hitPoints;
        if (this.health < 0.0f)
        {
            //death
            this.health = 0.0f;
            this.death(attacker);
        }
        else {
            Debug.Log("Health: " + this.health);
        }
    }

    public void heal(float hitPoints)
    {
        hitPoints = Mathf.Abs(hitPoints);
        this.health += hitPoints;
        if (this.health > this.maxHealth)
        {
            this.health = this.maxHealth;
        }
        Debug.Log("Health: " + this.health);
    }

    protected void death(Character attacker)
    {
        Debug.Log(attacker + " has slain " + this);
    }

    override public string ToString()
    {
        return "default_character";
    }
}
