using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortkeys : MonoBehaviour
{
    public GameObject Player;
    void Start()
    {
        
    }

    
    void Update()
    {
        Spawn();
    }
    void Spawn()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            GameObject player = Instantiate(Player) as GameObject;
            player.transform.position = new Vector2(-15, 0.5f);
            Destroy(this.gameObject);
        }
        
    }
}
