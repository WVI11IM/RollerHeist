using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class IdleCharacterSkin : MonoBehaviour
{
   public int animationNumber;

   public Animator animator;

   public void RandomizeAnimationNumber()
   {
    animationNumber = Random.Range(0,2);

    animator.SetInteger("number",animationNumber);
   }
   
   
 
}
 
