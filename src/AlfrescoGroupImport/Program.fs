open Hopac
open HttpFs.Client
open FSharp.Data
open System.IO

type GroupResultDef = JsonProvider<"../../template/Group.json">
type GroupSourceDef =  CsvProvider<"../../template/Group.csv">
type SettingsDef =    JsonProvider<"../../template/Settings.json">

let replace (n1: string) (n2: string) (source: string) = 
    (source).Replace(n1, n2)

let appendHeader request =
    request
    |> Request.basicAuthentication "admin" "admin"
    |> Request.setHeader (ContentType (ContentType.create ("application", "json") ))
    |> Request.bodyString "{}"
    |> Request.responseAsString

let createSubGroup baseUrl shortName fullName = 
    let url =
        "{baseUrl}/service/api/groups/{shortName}/children/{fullAuthorityName}"
        |> replace "{baseUrl}" baseUrl
        |> replace "{shortName}" shortName
        |> replace "{fullAuthorityName}" fullName

    Request.createUrl Post url 
    |> appendHeader
    |> run
    |> GroupResultDef.Parse

let createRootGroup baseUrl shortName = 
    let url =
        "{baseUrl}/service/api/rootgroups/{shortName}"
        |> replace "{baseUrl}" baseUrl
        |> replace "{shortName}" shortName

    Request.createUrl Post url
    |> appendHeader
    |> run
    |> GroupResultDef.Parse


[<EntryPoint>]
let main argv =
    let csv = File.ReadAllText argv.[0]
    let groups = 
        (GroupSourceDef.Parse csv).Rows
        |> Seq.map (fun x -> x.ShortName) 
        |> Seq.collect (String.split ',')
        |> Set.ofSeq

    let settings = SettingsDef.Parse(File.ReadAllText("Settings.json"))
    let baseUrl = settings.Url
    for group in groups do 
        createRootGroup baseUrl group |> printfn "%A"
    0 // return an integer exit code
