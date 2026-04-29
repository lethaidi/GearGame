using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bin : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject canvas;
    public int priceBin;
    public Animator animator;

    void Start()
    {
        canvas.SetActive(false);
    }
    void Update()
    {
        text.text = priceBin.ToString();
        ChooseGear();
    }
    void ChooseGear()
    {
        GameObject sold = GameObject.FindWithTag("GearBought");
        if (sold != null)
        {
            ControllGear gear = sold.GetComponent<ControllGear>();
            priceBin = gear.priceSold;
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }

    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("GearBought"))
        {
            animator.SetBool("isSold", true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GearBought"))
        {
            animator.SetBool("isSold", false);
        }
    }
}
