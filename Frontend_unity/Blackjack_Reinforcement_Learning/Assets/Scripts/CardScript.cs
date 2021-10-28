using UnityEngine;

public class CardScript : MonoBehaviour {

	public int Value { get; set; } // the card value's property


	/*
	 * Change the Card Sprite's value.
	 * Input: The Sprite to set the Card's Sprite to.
	 */
	public void SetSprite(Sprite newSprite)
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
	}
	
}
