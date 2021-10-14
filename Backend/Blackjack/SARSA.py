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


# Find optimal policy in SARSA after 1_000_000 games
def runSARSA(lamb=0, n_episodes=1_000_000):
    sarsa_Q = SARSA(lamb=lamb, n_episodes=n_episodes, N_0=100)
    sarsa_Q.learn_q_value_function()    
    states = list(sarsa_Q.Q.keys())
    best_policy_sarsa = [ [ 0 for i in range(dealer_score_max) ] for j in range(player_score_max - 2) ]
    row = 0
    column = 0
    for i in range(len(states) - dealer_score_max*2):
        best_action = max(sarsa_Q.Q[states[i]], key=sarsa_Q.Q[states[i]].get)
        best_policy_sarsa[row][column] = best_action
        column += 1
        if (column == dealer_score_max):
            row += 1
            column = 0
        
    return best_policy_sarsa


# Class identifying the Monte-Carlo Control Approach
class SARSA():
    
  def __init__(self, lamb, n_episodes, N_0=100):
        self.actions = ("hit", "stick") 
        self.lamb = lamb                # lambda parameter of the SARSA algorithm
        self.n_episodes = n_episodes    # number of episodes (games) to sample in order to make the agent learn
        self.N_0 = N_0                  # constant parameter (influence the exploration/exploitation behavior when starting to learn)
        
        self.Q = self.init_to_zeros()   # init Q function to zeros
        self.N = self.init_to_zeros()   # init the counter traces to zeros
        
        # used for plot
        self.Q_history = {} 
        self.list_n_episodes = np.linspace(10, n_episodes-1, 30, dtype=int)



  def learn_q_value_function(self):
        """
        Update the Q function until optimal value function is reached.
        
        Output
        ----------
        Q : {state: (action)}, Q value for every state-action pair
        """
        for i in range(self.n_episodes):
            self.eligibilty_traces = self.init_to_zeros()    # init eligibilty traces to zeros
            bj_game = Blackjack()                            # init a game sequence
            state = bj_game.state.copy()                     # init state    
            action = self.e_greedy_policy(state)             # pick a first action
            self.increment_counter(state, action)
            
            while state != "terminal":      
                next_state, reward = deepcopy(bj_game.step(state, action))
                
                if next_state == "terminal":
                    next_action = None
                    delta = self.compute_delta(state, action, next_state, next_action, reward)
                    
                else:   
                    next_action = self.e_greedy_policy(next_state)   
                    delta = self.compute_delta(state, action, next_state, next_action, reward)
                    self.increment_counter(next_state, next_action)
                                                

                self.increment_eligibility_traces(state, action)
                self.update_step(delta)
                
                action = next_action
                state = next_state

            
            if i in self.list_n_episodes: 
                self.Q_history[i] = deepcopy(self.Q)
            
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


 
  def update_step(self, delta):
        """
        Update the Q value towards the error term and eligibility traces . 
        
        Input
        ----------
        delta : float, the delta factor of the current state-action pair
        """
        for state in self.Q.keys():
            for action in self.actions:
                alpha = 1 / (self.get_state_action_counter(state, action) + 1)
                self.Q[state][action] += alpha * delta * self.eligibilty_traces[state][action]
                # Here is where the lambda parameter intervene. The higher, the longer the eligibility trace
                # associated to a state-action pair will remain 
                self.eligibilty_traces[state][action] *= self.lamb
        return None


    
  def compute_delta(self, state, action, next_state, next_action, reward):
        """
        Update Q value towards the error term, it is the TD learning step. 
        
        Input
        ----------
        state : state, the current state
        action : string, the current action
        reward : int, the current score
        next_state : int, the state we end after taking the action
        next_action : int, the action we take in next state following the policy (e greedy)
        
        Output
        ----------
        delta : float, the TD error term
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        if next_state == "terminal":
            delta = reward - self.Q[lookup_state][action]
        else:
            next_lookup_state = (next_state["dealer_score"], next_state["player_score"])
            delta = reward + self.Q[next_lookup_state][next_action] - self.Q[lookup_state][action]
        return delta



  def increment_eligibility_traces(self, state, action):
        """
        Increment N counter for every action-state pair encountered in an episode.
        
        Input
        ----------
        state : state, the current score
        action : string, the current score
        """
        lookup_state = (state["dealer_score"], state["player_score"])
        self.eligibilty_traces[lookup_state][action] += 1  
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
        counter = self.N[state][action]
        
        return counter
