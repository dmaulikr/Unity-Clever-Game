using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
	protected int score = 0;
	private Text scoreUI;
	protected int result = 0;
	protected int sum = 0;
	public int op = 1;
	private double timed = 5.0f;
	private float byTime;
	private int[] val = new int[10];
	private int[] free = new int[10];
	private string[] qt = new string[10];
	public Text _question;
	public Text _answer;
	public Image load;
	private int index = 1;
	private string x;
	private string st;
	private bool isLoad = false;
	private bool clear = false;
	public GameObject _current;
	public Text _score;
	public Text _best;
	public Text _new;
	private Text _bestScene;
	private bool begin = false;
	protected bool gameOver = false;
	bool next = false;
	Camera cam;
	public Transform minus;
	public Transform plus;
	public RectTransform _pointer;

	public Animator useAnimMS;
	public Animator clickAnimMinus;
	public Animator clickAnimPlus;
	public Animator animMoscot;

	void Start () {
		Time.timeScale = 1;

		cam = GetComponent<Camera> ();
		RandomColorCamera ();
		gameOver = false;
		begin = false;
		if (!begin) {
			load.fillAmount = 1;
		}
		Reset ();
		scoreUI = GameObject.Find ("Score").GetComponent<Text>();
		_bestScene = GameObject.Find ("Best").GetComponent<Text> ();
		_bestScene.text = "Best : " + Constants.getBestScore ();
		_current.SetActive(false);
		_new.enabled = false;
		_question.supportRichText = true;
	}
	
	void Update () {
		Answer (); 
		scoreUI.text = "Score : " + score;
		if (clear && !gameOver) {
			Reset();
		}
		if (timed <= 0 || gameOver) {
			this.StartCoroutine(CurrentScore());
		}
	}
	private void Question(){
		IndexRandom (9);
		Condition ();
		TimeIndex ();
		byTime = (float)timed;
//		QuestionColor ();
		for (int i=1; i<=op; i++) {
			if(i == 1){
				_question.text += Mathf.Abs(val[i]) + " ";
			}else{
//				if(val[i] < 0)
//				_question.text += "- " + Mathf.Abs(val[i]) + " ";
				_question.text += qt[i] + Mathf.Abs(val[i]) + " ";
//				else
//				_question.text += "+ " + Mathf.Abs(val[i]) + " ";
			}
		}
//		_answer.color = _question.color;
		useAnimMS.SetTrigger("message");
		result = Calculate (op);
		_question.text += "= " + result;
//		_answer.text = "= " + result;
		sum = val [1];
		st = _question.text;
		FindStringIndex (st, "?", free);
		StartCoroutine("CountDown");
	}
	private void IndexRandom(int value){
		for (int i=1; i<val.Length; i++) {
			if(i == 1)
				val[i] = Random.Range(0,value);
			else
				val[i] = Random.Range(-value,value);
				qt[i] = "<color=red>?</color> ";
		}
	}
	private int Calculate(int op){
		int cal = 0;
		for (int i=1; i<=op; i++) {
			cal += val[i];
		}
		return cal;
	}
	public void GetButton(string get){
		switch (get) {
		case "+":
			clickAnimPlus.SetTrigger("ClickPlus");
			if(index <= (op-1)){
				sum += Mathf.Abs(val[index+1]);
				st = st.Remove(free[index],1);
				st = st.Insert(free[index], "+"); // "<color=lime>+</color> "
				index++;
				_question.text = st;
				next = true;
				Pointer(op, index);
			}
			break;
		case "-":
			clickAnimMinus.SetTrigger("ClickMinus");
			if(index <= (op-1)){
				sum -= Mathf.Abs(val[index+1]);
				st = st.Remove(free[index],1);
				st = st.Insert(free[index], "-");
				index++;
				_question.text = st;
				next = true;
				Pointer(op, index);
			}
			break;
		}
	}
	private void Reset(){
		StopAllCoroutines ();
		isLoad = false;
		clear = false;
		next = false;
		sum = 0;
		result = 0;
		index = 1;
		op = 1;
//		timed = 5.0f;
		GameObject.Find ("True").GetComponent<Image> ().enabled = false;
		GameObject.Find ("False").GetComponent<Image> ().enabled = false;
		_question.text = "";
		Question ();
		Pointer(op, index);
	}
	private void Answer(){
		if (index == op && !clear) {
			if (sum == result && next) {
//				GameObject.Find ("True").GetComponent<Image> ().enabled = true;
				animMoscot.SetTrigger("Ans True");
				score++;
				next = false;
			}else if(sum != result){
//				GameObject.Find ("False").GetComponent<Image> ().enabled = true;
				animMoscot.SetTrigger("Ans False");
				gameOver = true;
			}
			begin = true;
			this.StartCoroutine(Wait());
		}
	}
	private void FindStringIndex(string words, string find, int[] index){
		int k = 1;
		for (int i=0; i<words.Length; i++) {
			if(words[i].ToString().Equals(find)){
				index[k++] = i;
			}
		}
	}
	IEnumerator Wait(){
		isLoad = true;
		yield return new WaitForSeconds (1f);
		clear = true;
		yield return 0;
	}
	IEnumerator CountDown(){
		while (timed > 0) {
			if(!isLoad && begin){
				timed -= Time.deltaTime;
				load.fillAmount = 1-(1-(float)timed/byTime);
				if(timed > byTime/2){
					load.color = new Color32((byte)MapValues((float)timed, byTime/2, byTime, 255, 0),255,0,255);
				}else{
					load.color = new Color32(255,(byte)MapValues((float)timed, 0, byTime/2, 0, 255),0,255);
				}
			}
			yield return 0;
		}
	}
	IEnumerator CurrentScore(){
		if (gameOver) {
			yield return new WaitForSeconds (2.5f);
		}
		StopAllCoroutines ();
		Button.instance.bannerView.Show ();
		_current.SetActive(true);
		if (score > Constants.getBestScore ()) {
			Constants.setBestScore(score);
			_new.enabled = true;
		}
		_score.text = score + "";
		_best.text = "Best Score : " + Constants.getBestScore ();
		Time.timeScale = 0;
	}
	public int Condition(){
		if (score >= 0 && score < 5) { //Random.Range(min, max-1);
			op = Random.Range(2,3); // 2-3
		} else if (score >= 5 && score < 10) {
			op = Random.Range(2,4); // 2-4
		} else if (score >= 10 && score < 20) {
			op = Random.Range(3,4);
		} else if (score >= 20 && score < 35) {
			op = Random.Range(3,5);
		} else if (score >= 35){
			op = Random.Range(4,5);
		}
		return op;
	}
	void TimeIndex(){
		switch(op){
		case 2: timed = 4.0f;
			break;
		case 3: timed = 6.0f;
			break;
		case 4: timed = 13.0f;
			break;
		default : timed = 5.0f;
			break;
		}
	}
	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax){
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
	private void RandomColorCamera(){
		int range = Random.Range (1, 6);
		switch (range) {
		case 1: cam.backgroundColor = new Color32(236,90,90,5);
			break;
		case 2: cam.backgroundColor = new Color32(146,146,146,5);
			break;
		case 3: cam.backgroundColor = new Color32(109,176,255,5);
			break;
		case 4: cam.backgroundColor = new Color32(160,255,0,0);
			break;
		case 5: cam.backgroundColor = new Color32(133,103,255,0);
			break;
		default : cam.backgroundColor = new Color32(236,90,90,5);
			break;
		}
	}
	private Color QuestionColor(){
		switch (op) {
		case 2: _question.color = Color.green;
			break;
		case 3: _question.color = new Color32(255,121,0,255);
			break;
		case 4: _question.color = Color.red;
			break;
		}
		return _question.color;
	}
	private void Pointer(int op, int index){
		int cursor = 0;
		cursor = PointerCursor();
		switch (op) 
		{
		case 2:  _pointer.anchoredPosition = new Vector2(-140 + cursor ,_pointer.anchoredPosition.y);
			break;

		case 3:switch(index){
			case 1: _pointer.anchoredPosition = new Vector2(-225 + cursor ,_pointer.anchoredPosition.y);
				break;
			case 2: _pointer.anchoredPosition = new Vector2(-39 + cursor ,_pointer.anchoredPosition.y);
				break;
			}
			break;

		case 4:switch(index){
			case 1: _pointer.anchoredPosition = new Vector2(-335 + cursor ,_pointer.anchoredPosition.y);
				break;
			case 2: _pointer.anchoredPosition = new Vector2(-140 + cursor ,_pointer.anchoredPosition.y);
				break;
			case 3: _pointer.anchoredPosition = new Vector2(58 + cursor ,_pointer.anchoredPosition.y);
				break;
			}
			break;
		}
	}
	int PointerCursor(){
		int cursor = 0;
		switch (_question.text.Length) {
		case 28: cursor = 23;
			break;
		case 52: cursor = -10;
			break;
		case 53: cursor = -32;
			break;
		case 74: cursor = +33;
			break;
		case 76: cursor = -15;
			break;
		default : cursor = 0;
			break;
		}
		return cursor;
	}

}
