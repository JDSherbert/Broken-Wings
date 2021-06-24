using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JDH_HealthManager_script : MonoBehaviour
{

    public class HealthData
    {
        public int HP;
        public int MaxHP;
        public float invulnerableTimerMax = 1.0f;
        public float invtimer = 0;
        public int damage = 1;
 

    }

    [System.Serializable]
    public class Components
    {
        public GameObject[] hearts;
        public JDH_SideScrollerMovement_script playerscript;
        public AudioSource audiosource;
        public ParticleSystem ps;
    }

    public HealthData health = new HealthData();
    public Components component = new Components();

    public void Start()
    {
        Get();
    }

    public void Update()
    {
        CheckDamage();
        CheckDead();
    }

    public void CheckDamage()
    {
        if (component.playerscript.player.isDead == true)
        {
            Debug.Log("Dead.");
            component.ps.Play();
            health.HP = 0;
            for (int i = 0; i < component.hearts.Length; i++)
            {
                component.hearts[i].SetActive(false); //Issue here?
            }
            CheckDead();
        }
        else 
        {
            if (health.invtimer <= health.invulnerableTimerMax)
            {

                health.invtimer += 0.01f;
            }

            if (health.invtimer >= health.invulnerableTimerMax)
            {

                if (component.playerscript.player.isDamaged == true)
                {
                    Debug.Log("Damage taken.");
                    component.ps.Play();

                    health.HP -= health.damage;
                    health.invtimer = 0;

                    for (int i = 0; i < component.hearts.Length; i++)
                    {
                        if (i == health.HP)
                        {
                            component.hearts[i].SetActive(false);
                        }
                    }
                }
                else
                {
                    component.ps.Stop();
                }
            }
        }   




    }

    public void CheckDead()
    {
        if (health.HP == 0)
            component.playerscript.KillPlayer();
    }

    public void Get()
    {
        health.MaxHP = component.hearts.Length;
        health.HP = health.MaxHP;

        if (GameObject.FindWithTag("Player").GetComponent<JDH_SideScrollerMovement_script>())
            component.playerscript = GameObject.FindWithTag("Player").GetComponent<JDH_SideScrollerMovement_script>();

        if (GetComponent<AudioSource>())
            component.audiosource = GetComponent<AudioSource>();
            
    }
}
