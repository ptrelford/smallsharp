open Library

do  GraphicsWindow.Title <- "Turtle Example"
    GraphicsWindow.Show()
    rt 45
    penup ()
    fd 300
    pendown ()
    pencolor red    
    repeat 10 (fun () -> rt 36; repeat 5 (fun () -> fd 54; rt 72))
    
