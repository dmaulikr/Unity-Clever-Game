using UnityEngine;
using System.Collections;

public class Constants : MonoBehaviour {
	public static int getBestScore(){
		return PlayerPrefs.GetInt ("BEST_SCORE", 0);
	}
	public static void setBestScore(int score){
		PlayerPrefs.SetInt ("BEST_SCORE", score);
	}
}
