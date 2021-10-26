using Assets.Scripts;
using UnityEngine;

public class DeckScript : MonoBehaviour {

	public Sprite[] cardSprites; // contains all sprites
	int[] cardValues = new int[Constants.numberCardsDeck]; // contains values relative to sprites
	int currentTopDeckIndex = 0; // an index to point at the top non-dealt card of the deck.

	/*
	 * Start Method
	 */
	void Start () {
		// Assign card Values in Deck
		GetCardValues();		
	}

	/*
	 * Method used to assign Values to the different Cards while respecting the same order as the Sprites.
	 * Uses cardSprites to assign values in cardValues.
	 */
	void GetCardValues()
	{
		int num;
		int offset = Constants.minValue;

		for (int spriteCount = 0; spriteCount < cardSprites.Length; spriteCount++)
		{
			// Since there multiple repitions of the same numbers, we use a mod operation to repeat the values
			num = spriteCount % Constants.numberCardsPerColor;

			if(num >= Constants.jackPosition)
			{
				cardValues[spriteCount] = Constants.jackQueenKingValue;
			}
			else if (num == Constants.acePosition)
			{
				cardValues[spriteCount] = Constants.aceValue;
			}
			else
			{
				cardValues[spriteCount] = num + offset;
			}
		}

		cardValues[Constants.numberCardsDeck - 1] = Constants.cardBackValue;
	}

	/*
	 * Method called to Randomly Shuffle the Deck by swapping random elements in the array.
	 */
	public void Shuffle()
	{
		for (int spriteCount = cardSprites.Length - 1; spriteCount > 0; --spriteCount)
		{
			// select random value in array
			int randomCardPosition = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * (cardSprites.Length - 1));
			// switch sprites
			Sprite cardSprite = cardSprites[spriteCount];
			cardSprites[spriteCount] = cardSprites[randomCardPosition];
			cardSprites[randomCardPosition] = cardSprite;
			// switch values
			int cardValue = cardValues[spriteCount];
			cardValues[spriteCount] = cardValues[randomCardPosition];
			cardValues[randomCardPosition] = cardValue;
		}
	}

	/*
	 * Method used to deal cards at the start of the game.
	 * Input: A CardScript to pass the Sprites to.
	 */
	public int DealCard(CardScript cardScript, bool updateSprite = true)
	{
		// Initialize value to 0
		cardScript.Value = 0;
		// Number of attempts to do 
		int attemptGetValue = Constants.attemptsGetValue;
		// Loop of few time to make sure the value is correctly read
		while(cardScript.Value == 0 && attemptGetValue > 0)
		{
			// Update Sprite (if on) and value
			if (updateSprite)
			{
				RenderCardSprite(cardScript, this.currentTopDeckIndex);
			}
			cardScript.Value = cardValues[this.currentTopDeckIndex];

			// Decrement Attempts
			attemptGetValue--;
		}
		// Incerement Top Card Position in Deck
		this.currentTopDeckIndex++;
		return cardScript.Value;
	}
	/*
	 * Method used to update a specified card sprite from the card back sprite to the value sprite.
	 */
	public void RenderCardSprite(CardScript cardScript, int spritePosition)
	{
		cardScript.SetSprite(cardSprites[spritePosition]);
	}

	/*
	 * Returns the Back Card Sprite.
	 */
	public Sprite GetCardBack()
	{
		return cardSprites[Constants.backCardPosition];
	}

}
