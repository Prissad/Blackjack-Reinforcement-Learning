using UnityEngine;


[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class Music : MonoBehaviour {

    private static Music instance = null; // Singleton Instance of Music
    public static Music Instance // the instance's property
    {
        get
        {
            if (instance == null)
            {
                instance = (Music)FindObjectOfType(typeof(Music));
            }
            return instance;
        }
    }

    /*
     * Keep Game Object when going between Scenes.
     */
    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
