// Checks SVN commit and rejects if any file has tabs

open System
open System.IO
open System.Diagnostics
open System.Text.RegularExpressions
open FSharpx.Strings
open FSharpx.Regex

type SvnChange = 
    | Update
    | NoChange

type Change = { SvnCode : SvnChange; FileName : string} 

let (|Match|_|) pattern input =
    let m = Regex.Match(input, pattern) in
    if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) else None

let launchProcess args = 

    let processStartInfo = new ProcessStartInfo( 
                                                    FileName = "svnlook.exe", 
                                                    UseShellExecute = false, 
                                                    CreateNoWindow = true, 
                                                    RedirectStandardOutput = true, 
                                                    RedirectStandardError = true, 
                                                    Arguments = args )

    let proc = Process.Start(processStartInfo)
    let output = proc.StandardOutput.ReadToEnd()
    
    proc.WaitForExit()
    
    output

let tab = (=) '\t'

let leadingWhiteSpaceHas predicate (str:string) = Seq.takeWhile Char.IsWhiteSpace str |> Seq.exists predicate

let getFileContents transaction repoPath filePath = launchProcess <| sprintf "cat -t %s %s %s" transaction repoPath filePath

let createChangeRecord line:Change = 
     match line with
        | Match "^[AU].\s\s(.+)$" result -> { SvnCode = Update; FileName = result.Head }
        | _ ->  { SvnCode = NoChange; FileName = "" }

let printFileName fileName = eprintfn "%s" fileName

[<EntryPoint>]
let main(args) = 

    let repoPath = args.[0]
    let transaction = args.[1]

    let results = launchProcess <| sprintf "changed -t %s %s" transaction repoPath
    let filePaths = toLines results
    
    let hasChanged (t:Change) = t.SvnCode <> NoChange
    let isValidFile (t:Change) = [".cs";".as"] |> List.exists (fun ext -> t.FileName.EndsWith(ext))
    let changeHasTabs change = getFileContents transaction repoPath change.FileName |> leadingWhiteSpaceHas tab        

    let found = filePaths 
                        |> Seq.map createChangeRecord 
                        |> Seq.filter (fun change -> hasChanged change && isValidFile change) 
                        |> Seq.choose (fun change -> if changeHasTabs change then Some(change.FileName) else None)

    

    if not <| Seq.isEmpty found then
        eprintfn "%s" "Change set has files with tabs:\r\n"

        found |> Seq.iter printFileName

        1
    else
        0

