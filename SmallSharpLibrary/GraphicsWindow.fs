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
    static let penColor = Colors.Blue
    static let penWidth = 1.0
    static let mutable mousePosition = 0.0,0.0  
    static let mutable lastKey = Key.None
    static let window = 
        let createWindow () =
            let win = Window(SizeToContent=SizeToContent.WidthAndHeight)
            win.Content <- Canvas(Width=512.0,Height=384.0)
            win.MouseMove |> Event.add (fun e -> 
                mousePosition <- let p = e.GetPosition(win) in p.X, p.Y
            )
            win.KeyDown |> Event.add (fun e -> 
                lastKey <- e.Key
            )
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
    static member internal Draw f = draw f
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
    static member Show() =               
        invoke (fun window -> window.Show())
    static member Hide() =               
        invoke (fun window -> window.Hide())    
    static member DrawLine(x1:float,y1:float,x2:float,y2:float) =
        draw (fun canvas ->             
            let line = Line(X1=x1,Y1=y1,X2=x2,Y2=y2)           
            line.Stroke <- SolidColorBrush penColor
            line.StrokeThickness <- penWidth
            line |> canvas.Children.Add |> ignore
        )
    static member DrawEllipse(x:float,y:float,width:float,height:float) =
        draw (fun canvas ->             
            let ellipse = Ellipse(Width=width,Height=height)           
            ellipse.Stroke <- SolidColorBrush penColor
            ellipse.StrokeThickness <- penWidth
            Canvas.SetLeft(ellipse,x)
            Canvas.SetTop(ellipse,y)
            ellipse |> canvas.Children.Add |> ignore
        )
    static member DrawText(x:float,y:float,text:string) =
        draw (fun canvas ->             
            let block = TextBlock(Text=text)
            Canvas.SetLeft(block,x)
            Canvas.SetTop(block,y)
            block |> canvas.Children.Add |> ignore
        )
    [<CLIEvent>]
    static member MouseDown = window.Value.MouseDown
    [<CLIEvent>]
    static member MouseUp = window.Value.MouseUp
    [<CLIEvent>]
    static member MouseMove = window.Value.MouseMove      
    static member MouseX with get () = fst mousePosition
    static member MouseY with get () = snd mousePosition
    [<CLIEvent>]
    static member KeyDown = window.Value.KeyDown      
    [<CLIEvent>]
    static member KeyUp = window.Value.KeyUp
    static member LastKey with get () = lastKey