namespace Library 

open System
open System.Threading        
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Shapes
       
[<Sealed>]
type GraphicsWindow private () =
    static let penColor = Colors.Blue    
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
    static let window = 
        let createWindow () =
            let win = Window(SizeToContent=SizeToContent.WidthAndHeight)
            win.Content <- Canvas(Width=512.0,Height=384.0)
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
    static member Show() =               
        invoke (fun window -> window.Show())
    static member Hide() =               
        invoke (fun window -> window.Hide())    
    static member DrawEllipse(x:float,y:float,width:float,height:float) =
        draw (fun canvas ->             
            let e = Ellipse(Width=width,Height=height)           
            e.Fill <- SolidColorBrush penColor
            Canvas.SetLeft(e,x)
            Canvas.SetTop(e,y)
            e |> canvas.Children.Add |> ignore
        )