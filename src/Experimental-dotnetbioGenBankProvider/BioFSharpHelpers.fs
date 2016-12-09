namespace Bio.FSharp.Experimental

open System
open System.IO
open System.Net
open System.IO.Compression

module Helpers = 
    /// Method to download a GenBank file from NCBI's FTP server. 
    /// Takes two parameters:
    /// 1. The name of the folder containing the desired sequence (e.g. GCA_000005845.2_ASM584v2)
    /// 2. The directory to save the file in.
    /// Useage: downloadAndSaveGenbankFile sequenceFolderName targetDirectory
    let downloadAndSaveGenbankFile sequenceFolderName targetDirectory =
        let remoteUri = "ftp://ftp.ncbi.nlm.nih.gov/genomes/genbank/bacteria/Escherichia_coli/latest_assembly_versions/" 
        let remoteFileName = sequenceFolderName + "_genomic.gbff.gz"
        use destFile = targetDirectory + "/" + sequenceFolderName + ".gbk" |> File.Create
        let webClient = new WebClient()
        try 
            use srcStream = remoteUri + sequenceFolderName + "/" + remoteFileName |> webClient.OpenRead
            use uncompressedStream = new StreamReader(new GZipStream(srcStream, CompressionMode.Decompress))
            uncompressedStream.BaseStream.CopyTo(destFile)
            Console.WriteLine("{0} saved to {1}", sequenceFolderName, targetDirectory + "/" + sequenceFolderName)
        with
            |_ -> failwith "Unable to download and save file"

