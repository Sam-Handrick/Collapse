using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class BoardScript : MonoBehaviour {

	public Object block;

	//The UI Text object that shows how many blocks maximum have been burst by the player on screen
    public Text outTopBurst;

	//The UI Text object that shows how many blocks were burst by the player last succesful click on screen
	public Text outLastBurst;

	//The UI Text object that shows the elapsed time
	public Text outTime;

	GameObject[] allBlocks;

	private PieceScript pScript;

	//The int that tracks the most amount of blocks a player has destroyed in one click
    int topBurst;

	//The int that tracks the amount of blocks destroyed in the last succesful click by a player
	int lastBurst;

    //The int value that delineates how large a single row is.
    int rowSizeNum;

    //The int value that delineates how large a single column is
    int colSizeNum;

	//The Vectors for getting and transferring the mouse position to a usable vector for getting the users clicks.
	Vector3 pos3D;
	Vector2 pos2D;

	// Use this for initialization
	void Start () {
		//Setting both the 2D and 3D version of the position vector to zeroed vectors
		pos3D = new Vector3 (0, 0, 0);
		pos2D = new Vector2 (0, 0);

		//Setting the top burst and last burst values to zero initially, as players begin having not burst any blocks.
        topBurst = 0;
		lastBurst = 0;

		//A 10-block long, 15-block high game board works pretty well, giving 150 overall blocks to use in the game.
        rowSizeNum = 10;
        colSizeNum = 15;

		//Initializing the allBlocks array with the size corresponding to the rowSizeNum and colSizeNum, coming out to the overall size of blocks.
		allBlocks = new GameObject[rowSizeNum*colSizeNum];
		//Going through each number i which represents the overall amount of rows (the size of a column)
		for(int i=0;i<colSizeNum;i++)
		{	
			//Then going through each number j which represents the overall amount of columns (the size of a row)
			for(int j=0;j<rowSizeNum;j++)
			{
				//Then we go through our array and take the current row (i* the number of blocks in a row) plus the number j we are in the row and create a new block in that position
                allBlocks[(i * rowSizeNum) + j] = (GameObject)Instantiate(block, new Vector2(j - 2, i - 2), Quaternion.identity);
				//Then we get the script component for the PieceScript of the block and set its color to a random value
                pScript = allBlocks[(i * rowSizeNum) + j].GetComponent<PieceScript>();
				pScript.ChooseColor((int)(Random.value*3));
			}
		}
	}



	//This method checks if any of the blocks are colliding with the mouse position.
	 public int CollisionCheck(Vector2 pos){
		//First we iterate through all the blocks on the board
		for (int i=0; i< (colSizeNum*rowSizeNum); i++) {
			//If one of the blocks is overlapping with the point (the mouse position), when the method is called then it returns the index of the block overlapping with the mouse.
			if(allBlocks[i].GetComponent<Collider2D>().OverlapPoint(pos))
			{
				return i;
			}
		}
		//If it is not overlapping we just return -1 as a sign that there are no collisions.
		return -1;
	}

	//The method that registers a click on a block and checks if there are at least two more adjacent blocks
	public void RegisterClick(int index)
	{
		//We call the GetAllAdjacent method which will return a list of all blocks in the same adjacency group of the same color simplyby passing in the index for the block
		List<int> adjList = GetAllAdjacent (index);
		//Then we simply check to make sure there are at least 3 blocks in the adjacency group
		if (adjList.Count > 2) {
			//If it is 3 or above then we go through every block in the group to destroy it and update the lastBurst and potentially the topBurst
			for(int i=0;i<adjList.Count;i++)
			{
				//We're going to update the topBurst number if the length of the adjacents (amount popped) are more than the current topBurst number
				if (adjList.Count > topBurst)
				{
					topBurst = adjList.Count;
				}
				//Then, regardless whether or not it is the highest Burst yet, we make it the current Burst (lastBurst)
				lastBurst = adjList.Count;

				//Then we'll play the pop sound for the block right before it is destroyed
				allBlocks[adjList[i]].GetComponent<AudioSource>().Play();

				//Then we finally set the block to destroyed
                allBlocks[adjList[i]].GetComponent<PieceScript>().SetDestroyed();
			}
		}
	}

	//This method takes in the index of one block in the game board and checks how many adjacent blocks are the same color, then returns a List of adjacents
	List<int> GetAllAdjacent (int index)
	{
		//Making the array of block GameObject that will be made up of all adjacent blocks of the same color
		List<int> adjacents= new List<int>();

		//First we add the original block's index passed in to the List
		adjacents.Add (index);

		//Then we call the recursive RecursiveAdjacent method which goes through all the adjacent blocks and if they are the same color recursively checks if they have adjacent blocks of the same color, and so on.
		adjacents = RecursiveAdjacent (adjacents, index);

		return adjacents;
	}

	//The RecursiveAdjacent Method which takes the overall list of the adjacent blocks and the index of the current block being checked.
	List<int> RecursiveAdjacent (List<int> inList, int index)
	{
		//Checking the four possible adjacent blocks: the one on the index before, the one on the index after, and the ones one row's worth ahead and one row's worth behind.
		//For each of these we check the following things if it is to be added to the adjacency list: if the index is over the block array's max or under its min, if it is already in the adjacency list, and if it is destroyed. Then we simply check if it is the same color.
		//First checking the block one index ahead. For this and the next check we also make sure the block being compared is in the same row as the one being compared to (because we're using a 1D array for a 2D set up)
        if ((index + 1 < rowSizeNum*colSizeNum) && !inList.Contains(index + 1) && ((int)(index / rowSizeNum) == (int)((index + 1) / rowSizeNum)) && (!allBlocks[index + 1].GetComponent<PieceScript> ().GetDestroyed()))
        {
			if(allBlocks [index].GetComponent<PieceScript> ().GetComponent<Renderer>().material.color == allBlocks[index + 1].GetComponent<PieceScript> ().GetComponent<Renderer>().material.color)
			{
				inList.Add(index+1);
				inList= RecursiveAdjacent(inList,index+1);
			}
		}
		//Then checking the block one index behind
        if ((index - 1 >= 0) && !inList.Contains(index - 1) && ((int)(index / rowSizeNum) == (int)((index - 1) / rowSizeNum)) && (!allBlocks[index - 1].GetComponent<PieceScript>().GetDestroyed()))
        {
			if(allBlocks[index].GetComponent<PieceScript>().GetComponent<Renderer>().material.color == allBlocks [index - 1].GetComponent<PieceScript> ().GetComponent<Renderer>().material.color)
			{
				inList.Add(index-1);
				inList= RecursiveAdjacent(inList,index-1);
			}
		}
		//Then checking the block one row ahead
        if ((index + rowSizeNum < rowSizeNum*colSizeNum) && !inList.Contains(index + rowSizeNum) && (!allBlocks[index + rowSizeNum].GetComponent<PieceScript>().GetDestroyed()))
        {
            if (allBlocks[index].GetComponent<PieceScript>().GetComponent<Renderer>().material.color == allBlocks[index + rowSizeNum].GetComponent<PieceScript>().GetComponent<Renderer>().material.color)
			{
                inList.Add(index + rowSizeNum);
                inList = RecursiveAdjacent(inList, index + rowSizeNum);
			}
		}
		//And finally one row behind
        if ((index - rowSizeNum >= 0) && !inList.Contains(index - rowSizeNum) && (!allBlocks[index - rowSizeNum].GetComponent<PieceScript>().GetDestroyed()))
        {
            if (allBlocks[index].GetComponent<PieceScript>().GetComponent<Renderer>().material.color == allBlocks[index - rowSizeNum].GetComponent<PieceScript>().GetComponent<Renderer>().material.color)
			{
                inList.Add(index - rowSizeNum);
                inList = RecursiveAdjacent(inList, index - rowSizeNum);
			}
		}

		return inList;
	}

    //The method that checks to see if a block can be moved down, and if so, it does. It takes in the index of the block array to check each.
    void CheckGravity(int index)
    {
        //First, we check to make sure the block we're checking isn't destroyed
        if (!allBlocks[index].GetComponent<PieceScript>().GetDestroyed())
        //Next we check if it is in the bottom row, in which case it won't be moving down anyway
        if ((index >= rowSizeNum))
        {
            //Now we check the block on row below. If it is destroyed then we set it to the current block and set the current block to destroyed (as it is has moved down)
            if (allBlocks[index - rowSizeNum].GetComponent<PieceScript>().GetDestroyed())
            {
                allBlocks[index - rowSizeNum].GetComponent<PieceScript>().GetComponent<Renderer>().material.color = allBlocks[index].GetComponent<PieceScript>().GetComponent<Renderer>().material.color;
                allBlocks[index].GetComponent<PieceScript>().SetDestroyed();
                allBlocks[index - rowSizeNum].GetComponent<PieceScript>().UndoDestroyed();
            }
        }
    }

    //This method checks the top row and if there are any empty spaces, it immediatly fills them.
    void FillTop()
    {
        //We begin at the start of the top row and continue through each block in it
        for (int i = (colSizeNum * rowSizeNum) - rowSizeNum; i < rowSizeNum * colSizeNum; i++)
        {
            if(allBlocks[i].GetComponent<PieceScript>().GetDestroyed()){
                allBlocks[i].GetComponent<PieceScript>().UndoDestroyed();
                allBlocks[i].GetComponent<PieceScript>().ChooseColor((int)(Random.value*3));
            }
        }
    }

	//This method sets the position vectors we use to track the mouse position equal to the mouse position itself
	void UpdateMousePos(){
		//Getting the position of the mouse on screen, then simply setting the first two coordinates to the coordinates of the 2D vectors
		pos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		pos2D.x = pos3D.x;
		pos2D.y = pos3D.y;
		
		//Checking if the player has clicked the left mouse button
		if (Input.GetMouseButtonDown (0)) 
		{
			//Then we check if the mouse position is actually colliding with any block and get the index of that block if so
			int num = CollisionCheck(pos2D);
			//If the click was on an acual block, then we register the click, but if we get a -1 returned then we know it was not clicked on a block and we do not need to register the click
			if (num != -1) {
				RegisterClick (num);
			}
		} 
	}

    // Update is called once per frame
    void Update()
    {
		//We call the UpdateMousePos to check for block clicks and the mouse position.
		UpdateMousePos ();

	    //Checking each block in the array (going backwards from the highest value as that is the bottom) and having them fall down if there is a destroyed block under them
        for (int i = rowSizeNum*colSizeNum-1; i >= 0; i--)
        {
            CheckGravity(i);
        }

		//Then we call the FillTop method to check the top row and fill it with blocks if it is not already filled
        FillTop();

		//Setting the outTopBurst text for the UI equal to the topBurst variable
		outTopBurst.text = "" + topBurst;

		//And setting the outLastBurst text for the UI equal to the topBurst variable
		outLastBurst.text = "" + lastBurst;

		//Updating the elapsed time (in full seconds);
		outTime.text = "" + (int)(Time.time);
	}
}
