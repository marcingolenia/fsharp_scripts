#r "nuget: Microsoft.Playwright, 1.13-next-1"

open Microsoft.Playwright

type Run = { 
    Browser: IBrowser
    Page: IPage
}


type PlaywrightBuilder() =
    /// Required - creates default "starting" values
    member _.Yield _ =
        let web = Playwright.CreateAsync() 
                    |> Async.AwaitTask 
                    |> Async.RunSynchronously
        let browser = web.Firefox.LaunchAsync(BrowserTypeLaunchOptions(Headless = true)) 
                        |> Async.AwaitTask 
                        |> Async.RunSynchronously
        { 
            Browser = browser
            Page = browser.NewPageAsync() 
                    |> Async.AwaitTask 
                    |> Async.RunSynchronously 
        }

    [<CustomOperation "visit">]
    member _.Visit (run: Run, url) = 
        run.Page.GotoAsync(url) 
            |> Async.AwaitTask 
            |> Async.RunSynchronously 
            |> ignore
        run

    [<CustomOperation "screenshot">]
    member _.Screenshot (run: Run, name) = 
        run.Page.ScreenshotAsync(PageScreenshotOptions(Path = $"{name}.png"))
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        run

    [<CustomOperation "write">]
    member _.Write (run: Run, selector, value) = 
        run.Page.FillAsync(selector, value)
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        run

    [<CustomOperation "click">]
    member _.Click (run: Run, selector) = 
        run.Page.ClickAsync(selector)
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        run

    [<CustomOperation "wait">]
    member _.Wait (run: Run, seconds) = 
        run.Page.WaitForTimeoutAsync(seconds)
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        run

    [<CustomOperation "waitFor">]
    member _.WaitFor (run: Run, selector) = 
        run.Page.WaitForSelectorAsync(selector)
            |> Async.AwaitTask
            |> Async.RunSynchronously
            |> ignore
        run

let playwright = PlaywrightBuilder()

playwright {
    visit "https://duckduckgo.com/"
    write "input" "mcode.it"
    click "input[type='submit']"
    click "text=mcode.it - Marcin Golenia Blog"
    waitFor "text=Yet another IT blog?"
    screenshot "mcode-screen"
}
