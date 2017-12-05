open Hopac
open HttpFs.Client
open FSharp.Data
open System.IO

type GroupResult = JsonProvider<"../../template/Group.json">
type GroupSource =  CsvProvider<"../../template/Group.csv">
type Settings = JsonProvider<"../../template/Settings.json">

let replace (n1: string) (n2: string) (source: string) = 
    (source).Replace(n1, n2)

let appendHeader  user password  request=
    request
    |> Request.basicAuthentication user password 
    |> Request.setHeader (ContentType (ContentType.create ("application", "json") ))
    |> Request.bodyString "{}"
    |> Request.responseAsString

let createSubGroup (settings: Settings.Root) shortName fullName = 
    let url =
        "{baseUrl}/service/api/groups/{shortName}/children/{fullAuthorityName}"
        |> replace "{baseUrl}" settings.Url
        |> replace "{shortName}" shortName
        |> replace "{fullAuthorityName}" fullName

    Request.createUrl Post url 
    |> appendHeader settings.User settings.Password
    |> run
    |> GroupResult.Parse

let createRootGroup (settings: Settings.Root) shortName = 
    let url =
        "{baseUrl}/service/api/rootgroups/{shortName}"
        |> replace "{baseUrl}" settings.Url 
        |> replace "{shortName}" shortName

    Request.createUrl Post url
    |> appendHeader settings.User settings.Password
    |> run
    |> GroupResult.Parse

[<EntryPoint>]
let main argv =
    let csv = File.ReadAllText argv.[0]
    let groups = 
        (GroupSource.Parse csv).Rows
        |> Seq.map (fun x -> x.ShortName) 
        |> Seq.collect (String.split ',')
        |> Set.ofSeq

    let settings = 
            File.ReadAllText("Settings.json")
            |> Settings.Parse
    for group in groups do 
        createRootGroup settings group |> printfn "%A"
    0 // return an integer exit code
