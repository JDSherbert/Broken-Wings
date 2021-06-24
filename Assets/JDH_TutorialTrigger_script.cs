using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JDH_TutorialTrigger_script : MonoBehaviour
{
    [System.Serializable]
    public class Components
    {
        public GameObject textobject;
        public Collider collider;
    }

    public Components component = new Components();

    public void Start()
    {
        if (GetComponent<Collider>())
            GetComponent<Collider>();
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            component.textobject.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            component.textobject.SetActive(false);
        }
    }
}
