using snake;

int size = 13, portals = 5;

var testSnake = new snakeGame(size, portals);
testSnake.CreateBoard();
Thread.Sleep(500);
ConsoleKeyInfo cki;

do{
    testSnake.DisplayGame();
    cki = Console.ReadKey(true);
    testSnake.GetMove(cki);
} while (cki.Key != ConsoleKey.Escape && !testSnake.GameOver());

testSnake.DisplayEndStats();