using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

public class MyPlugin : MonoBehaviour {

#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern int startInitialization(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setPlaybackDoneCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setResultCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setCompletionCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setNewWordsCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int setNewAudioCallback(string callbackGameObjectName, string callbackMethodName);
	[DllImport("__Internal")]
	private static extern int startRecording(string sentence);
	[DllImport("__Internal")]
	private static extern int stopRecording();
	[DllImport("__Internal")]
	private static extern int setStrictness(int strictness);
	[DllImport("__Internal")]
	private static extern int startPlayback();
	[DllImport("__Internal")]
	private static extern int stopPlayback();
	[DllImport("__Internal")]
	private static extern int addWord(string word, string pronunciation);
#endif
	private static System.Timers.Timer timer;
	void Awake () {
		Debug.Log ("About to initialize plugin");
		string gameObjectName = "GameObject";
		string callbackName = "initCallback";
		startInitialization(gameObjectName, callbackName);
		Debug.Log ("Initialization started");
	}
	//public variables
	public int score;
	public int speed;
	public int recognizedSentenceindex;
	public int volumne;
	public string missingwords;
	public string initComplete;
	
	//public lists
	public List<int> mispronouncedwordsindex  = new List<int> ();
	public List<int> notrecognizedwordslist  = new List<int> ();
	public List<String> sentences = new List<String> ();
	public List<String> sentences1 = new List<String> ();

	private	int someInt = 40;

	//private list
	private List<int> sentenceindex = new List<int> ();

	
	//private variables
	private int countofVariables;
	private int wordnumberpostion;
	private int numberofWords;
	private int complete;
	private int mispronoucedword;
	public int guiswitch = 0;
	public int recordinit = 0;


	void Start () {
	}

	void Update () {

	}

	public void initCallback(string status) {
		Debug.Log ("Plugin initialization done");
		Debug.Log (status);
		initComplete = status;
		//Setting callbacks
		setPlaybackDoneCallback ("GameObject", "playbackDoneCallback");
		setResultCallback ("GameObject", "resultCallback");
		setCompletionCallback ("GameObject", "completionCallback");
		setNewWordsCallback ("GameObject", "newWordsCallback");
		setNewAudioCallback ("GameObject", "newAudioCallback");

				
	}

	private void onRecordingDone(object source, ElapsedEventArgs e) {
		Debug.Log ("Stopping recording");
		stopRecording ();
		recordinit = 0;

	}

	public void playbackDoneCallback(string status) {
		Debug.Log ("Playback Done");
		Debug.Log (status);
		timer = new System.Timers.Timer (1000);
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	
	public void resultCallback(string status) {
		Debug.Log ("Result");
		Debug.Log (status);

		///parsing results string into a list
		char[] delimiterChars = {',', ' ', '-'};
		string[] words = status.Split (delimiterChars);

		//LISTS
		List<String> list = new List<String> ();
		List<String> listindex = new List<String> ();
		foreach (string s in words) {
			list.Add (s);
		}

		//Number of words 0 index
		if(list.Count> 2){
			countofVariables = list.Count;
			wordnumberpostion = (countofVariables - 2);  
			numberofWords = Int32.Parse(list[wordnumberpostion]);

			//word count of recognized sentence
			for(int c = 0; c <= sentenceindex[recognizedSentenceindex]; c++){
				string test = c.ToString();
				listindex.Add(test);
			}
		}else{
			numberofWords = 0;
			 }

		//mispronounced
		List<String> recognitionindex = new List<String> ();

		//If the system recognized anywords
		if (countofVariables > 5) {

			//recognized sentence #
			recognizedSentenceindex = Int32.Parse(list[2]); 

				//mispronounced word
				for (int i=5; i<countofVariables+1; i+=3) {
					int mispronouncedwordsdata = Int32.Parse(list [i - 1]);
						if(mispronouncedwordsdata == 1){
							mispronouncedwordsindex.Add(Int32.Parse(list [i - 2]));							
						}else{}
								recognitionindex.Add( list[i - 2]);
				}	

			//Mispronounciation list
			List<string> ThirdList =  listindex.Except(recognitionindex).ToList();
			notrecognizedwordslist = ThirdList.Select(s => Convert.ToInt32(s)).ToList();
			mispronouncedwordsindex.ForEach(Console.WriteLine);
			System.Console.WriteLine(mispronouncedwordsindex.Count);


		}
		else{
			countofVariables = 3;
			System.Console.WriteLine ("I didn't hear anything");
			}


		System.Console.WriteLine (notrecognizedwordslist.Count);

			//Parsing the recognized sentence(not really working only if two words not rec)
			sentences1.Clear ();
			char[] delimiterChars1 = {' '};
			string[] words1 = sentences [recognizedSentenceindex].Split (delimiterChars1);
			foreach (string s1 in words1) {
				sentences1.Add (s1);
			}


		//Set the score public Variable
		score = Int32.Parse(list[0]);

		//Set the speed public variable
		speed = Int32.Parse(list[1]);

		//timer reset
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onStartPlayback;
		timer.AutoReset = false;
		timer.Enabled = true;
	}


 void onStartPlayback(object source, ElapsedEventArgs e) {
		Debug.Log ("Starting playback");
		startPlayback();

	}

	public void completionCallback(string status) {
		Debug.Log ("Completion");
		Debug.Log (status);
		complete = Int32.Parse (status);
	}
	
	public void newWordsCallback(string status) {
		Debug.Log ("New words");
		Debug.Log (status);
	}
	
	public void newAudioCallback(string status) {
		Debug.Log ("New audio");
		Debug.Log (status);
			
			//parsing the audio callback
			char[] delimiterChars = {','};
			string[] words = status.Split (delimiterChars);

			List<String> audiolist = new List<String> ();
				foreach (string s in words) {
					audiolist.Add (s);
					//global volumne variable
					volumne = Int32.Parse(audiolist[0]);

				}

	}

	void OnCollisionEnter (Collision col)
	{
		//Target collison with Master_knight
		if(col.gameObject.name == "master_knight")
		{
			recordinit = 2;

			//set string for re
			string recordingphrase;
			sentences.Clear ();
			//recording snipet
			recordingphrase = "I am learning English,I love to develop apps,on the weekend I play golf";
			guiswitch = 1;
		
				//parsing of sentences
				char[] delimiterChars = {','};
				string[] words = recordingphrase.Split (delimiterChars);
				foreach (string s in words) 
				{
					sentences.Add (s);
					//indexing of sentence word count
					int count = s.Count (c => c == ' ');
					sentenceindex.Add (count);
				}

				
		

			//play audio delay recording
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += recordknight1;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

		//Target collison with magic_archer
		if(col.gameObject.name == "magic_archer")
		{
			recordinit = 2;

			string recordingphrase1;
			sentences.Clear ();
			//recording snipet
			recordingphrase1 = "this is a demo,we are making a game,it will be huge";
			guiswitch = 1;
			
				//parsing of sentences
				char[] delimiterChars = {','};
				string[] words1 = recordingphrase1.Split (delimiterChars);
				foreach (string s in words1) 
				{
					sentences.Add (s);
					//indexing of sentence word count
					int count = s.Count (c => c == ' ');
					sentenceindex.Add (count);
				}

				
			// Play audio delay recoridng
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += recordarcher1;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

		//Target collison with elf
		if(col.gameObject.name == "elf")
		{
			recordinit = 2;

			string recordingphrase2;
			sentences.Clear ();
			//recording snipet
			recordingphrase2 = "we are making something,it will change,everybody can speak";
			guiswitch = 1;
			
				//parsing of sentences
				char[] delimiterChars = {','};
				string[] words2 = recordingphrase2.Split (delimiterChars);
				foreach (string s in words2) 
				{
					sentences.Add (s);
					//indexing of sentence word count
					int count = s.Count (c => c == ' ');
					sentenceindex.Add (count);
				}


			//play audio delay recording

			timer = new System.Timers.Timer (3000);
			timer.Elapsed += recordelf1;
			timer.AutoReset = false;
			timer.Enabled = true;

		}

		//Target collison with orc
		if(col.gameObject.name == "orc")
		{
			recordinit = 2;

			string recordingphrase3;
			sentences.Clear ();
			//recording snipet
			recordingphrase3 = "forever people will,our product rule,this is great";
			guiswitch = 1;
			
				//parsing of sentences
				char[] delimiterChars = {','};
				string[] words3 = recordingphrase3.Split (delimiterChars);
				foreach (string s in words3) 
				{
					sentences.Add (s);
					//indexing of sentence word count
					int count = s.Count (c => c == ' ');
					sentenceindex.Add (count);
				}
				

			//play audio delay recording

			timer = new System.Timers.Timer (3000);
			timer.Elapsed += recordorc1;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

	}
	private void recordknight1(object source, ElapsedEventArgs e){
		recordinit = 1; 
		startRecording("I am learning English,I love to develop apps,on the weekend I play golf");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	private void recordarcher1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("this is a demo,we are making a game,it will be huge");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordelf1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("we are making something,it will change,everybody can speak");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	
	private void recordorc1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("forever people will,our product rule,this is great");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	
	//lets set the possble recording answers on collsiom then the results of complete
	void OnGUI()
	{
	GUI.skin.label.fontSize = someInt;
	GUI.skin.box.fontSize = someInt;

			if(complete > 1 && complete < 99){
				GUI.Box (new Rect (Screen.width - 510, 10, 500, 250), "");
				GUI.Label (new Rect (Screen.width - 505, 90, 490, 330), "Calculating your score...");
			}
			else if (complete == 100) {
				GUI.Box (new Rect (Screen.width - 510, 10, 500, 250), "Your results");
				GUI.Label (new Rect (Screen.width - 505, 50, 490, 330), sentences [recognizedSentenceindex]);
				if(notrecognizedwordslist.Count > 0){
					GUI.Label (new Rect (Screen.width - 505, 90, 490, 330), "Missed: " + sentences1[notrecognizedwordslist[0]]);
				}else{
					GUI.Label (new Rect (Screen.width - 505, 90, 490, 330), "Missed: None");
				}
				if(mispronouncedwordsindex.Count > 0){
					GUI.Label (new Rect (Screen.width - 505, 130, 490, 330), "Mispronounced: " + sentences1[mispronouncedwordsindex[0]]);
				}else{
					GUI.Label (new Rect (Screen.width - 505, 130, 490, 330), "Mispronounced: None");
				}
				GUI.Label (new Rect (Screen.width - 505, 170, 490, 330), "Score: " + score.ToString ());
				GUI.Label (new Rect (Screen.width - 505, 210, 490, 330), "Speed: " + speed.ToString ()); 
		} else {
		}

		if (guiswitch == 1) {
			
			GUI.Box (new Rect (10, 10, 500, 250), "Your options");
			GUI.Label (new Rect (15, 60, 490, 330), sentences [0]);
			GUI.Label (new Rect (15, 110, 490, 330), sentences [1]);
			GUI.Label (new Rect (15, 160, 490,330), sentences [2]);

			if(recordinit == 0){
				GUI.Label (new Rect (15, 210, 260, 330),"Not Recording");
				GUI.Label (new Rect (280, 210, 490, 330),"Volumne: " + volumne.ToString ());
			}else if(recordinit == 2){
				GUI.Label (new Rect (15, 210, 260, 330),"Wait");
				GUI.Label (new Rect (280, 210, 490, 330),"Volumne: " + volumne.ToString ());
			}else{
				GUI.Label (new Rect (15, 210, 260, 330),"Speak Now! ");
				GUI.Label (new Rect (280, 210, 490, 330),"Volumne: " + volumne.ToString ());
			}
		} else {
		}
		if (initComplete == "0") {

		} else {
			GUI.Box (new Rect (100, Screen.width - 500, 790, 630),"");
		}
		
	}

}
