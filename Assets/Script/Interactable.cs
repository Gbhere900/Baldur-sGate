using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface Interactable 
{
   
    public void Interact(Action OnActionCompeted);
   
}
