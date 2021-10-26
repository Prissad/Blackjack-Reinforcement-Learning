using System;
using System.Collections;
using Assets.Scripts;
using UnityEngine;

public class DealerScript : PersonScript {

	
	private int hiddenCardValue = Constants.initialHandValue; // Hidden Dealer's Card


	/*
	 * Add 1 Card to Player's / Dealer's Hand
	 */
	protected override void GetCard()
	{
		// Test Dealer's number of drawn Cards
		if ( this.cardHandIndex == Constants.hiddenCardPositionHand ) // the dealer draws his first card
		{
			// Get a new Card from Deck without updating its Sprite
			int newHiddenCardValue = deckScript.DealCard(
				cardScript: hand[this.cardHandIndex].GetComponent<CardScript>(),
				updateSprite: false
				);
			// use the first card that the dealer draws as his hidden card
			this.hiddenCardValue = newHiddenCardValue;
		} 
		else // the dealer already has a card, so the drawn card will be shown
		{
			// Get a new Card from Deck and update its Sprite
			int newCardValue = deckScript.DealCard(
				cardScript: hand[this.cardHandIndex].GetComponent<CardScript>(),
				updateSprite: true
				);
			// Render the Card's Sprite
			hand[this.cardHandIndex].GetComponent<Renderer>().enabled = true;
			// Update Hand Value
			this.shownHandValue += newCardValue;
		}

		// Incerement index of Card in Hand
		this.cardHandIndex++;
	}


	/*
	 * Begins the Dealer's Turn with the specified rules at the end of the player's turn (stick).
	 */
	public bool BeginTurn()
	{
		// Reveal the hidden card
		RevealHiddenCard();
		// Test if the Dealer goes Bust after showing the hidden Card
		bool dealerBust = TestBust();
		// while the hand sum in under the specified min value, continue to draw cards
		while ( !(dealerBust)
			&& hand.Count < Constants.charlieRuleCardLimit
			&& this.shownHandValue < Constants.dealerMinStickValue)
		{
			// Dealer draws a card
			dealerBust = HitCardTestBust();
		}

		return dealerBust;
	}


	/*
	 * Reveal Dealer's hidden Card
	 */
	private void RevealHiddenCard()
	{
		// Render the hidden Card's Sprite
		deckScript.RenderCardSprite(
			hand[Constants.hiddenCardPositionHand].GetComponent<CardScript>(),
			Constants.hiddenCardPositionHand);
		// Update Hand Value
		this.shownHandValue += this.hiddenCardValue;
		// Reset hidden card value to zero
		this.hiddenCardValue = 0;
	}


	/*
	 * Adds a Card to the Dealer Hand.
	 */
	public override bool HitCardTestBust()
	{
		// Create a new GameObject Sprite Renderer to be the new Card
		var newCardObject = new GameObject(Constants.cardInitial + (this.cardHandIndex + 1));
		newCardObject.AddComponent<SpriteRenderer>();
		newCardObject.AddComponent<CardScript>();
		// Add new GameObject to the Dealer Object
		newCardObject.transform.SetParent(GameObject.Find(Constants.dealer).transform);
		// Set the GameObject's Position
		newCardObject.transform.position = new Vector3(
			GetHandCardPosition(Constants.dealer, Constants.firstCard, 'x') + Constants.positionStepX * this.cardHandIndex,
			GetHandCardPosition(Constants.dealer, Constants.firstCard, 'y') + Constants.positionStepY * this.cardHandIndex,
			Constants.cardPositionZ);
		// Add new Card to hand
		hand.Add(newCardObject);
		// Set the Card value and Sprite
		GetCard();

		// Check if Dealer goes Bust
		return TestBust();
	}

}
