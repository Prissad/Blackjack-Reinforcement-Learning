using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class AIHintsScript : MonoBehaviour {

	// Check if A.I trained
	public static bool isTrained = false;

	// Text to Modify
	public Text hintTxt;


	/*
	 * Start Method.
	 */
	void Start()
	{
		// Automatic assign value
		AssignHintText(Constants.modelNotTrained);
		// Check if trained
		if ( ReinforcementLearningService.reinforcementLearningResult != null 
			&& ReinforcementLearningService.reinforcementLearningResult.Length > 0 )
		{
			isTrained = true;
			AssignHintText(Constants.endRound);
		}
	}


	/*
	 * Given the two player's scores, determines the A.I hint.
	 */
	public void FindAction(int playerScore, int dealerScore)
	{
		// check if model trained
		if (!isTrained) return;
		// Set in progress text
		AssignHintText(Constants.checkingModel);
		// Get Hint
		string hint = ReinforcementLearningService.reinforcementLearningResult[playerScore - 1][dealerScore - 1];
		// Format the text (make first letter uppercase)
		hint = hint[0].ToString().ToUpper() + hint.Substring(1);
		// Show hint
		AssignHintText(hint);
	}


	/*
	 * Show End of round text.
	 */
	public void EndRoundHint()
	{
		// check if model trained
		if (!isTrained) return;
		// Set end of round text
		AssignHintText(Constants.endRound);
	}


	/*
	 * Change the hint text by the given value.
	 */
	private void AssignHintText(string textToShow)
	{
		hintTxt.text = textToShow;
	}
}
