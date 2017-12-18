using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    // VARIABLES

    // Texture to be used as start of CA input
    public Texture2D seedImage;
    
    // Number of frames to run which is also the height of the CA
    public int timeEnd = 2;
    int currentFrame = 0;

    //variables for size of the 3d grid
    int width;
    int length;
    int height;

    // Array for storing voxels
    GameObject[,,] voxelGrid;//???[,,]

    // Reference to the voxel we are using
    public GameObject voxelPrefab;

    // Spacing between voxels
    float spacing = 1.0f;

    //Layer Densities
    int totalAliveCells = 0;
    float layerdensity = 0;
    float[] layerDensities;//array
    float maxlayerdensity = 0;
    float minlayerdensity = 10000000;

    //DensityStructure
    //int alivecount = 0;

    //Max Age
    int maxAge = 0;

    //Max Densities
    int maxDensity3dMO = 0;
    int maxDensity3dVN = 0;

    // Setup Different Game of Life Rules
    GOLRule deathrule = new GOLRule();
    GOLRule rule1 = new GOLRule();
    GOLRule rule2 = new GOLRule();
    GOLRule rule3 = new GOLRule();
    GOLRule rule4 = new GOLRule();
    GOLRule rule5 = new GOLRule();
    GOLRule rule6 = new GOLRule();
    GOLRule rule7 = new GOLRule();
    GOLRule rule8 = new GOLRule();
    GOLRule rule9 = new GOLRule();
    GOLRule rule10 = new GOLRule();
    GOLRule rule11 = new GOLRule();
    GOLRule rule12 = new GOLRule();
    GOLRule rule13 = new GOLRule();
    GOLRule rule14 = new GOLRule();
    GOLRule rule15 = new GOLRule();
    GOLRule rule16 = new GOLRule();
    GOLRule rule17 = new GOLRule();
    GOLRule rule18 = new GOLRule();
    GOLRule rule19 = new GOLRule();

    int maxage = 5;
    //boolean switches
    //toggles pausing the game
    bool pause = false;
    int vizmode = 0;


    // FUNCTIONS

    // Use this for initialization
    void Start () {
        // Read the image width and height
        width = seedImage.width;
        length = seedImage.height;
        height = timeEnd;

        //Setup GOL Rules
        rule1.setupRule(1, 2, 2, 2); 
        rule2.setupRule(1, 2, 3, 3); //kill  rule !
        rule3.setupRule(1, 2, 3, 4);
        rule4.setupRule(1, 3, 3, 3); 
        rule5.setupRule(1, 3, 3, 6); 
        rule6.setupRule(2, 3, 3, 3); 
        rule7.setupRule(2, 3, 3, 4); 
        rule8.setupRule(3, 3, 2, 2); 
        rule9.setupRule(3, 4, 1, 1); 
        rule10.setupRule(3, 4, 3, 4); 
        rule11.setupRule(3, 6, 3, 3); 
        rule12.setupRule(4, 5, 2, 2);
        rule13.setupRule(5, 5, 1, 1);

        deathrule.setupRule(0, 0, 0, 0);

        //Layer Densities
        layerDensities = new float[timeEnd];//layerDensities Array

        // Create a new CA grid
        CreateGrid ();
        SetupNeighbors3d();
        Debug.Log(voxelGrid.Length);
        Debug.Log(totalAliveCells);
       
    }
    
    // Update is called once per frame
    void Update () {

        // Calculate the CA state, save the new state, display the CA and increment time frame
        if (currentFrame < timeEnd - 1)
        {
            if (pause == false)
            {
            
            // Calculate the future state of the voxels
            CalculateCA();
           

            // Update the voxels that are printing
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    GameObject currentVoxel = voxelGrid[i, j, 0];
                    currentVoxel.GetComponent<Voxel>().UpdateVoxel();
                }

            }
           
            
            // Save the CA state
            SaveCA();

            //Update 3d Densities
            updateDensities3d();

            DensityStructure();

            // Increment the current frame count
            currentFrame++;
            }

           
        }

        // Display the printed voxels
        // show different visual modes
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                for (int k = 1; k < height; k++)
                {
                   
                    if (vizmode == 0)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dVN(maxDensity3dVN);
                    }

                    if (vizmode == 1)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayAge(maxAge);
                    }

                    if (vizmode == 2)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayDensity3dMO(maxDensity3dMO);
                    }
                    if (vizmode == 3)
                    {
                        voxelGrid[i, j, k].GetComponent<Voxel>().VoxelDisplayLayerDensity(layerDensities[k], minlayerdensity, maxlayerdensity);////////////////////////////////////////////

                    }


                }
            }
        }

        // Export the voxel aggregation
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (GameObject currentGameObject in voxelGrid)
            {
                Voxel currentVoxel = currentGameObject.GetComponent<Voxel>();
                if (currentVoxel.GetState() == 0)
                {
                    Destroy(currentGameObject);
                }
            }
            print("Ready to export!");
        }




        KeyPressMethod();



    }




    /// <summary>
    /// Keies the press method.
    /// 
    /// </summary>
    public void KeyPressMethod(){
        // Spin the CA if spacebar is pressed (be careful, GPU instancing will be lost!)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameObject.GetComponent<ModelDisplay>() == null)
            {
                gameObject.AddComponent<ModelDisplay>();
            }
            else
            {
                Destroy(gameObject.GetComponent<ModelDisplay>());//这是什么意思？？？
            }
        }

        //toggle pause with "p" key
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == false)
            {
                pause = true;
            }
            else
            {
                pause = false;
            }
        }

        //toggle pause with "v" key
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (vizmode <= 3)
            {
                vizmode++;
            }
            if (vizmode >3)
            {
                vizmode = 0;
            }
        }
    }

    // Create grid function
        void CreateGrid(){
        // Allocate space in memory for the array
        voxelGrid = new GameObject[width, length, height];
        // Populate the array with voxels from a base image
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                for (int k = 0; k < height; k++) {
                    // Create values for the transform of the new voxel
                    Vector3 currentVoxelPos = new Vector3 (i*spacing,k*spacing,j*spacing);
                    Quaternion currentVoxelRot = Quaternion.identity;//旋

                    //create the game object of the voxel
                    GameObject currentVoxelObj = Instantiate (voxelPrefab, currentVoxelPos, currentVoxelRot);

                    //run the setupVoxel() function inside the 'Voxel' component of the voxelPrefab
                    //this sets up the instance of Voxel class inside the Voxel game object
                    currentVoxelObj.GetComponent<Voxel>().SetupVoxel(i,j,k,1);

                    // Set the state of the voxels
                    if (k == 0) {
                        // Create a new state based on the input image

                        float t = seedImage.GetPixel(i, j).grayscale;

                       /*
                        // white -> alive
                        if (t > 0.5f)
                            currentVoxelObj.GetComponent<Voxel>().SetState(1);
                        else
                            currentVoxelObj.GetComponent<Voxel>().SetState(0);
                      */

                        // or

                        // black - > alive
                        if (t > 0.8f)
                            currentVoxelObj.GetComponent<Voxel>().SetState(0);
                        else
                            currentVoxelObj.GetComponent<Voxel>().SetState(1);
                        
                        /*
                        // gray - > alive
                        if (t > 0.49 && t < 0.51)
                            currentVoxelObj.GetComponent<Voxel>().SetState(0);
                        else
                            currentVoxelObj.GetComponent<Voxel>().SetState(1);
                        */

                    } else {
                        // Set the state to death
                        currentVoxelObj.GetComponent<MeshRenderer>().enabled = false;
                        currentVoxelObj.GetComponent<Voxel> ().SetState (0);
                    }
                    // Save the current voxel in the voxelGrid array
                    voxelGrid[i,j,k] = currentVoxelObj;
                    // Attach the new voxel to the grid game object
                    currentVoxelObj.transform.parent = gameObject.transform;
                }
            }
        }
    }

    // Calculate CA function
    void CalculateCA(){
        // Go over all the voxels stored in the voxels array
        for (int i = 1; i < width-1; i++) {
            for (int j = 1; j < length-1; j++) {
                GameObject currentVoxelObj = voxelGrid[i,j,0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel> ().GetState ();
                int aliveNeighbours = 0;

                // Calculate how many alive neighbours are around the current voxel
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        GameObject currentNeigbour = voxelGrid [i + x, j + y,0];
                        int currentNeigbourState = currentNeigbour.GetComponent<Voxel> ().GetState();
                        aliveNeighbours += currentNeigbourState;
                    }
                }
                aliveNeighbours -= currentVoxelState;

                //CHANGE RULE BASED ON CONDITIONS HERE:
                GOLRule currentRule = rule19;
                //CHANGE RULE BASED ON CONDITIONS HERE:
                //..........

                //if (currentFrame > 40)
                //{
                //    currentRule = rule19;

                //}
                /*
                if (currentVoxelObj.GetComponent<Voxel>().GetAge()>3)
                {
                    currentRule = deathrule;
                }

                if (layerdensity < 0.2)
                {
                    currentRule = rule2;
                }
                */
                    //..........

                    //get the instructions
                int inst0 = currentRule.getInstruction(0);
                int inst1 = currentRule.getInstruction(1);
                int inst2 = currentRule.getInstruction(2);
                int inst3 = currentRule.getInstruction(3);

                // Rule Set 1: for voxels that are alive
                if (currentVoxelState == 1) {
                    // If there are less than two neighbours I am going to die
                    if (aliveNeighbours < inst0) {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (0);
                    }
                    // If there are two or three neighbours alive I am going to stay alive
                    if(aliveNeighbours >= inst0 && aliveNeighbours <= inst1)
                    {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (1);
                    }
                    // If there are more than three neighbours I am going to die
                    if (aliveNeighbours > inst1) {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (0);
                    }
                }
                // Rule Set 2: for voxels that are death
                if(currentVoxelState == 0){
                    // If there are exactly three alive neighbours I will become alive
                    if (aliveNeighbours >= inst2 && aliveNeighbours <= inst3)//if(aliveNeighbours >= inst2 && aliveNeighbours <= inst3)
                    {
                        currentVoxelObj.GetComponent<Voxel> ().SetFutureState (1);
                    }
                }

                //age - here is an example of a condition where the cell is "killed" if its age is above a threshhold
                // in this case if this rule is put here after the Game of Life rules just above it, it would override 
                // the game of lie conditions if this condition was true

                if (currentVoxelObj.GetComponent<Voxel>().GetAge() > 5)
                {
                    currentVoxelObj.GetComponent<Voxel>().SetFutureState(0);
                }


            }
        }
    }

    // Save the CA states - this is run after the future state of all cells is calculated to update/save
    //current state on the current level
    void SaveCA(){

        //counter stores the number of live cells on this level and is incremented below 
        //in the for loop for each cell with a state of 1
        totalAliveCells = 0;
        for(int i =0; i< width; i++){
            for (int j = 0; j < length; j++) {
                GameObject currentVoxelObj = voxelGrid[i, j, 0];
                int currentVoxelState = currentVoxelObj.GetComponent<Voxel>().GetState();
                // Save the voxel state
                GameObject savedVoxel = voxelGrid[i, j, currentFrame];
                savedVoxel.GetComponent<Voxel> ().SetState (currentVoxelState);                
                // Save the voxel age if voxel is alive
                if (currentVoxelState == 1) {
                   
                    int currentVoxelAge = currentVoxelObj.GetComponent<Voxel>().GetAge();
                    savedVoxel.GetComponent<Voxel>().SetAge(currentVoxelAge);

                    totalAliveCells++;
                  
                    //track oldest voxels
                    if (currentVoxelAge>maxAge)
                    {
                        maxAge = currentVoxelAge;
                    }
                }
            }

         
        }

        float totalcells = length * width;
        layerdensity = totalAliveCells/ totalcells;

        if(layerdensity>maxlayerdensity){
            maxlayerdensity = layerdensity;
        }

        if (layerdensity < minlayerdensity)
        {
            minlayerdensity = layerdensity;
        }


        //this stores the density of live cells for each entire layer of cells(each level)
        layerDensities[currentFrame] = layerdensity;
    }


    /// <summary>
    /// SETUP MOORES & VON NEUMANN 3D NEIGHBORS
    /// </summary>
    void SetupNeighbors3d()
    {
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < length-1; j++)
            {
                for (int k = 1; k < height-1; k++)
                {
                    //the current voxel we are looking at...
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    ////SETUP Von Neumann Neighborhood Cells////
                    Voxel[] tempNeighborsVN = new Voxel[6];

                    //left
                    Voxel VoxelLeft = voxelGrid[i - 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelLeft(VoxelLeft);
                    tempNeighborsVN[0] = VoxelLeft;

                    //right
                    Voxel VoxelRight = voxelGrid[i + 1, j, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelRight(VoxelRight);
                    tempNeighborsVN[2] = VoxelRight;

                    //back
                    Voxel VoxelBack = voxelGrid[i, j - 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBack(VoxelBack);
                    tempNeighborsVN[3] = VoxelBack;

                    //front
                    Voxel VoxelFront = voxelGrid[i, j + 1, k].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelFront(VoxelFront);
                    tempNeighborsVN[1] = VoxelFront;

                    //below
                    Voxel VoxelBelow = voxelGrid[i, j, k - 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelBelow(VoxelBelow);
                    tempNeighborsVN[4] = VoxelBelow;

                    //above
                    Voxel VoxelAbove = voxelGrid[i, j, k + 1].GetComponent<Voxel>();
                    currentVoxelObj.GetComponent<Voxel>().setVoxelAbove(VoxelAbove);
                    tempNeighborsVN[5] = VoxelAbove;

                    //Set the Von Neumann Neighbors [] in this Voxel
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dVN(tempNeighborsVN);

                    ////SETUP Moore's Neighborhood////
                    Voxel[] tempNeighborsMO = new Voxel[26];

                    int tempcount = 0;
                    for (int m = -1; m < 2; m++)
                    {
                        for (int n = -1; n < 2; n++)
                        {
                            for (int p = -1; p < 2; p++)
                            {
                                if ((i + m >= 0) && (i + m < width) && (j + n >= 0) && (j + n < length) && (k + p >= 0) && (k + p < height))
                                {
                                    GameObject neighborVoxelObj = voxelGrid[i + m, j + n, k + p];
                                    if (neighborVoxelObj != currentVoxelObj)
                                    {
                                        Voxel neighborvoxel = voxelGrid[i + m, j + n, k + p].GetComponent<Voxel>();
                                        tempNeighborsMO[tempcount] = neighborvoxel;
                                        tempcount++;
                                    }
                                }
                            }
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setNeighbors3dMO(tempNeighborsMO);
                }
            }
        }
    }
   
    /// <summary>
    /// Update 3d Densities for Each Voxel
    /// </summary>
    void updateDensities3d()
    {
        for (int i = 1; i < width-1; i++)
        {
            for (int j = 1; j < length-1; j++)
            {
                for (int k = 1; k < currentFrame; k++)
                {
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    //UPDATE THE VON NEUMANN NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsVN = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dVN();
                    int alivecount = 0;

                    foreach (Voxel vox in tempNeighborsVN)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dVN(alivecount);
                    if (alivecount> maxDensity3dVN) {
                        maxDensity3dVN = alivecount;
                    }

                    //UPDATE THE MOORES NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    Voxel[] tempNeighborsMO = currentVoxelObj.GetComponent<Voxel>().getNeighbors3dMO();
                   // alivecount = 0;
                   
                    foreach (Voxel vox in tempNeighborsMO)
                    {
                        if (vox.GetState() == 1)
                        {
                            alivecount++;
                        }
                   
                    }
                    currentVoxelObj.GetComponent<Voxel>().setDensity3dMO(alivecount);
                    if (alivecount > maxDensity3dMO)
                    {
                        maxDensity3dMO = alivecount;
                    }
                }
            }
        }
    }
   
    void DensityStructure()
    {
        float Densitytotal;
        int alivetotalcount = 0;

       
        for (int i = 1; i < width ; i++)
        {
            for (int j = 1; j < length ; j++)
            {
                for (int k = 1; k < timeEnd ; k++)
                {
                    GameObject currentVoxelObj = voxelGrid[i, j, k];

                    //UPDATE THE VON NEUMANN NEIGHBORHOOD DENSITIES FOR EACH VOXEL//
                    int Densitytotalstate = currentVoxelObj.GetComponent<Voxel>().GetState();


                    if (Densitytotalstate == 1)
                        {
                            alivetotalcount++;
                        }
                  

                    Densitytotal = (float)alivetotalcount / voxelGrid.Length;
                }
            }
        }

        Densitytotal = (float)alivetotalcount / voxelGrid.Length;
        // Debug.Log(Densitytotal);
        Debug.Log(Densitytotal);
    }
 
    /// <summary>
    /// TESTING VON NEUMANN NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void VonNeumannLookup()
    {
        //color specific voxel in the grid - [1,1,1]
        GameObject voxel_1 = voxelGrid[1, 1, 1];
        voxel_1.GetComponent<Voxel>().SetState(1);
        voxel_1.GetComponent<Voxel>().VoxelDisplay(1, 0, 0);

        //color specific voxel in the grid - [10,10,10]
        GameObject voxel_2 = voxelGrid[10, 10, 10];
        voxel_2.GetComponent<Voxel>().SetState(1);
        voxel_2.GetComponent<Voxel>().VoxelDisplay(1, 0, 0);

        //get neighbor right and color green
        Voxel voxel_1right = voxel_1.GetComponent<Voxel>().getVoxelRight();
        voxel_1right.SetState(1);
        voxel_1right.VoxelDisplay(0, 1, 0);

        //get neighbor above and color green
        Voxel voxel_1above = voxel_1.GetComponent<Voxel>().getVoxelAbove();
        voxel_1above.SetState(1);
        voxel_1above.VoxelDisplay(1, 0, 1);

        //get neighbor above and color magenta
        Voxel voxel_2above = voxel_2.GetComponent<Voxel>().getVoxelAbove();
        voxel_2above.SetState(1);
        voxel_2above.VoxelDisplay(1, 0, 1);

        //get all VN neighbors of a cell and color yellow
        //color specific voxel in the grid - [12,12,12]
        GameObject voxel_3 = voxelGrid[12, 12, 12];
        Voxel[] tempVNNeighbors = voxel_3.GetComponent<Voxel>().getNeighbors3dVN();
        foreach (Voxel vox in tempVNNeighbors)
        {
            vox.SetState(1);
            vox.VoxelDisplay(1, 1, 0);
        }

    }


    /// <summary>
    /// TESTING MOORES NEIGHBORS
    /// We can look at the specific voxels above,below,left,right,front,back and color....
    /// We can get all von neumann neighbors and color
    /// </summary>
    /// 
    void MooreLookup()
    {
        //get all MO neighbors of a cell and color CYAN
        //color specific voxel in the grid - [14,14,14]
        GameObject voxel_1 = voxelGrid[14, 14, 14];
        Voxel[] tempMONeighbors = voxel_1.GetComponent<Voxel>().getNeighbors3dMO();
        foreach (Voxel vox in tempMONeighbors)
        {
            vox.SetState(1);
            vox.VoxelDisplay(0, 1, 1);
        }

    }
}
