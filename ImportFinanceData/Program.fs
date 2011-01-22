// Learn more about F# at http://fsharp.net

namespace KillerRabbit.ImportFinanceData


    open System
    open System.IO

    open KillerRabbit.FinanceImportLib

    let pricegrabber =  

    let main() =
        let pricegrabber = new GoogleGrabber

