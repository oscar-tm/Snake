using Snake;

int size = 15, portals = 5;

var testSnake = new SnakeGame(size, portals);
testSnake.ResetGame();
Thread.Sleep(500);
ConsoleKeyInfo cki;

do
{
    testSnake.DisplayGame();
    cki = Console.ReadKey(true);
    testSnake.GetMove(cki);
} while (cki.Key != ConsoleKey.Escape && !testSnake.GameOver());

testSnake.DisplayEndStats();