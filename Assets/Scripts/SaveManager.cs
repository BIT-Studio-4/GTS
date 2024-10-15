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
            StartAutoSave();
        }
    }

    private Coroutine autoSaveCoroutine;

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
    public IEnumerator AutoSave(float cooldown)
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldown);

            SaveGame();
        }
    }

    /// <summary>
    /// Stops the auto save coroutine if its already going and starts a new one
    /// </summary>
    private void StartAutoSave()
    {
        if (autoSaveCoroutine != null)
        {
            StopAutoSave();
        }

        autoSaveCoroutine = StartCoroutine(AutoSave(autoSaveCooldown));
    }

    /// <summary>
    /// Stops the auto save coroutine
    /// </summary>
    private void StopAutoSave()
    {
        StopCoroutine(autoSaveCoroutine);
    }
}
