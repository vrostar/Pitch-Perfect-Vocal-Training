using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void PlayLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Sustain");
    }

        public void PlayZelda()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Zelda");
    }

            public void PlayTest()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Test");
    }
}
