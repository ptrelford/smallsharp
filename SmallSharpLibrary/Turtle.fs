namespace Library

open System.Windows.Controls
open System.Windows.Media
open System.Windows.Shapes

[<AutoOpen>]
module Math =
    let PI = System.Math.PI

[<Sealed>]
type Turtle private () =
    static let mutable isPenDown = true
    static let mutable penColor = Colors.Black
    static let mutable x = 0.0
    static let mutable y = 0.0
    static let mutable a = 0.0
    static member Left(degrees:float) =
        a <- a - degrees
    static member Left(degrees:int) = 
        Turtle.Left(float degrees)
    static member Right(degrees:float) =
        a <- a + degrees
    static member Right(degrees:int) =
        Turtle.Right(float degrees)
    static member Forward(distance:float) =
        let r = a * PI / 180.0
        let x' = x + distance * cos r
        let y' = y + distance * sin r        
        let addLine (canvas:Canvas) =
            let line = Line(X1=x, Y1=y, X2=x', Y2=y')
            line.Stroke <- SolidColorBrush penColor
            line.StrokeThickness <- 1.0
            canvas.Children.Add line |> ignore
        if isPenDown then GraphicsWindow.Draw addLine
        x <- x'
        y <- y'
    static member Forward(distance:int) =
        Turtle.Forward(float distance)
    static member PenUp() =
        isPenDown <- false    
    static member PenDown() =
        isPenDown <- true
    static member PenColor color =
        penColor <- color

[<AutoOpen>]
module turtle =
    type private distance = int
    type private angle = int
    let forward (distance:distance) = Turtle.Forward distance
    let fd (distance:distance) = Turtle.Forward distance
    let left (degrees:angle) = Turtle.Left degrees
    let lt (degrees:angle) = Turtle.Left degrees
    let right (degrees:angle) = Turtle.Right degrees
    let rt (degrees:angle) = Turtle.Right degrees
    let pencolor color = Turtle.PenColor color
    let penup () = Turtle.PenUp()
    let pendown () = Turtle.PenDown()
    let repeat n f = for i = 1 to n do f ()