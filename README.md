# .NET Bio GenBank Type Provider

This project is the work of an undergraduate computer science student at Queensland University of Technology, 
under the supervision of Associate Professor James Hogan. The aim of the project was an exploration of using FSharp with
the .NET Bio Bioinformatics library, and the eventual development of a Type Provider for GenBank data. 

Overview and Motivation
------

The [.NET Bio GitHub page](https://github.com/dotnetbio/bio) (2016) states that .NET Bio is an open source library of common Bioinformatics functions, intended to simplify the creation of life science applications.
It goes on to outline that the core library implements a range of file parsers and formatters for common file types, connectors to commonly-used web services such as NCBI BLAST, and standard algorithms for the comparison and assembly of DNA, RNA and protein sequences.

.NET Bio is an incredibly comprehensive library that offers a variety of useful functionality across a selection of different sequence data formats (e.g. GenBank, FASTA). However, by nature of this versatility, there are many layers of generality and abstraction that can make data access difficult or confusing. Given that the GenBank data format comes with rich metadata, it was decided to develop a type provider to work specifically with GenBank data formats, making use of the existing .NET Bio types. The idea being that the development of a type provider would provide users with rich contextual information about the sequence they are working with, as well as mimise the verbosity of the code required to develop Bioinformatic workflows. 

A simple demonstration of the difference between working with the type provider versus vanilla .NET Bio is outlined below. In this example, we are attempting to access the accession number of the sequence being worked with. 

### Example: Viewing the Accession Number
#### Vanilla .NET Bio
In order to get to the accession number with .NET Bio, we need to complete the following steps:

1. Parse a sequence from a file
2. Retrieve the GenBank metadata. It should be noted here that .NET Bio ISequences have a Metadata object associated with them, which is a generic dictionary. In the case of a GenBank data source, one of the entries in this dictionary has the key "GenBank"; this is the object we wish to retrieve.
3. Access the accession number

``` fsharp
  // First we parse a sequence from a file 
  let genomeSeq = 
    let stream = File.OpenRead "mysequencefile.gbk" 
    let parser = Bio.IO.SequenceParsers.FindParserByFileName "mysequencefile.gbk"
    parser.ParseOne stream
 
  // Second, we get the metadata
  let metadata = 
        genomeSeq.Metadata 
        |> (fun md -> 
            md.TryGetValue("GenBank") 
            |> (fun (exists, data) -> 
                if exists then 
                  // This is stored as a generic dictionary item, which we then need to cast as a GenBankMetadata object
                    data :?> Bio.IO.GenBank.GenBankMetadata
                else
                    failwith "No GenBankMetadata object associated with this sequence" 
            )
        )
  // Finally, we access the data
  metadata.Accession
```
#### Type Provider
With the type provider, the following two lines achieve the same effect:
``` fsharp
    type ``NC_012686-Chlamydia trachomatis`` = Provider.dotnetbioGenBankProvider<"mysequencefile.gbk">
    ``NC_012686-Chlamydia trachomatis``.Accession
```

### Intellisense
The added advantage of using a type provider is the ability to create highly customized xml documentation to be displayed with intellisense. This allows rich contextual information such as that seen in the figure below, where we are selecting the 'Locus' property of the provided type.

![Image showing intellisense when accessing locus info](https://github.com/jessicagrace17/Experimental-dotnetbioGenBankProvider/blob/master/docs/files/img/locus-intellisense.png)

The type provider also pulls out sequence features and exposes them in manner more conducive to quick scripting and browsing. We will again demonstrate this with an example.

### Example: Accessing Sequence Features
#### Vanilla .NET Bio
In .NET Bio, sequence features are stored as an array of objects; in order to access a particular feature, one would need to know the index of that feature. Assuming we have already completed the steps above, we have a metadata object. From here you could execute `metadata.Features.CodingSequences.[10]` and the coding sequence object would be shown in the fsi window. Finding a particular coding sequence would be a matter of identifying the correct index and then selecting it, or executing the above line with different indices until the correct gene was identified. 

#### Type Provider
However, with the type provider, these features are able to be browsed by their locus tag using intellisense to give a quick overview of that feature, as seen below. 

![Image showing intellisene when accessing a coding sequence](https://github.com/jessicagrace17/Experimental-dotnetbioGenBankProvider/blob/master/docs/files/img/cds-intellisense.png)

Dependencies 
------

How To Use
------

Known Issues and Future Directions
------
