using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    //instance fields
    private int type; //1 is blue, 2 is pink, 3 is black

    private bool isClicked;

    // Start is called before the first frame update
    void Start()
    {
        type = 3;
        isClicked = false;
    }

    void OnMouseDown()
    {
        Debug.Log("clicked");
        isClicked = true;    
    }

    public void cancelClick()
    {
        isClicked = false;
    }

    public bool getClick()
    {
        return isClicked;
    }

    public void changeType(int newType)
    {
        type = newType;
        setColor();
    }

    public int getType()
    {
        return type;
    }


    private void setColor()
    {
        if(type == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color32(51, 255, 255, 255);
        } else if(type == 2)
        {
            GetComponent<SpriteRenderer>().color = new Color32(255, 153, 204, 255);
        } else if(type == 3)
        {
            GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 255);
        }
    }
    
}
