(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#I "../../bin"

(**
Experimental-dotnetbioGenBankProvider
======================

Documentation

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The Experimental-dotnetbioGenBankProvider library can be <a href="https://nuget.org/packages/Experimental-dotnetbioGenBankProvider">installed from NuGet</a>:
      <pre>PM> Install-Package Experimental-dotnetbioGenBankProvider</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

This example demonstrates using a function defined in this sample library.

*)
#r "Experimental-dotnetbioGenBankProvider.dll"
open Experimental-dotnetbioGenBankProvider

printfn "hello = %i" <| Library.hello 0

(**
Some more info

Samples & documentation
-----------------------

The library comes with comprehensible documentation. 
It can include tutorials automatically generated from `*.fsx` files in [the content folder][content]. 
The API reference is automatically generated from Markdown comments in the library implementation.

 * [Tutorial](tutorial.html) contains a further explanation of this sample library.

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.
 
Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork 
the project and submit pull requests. If you're adding a new public API, please also 
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under Public Domain license, which allows modification and 
redistribution for both commercial and non-commercial purposes. For more information see the 
[License file][license] in the GitHub repository. 

  [content]: https://github.com/fsprojects/Experimental-dotnetbioGenBankProvider/tree/master/docs/content
  [gh]: https://github.com/fsprojects/Experimental-dotnetbioGenBankProvider
  [issues]: https://github.com/fsprojects/Experimental-dotnetbioGenBankProvider/issues
  [readme]: https://github.com/fsprojects/Experimental-dotnetbioGenBankProvider/blob/master/README.md
  [license]: https://github.com/fsprojects/Experimental-dotnetbioGenBankProvider/blob/master/LICENSE.txt
*)
