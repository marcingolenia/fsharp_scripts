// LoggerDecorator - logs
type Logger() = 
    let log p = printfn "expression is %A" p
    member _.Bind(x, f) =
        log x
        f x
    member _.Return(x) = x
    

let logger = Logger()

logger {
    let! x = "Marcin"
    let! y = {| Well = 5|}
    return "So that was it!"
} |> printfn "Returned: %A"




