using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemy;
    [SerializeField] private Image healthBarFill;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0,180,0);
        healthBarFill.fillAmount = Mathf.Clamp01(enemy.GetComponent<Enemy>().health / enemy.GetComponent<Enemy>().maxHealth);
    }  
}
