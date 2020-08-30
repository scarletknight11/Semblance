using UnityEngine;
using AudioRecorder;

public class Example : MonoBehaviour {
	 
	Recorder recorder;
	AudioSource audioSource;

	public bool autoPlay;

	string log = "";

	void OnEnable()
	{
		recorder = new Recorder();
		Recorder.onInit += OnInit;
		Recorder.onFinish += OnRecordingFinish;
		Recorder.onError += OnError;
		Recorder.onSaved += OnRecordingSaved;
	}
	void OnDisable()
	{
		Recorder.onInit -= OnInit;
		Recorder.onFinish -= OnRecordingFinish;
		Recorder.onError -= OnError;
		Recorder.onSaved -= OnRecordingSaved;
	}

	//Use this for initialization  
	void Start()   
	{  
		audioSource = GameObject.FindObjectOfType<AudioSource>();
		recorder.Init();
	}  
	
	void OnGUI()   
	{  
		GUILayout.Label (log);

		if(recorder.IsReady)  
		{  
			if(!recorder.IsRecording)  
			{  
				if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-100, 300, 60), "Start"))  
				{  
					recorder.StartRecording(false,60);
				}  
			}  
			else
			{  
				if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-100, 300, 60), "Stop"))  
				{  
					recorder.StopRecording();
				}   
				
				GUI.Label(new Rect(Screen.width/2-150, 50, 300, 60), "Recording...");  
			} 

			if(recorder.hasRecorded)
			{
				if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2-30, 300, 60), "Play"))  
				{  
					recorder.PlayRecorded(audioSource);
				} 
				if(GUI.Button(new Rect(Screen.width/2-150, Screen.height/2+40, 300, 60), "Save"))  
				{  
					recorder.Save(System.IO.Path.Combine(Application.persistentDataPath,"Audio"+Random.Range(0,10000)+".wav"),recorder.Clip);
				} 
			}
		}  
	}  

	void OnInit(bool success)
	{
		Debug.Log("Success : "+success);
		log += "\nSuccess";
	}

	void OnError(string errMsg)
	{
		Debug.Log("Error : "+errMsg);
		log += "\nError " + errMsg;
	}

	void OnRecordingFinish(AudioClip clip)
	{
		if(autoPlay)
		{
			recorder.PlayAudio (clip, audioSource);

			// or you can use
			//recorder.PlayRecorded(audioSource);
		}
	}

	void OnRecordingSaved(string path)
	{
		Debug.Log("File Saved at : "+path);
		log += "\nFile save at : "+path;
		recorder.PlayAudio (path, audioSource);
	}
}
