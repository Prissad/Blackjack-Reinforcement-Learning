﻿using System;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	// Game Buttons
	public Button dealBtn;
	public Button hitBtn;
	public Button stickBtn;
	public Button restartGameBtn;
	public Button exitBtn;
	// Game Result Text
	public Text resultTxt;

	// Access Player's & Dealer's Scripts
	public PlayerScript playerScript;
	public DealerScript dealerScript;

	// Access A.I Hints Manager
	public AIHintsScript aiHintsScript;

	// Test if any of the player goes Bust
	private bool personBust = false;
	// Test if Player gets a Blackjack
	private bool playerBlackjack = false;


	/*
	 * Start Method
	 */
	void Start () {
		// Hide Result Text
		resultTxt.gameObject.SetActive(false);
		// Disable Restart Game Button
		restartGameBtn.interactable = false;
		// Disable Hit & Stick Buttons until round starts
		hitBtn.interactable = false;
		stickBtn.interactable = false;
		// Add Listeners to Buttons
		dealBtn.onClick.AddListener(() => DealClicked());
		hitBtn.onClick.AddListener(() => HitClicked());
		stickBtn.onClick.AddListener(() => StickClicked());
		restartGameBtn.onClick.AddListener(() => RestartGameClicked());
		exitBtn.onClick.AddListener(() => ExitClicked());
	}


	/*
	 * Deal Button Method.
	 */
	private void DealClicked()
	{
		// Disable Deal Button
		dealBtn.interactable = false;
		// Enable Hit & Stick Buttons
		hitBtn.interactable = true;
		stickBtn.interactable = true;
		// Shuffle the Deck.
		GameObject.Find(Constants.deck).GetComponent<DeckScript>().Shuffle();
		// Fill both Player's Hands Starting with the Dealer (to make traking the hidden card's position easier).
		dealerScript.FillHand();
		playerScript.FillHand();
		// Test for Bust and Blackjack
		TestBustAndBlackjackAndUpdateHint(drawCard: false);
	}

	/*
	 * Hit Button Method.
	 */
	private void HitClicked()
	{
		// If Charlie Rule limit reached, disable Deal Button
		if ( playerScript.hand.Count >= (Constants.charlieRuleCardLimit - 1) )
		{
			hitBtn.interactable = false;
		}
		// Draw a Card and Test for Bust and Blackjack
		TestBustAndBlackjackAndUpdateHint(drawCard: true);
	}

	/*
	 * Stick Button Method.
	 */
	private void StickClicked()
	{
		// Disable All Buttons
		hitBtn.interactable = false;
		stickBtn.interactable = false;
		// Play Dealer's Turn. At the end test if he goes Bust.
		if(dealerScript.BeginTurn())
		{
			SetResultText(Constants.dealerBust);
			this.personBust = true;
		}

		// Finish the Game and show the Results if no one is Bust
		EndGame(personBust: personBust, playerBlackjack: playerBlackjack);
	}


	/*
	 * Routine to check for Bust and Blackjack and update Hint Area accordingly.
	 */
	void TestBustAndBlackjackAndUpdateHint(bool drawCard)
	{
		// Draws a card if Hit, doesn't if Deal and Test Bust in both cases
		bool playerBust = drawCard ? playerScript.HitCardTestBust() : playerScript.TestBust();
		// Manage Bust
		if (playerBust)
		{
			SetResultText(Constants.playerBust);
			this.personBust = true;
			EndGame(personBust: personBust, playerBlackjack: playerBlackjack);
		}
		// Test Blackjack
		else if (playerScript.TestBlackjack())
		{
			SetResultText(Constants.playerBlackjack);
			playerBlackjack = true;
			EndGame(personBust: personBust, playerBlackjack: playerBlackjack);
		}
		// Update A.I Hint
		else
		{
			aiHintsScript.FindAction(playerScript.shownHandValue, dealerScript.shownHandValue);
		}
	}


	/*
	 * Changes the Result Text and shows it.
	 */
	void SetResultText(String resultText)
	{
		// changes the text
		resultTxt.text = resultText;
		// shows the text
		resultTxt.gameObject.SetActive(true);
	}


	/*
	 * Test who is the Winner.
	 */
	 void TestWinner()
	{
		if(playerScript.shownHandValue > dealerScript.shownHandValue)
		{
			SetResultText(Constants.playerWin);
		}
		else if(playerScript.shownHandValue < dealerScript.shownHandValue)
		{
			SetResultText(Constants.dealerWin);
		}
		else
		{
			SetResultText(Constants.tie);
		}
	}


	/*
	 * Finish Game by Disabling all Buttons
	 */
	void EndGame(bool personBust, bool playerBlackjack)
	{
		if ( personBust || playerBlackjack )
		{
			dealBtn.interactable = false; // Disable Deal Button
			hitBtn.interactable = false; // Disable Hit Button
			stickBtn.interactable = false; // Disable Stick Button
		}
		else
		{
			TestWinner();
		}

		// Update A.I Hints
		aiHintsScript.EndRoundHint();

		// Enable Restart Game Button
		restartGameBtn.interactable = true;
	}


	/*
	 * Start a new Game.
	 */
	private void RestartGameClicked()
	{
		SceneManager.LoadScene(Constants.blackjackTableScene);
	}


	/*
	 * Return To Main Menu.
	 */
	 private void ExitClicked()
	{
		SceneManager.LoadScene(Constants.mainMenuScene);
	}
}
