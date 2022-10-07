using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuertaGeneral : MonoBehaviour
{
    public Animator puerta;
    private bool enZona;
    private bool active;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit") && enZona == true)
        {
            active = !active;

            if(active == true)
            {
                puerta.SetBool("puertaActive", true);
            }

            if(active == false)
            {
                puerta.SetBool("puertaActive", false);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag == "Player")
        {
            enZona = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.tag == "Player")
        {
            enZona = false;
        }
    }
}
