open Library

do  TextWindow.WriteLine("Hello World")
    GraphicsWindow.Title <- "Hello"
    GraphicsWindow.Show()
    GraphicsWindow.FillColor <- yellow
    GraphicsWindow.BrushColor <- black
    GraphicsWindow.BrushWidth <- 5.0
    GraphicsWindow.DrawEllipse(50.0,50.0,200.0,200.0)
    GraphicsWindow.DrawEllipse(90.0,90.0,120.0,120.0)
    GraphicsWindow.BrushWidth <- 0.0
    GraphicsWindow.DrawRectangle(90.0,90.0,120.0,60.0)
    GraphicsWindow.BrushWidth <- 10.0
    GraphicsWindow.DrawEllipse(90.0,100.0,10.0,10.0)
    GraphicsWindow.DrawEllipse(200.0,100.0,10.0,10.0)
    