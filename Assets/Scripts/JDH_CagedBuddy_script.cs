using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JDH_CagedBuddy_script : MonoBehaviour
{
    [System.Serializable]
    public class Components
    {
        public Collider collider;
        public AudioSource audiosource;
        public AudioClip cageshakesfx;
        public AudioClip cagedestroysfx;
        public GameObject buddy;

    }
    [System.Serializable]
    public class Behaviours
    {
        public bool isDestroyed;
        public float shaketimer;
        public float shaketimermax = 3.0f;
    }

    public Behaviours behaviour = new Behaviours();
    public Components component = new Components();

    public void Start()
    {
        Get();
    }

    public void LateUpdate()
    {
        behaviour.shaketimer += 0.1f;

        if(behaviour.shaketimer >= behaviour.shaketimermax)
        {
            component.audiosource.PlayOneShot(component.cageshakesfx);
            behaviour.shaketimer = 0;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<JDH_SideScrollerMovement_script>().player.isDashing == true)
        {
            CheckDestroy();
        }
    }

    public void CheckDestroy()
    {
        behaviour.isDestroyed = true;
        Debug.Log("Buddy Released.");
        component.audiosource.PlayOneShot(component.cagedestroysfx);
        Instantiate(component.buddy, new Vector3 (transform.position.x, transform.position.y, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void Get()
    {
        if(GetComponent<Collider>())
            component.collider = GetComponent<Collider>();
        if (GetComponent<AudioSource>())
            component.audiosource = GetComponent<AudioSource>();

    }

    //Player breaks cage by dashing into it
    //Drops 1 buddy
    //Destroy all gameobject, instantiate one buddy




}
