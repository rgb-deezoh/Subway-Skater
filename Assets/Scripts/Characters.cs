using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public GameObject[] characters;
    private int selectedChracter = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject ch in characters)
        {
            ch.SetActive(false);
        }
        characters[selectedChracter].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeCharacters(int newCharacters)
    {
        characters[selectedChracter].SetActive(false);
        characters[newCharacters].SetActive(true);
        selectedChracter = newCharacters;
    }
}
