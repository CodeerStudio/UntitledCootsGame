using UnityEngine;

public class FaceCamera : MonoBehaviour{

    [SerializeField] private bool invert;
    
    private Transform camTransform;

    private void Start(){
        camTransform = Camera.main.transform;
    }

    private void LateUpdate(){
        if(invert){
            transform.LookAt(2 * transform.position - camTransform.position);
            return;
        }
        
		//face the direction of the main camera
        transform.LookAt(2 * camTransform.transform.position - transform.position);
    }
}
