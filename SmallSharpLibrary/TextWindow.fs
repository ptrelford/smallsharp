namespace Library

open System

[<Sealed()>]
type TextWindow private () =
    static member Write(text:int) = Console.Write text
    static member Write(text:string) = Console.Write text
    static member WriteLine(text:int) = Console.WriteLine text
    static member WriteLine(text:string) = Console.WriteLine text
    static member Read() = Console.Read()
    static member ReadLine() = Console.ReadLine()
    