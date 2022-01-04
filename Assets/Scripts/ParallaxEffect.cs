using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float length, startpos,ypos;
    public GameObject cam;
    public float parallexEffect;
    void Start()
    {
        startpos = transform.position.x;
        ypos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallexEffect));
        float dist = (cam.transform.position.x * parallexEffect);
        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
