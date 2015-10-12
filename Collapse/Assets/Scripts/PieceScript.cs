using UnityEngine;
using System.Collections;

//This script is attached to the block prefab and manages the information for a block being destroyed and for setting the block's color
public class PieceScript : MonoBehaviour {

    //The boolean that tells whether or not this piece has been destroyed and is awaiting replacement.
    private bool destroyed;

	// Use this for initialization
	void Start () {
		//Initially, all the blocks are created in a non-destoryed state.
        destroyed = false;
	}

	//The method that takes in an int from 0 to 2 and applies a color corresponding to the value. 0-blue, 1-red, 2-green
	public void ChooseColor(int value)
	{
		//This switch assigns colors depending on the values passed through. 0-blue, 1-red, 2-green, and if there is an int that is not any of those the block becomes gray.
		switch (value) {
		case 0:
			this.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
			break;

		case 1:
			this.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
			break;

		case 2:
			this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
			break;

		default:
			this.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//We give the destroyed bool some significance here by disabling the block's sprite from rendering when it is destroyed
        if (destroyed)
        {
            this.GetComponent<Renderer>().enabled = false;
        }

		//In the same vein as above, if it is not destroyed, then we enable the block's sprite
        if (!destroyed)
        {
            this.GetComponent<Renderer>().enabled = true;
        }
	}

	//This is a setting method used to set the destroyed boolean to true 
    public void SetDestroyed()
    {
        destroyed = true;
    }

	//This is a setting method used to set the destroyed boolean to false
    public void UndoDestroyed()
    {
        destroyed = false;
    }

	//This is a getting method used to get the value of the destroyed boolean.
    public bool GetDestroyed()
    {
        return destroyed;
    }
}
