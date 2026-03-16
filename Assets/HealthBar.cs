using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Image healthBarFill;
    void Start()
    {
        healthBarFill.fillAmount = .5F;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarFill.fillAmount += .01F;
    }
}
