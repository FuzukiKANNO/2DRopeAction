using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{

    public int spriteCount = 1;
    float rightOffset = 1.6f;
    float leftOffset = -0.6f;

    Transform bgTfm;
    SpriteRenderer mySpriteRndr;
    float width;

    void Start()
    {
        bgTfm = transform;
        mySpriteRndr = GetComponent<SpriteRenderer>();
        width = mySpriteRndr.bounds.size.x;
    }


    void Update()
    {
        Vector3 myViewport = Camera.main.WorldToViewportPoint(bgTfm.position);

        if (myViewport.x < leftOffset)
        {
            bgTfm.position += Vector3.right * (width * spriteCount);
        }
        else if (myViewport.x > rightOffset)
        {
            bgTfm.position -= Vector3.right * (width * spriteCount);
        }
    }
}