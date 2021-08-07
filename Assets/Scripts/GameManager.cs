using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //prefabs
    public Tile tilePrefab;


    //2d array representing the tiles
    private Tile[,] board; //representation of the board on the screen, except when imagining as 2d array with rows and columns, on the screen it would be flipped
    //i refers to y and j refers to x

    public Text winnerText;
    public Text depthText;

    public Slider depthSlider;

    //whose turn
    private int turn;

    //game over
    private bool gameover;
    private int gamesplayed;
    private int userwon;

    //game settings
    public int BOARD_SIZE = 7;
    public int DEPTH = 1;
    


    // Start is called before the first frame update
    void Start()
    {
        //creates all the tiles
        board = new Tile[BOARD_SIZE, BOARD_SIZE];

        for(int i = 0; i < BOARD_SIZE; i++)
        {
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                board[i, j] = Instantiate(tilePrefab) as Tile;
                board[i, j].transform.position = new Vector2(j * 2, (BOARD_SIZE - 1 - i ) * 2);
            }
        }

        gameover = false;
        turn = 0;
        gamesplayed = 0;
        userwon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //checks to see if any of the tiles have been clicked


        if(gameover && Input.GetMouseButtonDown(0))
        {
            Restart();
        }

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                if(board[i, j].getClick()) //was clicked
                {

                    //see if you can place in that row

                    int row = BOARD_SIZE-1;

                    while(row >= 0  && board[row, j].getType() != 3) //gets the top spot that isn't filled
                    {
                        row--;
                    }

                    if (row >= 0) //is an actual spot
                    {

  
                        board[row, j].changeType(1);
                        
                    }


                    int winner = CheckWin(GetBoard(board), row, j, turn % 2, turn);

                    if (winner == 0)
                    {
                        winnerText.text = "BLUE WON\nTAP TO PLAY AGAIN";

                        board[i, j].cancelClick();
                        turn++;
                        gameover = true;
                        userwon++;
                        return;
                    }

                    turn++;

                    AITurn(); //ai goes

                    //cancel clicks and update turn
                    board[i, j].cancelClick();

                    return;
                }
            }
        }


    }

    private void AITurn()
    {//user turn over, now ai turn

        int ai = Minimax(CloneBoard(GetBoard(board)), -10000000, 1000000, false, DEPTH);
        //Debug.Log("spot with turn " + turn + " is " + ai);

        //now place the ai tile

        int row = BOARD_SIZE - 1;

        while (row >= 0 && board[row, ai].getType() != 3) //gets the top spot that isn't filled
        {
            row--;
        }

        board[row, ai].changeType(2);

        int winner = CheckWin(GetBoard(board), row, ai, turn % 2, turn);

        if (winner == 1)
        {
            winnerText.text = "PINK WON\nTAP TO PLAY AGAIN";
            gameover = true;
        }

        turn++;

    }

    private void Restart()
    {
        gamesplayed++;
        for(int i = 0; i < BOARD_SIZE; i++)
        {
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                board[i, j].changeType(3); //make all the same
                board[i, j].cancelClick();
                winnerText.text = "IT'S YOUR TURN\nBLUE WON: " + userwon + "\nPINK WON: " + (gamesplayed - userwon);
            }
        }

        //let pink go 
        gameover = false;
        turn = 0;
    }
    
    private string getName(int i)
    {
        if(i == 0)
        {
            return "BLUE";
        }
        return "PINK";
    }

    private bool NotOutOfRange(int x, int y) //don't wanna simplify, too lazy
    {
        return !(x < 0 || x >= BOARD_SIZE || y < 0 || y >= BOARD_SIZE);
    }

    private int[,] GetBoard(Tile[,] board) //returns int version of the board
    {
        int[,] newBoard = new int[BOARD_SIZE, BOARD_SIZE];
        for(int i = 0; i < BOARD_SIZE; i++)
        {
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                newBoard[i, j] = board[i, j].getType();
            }
        }
        return newBoard;
    }

    private int CheckWin(int[,] board, int row, int column, int player, int turn) //returns -1 if no one won, else the player parameter if someone won; parameters are the i and j position of where the tile was placed
    {
        //you win if there are four of the same number in a row in the board array
        int counter = 0;
        for (int z = -3; z <= 3; z++)
        {
            if (NotOutOfRange(row + z, column))
            {
                if (board[row + z, column] == (turn % 2) + 1) //match
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }

            if (counter == 4)
            {
                return player;
            }

        }

        counter = 0;
        for (int z = -3; z <= 3; z++)
        {
            if (NotOutOfRange(row, column + z))
            {
                if (board[row, column + z] == (turn % 2) + 1) //match
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }

            if (counter == 4)
            {
                return player;
            }

        }

        counter = 0;
        for (int z = -3; z <= 3; z++)
        {
            if (NotOutOfRange(row + z, column + z))
            {
                if (board[row + z, column + z] == (turn % 2) + 1) //match
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }

            if (counter == 4)
            {
                return player;
            }

        }

        counter = 0;
        for (int z = -3; z <= 3; z++)
        {
            if (NotOutOfRange(row + z, column - z))
            {
                if (board[row + z, column - z] == (turn % 2) + 1) //match
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }
            }

            if (counter == 4)
            {
                return player;
            }

        }

        return -1;
    }

    private int ScoreBoard(int[,] b, bool isMaximizing) //parameters are an int representation of the board - 1 is user, 2 is ai, 3 is empty, and ismaximizing is either true (user) or false (ai); returns an int that represents the score of the board - user wants to maximize, AI wants to minimize
    {
        //the way this strategy works: place a tile at every empty spot and see if it results in a win - if it does, add onto score

        int[,] board = CloneBoard(b);

        int score = 0;

        for(int i = 0; i < BOARD_SIZE; i++)
        {
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                if(board[i, j] == 3) //is empty
                {
                    //does blue
                    board[i, j] = 1;

                    if(CheckWin(board, i, j, 1, 0) == 1) //can win if the blue tile is placed there
                    {
                       // Debug.Log("blue");
                        if(isMaximizing && IsTop(board, i, j)) //blues turn and the player can win
                        {
                            score += 10000;
                        } else
                        {
                            score += 1;
                        }
                  
                    }

                    //now does pink
                    board[i, j] = 2;

                    if(CheckWin(board, i, j, 2, 1) == 2)
                    {
                        //Debug.Log("pink");
                        if(!isMaximizing && IsTop(board, i, j)) //pink can win this turn
                        {
                            score -= 10000;
                        } else
                        {
                            score -= 1;
                        }
                    }

                    //changes back
                    board[i, j] = 3;
                }
            }
        }
        return score;
    }

   

    private bool IsTop(int[,] board, int row, int column) //returns true if the row and column is the lowest empty spot in its column
    {
        //the pre condition is that the row and column spot is empty(though in the board array, it may be a 1 or 2)

        if(NotOutOfRange(row+1, column)) //there is a spot below this one
        {
            return board[row + 1, column] != 3; //returns true if the spot below is full
        }
        return true; //lowest spot, and is empty
    }

    private int Minimax(int[,] board, int alpha, int beta, bool isMaximizing, int depth) //returns either a score or position of where the AI should play
    {
        //check if game is over

        for(int row = 0; row < BOARD_SIZE; row++)
        {
            for(int column = 0; column < BOARD_SIZE; column++)
            {
                int blue_won = CheckWin(CloneBoard(board), row, column, 1, 0);
                int pink_won = CheckWin(CloneBoard(board), row, column, 2, 1);

                if (blue_won == 1 || pink_won == 2) //game is over since either blue or pink won
                {
                    if (isMaximizing) //is the users turn, so previous turn was the computers
                    {
                        //Debug.Log("gameover at: " + depth + " return -1000000");
                        return -1000000; //computer just won
                    }
                    else
                    {
                        //Debug.Log("gameover at: " + depth + " return 1000000");

                        return 1000000; //user just won

                    }
                }
            }
        }


        //check if the board is filled

        bool filled = true; //assume board is filled

        for(int i = 0; i < BOARD_SIZE; i++)
        {
            bool quit = false;
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                if(board[i, j] == 3) //found out board isn't filled yet
                {
                    filled = false;
                    quit = true;
                    break;
                }
            }
            if(quit)
            {
                break;
            }
        }

        if(filled) //board is filled
        {
            //Debug.Log("board is filled at depth " + depth + " return 0");
            return 0;
        }


        //if not, do base case

        if(depth == 0)
        {
            //Debug.Log("return score at depth " + depth + " is " + ScoreBoard(CloneBoard(board), isMaximizing));
            return ScoreBoard(CloneBoard(board), isMaximizing); //returns the score of the board
        }

        //Debug.Log("depth: " + depth);

        //finally, branch out

        List<int> positions = AvailablePositions(CloneBoard(board));

        if(isMaximizing) //users turn
        {
            int maxEval = -1000000000;
            int maxPos = -1;
            foreach(int pos in positions)
            {
                int[,] newboard = CloneBoard(UpdateBoard(board, pos, 0)); //update the board
                int eval = Minimax(newboard, alpha, beta, false, depth - 1);
                //print("eval with depth " + depth + " is " + eval);
                if(eval > maxEval)
                {
                    maxEval = eval;
                    maxPos = pos;
                }
                alpha = Max(alpha, eval);
                if (beta <= alpha) { 
                    break;
                }
            }
             
            if(depth == DEPTH) //if it is the top of the recursion tree, we want the actual position and not the score of the board
            {
                //Debug.Log("highest layer, return " + maxPos);
                return maxPos;
            }
            //Debug.Log("return at depth " + depth + " is " + maxEval);
            return maxEval;
        } else
        {
            int minEval = 1000000000;
            int minPos = -1;
            foreach(int pos in positions)
            {
                int[,] newboard = CloneBoard(UpdateBoard(board, pos, 1)); //update the board
                int eval = Minimax(newboard, alpha, beta, true, depth - 1);
                //print("eval with depth " + depth + " is " + eval);

                if (eval < minEval)
                {
                    minEval = eval;
                    minPos = pos;
                }
                beta = Min(beta, eval);
                if(beta <= alpha)
                {
                    break; 
                }
            }

            if(depth == DEPTH) //if it is the top of the recursion tree, we want the actual position and not the score of the board
            {
                //Debug.Log("highest layer, return " + minPos);

                return minPos;
            }
            //Debug.Log("return at depth " + depth + " is " + minEval);

            return minEval;
        }

    }

    private List<int> AvailablePositions(int[,] board)
    {
        List<int> positions = new List<int>();

        for(int i = 0; i < BOARD_SIZE; i++)
        {
            if(board[0, i] == 3)
            {
                positions.Add(i);
            }
        }


        return positions;

    }

    private int[,] UpdateBoard(int[,] b, int position, int turn) //updates the board with a tile in a certain position(slot)
    {

        int[,] board = CloneBoard(b);

        int row = BOARD_SIZE - 1;

        while (row >= 0 && board[row, position] != 3) //gets the top spot that isn't filled
        {
            row--;
        }

        if (row >= 0) //is an actual spot
        {
            //for the turns, user is 0 and ai is 1
            if (turn % 2 == 0)
            {
                board[row, position] = 1;
            }
            else
            {
                board[row, position] = 2;
            }
        }

        return board;
    }

    private int[,] CloneBoard(int[,] board) //returns a clone of the board
    {
        int[,] cloned = new int[BOARD_SIZE, BOARD_SIZE];
        for(int i = 0; i < BOARD_SIZE; i++)
        {
            for(int j = 0; j < BOARD_SIZE; j++)
            {
                cloned[i, j] = board[i, j];
            }
        }
        return cloned;
    }

    //returns max and min of two numbers

    private int Max(int a, int b)
    {
        if (a > b) return a;
        return b;
    }

    private int Min(int a, int b)
    {
        if (a < b) return a;
        return b;
    }

    //detect middle, not just edges - make sure win code and score code sees edges

    public void changeDepth()
    {
        DEPTH = (int)depthSlider.value;
        depthText.text = "THE AI CAN SEE " + DEPTH + " MOVES AHEAD";
    }

    
}
