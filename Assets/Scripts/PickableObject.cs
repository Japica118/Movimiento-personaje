using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
   public bool isPickable = true;

   void OnTriggerEnter(Collider collision)
   {
       if(collision.gameObject.tag == "InteractionZone")
       {
           collision.gameObject.GetComponentInParent<ThirdPersonController>().objectToPick = this.gameObject;
       }
   }

   void OnTriggerExit(Collider collision)
   {
       if(collision.gameObject.tag == "InteractionZone")
       {
           collision.gameObject.GetComponentInParent<ThirdPersonController>().objectToPick = null;
       }
   }
}
