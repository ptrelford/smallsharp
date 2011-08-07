﻿namespace Library

open System.Collections.Generic
open System.Windows
open System.Windows.Controls

[<Sealed>]
type Form private () =    
    static let mutable validate : obj seq -> bool = fun _ -> true
    static let mutable submit : obj seq -> unit = ignore
    static let mutable answers = List<_>()
    static let createForm () = 
        GraphicsWindow.InvokeWithReturn(fun window ->
            GraphicsWindow.Show()
            let table = Grid(Margin=Thickness(8.0))                       
            table.ColumnDefinitions.Add(ColumnDefinition(Width=GridLength(2.0,GridUnitType.Star)))
            table.ColumnDefinitions.Add(ColumnDefinition(Width=GridLength(1.0,GridUnitType.Star)))            
            window.Content <- table
            let button = Button(Content="Submit")
            button.Click.Add(fun _ -> answers |> Seq.map (fun f -> f()) |> submit) 
            table, button
        )   
    static let form = lazy (createForm())
    static let mutable rowIndex = 0
    static let margin = Thickness(4.0)
    static let addRow(rowFun:Grid -> unit) =
        GraphicsWindow.Invoke(fun _ ->
            let table,submit = form.Force()            
            table.RowDefinitions.Add(RowDefinition())
            if rowIndex = 0 then
                table.RowDefinitions.Add(RowDefinition())
                Grid.SetColumnSpan(submit, 2)
                table.Children.Add(submit) |> ignore
            Grid.SetRow(submit,rowIndex+1)
            rowFun table
            rowIndex <- rowIndex + 1            
        )
    static let addQuestion(label, create:unit -> #FrameworkElement * (unit -> obj)) =
        let controls (table:Grid)  =
            let question = TextBlock(Text=label, Margin=margin)
            Grid.SetRow(question, rowIndex)      
            table.Children.Add question |> ignore       
            let control, read = create()
            control.Margin <- margin
            Grid.SetColumn(control, 1)
            Grid.SetRow(control, rowIndex)
            table.Children.Add control |> ignore
            answers.Add read
        addRow controls               
    static member Title
        with get () = GraphicsWindow.Title
        and set(value) = GraphicsWindow.Title <- value
    static member Clear() =
        rowIndex <- 0        
        answers.Clear()
        GraphicsWindow.Invoke(fun _ ->
            let table, _ = form.Force()
            table.RowDefinitions.Clear()
            table.Children.Clear()
        )
    static member Group(group) =
        ()
    static member Information(label) =
        let create (table:Grid) = 
            let control = TextBlock(Text=label, Margin=margin)
            Grid.SetRow(control, rowIndex)
            Grid.SetColumnSpan(control, 2)
            table.Children.Add control |> ignore 
        addRow create
    static member Question(label, isValidCallback:string -> bool) =       
        let create () =
            let control = TextBox()
            control, fun () -> box control.Text        
        addQuestion(label,create)
    static member Question(label) =
        Form.Question(label, (fun _ -> true))
    static member NumericalQuestion(label, isValidCallback:int -> bool) =        
        Form.Question(label)
    static member Options(label, options:string seq, selectedItem) =
        let create () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedItem <- selectedItem
            answer, fun () -> answer.SelectedItem
        addQuestion(label,create)
    static member Options(label, options:string seq) =
        Form.Options(label, options, 0)
    static member Options(label, options:int seq, selectedItem) =
        let create () =
            let answer = ComboBox()
            options |> Seq.iter (answer.Items.Add >> ignore)
            answer.SelectedItem <- selectedItem
            answer, fun () -> answer.SelectedItem
        addQuestion(label,create)
    static member Options(label, options:int seq) =
        Form.Options(label, options, options.First())
    static member Choice(label) =        
        let create () =
            let answer = CheckBox()
            answer, fun () -> box answer.IsChecked
        addQuestion(label,create)
    static member OnValidate (validateCallback:obj seq -> bool) =
        validate <- validateCallback
    static member OnSubmit (submitCallback:obj seq -> unit) =
        submit <- submitCallback