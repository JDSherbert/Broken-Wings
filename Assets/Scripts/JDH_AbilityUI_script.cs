using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JDH_AbilityUI_script : MonoBehaviour
{
    [System.Serializable]
    public class Components
    {
        public JDH_SideScrollerMovement_script playerscript;
        public GameObject dash;
        public GameObject doublejump;
        public GameObject walljump;
        public GameObject glide;
    }

    public Components component = new Components();

    public void Start()
    {
        Get();
    }

    public void Get()
    {
        if (GameObject.FindWithTag("Player").GetComponent<JDH_SideScrollerMovement_script>())
            component.playerscript = GameObject.FindWithTag("Player").GetComponent<JDH_SideScrollerMovement_script>();
    }

    public void FixedUpdate()
    {
        component.dash.SetActive(component.playerscript.ability.dash);
        component.doublejump.SetActive(component.playerscript.ability.doublejump);
        component.walljump.SetActive(component.playerscript.ability.walljump);
        component.glide.SetActive(component.playerscript.ability.glide);
    }
}
