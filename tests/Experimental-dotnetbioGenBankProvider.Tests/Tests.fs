module GenBankDataHelperMethods.Tests

open NUnit.Framework
open Bio.FSharp.GenBankTypeProvider.GenBankDataHelperMethods
open Bio.Core
open System.IO




// Tests for GetSeqFromFile
[<Test>]
    let ``parses GCF_000005845.2_ASM584v2_genomic.gbk correctly``() = 
        let dataDir =  Path.Combine(__SOURCE_DIRECTORY__, @"data\")
        let sequence = 
            GetSeqFromFile (dataDir + "GCF_000005845.2_ASM584v2_genomic.gbk")
        Assert.AreEqual(sequence.Alphabet, Bio.Alphabets.DNA)

[<Test; ExpectedException>]
    let ``GetSeqFromFile incorrect filename exception``() = 
        let dataDir =  Path.Combine(__SOURCE_DIRECTORY__, @"data\")
        GetSeqFromFile (dataDir + "GCF_000005845.2_ASM584v2_genomic.gbl")

[<Test; ExpectedException>]
    let ``GetSeqFromFile incorrect data format exception``() =
        let dataDir =  Path.Combine(__SOURCE_DIRECTORY__, @"data\")
        GetSeqFromFile (dataDir + "sequence.gbk")

[<Test; ExpectedException>]
    let ``GetSeqFromFile empty filename exception``() = 
        GetSeqFromFile ""

//// Tests for getMetadata
[<Test>]
    let ``correct genbank metadata recorded``() = 
        let dataDir =  Path.Combine(__SOURCE_DIRECTORY__, @"data\")
        let metadata = GetGenBankMetadataFromFile (dataDir + "GCF_000005845.2_ASM584v2_genomic.gbk")
        Assert.AreEqual(metadata.Accession.Primary, "NC_000913")
        Assert.AreEqual(metadata.Definition, "Escherichia coli str. K-12 substr. MG1655, complete genome.")
