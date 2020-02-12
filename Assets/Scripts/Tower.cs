using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public bool isClicked = false;
    // Start is called before the first frame update
    private void OnMouseDown() {
        isClicked = true;
        Debug.Log("Activated");
    }

    public void OnMouseUp() {
        isClicked = false;
        Debug.Log("Deactived");
    }
}
