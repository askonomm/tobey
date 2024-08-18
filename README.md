# Tobey

A (in-progress) static site generator written in C++. It follows a "not invented here" approach where everything that doesn't come with C++ standard library is written from scratch, such as the Markdown, FrontMatter and YAML parsers used.

## Goal

- Have a simple, fast and easy to use static site generator.
- Built from zero and with no dependencies.
- Built and published a Markdown parser, a YAML parser and a FrontMatter parser.

## Progress

- [todo] Static Site Generator
  - [todo] Read Markdown in directories
  - [todo] Generate HTML files
  - [todo] Use `layout` frontmatter key to build views out of templates
- [todo] FrontMatter parser
- [todo] YAML parser
- [todo] Markdown parser
  - [todo] Block parsers
    - **[done]** ~Paragraph block parser~
    - **[done]** ~Heading block parser~
    - [todo] List block parser
    - **[done]** ~Fenced code block parser~
    - [todo] Indented code block parser
    - **[done]** ~Line break parser~
    - **[done]** ~Quote block parser~
  - **[done]** ~Inline parsers~
    - **[done]** ~Bold parser~
    - **[done]** ~Italic parser~
    - **[done]** ~Image/Link parser~
    - **[done]** ~Inline code parser~
    - **[done]** ~Strikethrough parser~
  - [todo] Stitchers
    - **[done]** ~Fenced code block stitcher~
    - [todo] Indented code block stitcher
