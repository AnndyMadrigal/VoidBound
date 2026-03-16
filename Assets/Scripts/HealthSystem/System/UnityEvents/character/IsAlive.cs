using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void PlayHeroKnight_Death(bool isAlive)
    {
        bool characterIsDead = !isAlive;
        if (characterIsDead)
        {
            PlayHeroKnight_Death();
        }
    }


    void PlayHeroKnight_Death()
    {
        // Add death animation logic here
    }

    // Update is called once per frame
    void Update()
    {

    }
}
