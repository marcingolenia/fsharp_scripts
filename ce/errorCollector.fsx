// ErrorCollector - accumulates errors
type ErrorCollector() =
    member _.Return(good) = good, List.empty
    member _.Bind((good, bad), f) =
        let newGood, newBad = f good
        (newGood, newBad @ bad)

let errorCollector = ErrorCollector()

// usage
let lessThan boundary numbers = 
    let res = numbers |> List.filter(fun no -> no < boundary)
    let err = numbers |> List.filter(fun no -> no >= boundary)
    res, err

let result, errors = 
    errorCollector {
        let! my0_4 = [0..10] |> lessThan 5 
        let! my0_2 = my0_4 |> lessThan 3 
        return my0_2
    }

printfn "Ok %A" result
printfn "Errors %A" errors