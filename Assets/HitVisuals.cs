using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitVisuals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,
            GetComponent<SphereCollider>().radius * 1.1f);
    }
}
