using UnityEngine;

public class OutdoorsSound : MonoBehaviour{

    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;
    [SerializeField] private float zThreshold;
    [SerializeField] private float zRange;
    
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
        if(playerController.WasCaught()){
            if(source.volume > 0)
                source.volume -= Time.deltaTime * 2f;

            return;
        }

        float volume = (player.position.z - zThreshold) * zRange;
        volume = Mathf.Clamp(volume, minVolume, maxVolume);

        if(player.position.x < -6)
            volume = 1;

        source.volume = volume;
    }
}
