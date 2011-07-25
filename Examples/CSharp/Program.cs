using Library;

class Program
{
    static void Main(string[] args)
    {
        GraphicsWindow.Show();      
        Turtle.PenName("Red");
        (1000).Times(i =>
        {
            Turtle.Forward(6);
            Turtle.Right(i * 7);
        });
    }
}

