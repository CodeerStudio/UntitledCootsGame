using UnityEngine;

public class RoadSounds : MonoBehaviour{

    [SerializeField] private float maxVolume;
    [SerializeField] private float xThreshold;
    [SerializeField] private float xRange;
    
    private AudioSource source;

    private PlayerController playerController;
    private Transform player;

    private void Start(){
        source = GetComponent<AudioSource>();

        playerController = FindObjectOfType<PlayerController>(true);
        player = playerController.transform;
    }

    private void Update(){
        //not a great implemenation lol
        //very specific to this particular scene
        float volume = (xThreshold - player.position.x) * xRange;
        volume = Mathf.Clamp(volume, 0, maxVolume);

        source.volume = volume;
    }
}
