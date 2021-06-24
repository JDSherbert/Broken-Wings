using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JDH_Follow_script : MonoBehaviour
{
    [System.Serializable]
    public class Params
    {
        // Tag for things we want to collect.
        public string thingTag = "Buddies";

        // Maximum amount of things we can collect.
        public int maxThings = 8;

        public TextMeshProUGUI tmprobuddyrescuecounter;
        public Collider collider;
        public Rigidbody rb;
        public AudioSource audiosource;
        public AudioClip buddyacquired;
        public AudioClip buddyrescued;
        public int totalbuddysaved = 0;


        // List to hold all collected things.
        public List<GameObject> collected;

    }

    public Params param = new Params();

    public void Start()
    {
        // Initialize lists. Makes sure they're not null and empty.
        param.collected = new List<GameObject>();
    }

    public void AddThing(GameObject obj)
    {
        param.audiosource.PlayOneShot(param.buddyacquired);
    }


    public void FreeBuddy()
    {
        if(param.collected.Count > 0)
        {
            for (int i = 0; i < param.collected.Count; i++)
            {
                if (param.collected[i] != null)
                {
                    param.totalbuddysaved++;
                    param.tmprobuddyrescuecounter.text = param.totalbuddysaved.ToString();
                    param.audiosource.PlayOneShot(param.buddyrescued);
                }
                Destroy(param.collected[i]);
            }
        }

    }
}
