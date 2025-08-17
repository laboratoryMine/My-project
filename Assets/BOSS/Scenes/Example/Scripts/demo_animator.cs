using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class demo_animator : MonoBehaviour {


    public Slider slider;
    public Text infoText;
    public GameObject asset;
    private Animator anim;

    private bool gunBarrelSpinning = false;

    public Material[] mats;

    private int currentTexIndex = 0;

    public Button btnFire1;
    public Button btnFire2;
    public Button btnFire3;

    void Start () {
       
        anim = asset.GetComponent<Animator>();
        infoText.text = "";
          ToggleFiring(false);
    }	
	public void Walk1()
    {
        anim.SetTrigger("walk");
        ToggleFiring(false);
    }
    public void fly()
    {
        anim.SetTrigger("fly");
        ToggleFiring(false);
    }
    public void land()
    {
        anim.SetTrigger("land");
        ToggleFiring(false);
    }
    public void forward_stance()
    {
        anim.SetTrigger("forward_stance");
         ToggleFiring(true);
    }
    public void Idle()
    {
        anim.SetTrigger("idle");
        ToggleFiring(false);
    }
    public void interact1()
    {
        anim.SetTrigger("interact1");
        ToggleFiring(false);
    }

    public void interact2()
    {
        anim.SetTrigger("interact2");
        ToggleFiring(false);
    }
    public void fire1()
    {
        anim.SetTrigger("fire1");
    }
    public void fire2()
    {
        anim.SetTrigger("fire2");
    }
    public void fire3()
    {
        anim.SetTrigger("fire3");
    }

    public void die_walking()
    {
        anim.SetTrigger("die_walking");
        ToggleFiring(false);
    }

    public void die_flying()
    {
        anim.SetTrigger("die_flying");
        ToggleFiring(false);
    }

    public void die_forward_stance()
    {
        anim.SetTrigger("die_forward_stance");
        ToggleFiring(false);
       
    }

    public void SpeedSliderChange()
    {
        anim.speed = slider.value;
    }



    public void ToggleFiring(bool state) {
        btnFire1.interactable = state;
        btnFire2.interactable = state;
        btnFire3.interactable = state;

        if(state == false)
        {
            anim.SetTrigger("stop_firing");
        }
    }

   
    public void NextTexture()
    {
        currentTexIndex++;

        if(currentTexIndex >= mats.Length)
        {
            currentTexIndex = 0;
        }
      
        Renderer rend = asset.GetComponentInChildren<Renderer>();
        Debug.Log(rend);
        rend.material = mats[currentTexIndex];
        infoText.text = rend.material.name;
    }
}
