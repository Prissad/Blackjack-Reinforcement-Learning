using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ModelSettingsScript : MonoBehaviour {

	// Main Menu
	public MainMenuManager mainMenuManager;
	// Settings Elements
	public Button trainModelBtn;
	public Button closeModelBtn;
	public Toggle learningMethodSARSAToggle;
	public InputField numberGamesInput;
	public Slider lambdaValueSlider;
	public Text lambdaValuePrinter;
	// Service
	public ReinforcementLearningService reinforcementLearningService;
	// Result Dialog Elements
	public GameObject resultDialog;
	public Text resultTitleTxt;
	public Text resultInfoTxt;
	public Button resultContinueBtn;


	/*
	 * Start Method
	 */
	void Start () {
		// Hide result Dialog
		resultDialog.SetActive(false);
		// Add Listener to Buttons
		trainModelBtn.onClick.AddListener(() => TrainModelClicked());
		closeModelBtn.onClick.AddListener(() => CloseModelClicked());
		resultContinueBtn.onClick.AddListener(() => CloseResultDialogClicked());
		// Add Listeners to value changes
		numberGamesInput.onValueChanged.AddListener(delegate { AllowOnlyPositiveNumbers(); });
		lambdaValueSlider.onValueChanged.AddListener(delegate { UpdateLambdaValuePrinter(); });
		learningMethodSARSAToggle.onValueChanged.AddListener(delegate { ToggleLambdaValue(); });
		// Disable Lambda Attribute
		lambdaValueSlider.interactable = false;
		// Set default Setting Values
		numberGamesInput.text = Constants.defaultNumberGames.ToString();
		lambdaValueSlider.value = Constants.defaultLambdaValue;
	}


	/*
	 * Calls the Reinforcement Learning REST Service.
	 */
	public void TrainModelClicked()
	{		
		// Find Needed Parameters
		string learningMethod = learningMethodSARSAToggle.isOn ? Constants.apiUriSARSA : Constants.apiUriMCC;
		string numberGames = numberGamesInput.text;
		string lambdaValue = lambdaValueSlider.value.ToString();
		// Call Service
		StartCoroutine(
			this.reinforcementLearningService.GetReinforcementLearningResult(
				learningMethod: learningMethod,
				numberGames: numberGames,
				lambda: lambdaValue)
		);		
	}


	/*
	 * Method that the Service will call BEFORE launching the http Request.
	 */
	public void BeforeApiCall()
	{
		// Disable Button
		trainModelBtn.interactable = false;
		// Update Button Text
		trainModelBtn.GetComponentInChildren<Text>().text = Constants.trainBtnProgressTxt;
	}


	/*
	 * Method that the Service will call AFTER launching the http Request.
	 */
	public void AfterApiCall(UnityWebRequest getRequest)
	{
		// Show Success or Fail Message to User
		if (!getRequest.isNetworkError && !getRequest.isHttpError && getRequest.isDone)
		{
			DisplayResultDialog(
				Constants.dialogBoxSuccessTitle,
				Constants.dialogBoxSuccessInfo,
				Constants.dialogBoxOkButton);
		}
		else
		{
			DisplayResultDialog(
				Constants.dialogBoxFailTitle,
				Constants.dialogBoxFailInfo,
				Constants.dialogBoxOkButton);
		}
		// Reset Button Text
		trainModelBtn.GetComponentInChildren<Text>().text = Constants.trainBtnDefaultTxt;
		// Enable Button
		trainModelBtn.interactable = true;
	}


	/*
	 * Configure and Show the Result Modal/Dialog.
	 */
	private void DisplayResultDialog(string resultTitle, string resultInfo, string resultBtnText)
	{
		// Set the Dialog Box Content
		resultTitleTxt.text = resultTitle;
		resultInfoTxt.text = resultInfo;
		resultContinueBtn.GetComponentInChildren<Text>().text = resultBtnText;
		// Show the Dialog Box
		resultDialog.SetActive(true);
	}


	/*
	 * Make Lambda value only available when SARSA method is on.
	 */
	private void ToggleLambdaValue()
	{
		lambdaValueSlider.interactable = learningMethodSARSAToggle.isOn;
	}


	/*
	 * Stops the Input of negative sign in number's field AND sets a max number of digits.
	 */
	private void AllowOnlyPositiveNumbers()
	{
		if ( numberGamesInput.text == Constants.negativeSign )
		{
			numberGamesInput.text = string.Empty;
		}
		else if ( numberGamesInput.text.Length > Constants.maxNumberGamesDigitsAllowed )
		{
			numberGamesInput.text = numberGamesInput.text.Remove(numberGamesInput.text.Length - 1, 1);
		}
	}


	/*
	 * Calls the Reinforcement Learning REST Service.
	 */
	private void UpdateLambdaValuePrinter()
	{
		lambdaValuePrinter.text = lambdaValueSlider.value.ToString();
	}


	/*
	 * Close the Model Settings Modal.
	 */
	private void CloseModelClicked()
	{
		mainMenuManager.modelModalCanvas.SetActive(false);
	}


	/*
	 * Close the Result Dialog.
	 */
	private void CloseResultDialogClicked()
	{
		resultDialog.SetActive(false);
	}

}
