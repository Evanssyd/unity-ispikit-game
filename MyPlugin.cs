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
		Debug.Log (startInitialization(gameObjectName, callbackName));
		Debug.Log ("Initialization started");
	}
	//public variables
	public int score;
	public int speed;
	public int recognizedSentenceindex;
	public int volumne;

	
	//public lists
	public List<String> sentences = new List<String> ();
	public List<int> mispronouncedwordsindex  = new List<int> ();
	public List<int> notrecognizedwordslist  = new List<int> ();

	//private list
	private List<int> sentenceindex = new List<int> ();
	
	//private variables
	private int countofVariables;
	private int wordnumberpostion;
	private int numberofWords;
	private int complete;
	private int mispronoucedword;
	private int guiswitch = 0;

	void Start () {
	}

	void Update () {

	}

	public void initCallback(string status) {
		Debug.Log ("Plugin initialization done");
		Debug.Log (status);

		//Setting callbacks
		setPlaybackDoneCallback ("GameObject", "playbackDoneCallback");
		setResultCallback ("GameObject", "resultCallback");
		setCompletionCallback ("GameObject", "completionCallback");
		setNewWordsCallback ("GameObject", "newWordsCallback");
		setNewAudioCallback ("GameObject", "newAudioCallback");

				
	}

	private static void onRecordingDone(object source, ElapsedEventArgs e) {
		Debug.Log ("Stopping recording");
		stopRecording ();
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

			notrecognizedwordslist.ForEach(Console.WriteLine);
			mispronouncedwordsindex.ForEach(Console.WriteLine);
		}
		else{
			countofVariables = 3;
			System.Console.WriteLine ("I didn't hear anything");
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
		guiswitch = 0;

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
			//set string for re
			string recordingphrase;
			sentences.Clear ();
			//recording snipet
			recordingphrase = "I am learning English,I am going to school today,one two three four";
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

		Debug.Log ("Starting recording");
		recordknight1();
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
		}

		//Target collison with magic_archer
		if(col.gameObject.name == "magic_archer")
		{
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
			//Recording Sequence
			Debug.Log ("Starting recording");
			recordarcher1();
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += onRecordingDone;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

		//Target collison with elf
		if(col.gameObject.name == "elf")
		{
			string recordingphrase2;
			sentences.Clear ();
			//recording snipet
			recordingphrase2 = "we are making something amazing,it will change the world,everybody can speak english";
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

			//recording sequence
			Debug.Log ("Starting recording");
			recordelf1();
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += onRecordingDone;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

		//Target collison with orc
		if(col.gameObject.name == "orc")
		{
			string recordingphrase3;
			sentences.Clear ();
			//recording snipet
			recordingphrase3 = "forever people will remember our name,our product will rule,this is great to have happen";
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

			//recording sequence
			Debug.Log ("Starting recording");
			recordorc1();
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += onRecordingDone;
			timer.AutoReset = false;
			timer.Enabled = true;
		}

	}
	private static void recordknight1(){
		startRecording("I am learning English,I am going to school today,one two three four");
	}

	private static void recordarcher1(){
		startRecording("this is a demo,we are making a game,it will be huge");
	}
	private static void recordelf1(){
		startRecording("we are making something amazing,it will change the world,everybody can speak english");
	}
	
	private static void recordorc1(){
		startRecording("forever people will remember our name,our product will rule,this is great to have happen");
	}

	
	//lets set the possble recording answers on collsiom then the results of complete
	void OnGUI()
	{
	if (complete == 100) {
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Your results");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), sentences [recognizedSentenceindex]);
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), " ");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), " ");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), score.ToString ());
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), volumne.ToString ()); 
		} else {
		}
		if (guiswitch == 1) {
			
			GUI.Box (new Rect (Screen.width - 610, 10, 250, 150), "Your options");
			GUI.Label (new Rect (Screen.width - 595, 30, 250, 30), sentences [0]);
			GUI.Label (new Rect (Screen.width - 595, 50, 250, 30), sentences [1]);
			GUI.Label (new Rect (Screen.width - 595, 70, 250, 30), sentences [2]);
			GUI.Label (new Rect (Screen.width - 595, 110, 250, 30), " ");
			GUI.Label (new Rect (Screen.width - 595, 130, 250, 30), "Hit button to record");
		} else {
		}
	}

}
