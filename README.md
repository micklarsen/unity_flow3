# Unity Flow 3 Handin

## Group

[Alexander Pihl](https://github.com/AlexanderPihl)  
[Jean-Poul](https://github.com/Jean-Poul)  
[Mick Larsen](https://github.com/micklarsen)  
[Morten Rasmussen](https://github.com/Amazingh0rse)  
[Per Kringelbach](https://github.com/cph-pk)

## Theme: Procedural Content Generation & Simple AI
  
![Gen](daw1.gif)  
  
### Simple Artificial Intelligence

Spawned NPC's will walk around, randomly, checking for collisions using raycast.
Check comments in snippet below

```cs

void Update() {
        StartWalking();
        CheckRayCollision();
}


void StartWalking() {
    //Move the character forward using deltatime to prevent fps from making problems.
    transform.Translate(Vector3.forward * (Time.deltaTime * 1.4f));
}

void CheckRayCollision() {

    //Get the NPC's current rotation and convert to eulerangle.
    Vector3 currentRotation = this.transform.localEulerAngles;
    Quaternion currentRotationQua = Quaternion.Euler(currentRotation);

    //Create a random rotation angle
    float newRotation = Random.Range(-110, 110);
    Quaternion newRotationQua = Quaternion.Euler(0, newRotation, 0);

    //Shoot a raycast forward from the NPC
    Ray ray = new Ray(transform.position, transform.forward);
    RaycastHit hit;

    //If the ray hits something within "10 units" the NPC will rotate to avoid collision
    if (Physics.SphereCast(ray, 1f, out hit)) {
        if (hit.distance < 10) {
            transform.rotation = Quaternion.Lerp(currentRotationQua, newRotationQua, Time.deltaTime * 15f);
        }
    }
 }
```

### Terrain generation

Utilized scripts for procedural content generation to create a textured mesh terrain.
To replace the voronai texture we added a public object that can hold a texture.

```cs
private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D((int)TileSize.x, (int)TileSize.y);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        //SELECT CUSTOM GROUND TEXTURE
        this.GetComponent<Renderer>().material.mainTexture = tx;
    }
```

### Random object generation

To spawn random objects in random places we created different, empty game objects responsible for spawning different objects.
One example could be the "Clutter" gameobject responsible for spawning rocks and trees.

The gameobject contains different types of clutter in an array and selects randomly between them within a certain range.

The functionality is explained with comments in this snippet:

```cs

//Array containing different prefabs
[SerializeField] private GameObject[] clutter;

private List<GameObject> houseList = new List<GameObject>();


public void Awake() {
    //Spawn objects! (See below)
    SpawnGrid();

    //Responsible for adding mesh colliders & rigidbody to spawned prefabs
    AddComponentsToInstance();
}


void SpawnGrid() {
//Start instantiating on X axis
for (int x = 0; x < m_width; x++) {

    //Instantiate m_height no of houses on Z
    for (int z = 0; z < m_height; z++) {

        //Select random range to spawn within
        int randRangeX = Random.Range(100, 900);
        int randRangeZ = Random.Range(100, 900);

        //Select random prefab from array
        int rand = Random.Range(0, clutter.Length);

        //Instantiate the prefab and make sure rotation is reset!
        GameObject houseInstance = Instantiate(clutter[rand], new Vector3(randRangeX,   100, randRangeZ), Quaternion.identity);

        //Rotate the spawned prefab randomly for variety's sake
        houseInstance.transform.Rotate(0, Random.Range(0, 359), 0);

        //Add spawned object to array so that AddComponentsToInstance() can loop through every object
        //And assign mesh colliders and rigidbody
        houseList.Add(houseInstance);
        }
    }
}
```
