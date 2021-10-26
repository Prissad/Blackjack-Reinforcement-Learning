using Assets.Scripts;
using UnityEngine;

public class CardScript : MonoBehaviour {

	private int value = 0; // the card value (initialized to 0)
	public int Value { get; set; } // the card value's property

	/*
	 * Returns the Card Sprite's name.
	 */
	public string GetSpriteName()
    {
		return GetComponent<SpriteRenderer>().sprite.name;
    }

	/*
	 * Change the Card Sprite's value.
	 * Input: The Sprite to set the Card's Sprite to.
	 */
	public void SetSprite(Sprite newSprite)
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
	}

	/*
	 * Return a shown card to make its backside show instead.
	 */
	public void ResetCard() {
		Sprite cardBack = GameObject.Find(Constants.deck).GetComponent<DeckScript>().GetCardBack();
		gameObject.GetComponent<SpriteRenderer>().sprite = cardBack;
		this.value = 0;
	}
	
}
