using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JDH_WinCondition_script : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    public class Components
    {
        public Collider collider;
        public JDH_GameController_script gc;
        public int totalbuddysavedforunlock;


    }

    public Components component = new Components();

    public void Start()
    {
        component.collider = GetComponent<Collider>();
        component.gc = GetComponent<JDH_GameController_script>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            component.totalbuddysavedforunlock = collision.gameObject.GetComponent<JDH_Follow_script>().param.totalbuddysaved;
        Debug.Log("Game Finished");
        component.gc.NextLevel();

    }

    public void Update()
    {
        DontDestroyOnLoad(gameObject);
    }

}
