using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JDH_Buddy_script : MonoBehaviour
{
    public float killtime = 3.0f;
    public bool followplayer;
    public float buddyspeed = 5.0f;

    [System.Serializable]
    public class Components
    {
        public Collider collider;
        public GameObject player;
        public JDH_Follow_script followlogic;

        public AudioSource audiosource;
        public AudioClip buddydead;
        public AudioClip buddyrescued;
        public AudioClip buddyacquired;

        public SpriteRenderer spriterenderer;
        public ParticleSystem ps;
    }

    public Components component = new Components();

    public void Start()
    {
        Get();
    }

    public void LateUpdate()
    {
        if(followplayer == true)
        {
            transform.position = Vector3.Slerp(
                new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z),
                new Vector3(
                    component.player.transform.position.x,
                    component.player.transform.position.y,
                    component.player.transform.position.z),
                    buddyspeed * Time.deltaTime);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && followplayer == false)
        {
            followplayer = true;
            component.collider.isTrigger = true;
            component.followlogic.param.collected.Add(gameObject);
        }
        else if(collision.gameObject.tag == "Baddies" || collision.gameObject.tag == "Damage" || collision.gameObject.tag == "Instakill")
        {
            KillBuddy();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Baddies" || other.gameObject.tag == "Damage" || other.gameObject.tag == "Instakill")
        {
            KillBuddy();
        }
        if(other.gameObject.tag == "Vent")
        {
            component.followlogic.FreeBuddy();
        }
    }

    public void KillBuddy()
    {
        component.audiosource.PlayOneShot(component.buddydead);
        component.ps.Play();
        component.spriterenderer.enabled = false;
        component.collider.enabled = false;
        Destroy(gameObject, killtime);
    }

    public void Get()
    {
        if (GetComponent<Collider>())
            component.collider = GetComponent<Collider>();
        if (GetComponent<AudioSource>())
            component.audiosource = GetComponent<AudioSource>();
        if (GetComponentInChildren<SpriteRenderer>())
            component.spriterenderer = GetComponentInChildren<SpriteRenderer>();
        if (GetComponentInChildren<ParticleSystem>())
            component.ps = GetComponentInChildren<ParticleSystem>();
        if (GameObject.FindWithTag("Player"))
        {
            component.player = GameObject.FindWithTag("Player");
            component.followlogic = GameObject.FindWithTag("Player").GetComponent<JDH_Follow_script>();
        }

    }
}
