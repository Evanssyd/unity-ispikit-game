using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;


public class knightbehavior : MonoBehaviour {

	//public variabless
	public Animator anim;
	public AudioClip learn;
	public AudioClip learn1;
	private AudioSource source;
	private MyPlugin tryit;

	public GameObject GameObject;
	
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		tryit = GameObject.GetComponent<MyPlugin>();
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	
	void OnCollisionEnter (Collision col)
	{ 

			if (col.gameObject.name == "GameObject") {
				if (tryit.recordinit == 0) {
					if(tryit.knightgrammarswitch == 0){
						source.PlayOneShot (learn);
						anim = GetComponent<Animator> ();
						StartCoroutine (PlayAnimInterval (5, 1F));
					}else if(tryit.knightgrammarswitch ==1){
						source.PlayOneShot (learn1);
						anim = GetComponent<Animator> ();
						StartCoroutine (PlayAnimInterval (5, 1F));
					}else{
					}
				} else {
				
				}
			}
	}
	
	private IEnumerator PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			anim.Play("sit", -1, 0F);
			--n;
			yield return new WaitForSeconds(time);
		}

	}
}