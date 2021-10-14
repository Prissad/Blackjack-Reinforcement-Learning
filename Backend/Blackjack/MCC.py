# Import packages
import numpy as np
import random
from copy import deepcopy


# Importing Other Python Scripts
from Blackjack.Blackjack import Blackjack


# constants for player turn of the game
dealer_score_min = 1
dealer_score_max = 11
player_score_min = 1
player_score_max = 22


# Find optimal policy in MCC after 1_000_000 games
def runMCC(n_episodes=1_000_000):
    mc = MCC(exploration_constant=100, n_episodes=n_episodes)
    mc.learn_q_value_function()    
    states = list(mc.Q.keys())
    best_policy_mcc = [ [ 0 for i in range(dealer_score_max) ] for j in range(player_score_max - 2) ]
    row = 0
    column = 0
    for i in range(len(states) - dealer_score_max*2):
        best_action = max(mc.Q[states[i]], key=mc.Q[states[i]].get)
        best_policy_mcc[row][column] = best_action
        column += 1
        if (column == dealer_score_max):
            row += 1
            column = 0
        
    return best_policy_mcc


# Class identifying the Monte-Carlo Control Approach
class MCC():

  def __init__(self, exploration_constant, n_episodes):
    self.actions = ("hit", "stick")
    self.N_0 = exploration_constant # constant parameter (influence the exploration/exploitation behavior when starting to learn)
    self.n_episodes = n_episodes    # number of games to sample in order to make the agent learn

    self.Q = self.init_to_zeros()   # init Q function to zeros
    self.N = self.init_to_zeros()   # init N to zeros
    self.policy = "random"          # arbitrarily init the MC learning with a random policy 



  def learn_q_value_function(self):
        """
        Update the Q function until optimal value function is reached.
        
        Output
        ----------
        Q : {state: (action)}, Q value for every state-action pair
        """
        for i in range(self.n_episodes):
            episode = self.play_episode() # run an episode using current policy
            self.policy = "e_greedy"      # policy switch from random to epsilon greedy
            for step in episode: 
                state, action, reward = step
                self.increment_counter(state, action) # increment state-action counter 
                self.update_Q(state, action, reward)  # update the Q value
                
        return self.Q 



  def init_to_zeros(self):
        """
        Init the Q function and the incremental counter N at 0 for every state-action pairs.
        
        Output
        ----------
        lookup_table : {state: (action)}, a dictionnary of states as keys and actions as value
        """
        dealer_scores = np.arange(dealer_score_min, dealer_score_max+1)
        player_scores = np.arange(player_score_min, player_score_max+1)
        states = [(dealer_score, player_score) for player_score in player_scores for dealer_score in dealer_scores]       
        lookup_table = {}
        for state in states:
            lookup_table[state] = {"hit": 0, "stick": 0}  
            
        return lookup_table



  def play_episode(self):
        """
        Run a complete Blackjack game sequence given a policy. 
        
        Output
        ----------
        episode : [(state, action, reward)], a list of (statec, action, reward)
        """
        bj_game = Blackjack()            # init a game sequence
        state = bj_game.state.copy()     # init state
        episode = []                     # list of the steps of the game sequence
        while state != "terminal":      
            # pick an action regarding the current state and policy
            if self.policy == "random":
                action = self.random_policy()
            if self.policy == "e_greedy":
                action = self.e_greedy_policy(state)
            next_state, reward = deepcopy(bj_game.step(state, action))
            step = (state, action, reward)
            state = next_state
            episode.append(step)   
            
        return episode



  def update_Q(self, state, action, reward):
        """
        Update Q value towards the error term. 
        
        Input
        ----------
        state : state, the current score
        action : string, the current score
        reward : int, the current score
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        
        # The learning rate, decaying regarding the number of times an action-state pair 
        # has been explored. It scale the amount of modification we want to bring to 
        # the Q value function.
        alpha_t = 1 / self.get_state_action_counter(state, action)
        
        # We adjust the Q value towards the reality (observed) minus what we estimated.
        # This term is usually descrived as the error term.
        self.Q[lookup_state][action] += alpha_t * (reward - self.Q[lookup_state][action]) 
        
        return None



  def increment_counter(self, state, action):
        """
        Increment N counter for every action-state pair encountered in an episode.
        
        Input
        ----------
        state : state, the current score
        action : string, the current score
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        self.N[lookup_state][action] += 1        

        return None



  def random_policy(self):
        """
        Return an action follwing a random policy (state free).
        
        Output
        ----------
        action : string, random action
        """
        action = random.choice(self.actions)
        
        return action
     
     

  def e_greedy_policy(self, state):
        """
        Return an action given an epsilon greedy policy (state based).  
        
        Input
        ----------
        state : state, state where we pick the action
        
        Output
        ----------
        action : string, action from epsilon greedy policy
        """
        e = self.N_0/(self.N_0 + self.get_state_counter(state))
        if e > random.uniform(0, 1): 
            action = random.choice(self.actions)
        else:  
            action = self.get_action_w_max_value(state)
            
        return action
    


  def get_action_w_max_value(self, state):
        """
        Return the action with the max Q value at a given state.
        
        Input
        ----------
        state : state, state 
        
        Output
        ----------
        action : string, action from epsilon greedy policy
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        list_values = list(self.Q[lookup_state].values())
        if list_values[0] == list_values[1]:
            return self.random_policy()
        else:
            action = max(self.Q[lookup_state], key=self.Q[lookup_state].get) 
            return action
    



  def get_state_counter(self, state):
        """
        Return the counter for a given state.
        
        Input
        ----------
        state : state, state 
        
        Output
        ----------
        counter : int, the number of times a state as been explored
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        counter = np.sum(list(self.N[lookup_state].values()))  
        
        return counter
    


  def get_state_action_counter(self, state, action):
        """
        Return the counter for a given action-state pair.
        
        Input
        ----------
        state : state 
        action : string
        
        Output
        ----------
        counter : int, the number of times an action-state pair as been explored
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        counter = self.N[lookup_state][action]
        
        return counter
