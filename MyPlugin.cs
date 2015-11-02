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
	public int mispronoucedword;
	public int volumne;
	public int complete;

	//public lists
	public List<String> sentences = new List<String> ();
	public List<int> sentenceindex = new List<int> ();
	public List<int> mispronouncedwordsindex  = new List<int> ();
	public List<int> notrecognizedwordslist  = new List<int> ();

	
	//private variables
	private int countofVariables;
	private int wordnumberpostion;
	private int numberofWords;

	void Start () {
	}

	void Update () {


	}

	public void initCallback(string status) {
		Debug.Log ("Plugin initialization done");
		Debug.Log (status);
		setPlaybackDoneCallback ("GameObject", "playbackDoneCallback");
		setResultCallback ("GameObject", "resultCallback");
		setCompletionCallback ("GameObject", "completionCallback");
		setNewWordsCallback ("GameObject", "newWordsCallback");
		setNewAudioCallback ("GameObject", "newAudioCallback");


	}
	public void onRecord(object source, ElapsedEventArgs e) {

		// recording setup
		string recordingphrase = "I am learning English,I am going to school today,one two three four";
		startRecording (recordingphrase);
	
		//parsing of sentences
		char[] delimiterChars = {','};
		string[] words = recordingphrase.Split (delimiterChars);
		foreach (string s in words) {
			sentences.Add (s);
		
			//indexing of sentence word count
			int count = s.Count (c => c == ' ');
			sentenceindex.Add (count);
		}
		Debug.Log ("Starting recording");
		//end of recording
	
	
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	private static void onRecordingDone(object source, ElapsedEventArgs e) {
		Debug.Log ("Stopping recording");
		stopRecording ();
	}
	
	public void onRestarting(object source, ElapsedEventArgs e) {
		Debug.Log ("Starting recording");
		startRecording ("I am learning English,I am going to school today,one two three four");



		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;

	}

	public void playbackDoneCallback(string status) {
		Debug.Log ("Playback Done");
		Debug.Log (status);
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onRestarting;
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
				for (int i=5; i<countofVariables+ 1; i+=3) {
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

		GetComponent<Renderer>().enabled = true;

		//timer reset
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += onStartPlayback;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	private static void onStartPlayback(object source, ElapsedEventArgs e) {
		Debug.Log ("Starting playback");
		startPlayback();
	}

	public void completionCallback(string status) {
		Debug.Log ("Completion");
		Debug.Log (status +"!!!");
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
		if(col.gameObject.name == "master_knight")
		{
			string recordingphrase = "I am learning English,I am going to school today,one two three four";
			startRecording (recordingphrase);
			
			//parsing of sentences
			char[] delimiterChars = {','};
			string[] words = recordingphrase.Split (delimiterChars);
			foreach (string s in words) {
				sentences.Add (s);
				
				//indexing of sentence word count
				int count = s.Count (c => c == ' ');
				sentenceindex.Add (count);
			}
			Debug.Log ("Starting recording");
			//end of recording
			
			
			timer = new System.Timers.Timer (3000);
			timer.Elapsed += onRecordingDone;
			timer.AutoReset = false;
			timer.Enabled = true;
		}
	}
	void OnGUI()
	{
	if (complete == 100) {
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Words possible");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), sentences [0]);
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), sentences [1]);
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), sentences [2]);
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), score.ToString ());
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), volumne.ToString ()); 
		} else {
			GUI.Box (new Rect (Screen.width - 260, 10, 250, 150), "Words possible");
			GUI.Label (new Rect (Screen.width - 245, 30, 250, 30), "test");
			GUI.Label (new Rect (Screen.width - 245, 50, 250, 30), "test");
			GUI.Label (new Rect (Screen.width - 245, 70, 250, 30), "test");
			GUI.Label (new Rect (Screen.width - 245, 110, 250, 30), "test");
			GUI.Label (new Rect (Screen.width - 245, 130, 250, 30), "test"); 
		}
	}

}
