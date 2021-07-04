#r "nuget: FSharp.Data"
#r "nuget: Newtonsoft.Json"

open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open Newtonsoft.Json

let shuffle list = 
    let rand = System.Random()
    list |> Array.sortBy(fun _ -> rand.Next())

let cybervadians =
    [|
      "@johnsmith"
      "@mgolenia"
      "@thom.johnson"
      "@btalker"
      "@rtychowsky"
      "@agrzeszczak"
      "@anoton"
      "@solip"
    |]

let praises =
    [|
      "Thank you!"
      "Good work, as always"
      "Outstanding work!"
      "This is truly above and beyond"
      "Thanks for your good work this month"
      "Excellent work!"
      "Thank you for your hard work and dedication"
      "Thank you so much"
      "Words cannot express my gratitude for your help and effort"
      "Thank you for bringing your best to work every single day."
      "Well done!"
      "Thanking you for your support!"
    |]

let createMessage hashTags amounts balance =
    let hashtag = hashTags |> shuffle |> Array.head
    let points = amounts |> shuffle |> Array.head
    let peopleAmount = balance / points
    let praise = praises 
                 |> shuffle
                 |> Array.head
    let cybervadiansToPraise = cybervadians 
                               |> shuffle
                               |> Array.take peopleAmount
                               |> String.concat(" ")
    $"+{points} {praise} {cybervadiansToPraise} {hashtag}"

type EcoBonusly = JsonProvider<""" {
   "result":{
      "give_amounts":[ 0 ],
      "company_hashtags":[" "]
   }
} """>

type ProfileBonusly = JsonProvider<""" {
   "result":{
      "giving_balance":0
   }
} """>

let apiKey = System.Environment.GetEnvironmentVariable "BONUSLY_KEY"

let (hashTags, giveAmounts) = 
    Http.RequestString($"https://bonus.ly/api/v1/companies/show?access_token={apiKey}") 
    |> EcoBonusly.Parse
    |> (fun response -> (response.Result.CompanyHashtags, response.Result.GiveAmounts))
    
let givingBalance = 
    Http.RequestString($"https://bonus.ly/api/v1/users/me?access_token={apiKey}") 
    |> ProfileBonusly.Parse
    |> (fun response -> response.Result.GivingBalance)

let isDryRun = fsi.CommandLineArgs 
                |> Array.tail 
                |> Array.tryHead
                |> function | Some arg -> arg = "dry" | _ -> false

let bonusMessage = {| reason = createMessage 
                                 hashTags 
                                 giveAmounts 
                                 givingBalance
                   |} |> JsonConvert.SerializeObject 

if isDryRun then
    printfn "DRY RUN \n%s" bonusMessage
elif givingBalance <= 1 then
    printfn "You are poor. No points to redeem."
else 
    Http.RequestString
      ( $"https://bonus.ly/api/v1/bonuses?access_token={apiKey}", 
        headers = [ ContentType HttpContentTypes.Json ],
        body = TextRequest bonusMessage) |> ignore
    printfn "Success"
    