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
	public AudioClip learn2;
	public AudioClip learn3;
	private AudioSource source;
	private Ispikit ispikit;

	public GameObject GameObject;
	
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		ispikit = GameObject.GetComponent<Ispikit>();
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	
	void OnCollisionEnter (Collision col)
	{ 

			if (col.gameObject.name == "GameObject") {
				if (ispikit.recordinit == 0) {
					if(ispikit.knightgrammarswitch == 0){
						source.PlayOneShot (learn);
						anim = GetComponent<Animator> ();
						StartCoroutine (PlayAnimInterval (5, 1F));
					}else if(ispikit.knightgrammarswitch == 1){
						source.PlayOneShot (learn1);
					}else if(ispikit.knightgrammarswitch == 2){
							source.PlayOneShot (learn2);
					}else if(ispikit.knightgrammarswitch == 3){
						source.PlayOneShot (learn3);
					}else{}
					
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