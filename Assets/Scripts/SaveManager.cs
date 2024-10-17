using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Debug.LogWarning($"Instance of SaveManager already exists, removing {this} on {gameObject}");
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Saves the game to the API
    /// </summary>
    public void SaveGame()
    {
        SaveGame saveSnapshot = GetCurrentSave();

    }

    /// <summary>
    /// Creates a save from the current state of the game
    /// </summary>
    /// <returns></returns>
    private SaveGame GetCurrentSave()
    {
        SaveGame saveGame = new SaveGame()
        {
            id = GameManager.Instance.User.id,
            Money = GameManager.Instance.Money,
            store = new Store()
            {

            },
            inventory = new Inventory()
            {

            }
        };

        return saveGame;
    }

    /// <summary>
    /// Loads the game from the api
    /// </summary>
    public void LoadGame()
    {

    }

    /// <summary>
    /// Loops every autoSaveCooldown minutes and saves the game
    /// </summary>
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
