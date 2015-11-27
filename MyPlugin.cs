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

	//grammar switching
	public int knightgrammarswitch = 0;
	public int archergrammarswitch = 0;	
	public int orcgrammarswitch = 0;
	public int elfgrammarswitch = 0;

	//public lists
	public List<int> mispronouncedwordsindex  = new List<int> ();
	public List<int> notrecognizedwordslist  = new List<int> ();
	public List<String> sentences = new List<String> ();
	public List<String> sentences1 = new List<String> ();

	
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

		//CHANGE RECORDING UI
		recordinit = 0;
		
	}
	
	public void playbackDoneCallback(string status) {
		Debug.Log ("Playback Done");
		Debug.Log (status);

		//turn back on collision for each character

	
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
	

		mispronouncedwordsindex.Clear ();
		
		//Number of words 0 index
		if(list.Count> 2){
			countofVariables = list.Count;
			wordnumberpostion = (countofVariables - 2);
			numberofWords = Int32.Parse(list[wordnumberpostion]);
			

		}else{
			numberofWords = 0;
		}

		List<String> recognitionindex = new List<String> ();
		
		//If the system recognized anywords
		if (countofVariables > 5) {
			
			//recognized sentence #
			recognizedSentenceindex = Int32.Parse(list[2]);

			//PARSE THE RECOGNIZED SENTENCE
			sentences1.Clear ();
			char[] delimiterChars1 = {' '};
			string[] words1 = sentences [recognizedSentenceindex].Split (delimiterChars1);
			foreach (string s1 in words1) {
				sentences1.Add (s1);
			}

			//word count of recognized sentence
			for(int c = 0; c < sentences1.Count; c++){
				string test = c.ToString();
				listindex.Add(test);
			}
			
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

		//Set the score public Variable
		score = Int32.Parse(list[0]);
		
		//Set the speed public variable
		speed = Int32.Parse(list[1]);

		//reaction functions knight
		if(score > 80 && sentences[recognizedSentenceindex] == "playing sports is fun"){
			knightgrammarswitch = 3;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "I love working with technology"){
			knightgrammarswitch = 2;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "I am learning English"){
			knightgrammarswitch = 1;
		}else{
			knightgrammarswitch = 0;
		}

		//reaction functions archer
		if(score > 80 && sentences[recognizedSentenceindex] == "nothing is better then art"){
			archergrammarswitch = 3;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "science is interesting"){
			archergrammarswitch = 2;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "my favorite is math"){
			archergrammarswitch = 1;
		}else{
			archergrammarswitch = 0;
		}
		
		//reaction functions orc
		if(score > 80 && sentences[recognizedSentenceindex] == "I want to run and jump"){
			orcgrammarswitch = 3;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "We should battle enemies"){
			orcgrammarswitch = 2;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "I want to talk to characters"){
			orcgrammarswitch = 1;
		}else{
			orcgrammarswitch = 0;
		}


		//reaction functions elf
		if(score > 80 && sentences[recognizedSentenceindex] == "Nothing is better than duck"){
			elfgrammarswitch = 3;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "pizza is my favorite"){
			elfgrammarswitch = 2;
		}else if(score > 80 && sentences[recognizedSentenceindex] == "I love to eat dumplings"){
			elfgrammarswitch = 1;
		}else{
			elfgrammarswitch = 0;
		}

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
		if (recordinit == 0) {
			//Target collison with Master_knight
			if (col.gameObject.name == "master_knight") {
				
				recordinit = 2;

				if (knightgrammarswitch == 0) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();

					//recording snipet
					recordingphrase = "I am learning English,I love working with technology,playing sports is fun";
					guiswitch = 1;
			
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
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

				} else if (knightgrammarswitch == 1) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "to talk with my friends,I want to study in America,learning languages is fun";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}

					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordknight2;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (knightgrammarswitch == 2) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I know how to code,science is my future,I want to help the world";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordknight3;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (knightgrammarswitch == 3) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I am the best at soccer,basketball is the most fun,i love to play golf";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordknight4;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else {

				}
			}
			//Target collison with magic_archer
			if (col.gameObject.name == "magic_archer") {
				
				recordinit = 2;
				
				if (archergrammarswitch == 0) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "my favorite is math,science is interesting,nothing is better than art";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordarcher1;
					timer.AutoReset = false;
					timer.Enabled = true;
					
				} else if (archergrammarswitch == 1) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "one,twenty five,I have no idea";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordarcher2;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (archergrammarswitch == 2) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I love astronomy,biology is the most interesting,computers are changing the world";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordarcher3;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (archergrammarswitch == 3) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "It has to be the French,I love Italian art,Japanese art is refined";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordarcher4;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else {
					
				}
			}

			//Target collison with elf
			if (col.gameObject.name == "elf") {
				
				recordinit = 2;
				
				if (elfgrammarswitch == 0) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase =  "I love to eat dumplings,pizza is my favorite,Nothing is better than duck";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
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
					
				} else if(elfgrammarswitch == 1) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I love chicken,it must have shrimp and pork,vegetable is all I need";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordelf2;
					timer.AutoReset = false;
					timer.Enabled = true;
				}else if(elfgrammarswitch == 2){
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "Plain peperoni is delicious,I like mushroom and sausage,combination is alway the best";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordelf3;
					timer.AutoReset = false;
					timer.Enabled = true;
				}
				else if(elfgrammarswitch == 3){
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I love to eat Chinese duck,eating duck fat is amazing,you must try duck liver";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordelf4;
					timer.AutoReset = false;
					timer.Enabled = true;
				}else{
					
				}
			}
		
			//Target collison with orc
			if (col.gameObject.name == "orc") {
				
				recordinit = 2;
				
				if (orcgrammarswitch == 0) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I want to talk to characters,We should battle enemies,I want to run and jump";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
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
					
				} else if (orcgrammarswitch == 1) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "tell funny stories,play games with my voice,we can talk about things I like";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordorc2;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (orcgrammarswitch == 2) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "we will battle dragons,destroy waves of zombies,I want to fight other characters";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordorc3;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else if (orcgrammarswitch == 3) {
					//set string for recording
					string recordingphrase;
					sentences.Clear ();
					
					//recording snipet
					recordingphrase = "I want a three dimension,I like to beat levels,there should be a world with games";
					guiswitch = 1;
					
					//parsing of sentences
					char[] delimiterChars = {','};
					string[] words = recordingphrase.Split (delimiterChars);
					foreach (string s in words) {
						sentences.Add (s);
						//indexing of sentence word count
						int count = s.Count (c => c == ' ');
						sentenceindex.Add (count);
					}
					
					//play audio delay recording
					timer = new System.Timers.Timer (3000);
					timer.Elapsed += recordorc4;
					timer.AutoReset = false;
					timer.Enabled = true;
				} else {
					
				}
			}
		}
	}

	private void recordknight1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I am learning English,I love working with technology,playing sports is fun");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;

	}
	private void recordknight2(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("to talk with my friends,I want to study in America,learning languages is fun");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordknight3(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I know how to code,science is my future,I want to help the world");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordknight4(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I am the best at soccer,basketball is the most fun,i love to play golf");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordarcher1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("my favorite is math,science is interesting,nothing is better than art");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	private void recordarcher2(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("one,twenty five,I have no idea");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordarcher3(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I love astronomy,biology is the most interesting,computers are changing the world");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordarcher4(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("It has to be the French,I love Italian art,Japanese art is refined");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordelf1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I love to eat dumplings,pizza is my favorite,Nothing is better than duck");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordelf2(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I love chicken,it must have shrimp and pork,vegetable is all I need");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordelf3(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("Plain peperoni is delicious,I like mushroom and sausage,combination is alway the best");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordelf4(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I love to eat Chinese duck,eating duck fat is amazing,you must try duck liver");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordorc1(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I want to talk to characters,We should battle enemies,I want to run and jump");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordorc2(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("tell funny stories,play games with my voice,we can talk about things I like");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordorc3(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("we will battle dragons,destroy waves of zombies,I want to fight other characters");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}
	private void recordorc4(object source, ElapsedEventArgs e){
		recordinit = 1;
		startRecording("I want a three dimension,I like to beat levels,there should be a world with games");
		timer = new System.Timers.Timer (3000);
		timer.Elapsed += onRecordingDone;
		timer.AutoReset = false;
		timer.Enabled = true;
	}

	//lets set the possble recording answers on collsiom then the results of complete
	void OnGUI()
	{
		//introduction box
		if (initComplete == "0") {	
		
		} else {
			if (Screen.width < 800 && Screen.width > 490) {
				GUI.skin.label.fontSize = 30;
				GUI.skin.box.fontSize = 45;
			} else if (Screen.width > 800) {
				GUI.skin.label.fontSize = 50;
				GUI.skin.box.fontSize = 60;
			}else {
				GUI.skin.label.fontSize = 18;
				GUI.skin.box.fontSize = 25;
			}
			
				GUI.Box (new Rect (5, 10, Screen.width - 10, 640), "");
			if(Screen.width > 500){
				GUI.Label (new Rect ((Screen.width / 4), 15, Screen.width - 10,600),"Ispikit English SDK" );
			}else{
				GUI.Label (new Rect ((Screen.width / 5), 15, Screen.width - 10,600),"Ispikit English SDK" );
			}
				GUI.Label (new Rect (10, 100, Screen.width - 15, 600), "Recognize what a student says than give them feedback  how well they said it. Our SDK is easy to setup and runs completely offline.");
				GUI.Label (new Rect (10, 300, Screen.width - 10, 600), "For this demo, collide with a character and speak one of the optional phrases.");
				GUI.Label (new Rect (10, 425, Screen.width - 10, 600), "The system is initializing, once this box disappears you are ready to begin!");
		}
		
		
		
		//options box
		if (guiswitch == 1) {
			if (Screen.width > 700){
				GUI.skin.label.fontSize = 40;
				GUI.skin.box.fontSize = 40;
			}else{
				GUI.skin.label.fontSize = 20;
				GUI.skin.box.fontSize = 20;
			}
			GUI.Box (new Rect (2, 10,  (Screen.width / 2) - 5, 250), "Your options");
			GUI.Label (new Rect (5, 50, (Screen.width / 2) - 10, 330), sentences [0]);
			GUI.Label (new Rect (5, 90,  (Screen.width / 2) - 10, 330), sentences [1]);
			GUI.Label (new Rect (5, 130,  (Screen.width / 2) - 10, 330), sentences [2]);
				
			if(recordinit == 0){
				GUI.Label (new Rect (5, 170,  (Screen.width / 2) - 10, 330),"Not Recording");
				GUI.Label (new Rect (5, 210, (Screen.width / 2) - 10, 330),"Volume: " + volumne.ToString ());
			}else if(recordinit == 2){
				GUI.Label (new Rect (5, 170,  (Screen.width / 2) - 10, 330),"Wait");
				GUI.Label (new Rect (5, 210, (Screen.width / 2) - 10, 330),"Volume: " + volumne.ToString ());
			}else{
				GUI.Label (new Rect (5, 170,  (Screen.width / 2) - 10, 330),"Speak Now! ");
				GUI.Label (new Rect (5, 210, (Screen.width / 2) - 10, 330),"Volume: " + volumne.ToString ());
			}
		} else {
		}
		
		//THE RESULTS BOX
		if(complete > 1 && complete < 99){

			if(Screen.width > 700){
				GUI.skin.label.fontSize = 40;
				GUI.skin.box.fontSize = 40;
			}else{
				GUI.skin.label.fontSize = 20;
				GUI.skin.box.fontSize = 20;
			}
			GUI.Box (new Rect ((Screen.width / 2) + 5, 10, (Screen.width / 2) + 5, 250), "");
			GUI.Label (new Rect ((Screen.width / 2) + 10, 100, (Screen.width / 2) + 10, 330), "Calculating your score...");
		}
		else if (complete == 100) {
			if(Screen.width > 700){
				GUI.skin.label.fontSize = 40;
				GUI.skin.box.fontSize = 40;
			}else{
				GUI.skin.label.fontSize = 20;
				GUI.skin.box.fontSize = 20;
			}
				GUI.Box (new Rect ((Screen.width / 2) + 5, 10, (Screen.width / 2) - 5, 250), "Your results");
				GUI.Label (new Rect ((Screen.width / 2) + 10, 50, (Screen.width / 2) - 10, 330), sentences [recognizedSentenceindex]);

			if(notrecognizedwordslist.Count > 0){
				GUI.Label (new Rect ((Screen.width / 2) + 10, 90, (Screen.width / 2) - 10, 330), "Missed: " + sentences1[notrecognizedwordslist[0]]);
			}else{
				GUI.Label (new Rect ((Screen.width / 2) + 10, 90, (Screen.width / 2) - 10, 330), "Missed: None");
			}

			if(mispronouncedwordsindex.Count > 0){
				GUI.Label (new Rect ((Screen.width / 2) + 10, 130, (Screen.width / 2) - 10, 330), "Mispronounced: " + sentences1[mispronouncedwordsindex[0]]);
			}else{
				GUI.Label (new Rect ((Screen.width / 2) + 10, 130, (Screen.width / 2) - 10, 330), "Mispronounced: None");
			}

				GUI.Label (new Rect ((Screen.width / 2) + 10, 170, (Screen.width / 2) - 10, 330), "Score: " + score.ToString ());
				GUI.Label (new Rect ((Screen.width / 2) + 10, 210, (Screen.width / 2) - 10, 330), "Speed: " + speed.ToString ());
		} else {
		}
	}
}

