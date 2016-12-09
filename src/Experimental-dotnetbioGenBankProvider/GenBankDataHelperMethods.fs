namespace Bio.FSharp.GenBankTypeProvider

open System
open System.IO

open Bio
open Bio.Core
open Bio.IO
open Bio.IO.GenBank

module GenBankDataHelperMethods = 

    /// Method to get the genome sequence contained in a file specified by the fileName parameter
    let getGenomeSeq fileName = 
        let stream = File.OpenRead fileName 
        let parser = Bio.IO.SequenceParsers.FindParserByFileName fileName
        parser.ParseOne stream

    /// Method to get the metadata associated with the sequence specified by the filename parameter
    let getMetadata filename = 
        let genomeSeq = getGenomeSeq filename
        genomeSeq.Metadata 
        |> (fun md -> 
            md.TryGetValue("GenBank") 
            |> (fun (exists, data) -> 
                if exists then 
                    data :?> Bio.IO.GenBank.GenBankMetadata
                else
                    new Bio.IO.GenBank.GenBankMetadata() 
            )
        )

    /// Method to concatenate a sequence of strings into a comma separated string
    let ConvertToCommaSeparatedString (value:seq<string>) =
        let rec convert (innerVal:List<string>) acc = 
            match innerVal with
                | [] -> acc
                | hd::[] -> convert [] (acc + hd)
                | hd::tl -> convert tl (acc + hd + ", ")           
        convert (Seq.toList value) ""

    /// Method to check that a string is not null or empty
    let GetStringMessage str = 
        match str with
        | null | "" -> "no information recorded."
        | _ -> str

             

        

