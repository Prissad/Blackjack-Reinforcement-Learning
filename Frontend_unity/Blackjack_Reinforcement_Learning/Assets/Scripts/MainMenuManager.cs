using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	// Game Buttons
	public Button startBtn;
	public Button modelBtn;
	public Button creditsBtn;
	public Button quitBtn;
	public Button closeCreditsBtn;
	public Button closeModelBtn;
	// Main Menu Modals
	public GameObject creditsModalCanvas;
	public GameObject modelModalCanvas;


	/*
	 * Start Method
	 */
	void Start () {
		// Hide Modals
		creditsModalCanvas.SetActive(false);
		modelModalCanvas.SetActive(false);
		// Add Listeners to Buttons
		startBtn.onClick.AddListener(() => StartGameClicked());
		modelBtn.onClick.AddListener(() => ModelSettingsClicked());
		creditsBtn.onClick.AddListener(() => CreditsClicked());
		quitBtn.onClick.AddListener(() => QuitClicked());
		closeCreditsBtn.onClick.AddListener(() => CloseCreditsClicked());
		closeModelBtn.onClick.AddListener(() => CloseModelClicked());
	}


	/*
	 * Start the Game.
	 */
	private void StartGameClicked()
	{
		SceneManager.LoadScene(Constants.blackjackTableScene);
	}


	/*
	 * Access Model Settings.
	 */
	private void ModelSettingsClicked()
	{
		modelModalCanvas.SetActive(true);
	}


	/*
	 * Show Credits.
	 */
	private void CreditsClicked()
	{
		creditsModalCanvas.SetActive(true);
	}


	/*
	 * Leave the Game.
	 */
	private void QuitClicked()
	{
		Application.Quit();
	}


	/*
	 * Close the Credits Modal.
	 */
	private void CloseCreditsClicked()
	{
		creditsModalCanvas.SetActive(false);
	}


	/*
	 * Close the Model Settings Modal.
	 */
	private void CloseModelClicked()
	{
		modelModalCanvas.SetActive(false);
	}
}
