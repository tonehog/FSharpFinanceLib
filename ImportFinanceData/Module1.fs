// Learn more about F# at http://fsharp.net

module Module1

// This file is a script that can be executed with the F# Interactive.  
// It can be used to explore and test the library project.
// Note that script files will not be part of the project build.

// Snagged from: http://channel9.msdn.com/Blogs/pdc2008/TL11

//#load "Module1.fs"
//open Module1

open System.Net
open System.IO

(*
    yahoo - http://chart.yahoo.com/table.csv?
            s = symbol
            a = from MM
            b = from DD
            c = from YYYY
            d = to MM
            e = to DD
            f = to YYYY
            g = d (?)
            q = q (?)
            y = 0 (?)
            z = symbol
            x = csv (formats?)
*)

(*
    TODO: google possibly mangles dates, reverses time series order
    google - http://finance.google.com/finance/historical?
            q         = symbol
            startdate = MM+DD+YYYY
            enddate   = MM+DD+YYYY
            output    = csv (?)
            sep       = sep (' ')

*)

let ticker      = "GLD"
let startdate   = "01+01+2009"
let enddate     = "01+01+2011"
let output      = "csv"
let sep         = " "
let url         = "http://finance.google.com/finance/historical?q=" + ticker +
                    "&startdate=" + startdate +
                    "&enddate=" + enddate +
                    "&output=" + output +
                    "&sep=" + sep

let req = WebRequest.Create(url)
let resp = req.GetResponse()
let stream = resp.GetResponseStream()
let reader = new StreamReader(stream)
let csv = reader.ReadToEnd()

let prices =
    csv.Split([|'\n'|])
    |> Seq.skip 1
    |> Seq.map (fun line -> line.Split([|','|]))
    |> Seq.filter (fun values -> values |> Seq.length = 7)
    |> Seq.map (fun values ->
        System.DateTime.Parse(values.[0]),
            float values.[1])
