// Learn more about F# at http://fsharp.net
namespace KillerRabbit.FinanceImportLib
    open System
    open System.Net
    open System.IO
    open Microsoft.FSharp.Collections

    module DataGrabbers =
    // Snagged from: http://channel9.msdn.com/Blogs/pdc2008/TL11
  
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
        type YahooGrabber =
            member x.GetPrices (ticker:string) (startdate:DateTime) (enddate:DateTime) (output:string) (sep:string) =
                let fromMonth   = startdate.Month.ToString()
                let fromDay     = startdate.Day.ToString()
                let fromYear    = startdate.Year.ToString()
                let toMonth     = enddate.Month.ToString()
                let toDay       = enddate.Day.ToString()
                let toYear      = enddate.Year.ToString()
                let url         = "http://chart.yahoo.com/table.csv?" + ticker +
                                    "&a=" + fromMonth + "&b=" + fromDay + "&c=" +
                                    fromYear + "&g=d&q=q&y=0&&z=" + ticker + "&x=" +
                                    output
                
                let req     = WebRequest.Create(url)
                let resp    = req.GetResponse()
                let stream  = resp.GetResponseStream()
                let reader  = new StreamReader(stream)
                let csv     = reader.ReadToEnd()
                let linesplit = [|sep|]
                let lines   = csv.Split([|'\n'|], StringSplitOptions.None)
                let prices  =
                    lines
                    |> Seq.skip 1
                    |> Seq.map( fun line -> line.Split(linesplit, StringSplitOptions.None) )
                    |> Seq.filter (fun values -> values |> Seq.length = 7)
                    |> Seq.map ( fun values -> System.DateTime.Parse(values.[0]), float values.[1] )
                
                prices
    
    (*
        TODO: google possibly mangles dates, reverses time series order
        google - http://finance.google.com/finance/historical?
                q         = symbol
                startdate = MM+DD+YYYY
                enddate   = MM+DD+YYYY
                output    = csv (?)
                sep       = sep (' ')
    *)
        type GoogleGrabber = 
            member x.GetPrices (ticker:string) (startdate:DateTime) (enddate:DateTime) (output:string) (sep:string) =
                (*
                    let ticker      = "GLD"
                    let startdate   = "01+01+2009"
                    let enddate     = "01+01+2011"
                    let output      = "csv"
                    let sep         = " "
                *)
                let url = "http://finance.google.com/finance/historical?q=" + ticker +
                            "&startdate=" + startdate.ToString("yyyy-MM-dd") +
                            "&enddate=" + enddate.ToString("yyyy-MM-d") +
                            "&output=" + output +
                            "&sep=" + sep
    
                let req     = WebRequest.Create(url)
                let resp    = req.GetResponse()
                let stream  = resp.GetResponseStream()
                let reader  = new StreamReader(stream)
                let csv     = reader.ReadToEnd()
                let linesplit = [|sep|]
                let lines   = csv.Split([|'\n'|])

                let prices =
                    lines
                    |> Seq.skip 1               // this is if a header is present
                    |> Seq.map (fun line -> line.Split(linesplit, StringSplitOptions.None) )
                    |> Seq.filter (fun values -> values |> Seq.length = 7)
                    |> Seq.map (fun values -> System.DateTime.Parse(values.[0]), float values.[1])

                prices
