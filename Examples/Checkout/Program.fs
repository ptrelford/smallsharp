﻿open Library

open System.Globalization

let countries =
    CultureInfo.GetCultures CultureTypes.SpecificCultures 
    |> Seq.map (fun culture -> RegionInfo(culture.LCID).EnglishName)
    |> Seq.sort

let isRequired (answer:string) = answer.Length > 0

do  Form.Title <- "Checkout"

    Form.Group("Delivery Address")
    Form.Question("Full Name", isRequired)   
    Form.Question("Address Line 1", isRequired)
    Form.Question("Address Line 2")
    Form.Question("Town/City", isRequired)
    Form.Question("County")
    Form.Question("Postcode", isRequired)
    Form.Options("Country", countries)
    Form.Question("Phone")

    Form.Group("Card")    
    Form.NumericalQuestion("Card number", fun n -> true)
    Form.Question("Name on card", isRequired)
    
    Form.Group("Expiration date")
    Form.Options("Month", [1..12])
    let year = Clock.Year
    Form.Options("Year", [year..year+20])
    
    Form.Group("Terms & Conditions")
    Form.Choice("I agree to the terms and conditions")
    
    Form.OnValidate(fun _ -> false)
    Form.OnSubmit(fun _ -> ())
