using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TypingGameKit.AsteroidTyping
{


    public class Debris : MonoBehaviour
    {
        //public AsteroidGameManager debris; 


        private void OnTriggerEnter2D(Collider2D collision)
        {
           FindObjectOfType<AsteroidGameManager>().GameOver();

        }

        void Exploding()
        {
            InputSequenceManager.RemoveAllSequences();
        }
    }
}
