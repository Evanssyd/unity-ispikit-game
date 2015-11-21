using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;


public class archerbehavior : MonoBehaviour {
	
	//public variabless
	public Animator anim;
	public MyPlugin MyPlugin;
	public AudioClip learn;
	private AudioSource source;
	
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	
	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.name == "GameObject") {
			source.PlayOneShot(learn);
			anim = GetComponent<Animator>();
			StartCoroutine(PlayAnimInterval(5, 1F));
			
			
		}
	}
	
	
	private IEnumerator    PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			anim.Play("sit", -1, 0F);
			--n;
			yield return new WaitForSeconds(time);
		}
	}
	
}

