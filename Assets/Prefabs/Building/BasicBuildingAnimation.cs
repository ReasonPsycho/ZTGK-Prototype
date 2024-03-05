using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBuildingAnimation : MonoBehaviour
{
    private float t = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime * 0.1f;
        Animator animator = GetComponent<Animator>();
        animator.SetFloat("Construction Percentage",t);
    }
}
