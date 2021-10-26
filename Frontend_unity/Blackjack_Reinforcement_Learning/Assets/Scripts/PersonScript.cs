using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public abstract class PersonScript : MonoBehaviour
{
	// import game scripts
	public DeckScript deckScript;

	public int shownHandValue = Constants.initialHandValue; // Player hand value

	public List<GameObject> hand; // Array of Cards representing the Player's hand

	protected int cardHandIndex = 0; // Index of Card in Hand that will be turned over


	// ABSTRACT METHODS
	protected abstract void GetCard();
	public abstract bool HitCardTestBust();


	/*
	 * Deal a Round.
	 */
	public void FillHand()
	{
		// Draw 2 Cards for each player
		GetCard();
		GetCard();
	}
	

	/*
	 * Find a Card Sprite position on the rendered Scene to add other Sprites relative to it
	 */
	protected static float GetHandCardPosition(String personGameObject, String cardGameObject, Char axis)
	{
		var position = UnityEngine.GameObject.Find("/" + personGameObject + "/" + cardGameObject).transform.position;
		switch (axis)
		{
			case 'x':
				return position.x;

			case 'y':
				return position.y;

			default:
				throw new Exception("Axis should be X or Y");
		}
	}


	/*
	 * Check if the Person who drew a Card goes Bust or not.
	 */
	protected bool TestBust()
	{
		if ( this.shownHandValue >= Constants.bustValue )
		{
			return true;
		}

		return false;
	}
}
