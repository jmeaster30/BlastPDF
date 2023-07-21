# BlastPDF
A C# library for PDF parsing and editing

## Purpose / Goals

My reason for writing starting this idea was the huge amount of closed source and/or very expensive PDF libraries. 
I had to purchase one of these (will not name) and couldn't help but thing "The PDF file spec can't be that complicated. Why can't I do this myself?".

With all my projects I try to challenge myself and learn as much as I can.
The following are those goals:
- Be able to generate any PDF I can imagine
  - I really want to create a copy of that one super cool DnD character sheet PDF that walks you through setting up a new character
- Have zero dependencies
  - I am allowed to use only Microsoft.CodeAnalysis for the code generators and Newtonsoft.JSON for debugging purposes.
  - I am also allowed to use any external dependency where I am the primary author. (i.e. SharperImage fka BlastIMG)
- Develop a templating language that uses code generators so the templates are type checked at compile time instead of runtime
- Create a PDF viewer / editor ALA Adobe Acrobat utilizing BlastPDF as the backend (I will allow myself to use external libraries for drawing stuff to the screen)
- Learn more about C#
## Projects in this repo

### BlastPDF

A PDF generation, parsing, and editing library

### BlastPDF.Template

Template language and source generators for compile-time generated pdf templates.

### BlastPDF.Test

Unit tests for the BlastPDF library

### BlastFont

A font loading library

### BlastSharp

Random utility things I wanted to have or try to make

### ShowCase

Meant for the purpose of experimenting with this library while developing.
