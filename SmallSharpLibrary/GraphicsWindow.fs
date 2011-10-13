namespace Library 

open System
open System.Threading        
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Shapes

type Key = System.Windows.Input.Key
type Colors = System.Windows.Media.Colors

[<Sealed>]
type GraphicsWindow private () =
    static let application, dispatcher =  
        let app = lazy(new Application())
        let autoEvent = new AutoResetEvent(false);
        let thread =       
            new Thread(fun () -> 
                let value = app.Force()
                autoEvent.Set() |> ignore
                value.Run() |> ignore
            )        
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        autoEvent.WaitOne() |> ignore
        app.Value, app.Value.Dispatcher
    static let mutable brushColor = Colors.Blue
    static let mutable brushWidth = 1.0
    static let mutable fillColor = Colors.Transparent
    static let mutable mousePosition = 0.0,0.0
    static let mutable mouseMove = Action(ignore)
    static let mutable mouseDown = Action(ignore)
    static let mutable mouseUp = Action(ignore)
    static let mutable lastKey = Key.None
    static let mutable keyDown = Action<Key>(ignore)
    static let mutable keyUp = Action<Key>(ignore)
    static let window = 
        let createWindow () =
            let win = Window(SizeToContent=SizeToContent.WidthAndHeight)
            win.Content <- Canvas(Width=512.0,Height=384.0)
            win.MouseMove |> Event.add (fun e -> 
                mousePosition <- let p = e.GetPosition(win) in p.X, p.Y
                mouseMove.Invoke()
            )
            win.MouseDown |> Event.add (fun _ -> mouseDown.Invoke())
            win.MouseUp |> Event.add (fun _ -> mouseUp.Invoke())
            win.KeyDown |> Event.add (fun e -> 
                lastKey <- e.Key
                keyDown.Invoke(e.Key)
            )
            win.KeyUp |> Event.add (fun e -> keyDown.Invoke(e.Key))
            win
        lazy(createWindow())
    static let invoke f = 
        dispatcher.Invoke(Action(fun () -> f window.Value)) |> ignore    
    static let invokeWithReturn (f:Window -> 'a) = 
        dispatcher.Invoke(System.Func<_>(fun () -> f window.Value)) :?> 'a
    static let draw f = 
        dispatcher.Invoke(Action(fun () -> 
            let canvas = window.Value.Content :?> Canvas
            f canvas)
        ) |> ignore
    static member internal Invoke f = invoke f
    static member internal InvokeWithReturn f = invokeWithReturn f
    static member internal Draw f = draw f
    static member Topmost
        with get() = 
            invokeWithReturn (fun window -> window.Topmost)
        and set(value) = 
            invoke (fun window -> window.Topmost <- value)
    static member Title
        with get() = 
            invokeWithReturn (fun window -> window.Title)
        and set(title) = 
            invoke (fun window -> window.Title <- title)
    static member Width
        with get() = 
            invokeWithReturn (fun window -> window.Width)
        and set(width) = 
            invoke (fun window -> window.Width <- width)
    static member Height
        with get() = 
            invokeWithReturn (fun window -> window.Height)
        and set(height) = 
            invoke (fun window -> window.Height <- height)
    static member BrushColor
        with get() = brushColor
        and set(color) = brushColor <- color
    static member BrushWidth
        with get() = brushWidth
        and set(width) = brushWidth <- width
    static member FillColor
        with get() = fillColor
        and set(color) = fillColor <- color
    static member Show() =
        invoke (fun window -> window.Show())
    static member Hide() =
        invoke (fun window -> window.Hide())
    static member DrawLine(x1:float,y1:float,x2:float,y2:float) =
        draw (fun canvas ->
            let line = Line(X1=x1,Y1=y1,X2=x2,Y2=y2)
            line.Fill <- SolidColorBrush fillColor
            line.Stroke <- SolidColorBrush brushColor
            line.StrokeThickness <- brushWidth
            line |> canvas.Children.Add |> ignore
        )
    static member DrawLine(x1:int,y1:int,x2:int,y2:int) =
        GraphicsWindow.DrawLine(float x1, float y1, float x2, float y2)
    static member DrawEllipse(x:float,y:float,width:float,height:float) =
        draw (fun canvas ->
            let ellipse = Ellipse(Width=width,Height=height)
            ellipse.Fill <- SolidColorBrush fillColor
            ellipse.Stroke <- SolidColorBrush brushColor
            ellipse.StrokeThickness <- brushWidth
            Canvas.SetLeft(ellipse,x)
            Canvas.SetTop(ellipse,y)
            ellipse |> canvas.Children.Add |> ignore
        )
    static member DrawEllipse(x:int,y:int,width:int,height:int) =
        GraphicsWindow.DrawEllipse(float x, float y, float width, float height)
    static member DrawRectangle(x:float,y:float,width:float,height:float) =
        draw (fun canvas ->
            let rectangle = Rectangle(Width=width,Height=height)
            rectangle.Fill <- SolidColorBrush fillColor
            rectangle.Stroke <- SolidColorBrush brushColor
            rectangle.StrokeThickness <- brushWidth
            Canvas.SetLeft(rectangle,x)
            Canvas.SetTop(rectangle,y)
            rectangle |> canvas.Children.Add |> ignore
        )
    static member DrawRectangle(x:int,y:int,width:int,height:int) =
        GraphicsWindow.DrawRectangle(float x, float y, float width, float height)
    static member DrawText(x:float,y:float,text:string) =
        draw (fun canvas ->
            let block = TextBlock(Text=text)
            Canvas.SetLeft(block,x)
            Canvas.SetTop(block,y)
            block |> canvas.Children.Add |> ignore
        )
    static member MouseDown (action:Action) = mouseDown <- action      
    static member MouseUp (action:Action) = mouseUp <- action     
    static member MouseMove (action:Action) = mouseMove <- action
    static member MouseX with get () = fst mousePosition
    static member MouseY with get () = snd mousePosition
    static member KeyDown (action:Action<Key>) = keyDown <- action
    static member KeyUp (action:Action<Key>) = keyUp <- action
    static member LastKey with get () = lastKey