namespace Bio.FSharp.GenBankTypeProvider

open System
open System.Reflection
open System.IO
open ProviderImplementation.ProvidedTypes
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations

open Bio
open Bio.IO
open Bio.Core
open Bio.IO.GenBank

open Bio.FSharp.GenBankTypeProvider.GenBankDataHelperMethods

[<TypeProvider>]
type public dotnetbioGenBankTypeProvider(config: TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()

    let ns = "Bio.FSharp.Experimental"
    let asm = Assembly.GetExecutingAssembly()

    // Type to export from the type provider
    let dotnetbioGenBankTY = ProvidedTypeDefinition(asm, ns, "GenBankProvider", Some(typeof<obj>), HideObjectMethods=true)

    // static parameter -> filename of genbank file
    let filename = ProvidedStaticParameter("filename", typeof<string>)

    // create the type
    do dotnetbioGenBankTY.DefineStaticParameters(
        [filename],
        (fun typeName [| :? string as filename |] -> 

            // Exception is thrown if the file is unable to be parsed
            let sequencestr = 
                try 
                     match filename.StartsWith("ftp://") with
                     | true -> GetSeqFromURL filename
                     | false -> System.IO.File.ReadAllText(Path.Combine(config.ResolutionFolder, filename))
                with 
                    _ -> failwith "Unable to parse file. Check the file path and format, and try again."

            // access to commonly used sequence properties
            let md = sequencestr |> GetGenBankMetadataFromString 
            let allFeatures = md.Features.All

            // define the provided type
            let ty = ProvidedTypeDefinition(asm, ns, typeName, Some(typeof<obj>))

            // ---------------.NET Bio Objects----------------- //

            // nested type for .NET Bio objects
            let dotnetbioty = ProvidedTypeDefinition(".NET Bio Objects", Some(typeof<obj>))
            dotnetbioty.AddXmlDoc "Provides access to the original .NET Bio objects representing this sequence"

            // Static Property: original iseq
            let iseq = ProvidedProperty("ISequence", typeof<ISequence>, IsStatic = true, GetterCode = 
                (fun [] -> <@@ sequencestr |> GetSeqFromString @@>)
            )
            iseq.AddXmlDoc "A .Net Bio ISequence object representation of the input sequence"
            dotnetbioty.AddMember iseq

            // Static Property: genbank metadata
            let gbmd = ProvidedProperty("GenBankMetaData", typeof<GenBankMetadata>, IsStatic = true, GetterCode = 
                (fun [] -> <@@ sequencestr |> GetGenBankMetadataFromString @@>)
            )
            gbmd.AddXmlDoc "The .NET Bio GenBankMetadata object for the input sequence"
            dotnetbioty.AddMember gbmd

            // add .NET Bio objects type to provided type
            ty.AddMember dotnetbioty

            // -------------- EXPOSED METADATA -------------- //
            // Static property: Accession
            let accession = ProvidedProperty("Accession", typeof<GenBankAccession>, IsStatic = true, GetterCode = 
                (fun [] -> 
                    <@@ 
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Accession
                    @@>)
            )
            accession.AddXmlDoc ("The accession number of the sequence. PRIMARY ACCESSION: " + (GetStringMessage md.Accession.Primary) 
                + ", SECONDARY ACCESSIONS: " + (GetStringMessage (ConvertToCommaSeparatedString md.Accession.Secondary))) 
            ty.AddMember accession

            // Static property: Comments
            let commentsStr = GetStringMessage (ConvertToCommaSeparatedString md.Comments)
            let comments = ProvidedProperty("Comments", typeof<string>, IsStatic = true, GetterCode = 
                (fun [] -> <@@ commentsStr @@>)
            )
            comments.AddXmlDoc ("Cross-references to other sequence entries, comparisons to other collections, notes of changes in LOCUS names, and other remarks. COMMENTS: " + commentsStr)
            ty.AddMember comments

            // Static Property: Contig
            let contigStr = GetStringMessage md.Contig
            let contig = ProvidedProperty("Contig", typeof<string>, IsStatic = true, GetterCode = 
                (fun [] -> <@@ contigStr @@>)
            )
            contig.AddXmlDoc ("Provides information about how individual sequence records can be combined to form larger-scale biological objects. CONTIG: " + contigStr)
            ty.AddMember contig

            // Static property: DbLinks

            // Static property: Definition
            let defstr = GetStringMessage md.Definition
            let definition = ProvidedProperty("Definition", typeof<string>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@ defstr @@>)
            )
            definition.AddXmlDoc ("DEFINITION: " + md.Definition)
            ty.AddMember definition

            // Static property: Keywords
            let keywordstr = GetStringMessage md.Keywords
            let keywords = ProvidedProperty("Keywords", typeof<string>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@ keywordstr @@>
                )
            )
            keywords.AddXmlDoc ("KEYWORDS: " + keywordstr)
            ty.AddMember keywords

            // Static property: Locus
            let locus = ProvidedProperty("Locus", typeof<GenBankLocusInfo>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Locus
                    @@>
                )
            )
            let txt = 
                match md.Locus with
                | null -> "no locus information recorded. "
                | _ -> 
                    "Date: " + md.Locus.Date.ToShortDateString() + ". Division Code: " + md.Locus.DivisionCode.ToString()
                    + ". Molecule Type: " + md.Locus.MoleculeType.ToString() + ". Name: " + md.Locus.Name + ". Sequence Length: " 
                    + md.Locus.SequenceLength.ToString() + ". Sequence Type: " + md.Locus.SequenceType.ToString() + ". Strand: "
                    + md.Locus.Strand.ToString() + ". StrandTopology: " + md.Locus.StrandTopology.ToString() + "."

            locus.AddXmlDoc ("LOCUS INFO: " + txt)
            ty.AddMember locus

            // Static property: Origin
            let origin = ProvidedProperty("Origin", typeof<string>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Origin
                    @@>
                )
            )
            let txt = 
                match md.Origin with
                | null -> "no origin location recorded"
                | _ -> "Specification of how the first base of the reported sequence is operationally located within the genome. Where possible, this includes its location within a larger genetic map. ORIGIN: " + md.Origin
            origin.AddXmlDoc txt
            ty.AddMember origin

            // Static property: Primary
            let primary = ProvidedProperty("Primary", typeof<string>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Primary
                    @@>
                )
            )
            let txt =
                match md.Primary with 
                | null -> "no primary information recorded"
                | _ -> "	Provides the reference to the primary GenBank files from which annotations in this file are derived. PRIMARY: " + md.Primary
                
            primary.AddXmlDoc txt
            ty.AddMember primary

            // Static property: References
            let references = ProvidedProperty("References", typeof<Collections.Generic.IList<CitationReference>>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.References 
                    @@>
                )
            )
            let txt = 
                match md.References with
                | null -> "no references information recorded"
                | _ -> 
                    md.References
                    |> Seq.map (fun x -> 
                            "    CITATION " + x.Number.ToString() + " Authors: " 
                            + x.Authors + ", Consortiums: " + x.Consortiums + ", Journal: " + x.Journal
                            + ", Location: " + x.Location + ", Medline: " + x.Medline
                            + ", Pubmed: " + x.PubMed + ", Remarks: " + x.Remarks + ", Title: " + x.Title
                        )
                    |> ConvertToCommaSeparatedString
            references.AddXmlDoc ("REFERENCES: " + txt)
            ty.AddMember references

            // Static property: Segment
            let segment = ProvidedProperty("Segment", typeof<SequenceSegment>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@ 
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Segment
                    @@>
                )
            )
            let txt = 
                match md.Segment with 
                | null -> "no segment information recorded"
                | _ -> "Segment provides the information on the order in which this entry appears in a series of discontinuous sequences from the same molecule. SEGMENT: Count: "+ md.Segment.Count.ToString() + ", Current: " + md.Segment.Current.ToString()
            segment.AddXmlDoc txt 
            ty.AddMember segment

            // Static property: Source
            let source = ProvidedProperty("Source", typeof<SequenceSource>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@ 
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Source
                    @@>
                )
            )
            let txt = 
                match md.Source with
                | null -> "no source information available"
                | _ -> "SOURCE information: Common Name: " + md.Source.CommonName + ", Organism: [Genus: " + md.Source.Organism.Genus + ", Class Level: " + md.Source.Organism.ClassLevels + ", Species: " + md.Source.Organism.Species + "]"
            source.AddXmlDoc txt
            ty.AddMember source

            // Static property: Version
            let version = ProvidedProperty("Version", typeof<GenBankVersion>, IsStatic=true, GetterCode=
                (fun [] -> 
                    <@@
                        let md = GetGenBankMetadataFromString sequencestr 
                        md.Version
                    @@>
                )
            )
            let txt = 
                match md.Version with 
                | null -> "no version information provided"
                | _ -> "A compound identifier consisting of the primary accession number and a numeric version number associated with the current version of the sequence data in the record. This is followed by an integer key (a 'GI') assigned to the sequence by NCBI. Accession.Version: " + md.Version.CompoundAccession + ", GI Number:" + md.Version.GiNumber
            version.AddXmlDoc txt
            ty.AddMember version

            // --------------FEATURES------------------ //
            allFeatures
            |> Seq.groupBy (fun x -> x.Key)
            |> Seq.iter (fun (name, features) -> 
                // create a type for the kind of feature
                let featureTy = ProvidedTypeDefinition(name.ToUpper() + "S", Some(typeof<obj>))
                features 
                |> Seq.iteri (fun i ft -> 
                    // name of provided property is ideally the locus tag, but could be missing
                    let n = 
                        match ft.Qualifiers.ContainsKey("locus_tag") with 
                            | true -> 
                                if (ft.Qualifiers.["locus_tag"].Count > 0)
                                    then 
                                        ft.Qualifiers.["locus_tag"].[0] 
                                    else i.ToString()
                            | false -> i.ToString()
                    
                    // create a provided property for each feature of the current featureTy
                    let featureProp = ProvidedProperty(n, typeof<FeatureItem>, IsStatic=true, GetterCode = 
                        (fun [] ->
                            <@@ 
                                let m = GetGenBankMetadataFromString sequencestr 
                                let thisFt = m.Features.All |> Seq.filter (fun f -> f.Key = name) |> Seq.toList
                                thisFt.[i]
                            @@>
                        )
                    )

                    // xml doc for the feature showing as much recorded info as possible
                    let featureDesc = 
                        ft.Qualifiers.Keys 
                        |> Seq.map (fun k -> k + ": " + (ConvertToCommaSeparatedString ft.Qualifiers.[k]))
                        |> ConvertToCommaSeparatedString

                    featureProp.AddXmlDoc featureDesc
                    featureTy.AddMember featureProp
                )
                // add each featureTy to the provided type
                ty.AddMember featureTy
            )

            // return the type
            ty
        )
    )

    // add type to the namespace
    do this.AddNamespace(ns, [dotnetbioGenBankTY])

// tell compiler that this assembly contains type proviers
[<assembly: TypeProviderAssembly>]
do()


