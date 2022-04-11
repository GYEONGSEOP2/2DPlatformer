using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector2 MIN_CAMERA_POS = new Vector2(-1.5f,-4.5f);
    private Vector2 MAX_CAMERA_POS = new Vector2(28f,10f);
    public GameObject go_Character;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCameraHorizontal(float moveVector)
    {
        transform.position = transform.position + new Vector3(moveVector,0,0);
        
        float limitValue = 0;
        if(moveVector > 0)
        {
            limitValue = Mathf.Min(MAX_CAMERA_POS.x,transform.position.x);
        }
        else
        {
            limitValue = Mathf.Max(MIN_CAMERA_POS.x,transform.position.x);
        }
        transform.position = new Vector3(limitValue,transform.position.y,transform.position.z);
    }
    public void MoveCameraVertical(float moveVector)
    {
        transform.position = transform.position + new Vector3(0,moveVector,0);

        float limitValue = 0;
        if(moveVector > 0)
        {
            limitValue = Mathf.Min(MAX_CAMERA_POS.y,transform.position.y);
        }
        else
        {
            limitValue = Mathf.Max(MIN_CAMERA_POS.y,transform.position.y);
        }
        transform.position = new Vector3( transform.position.x ,limitValue,transform.position.z);
    }
}
