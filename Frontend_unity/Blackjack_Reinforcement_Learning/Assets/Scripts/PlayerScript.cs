using Assets.Scripts;
using UnityEngine;

public class PlayerScript : PersonScript {


	/*
	 * Add 1 Card to Player's / Dealer's Hand
	 */
	protected override void GetCard()
	{
		// Get a new Card from Deck
		int newCardValue = deckScript.DealCard(
			cardScript: hand[this.cardHandIndex].GetComponent<CardScript>(), 
			updateSprite: true
			);
		// Render the Card's Sprite
		hand[this.cardHandIndex].GetComponent<Renderer>().enabled = true;
		// Update Hand Value
		this.shownHandValue += newCardValue;
		// Incerement index of Card in Hand
		this.cardHandIndex++;
	}


	/*
	 * Adds a Card to the Player Hand.
	 */
	public override bool HitCardTestBust()
	{
		// Create a new GameObject Sprite Renderer to be the new Card
		var newCardObject = new GameObject(Constants.cardInitial + (this.cardHandIndex + 1));
		newCardObject.AddComponent<SpriteRenderer>();
		newCardObject.AddComponent<CardScript>();
		// Add new GameObject to the Player Object
		newCardObject.transform.SetParent(GameObject.Find(Constants.player).transform);
		// Set the GameObject's Position
		newCardObject.transform.position = new Vector3(
			GetHandCardPosition(Constants.player, Constants.firstCard, 'x') + Constants.positionStepX * this.cardHandIndex,
			GetHandCardPosition(Constants.player, Constants.firstCard, 'y') + Constants.positionStepY * this.cardHandIndex,
			Constants.cardPositionZ);
		// Add new Card to hand
		hand.Add(newCardObject);
		// Set the Card value and Sprite
		GetCard();		

		// Check if Player goes Bust
		return TestBust();
	}


	/*
	 * Test Blackjack for instant Win.
	 */
	public bool TestBlackjack()
	{
		return this.shownHandValue == Constants.blackjackValue;
	}
	
}
