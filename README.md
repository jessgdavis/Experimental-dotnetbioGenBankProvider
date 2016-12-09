# .NET Bio GenBank Type Provider

This project is the work of an undergraduate computer science student at Queensland University of Technology, 
under the supervision of Associate Professor James Hogan. The aim of the project was an exploration of using FSharp with
the .NET Bio Bioinformatics library, and the eventual development of a Type Provider for GenBank data. 

## Contents

* [Overview and Motivation](#overview)
* [Using the .NET Bio GenBank Type Provider](#using)
* [Building and Contributing](#build)
* [Known Issues and Future Directions](#issues) 
* [Tutorials & Wiki](https://github.com/jessicagrace17/Experimental-dotnetbioGenBankProvider/wiki)

<a name="overview">Overview and Motivation</a>
------

The [.NET Bio GitHub page](https://github.com/dotnetbio/bio) (2016) states that .NET Bio is an open source library of common Bioinformatics functions, intended to simplify the creation of life science applications.
It goes on to outline that the core library implements a range of file parsers and formatters for common file types, connectors to commonly-used web services such as NCBI BLAST, and standard algorithms for the comparison and assembly of DNA, RNA and protein sequences.

.NET Bio is an incredibly comprehensive library that offers a variety of useful functionality across a selection of different sequence data formats (e.g. GenBank, FASTA). However, by nature of this versatility, there are many layers of generality and abstraction that can make data access difficult or confusing. Given that the GenBank data format comes with rich metadata, it was decided to develop a type provider to work specifically with GenBank data formats, making use of the existing .NET Bio types. The idea being that the development of a type provider would provide users with rich contextual information about the sequence they are working with, as well as mimise the verbosity of the code required to develop Bioinformatic workflows. 

A simple demonstration of the difference between working with the type provider versus vanilla .NET Bio is outlined below. In this example, we are attempting to access the accession number of the sequence being worked with. 

### Example: Viewing the Accession Number
#### Vanilla .NET Bio
In order to get to the accession number with .NET Bio, we need to complete the following steps:

1. Parse a sequence from a file
2. Retrieve the GenBank metadata. 

   It should be noted here that .NET Bio ISequences have a Metadata object associated with them, which is a generic dictionary. In the case of a GenBank data source, one of the entries in this dictionary has the key "GenBank"; this is the object we wish to retrieve.
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
    type mygenome = Bio.FSharp.Experimental.GenBankProvider<"mysequencefile.gbk">
    mygenome.Accession
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

<a name="using">Using the .NET Bio GenBank Provider in your application</a>
------

<a name="build">Building and Contributing</a>
------

<a name="issues">Known Issues and Future Directions</a>
------
### TO DO
* Write test suite
* Prepare for release

### Comments
The version of the type provider made available in this repository is one of many versions that were created over the course of this project. Some indecision remains as to what the best approach is, and there is an inherent lack of refinement due to my own lack of experience. As such, any advice is welcomed and very much appreciated. 

This type provider was one aspect of the overall project - my report can be read [here](https://github.com/jessicagrace17/Experimental-dotnetbioGenBankProvider/blob/master/docs/files/fsharpReport.pdf) (see chapter 4).

Part of my report states the following:
> As an exploration, two ProvidedTypeDefinitions have also been added to the overall type provider - one of genes and one for coding sequences. These ProvidedTypeDefinitions generate a nested type for every recorded gene or coding sequence recorded in the file which was passed to the top level type as a static parameter. Using Intellisense, these features show up with their locus tag as the type name, and their sequences are associated with their type as a ProvidedProperty. This means that interactions can be such as that seen in figures 4.2 and 4.3 [refer images from above examples]. This is an interesting feature, as it provides quick and easy access to any desired feature in a genome, as opposed to having to iterate over a list. However, this is limited, in that it is only possible to provide this feature to the top level type. That is, these genes will not be updated every time a new object is created, as it is impossible to get the argument from a constructor (an FSharp quotation) and then use this to generate new types. At this stage, these features have been included with the overall type provider, but it is likely that these will be pulled out and made part of a separate provider offering.

The type provider in this repository is in fact the 'pulled out' provider mentioned above. The drawback here is that with this model, every quotation is parsing the GenBank file from the filename, due to (what I understand to be) limitations regarding non-primitive types. The same issue was not encountered in the initial model, as it had a constructor which evaluated the ISequence, and this ISequence was then passed as the argument to each property. However, as mentioned above, this did not allow for dynamically generating nested types for the different sequence features, which was a key feature we wished to integrate. 

At this stage there do not appear to be any significant bugs with the type provider, though it still needs proper testing. There is a lot of scope for integrating further features. For example, it would be nice to add method properties to the provided type to allow simplified calls to get the upstream region of a given gene. It might even be possibe to incorporate rich pattern matching into the type provider - these are all possibilities that can be explored.
