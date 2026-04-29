using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Price : MonoBehaviour
{
    [Header("Price Settings")]
    public TextMeshProUGUI text;
    public int price;
    public Coin coin;
    public bool isPrice = false;
    [Header("Color of Price")]
    public GameObject priceColor1;
    public GameObject priceColor2;

    public Animator animator;
    public bool isAnim = false;
    [Header("Sale")]
    public ScriptableSkills skillLuck;
    public GameObject luckIcon;
    void Start()
    {
        int disCount = skillLuck.valueSkill;
        int roll = Random.Range(0, 100);
        if (roll < disCount && luckIcon != null)
        {
            luckIcon.SetActive(true);
            price /= 2;
        }

        if (coin == null)
        {
            coin = FindObjectOfType<Coin>();
        }
        
    }

    void Update()
    {
        StartCoroutine(Animate());
        CheckPrice();
        text.text = price.ToString();
    }
    void CheckPrice()
    {
        if (coin.CoinValue >= price)
        {
            priceColor1.SetActive(true);
            priceColor2.SetActive(false);
            isPrice = true;
        }
        else
        {
            priceColor2.SetActive(true);
            priceColor1.SetActive(false);
            isPrice = false;
        }
    }
    IEnumerator Animate()
    {
        if(!isAnim) yield break;

        if (isAnim)
        {
            animator.SetBool("isNotAvailable", true);
            isAnim = false;
        }
        yield return new WaitForSeconds(1f);
        animator.SetBool("isNotAvailable", false);
    }
}
