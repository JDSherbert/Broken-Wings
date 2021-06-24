using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JDH_Tally_script : MonoBehaviour
{
    [System.Serializable]
    public class Components
    {
        public TextMeshProUGUI buddycount;
        public int buddies;
        public int totalbuddiesingame = 7;
        public GameObject wincube;
        public GameObject developerplaygroundoption;
    }
    public Components component = new Components();


    // Start is called before the first frame update
    void Start()
    {
        component.wincube = GameObject.FindWithTag("Finish");
        component.buddies = component.wincube.GetComponent<JDH_WinCondition_script>().component.totalbuddysavedforunlock;
        component.buddycount.text = component.buddies.ToString();
        if(component.buddies >= component.totalbuddiesingame)
        {
            component.developerplaygroundoption.SetActive(true);
        }
    }

}
