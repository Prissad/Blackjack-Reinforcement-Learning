# Import packages
import random

  
# Class identifying the Blackjack set rules
class Blackjack():
  
  def __init__(self):
    """
    Start the game by picking 2 cards for the player and the dealer.
    Dealer: 1 shown card + 1 hidden card
    Player: 2 shown cards
    """
    dealer_score, dealer_hidden_score = self.draw_two_cards_dealer()
    player_score = self.draw_two_cards_player()
    self.state = {"dealer_score": dealer_score, "dealer_hidden_score": dealer_hidden_score, "player_score": player_score} # initial state
    self.actions = ("hit", "stick")

    init_state = self.state.copy() # game history, recording (state, reward) and action of each step
    self.history = [init_state]



  def step(self, state, action):
        """
        Compute a step in Blackjack. 
        
        Input
        ----------
        state : state, the current state
        action : string, the action to pick
        
        Output
        -------
        state : state, new state reached given the picked action
        reward : int, the reward we get in this new state
        """
        self.history.append({"player": action})
        
        # player hits
        if action == "hit":
            value = self.draw_card()
            self.state['player_score'] = self.compute_new_score(value, current_score = self.state['player_score'])
            
            new_state = self.state.copy()
            
            if self.goes_bust(self.state['player_score']):
                # player goes bust/over 21
                reward = -1
                state = "terminal"
                self.history.append(new_state)
                self.history.append(state)
                return state, reward
            
            else:
                reward = 0
                self.history.append(new_state)
                return self.state, reward
            
        # player sticks   
        else:
            new_state = self.state.copy()
            self.history.append(new_state)
            
            state, reward = self.dealer_moves()
            return state, reward



  def draw_card(self):
        """
        Each draw from the deck results in a value between 1 and 13 (uniformly
        distributed) as follows:

        2 to 9 --> that Number card
        10 --> 10 or Jack or Queen or King
        11 --> Ace
        """
        value = random.randint(2, 11)
        return value



  def draw_two_cards_player(self):
        """
        Player Draws two cards at the start
        """
        initial_player_score = self.draw_card() + self.draw_card()
        return initial_player_score



  def draw_two_cards_dealer(self):
        """
        Dealers Draws two cards at the start
        BUT one of them should remain hidden until later
        """
        initial_dealer_score = self.draw_card()
        hidden_dealer_score = self.draw_card()
        return initial_dealer_score, hidden_dealer_score


  
  def goes_bust(self, score):
        """
        Tells if the player/dealer goes bust (over 21 points)
        
        Input
        ----------
        score : int, the current score
        
        Output
        -------
        bool : either goes bust 
        """
        return (score > 21)



  def compute_new_score(self, value, current_score):
        """
        Compute the new score given the value of the pulled card
        
        Input
        ----------
        value : int, card's value
        current_score : int, the current score to update
        
        Output
        -------
        new_score : integer       
        """
        new_score = current_score + value
        return new_score



  def dealer_moves(self): 
        """
        Fixed dealer policy
        
        Returns
        -------
        state : state, the terminal state of the game sequence
        reward : int, the reward obtained in the terminal state of the game sequence
        """
        # Add the hidden card score to the Dealer's score as his turns starts
        self.state['dealer_score'] += self.state['dealer_hidden_score']
        self.state['dealer_hidden_score'] = 0

        # dealer hits as long as his score is < 17
        while self.state['dealer_score'] < 17:
            value = self.draw_card()
            new_dealer_score = self.compute_new_score(value, current_score = self.state['dealer_score'])
            self.state['dealer_score'] = new_dealer_score
            
            new_state = self.state.copy()
            self.history.append({"dealer": "hit"})
            self.history.append(new_state)
            
            
            if self.goes_bust(new_dealer_score):
                # dealer goes bust, player wins
                reward = 1
                state = "terminal"
                self.history.append(state)
                return state, reward
            
        self.history.append({"dealer": "stick"})  
        
        player_score = self.state['player_score']
        dealer_score = self.state['dealer_score']  
        
        # score > 17 -> dealer sticks
        state = "terminal"
        self.history.append(state)
        if dealer_score < player_score: # player wins
            reward = 1
            return state, reward                    
        if dealer_score == player_score: # draw
            reward = 0
            return state, reward                 
        if dealer_score > player_score: # player loses
            reward = -1
            return state, reward

