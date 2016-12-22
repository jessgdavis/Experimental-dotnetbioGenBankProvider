namespace Bio.FSharp.GenBankTypeProvider

open System
open System.IO
open System.Net
open System.IO.Compression

open Bio
open Bio.Core
open Bio.IO
open Bio.IO.GenBank

module GenBankDataHelperMethods = 

    /// Method to get the genome sequence contained in a file specified by the fileName parameter
    let GetSeqFromFile fileName = 
        let stream = File.OpenRead fileName 
        let parser = Bio.IO.SequenceParsers.GenBank
        parser.ParseOne stream

    // Method to get an ISequence object representing the data contained in the sequencestr parameter
    let GetSeqFromString (sequenceStr:string) = 
        use stream = new MemoryStream()
        use writer = new StreamWriter(stream)
        writer.Write(sequenceStr)
        stream.Position <- int64(0)
        let parser = Bio.IO.SequenceParsers.GenBank
        parser.ParseOne stream

    /// Method to get the metadata associated with the sequence specified by the sequence string parameter
    let GetGenBankMetadataFromString (sequenceStr:string) = 
        let genomeSeq = GetSeqFromString sequenceStr
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

    /// Method to get a sequence from a specified url 
    let GetSeqFromURL (url:string) = 
        let webClient = new WebClient()
        let fullURL = url + (url.Split('/') |> Seq.last |> (fun x -> "/" + x + "_genomic.gbff.gz")) 
        try 
            use srcStream = fullURL |> webClient.OpenRead
            use uncompressedStream = new StreamReader(new GZipStream(srcStream, CompressionMode.Decompress))
            uncompressedStream.ReadToEnd()
        with
            |_ -> failwith "Unable to read data at specified URL"



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

             

        

