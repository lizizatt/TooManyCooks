using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class HighScores : MonoBehaviour {

	//schema:  <string> <number> <newline>

	private string filePath = "";

	//first object is the title line
	//next 5 lines are scores
	public GameObject highScoreUIObject = null;

	//has a button and a text entry
	public GameObject newScoreEntry = null;



	class HighScore
	{
		public string name;
		public int value;
		public HighScore(string name, int value)
		{
			this.name = name;
			this.value = value;
		}

		public HighScore(string originalString)
		{
			string[] split = originalString.Split(' ');
			if (split.Length == 2) {
				name = split[0];
				value = int.Parse(split[1]);
			}
		}

		public string toString()
		{
			return name + " " + value;
		}
			
	}

	private static HighScores _instance = null;
	public static HighScores Instance
	{
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<HighScores>();
			}
			return _instance;
		}
	}

	private List<HighScore> scores = new List<HighScore>();

	public void NewScore(string name, int score)
	{
		scores.Add (new HighScore (name, score));
		sort ();
		while (scores.Count > 5) {
			scores.RemoveAt (5);
		}
		save ();

		UpdateText ();
	}

	void sort()
	{
		for (int j = 0; j < scores.Count; j++) {
			int maxScore = j;
			for (int i = j; i < scores.Count; i++) {
				if (scores [i].value > scores [maxScore].value) {
					maxScore = i;
				}
			}
			HighScore a = scores [maxScore];
			scores.RemoveAt (maxScore);
			scores.Insert (j, a);
		}
		//scores.Sort ();
	}

	void save()
	{
		sort ();
		string[] toWrite = new string[scores.Count];;
		for (int i = 0; i < scores.Count; i++) {
			toWrite [i] = scores [i].toString ();
		}
		System.IO.File.WriteAllLines (filePath, toWrite);
	}

	void load()
	{
		if (System.IO.File.Exists(filePath)) {
			string[] split = System.IO.File.ReadAllLines (filePath);
			for (int i = 0; i < split.Length; i++) {
				scores.Add (new HighScore (split [i]));
			}
			sort ();
		}
	}

	public void Start()
	{
		filePath = Application.dataPath + "/highscores.txt";  

		load ();

		newScoreEntry.GetComponentInChildren<Button> ().onClick.AddListener (EnterNewScore);

		newScoreEntry.SetActive (false);

		UpdateText ();
	}

	public void UpdateText()
	{
		Text[] texts = highScoreUIObject.GetComponentsInChildren<Text> ();
		for (int i = 0; i < texts.Length; i++) {
			if (i >= scores.Count) {
				texts [i].text = "0";
				continue;
			}
			texts [i].text = scores [i].toString ();
		}
	}

	public void ToggleNewScoreEntry()
	{
		highScoreUIObject.SetActive (false);
		newScoreEntry.SetActive (true);
		newScoreEntry.GetComponentInChildren<InputField> ().text = "";
	}

	public void EnterNewScore()
	{
		string name = newScoreEntry.GetComponentInChildren<InputField> ().text;
		int score = SceneController.Instance.GetNumCooks ();
		NewScore (name, score);
		highScoreUIObject.SetActive (true);
		newScoreEntry.SetActive (false);
		save ();
	}

	public bool IsNewHighScore(int num)
	{
		if (scores.Count > 0) {
			return num > scores [scores.Count - 1].value;
		}
		return true;
	}
}
