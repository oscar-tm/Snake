namespace Snake;

public class SnakeGame{
    private int[] headLocation, appleLocation;
    private int[,] board, snakeCoords, portalCoords;
    private int score, boardSize, numberOfMoves;
    private bool gameOver;
    private int numberOfPortals;

    //Creates a board with size "size" and randomly places items to start the game
    public void CreateBoard(){
        var randGen = new Random(); //mem leak?

        for (int i = 0; i < 2; ++i){
            headLocation[i] = randGen.Next(boardSize);
            do{
                appleLocation[i] = randGen.Next(boardSize);
            } while(headLocation[i] == appleLocation[i]);
        }

        board[headLocation[0], headLocation[1]] = 1;
        board[appleLocation[0], appleLocation[1]] = 3;

        bool OverlapCheck(int j){
            if (portalCoords[j, 0] == portalCoords[j, 2] && portalCoords[j, 0] == portalCoords[j, 2] && portalCoords[j, 0] == portalCoords[j, 3])
                return true;
            for (int i = 0; i < numberOfPortals; ++i){
                if (i == j) {}
                else if (portalCoords[j, 0] == portalCoords[i, 0] && portalCoords[j, 1] == portalCoords[i, 1] || portalCoords[j, 2] == portalCoords[i, 2] && portalCoords[j, 3] == portalCoords[i, 3])
                    return true;
            }
            return false;
        }

        for(int i = 0; i < numberOfPortals; ++i){
            do{
                if (randGen.Next(2) == 0){
                    portalCoords[i, 0] = randGen.Next(boardSize);
                    portalCoords[i, 1] = randGen.Next(2) > 0? boardSize : 0;
                    portalCoords[i, 2] = randGen.Next(2) > 0? boardSize : 0;
                    portalCoords[i, 3] = randGen.Next(boardSize);
                }
                else{
                    portalCoords[i, 0] = randGen.Next(2) > 0? boardSize : 0;
                    portalCoords[i, 1] = randGen.Next(boardSize);
                    portalCoords[i, 2] = randGen.Next(boardSize);
                    portalCoords[i, 3] = randGen.Next(2) > 0? boardSize : 0;
                }
            } while(OverlapCheck(i));
        }
    }

    //Updates the board to reflect current state
    private void UpdateBoard(){
        Array.Clear(board);
        board[headLocation[0], headLocation[1]] = 1;
        board[appleLocation[0], appleLocation[1]] = 3;

        for (int i = 0; i < score; ++i){
            board[snakeCoords[i, 0], snakeCoords[i, 1]] = 2;
        }
    }

    //Displays the game, currently only in Console
    public void DisplayGame(){
        string PortalLetter(int x, int y){
            string[] letters = {"R", "G", "B"};
            if(x == -1)
                x = 0;
            if(y == -1)
                y = 0;
            for (int i = 0; i < numberOfPortals; ++i){
                if (portalCoords[i, 0] == x && portalCoords[i, 1] == y || portalCoords[i, 2] == x && portalCoords[i, 3] == y)
                    return letters[i];
            }
            return "#";
        }

        string GameLetters(int gameNumber){
            string[] letters = {"0", "H", "T", "A", "#"};
            return letters[gameNumber];
        }

        Console.Clear();
        Console.WriteLine("Press 'ESC' to exit before game over");
        Console.WriteLine($"Score: {score}");
        Console.WriteLine("");
        for(int j = -1; j < boardSize + 1; ++j){
            Console.Write("\t");
            for (int i = -1; i < boardSize + 1; ++i){
                if (j == -1 || i == -1 || j == boardSize || i == boardSize){
                    if(PortalCheck(i, j)){
                        Console.Write($"{PortalLetter(i, j)} ");
                    }
                    else{
                        Console.Write("# ");
                    }
                }
                else{
                    Console.Write($"{GameLetters(board[i, j])} ");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    //Checks if a portal exists at the current location
    private bool PortalCheck(int x, int y){
        if (x == -1 && y == -1 || x == -1 && y == boardSize || x == boardSize && y == -1)
            return false;
        if(x == -1)
            x = 0;
        if(y == -1)
            y = 0;
        for (int i = 0; i < numberOfPortals; ++i){
            if (portalCoords[i, 0] == x && portalCoords[i, 1] == y || portalCoords[i, 2] == x && portalCoords[i, 3] == y)
                return true;
        }
        return false;
    }

    //Checks if the game is over and returns true if game is still on
    public bool GameOver(){
        return gameOver;
    }

    public void GetMove(ConsoleKeyInfo cki){
        MakeMove(KeyToMove(cki.Key.ToString()));
    }

    //Converts the string of a key to correct move
    private int KeyToMove(string key){
        switch(key){
            case "UpArrow":
                return 0;
            case "RightArrow":
                return 1;
            case "DownArrow":
                return 2;
            case "LeftArrow":
                return 3;
            default: //For other keys except 'ESC' do nothing
                return 4;
        }
    }

    //Moves the snake up (0), right (1), down(2) or left (3)
    private void MakeMove(int move){
        int coord = -1, coordValue = 0;
        switch(move){
        case 0:
            coord = 1;
            coordValue = -1;
            break;
        case 1:
            coord = 0;
            coordValue = 1;
            break;
        case 2:
            coord = 1;
            coordValue = 1;
            break;
        case 3:
            coord = 0;
            coordValue = -1;
            break;
        default:
            //If wrong key is pressed dont do anything
            coord = 0;
            coordValue = 0;
            break;
        }
        headLocation[coord] += coordValue;
        ++numberOfMoves;
        if (headLocation[coord] < 0 || headLocation[coord] > boardSize - 1){ //Going outside of the board results in gameover
            if (PortalCheck(headLocation[0],headLocation[1])){
                PortalMove(coord, coordValue);
                UpdateBoard();
            }
            else{
                gameOver = true;
            }
        }
        else if (coordValue == 0){--numberOfMoves;} //pass
        else if (CollisonCheck()){
            gameOver = true;
        }
        else if (OnApple()){
            LengthenSnake(coord, coordValue);
            NewAppleLocation();
            score += 1;
            UpdateBoard();
        }
        else{
            MoveSnake(coord, coordValue);
            UpdateBoard();
        }
    }

    //Moves the snake through a portal
    private void PortalMove(int coord, int coordValue){
        //Remove the tail from the list
        if (score > 0){ //Only need to move snake if it has a body
            for (int i = 0; i < score - 1; ++i){
                snakeCoords[i, 0] = snakeCoords[i + 1, 0];
                snakeCoords[i, 1] = snakeCoords[i + 1, 1];
            }
            score -= 1;
            LengthenSnake(coord, coordValue);
            score += 1;
        }

        int portal = 100;
        int index = 100; //Throw error if we are here without reason

        int x = headLocation[0] < 0? 0 : headLocation[0];
        int y = headLocation[1] < 0? 0 : headLocation[1];

        //Update head location to otherside of portal
        for (int i = 0; i < numberOfPortals; ++i){
            if (portalCoords[i, 0] == x && portalCoords[i, 1] == y){
                portal = 2;
                index = i;
            }
            else if (portalCoords[i, 2] == x && portalCoords[i, 3] == y){
                portal = 0;
                index = i;
            }
        }
        if (portalCoords[index, portal] == 0){
            headLocation[0] = 0;
            headLocation[1] = portalCoords[index, portal + 1];
        }
        else if (portalCoords[index, portal] == boardSize){
            headLocation[0] = boardSize - 1;
            headLocation[1] = portalCoords[index, portal + 1];
        }
        else if (portalCoords[index, portal + 1] == 0){
            headLocation[0] = portalCoords[index, portal];
            headLocation[1] = 0;
        }
        else{
            headLocation[0] = portalCoords[index, portal];
            headLocation[1] = boardSize - 1;
        }
    }

    //Checks if the snake will collide with itself after moving
    private bool CollisonCheck(){
        if (score == 0) //cannot collide with itself it only is a head
            return false;

        for (int i = 0; i < score; ++i){
            if (headLocation[0] == snakeCoords[i, 0] && headLocation[1] == snakeCoords[i, 1]){
                return true;
            }
        }
        return false;
    }

    //Checks if the head is on a apple
    private bool OnApple(){
        if (headLocation[0] == appleLocation[0] && headLocation[1] == appleLocation[1])
            return true;
        else
            return false;
    }

    //Adds the old head to the snake
    private void LengthenSnake(int coord, int coordValue){
        snakeCoords[score, 0] = headLocation[0];
        snakeCoords[score, 1] = headLocation[1];
        snakeCoords[score, coord] -= coordValue;
    }

    //Randomly generates a new apple
    private void NewAppleLocation(){
        bool OverlapCheck(){
            if (appleLocation[0] == headLocation[0] && appleLocation[1] == headLocation[1])
                return true;
            for (int i = 0; i < score; ++i){
                if (appleLocation[0] == snakeCoords[i, 0] && appleLocation[1] == snakeCoords[i, 1])
                    return true;
            }
            return false;
        }
        var randGen = new Random(); //mem leak?
        do{
            appleLocation[0] = randGen.Next(boardSize);
            appleLocation[1] = randGen.Next(boardSize);
        } while (OverlapCheck());
    }

    //Moves the snake on the board
    private void MoveSnake(int coord, int coordValue){
        //Remove the tail from the list
        if (score > 0){ //Only need to move snake if it has a body
            for (int i = 0; i < score - 1; ++i){
                snakeCoords[i, 0] = snakeCoords[i + 1, 0];
                snakeCoords[i, 1] = snakeCoords[i + 1, 1];
            }
            score -= 1;
            LengthenSnake(coord, coordValue);
            score += 1;
        }
    }

    //Used to display some stats after gameovering
    public void DisplayEndStats(){
        Console.WriteLine("Game over!");
        Console.WriteLine($"You got a final score of {score}.");
        Console.WriteLine($"You did a total of {numberOfMoves} moves.");
    }

    public SnakeGame(int size, int portals){
        score = 0;
        numberOfMoves = 0;
        boardSize = size;
        numberOfPortals = portals > 3? 3 : portals; //max 3 portals
        headLocation = new int[2];
        appleLocation = new int[2];
        board = new int[boardSize, boardSize];
        snakeCoords = new int[boardSize*boardSize, 2]; //max length of snake is complete board
        portalCoords = new int[numberOfPortals, 4];
        gameOver = false;
    }
}