namespace Library

open System.Windows
open System.Windows.Controls

[<Sealed>]
type Form private () =
    static let createTable () = 
        GraphicsWindow.InvokeWithReturn(fun window ->
            let grid = Grid()                       
            grid.ColumnDefinitions.Add(ColumnDefinition(Width=GridLength(2.0,GridUnitType.Star)))
            grid.ColumnDefinitions.Add(ColumnDefinition(Width=GridLength(1.0,GridUnitType.Star)))
            window.Content <- grid
            grid
        )
    static let table = lazy (createTable())
    static let mutable rowIndex = 0
    static let addQuestion(label, answer) =
        GraphicsWindow.Invoke(fun _ ->
            let table = table.Force()
            table.RowDefinitions.Add(RowDefinition())
            let question = TextBlock(Text=label)
            Grid.SetRow(question, rowIndex)      
            table.Children.Add question |> ignore       
            let answer = answer()
            Grid.SetColumn(answer, 1)
            Grid.SetRow(answer, rowIndex)
            table.Children.Add answer |> ignore
            rowIndex <- rowIndex + 1
            GraphicsWindow.Show()
        )
    static let mutable title = "Form"
    static member Title
        with get () = title
        and set(value) = title <- value
    static member Clear() =
        title <- ""
    static member Group(group) =
        ()
    static member Information(label, value) =
        raise (new System.NotImplementedException())
    static member Question(label, isValidCallback:string -> bool) =       
        addQuestion(label,fun _ -> TextBox())
    static member Question(label) =
        Form.Question(label, (fun _ -> true))
    static member NumericalQuestion(label, isValidCallback:int -> bool) =        
        addQuestion(label,fun _ -> TextBox())
    static member Options(label, options:string seq) =
        let createOptions () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedIndex <- 0
            answer
        addQuestion(label,createOptions)
    static member Options(label, options:int seq) =
        let createOptions () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedIndex <- 0
            answer
        addQuestion(label,createOptions)
    static member Choice(label) =        
        addQuestion(label,fun _ -> CheckBox())
    static member OnValidate (validateCallback:obj seq -> bool) =
        ()
    static member OnSubmit (submitCallback:obj seq -> unit) =
        ()