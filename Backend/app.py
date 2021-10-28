# Setting Flask App
from flask import Flask, request
from flask_cors import CORS
app = Flask(__name__)
CORS(app)

# Importing Other Python Scripts
from Blackjack.MCC import runMCC
from Blackjack.SARSA import runSARSA


# Routing Web App
@app.route('/')
# Home Page
def mainRoute():
    return 'Welcome to Reinforcement Learning applied to Blackjack! Credits: Prissad'

@app.route('/mcc')
# Test Reinforcement Learning using MCC
def mccRoute():
    number_games = request.args.get('games')
    dealer_score = request.args.get('dealer')
    player_score = request.args.get('player')
    if( number_games is not None ):
        number_games = int(number_games)
        mcc_result = runMCC(number_games)
    else:
        mcc_result = runMCC()
    if ( dealer_score is not None and player_score is not None ):
        dealer_score = int(dealer_score)
        player_score = int(player_score)
        response = mcc_result[player_score][dealer_score]
    else:
        response = mcc_result
    return str(response)

@app.route('/sarsa')
# Test Reinforcement Learning using SARSA
def sarsaRoute():
    number_games = request.args.get('games')
    lambda_parameter = request.args.get('lambda')
    dealer_score = request.args.get('dealer')
    player_score = request.args.get('player')
    if ( number_games is not None and lambda_parameter is not None ):
        lambda_parameter = float(lambda_parameter)
        number_games = int(number_games)
        sarsa_result = runSARSA(lambda_parameter, number_games)
    elif ( number_games is not None ):
        number_games = int(number_games)
        sarsa_result = runSARSA(n_episodes=number_games)
    elif ( lambda_parameter is not None ):
        lambda_parameter = float(lambda_parameter)
        sarsa_result = runSARSA(lamb=lambda_parameter)
    else:
        sarsa_result = runSARSA()
    if ( dealer_score is not None and player_score is not None ):
        dealer_score = int(dealer_score)
        player_score = int(player_score)
        response = sarsa_result[player_score][dealer_score]
    else:
        response = sarsa_result
    return str(response)



# Main method for debugging
def main():
    print("app is working fine!")

if __name__ == '__main__':
    main()
