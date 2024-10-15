using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Minutes between auto saves
    [SerializeField, Range(1f, 20f)] private int autoSaveCooldown = 5;
    public int AutoSaveCooldown 
    { 
        get =>  autoSaveCooldown; 
        set 
        {
            autoSaveCooldown = value;
            StopCoroutine(AutoSaveCoroutine);
        }
    }

    private Coroutine AutoSaveCoroutine;

    /// <summary>
    /// Saves the game to the API
    /// </summary>
    public void SaveGame()
    {

    }

    /// <summary>
    /// Loops every autoSaveCooldown minutes and saves the game
    /// </summary>
    /// <returns></returns>
    public IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveCooldown);

            SaveGame();
        }
    }
}
