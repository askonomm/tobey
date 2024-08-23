# Tobey

A static site generator written in C++. I've created this as a means to learn C++, and thus I've written as much as I  
reasonably wanted from scratch myself, such as the YAML, Markdown and FrontMatter parsers. The only external libary used 
here is the [Mustache](https://github.com/kainjow/Mustache/blob/master/mustache.hpp) templating library.

## Progress

- [todo] Static Site Generator
  - **[done]** ~Read Markdown in directories~
  - **[done]** ~Generate HTML files~
  - **[done]** ~Use `layout` frontmatter key to build views out of templates~
- **[done]** ~FrontMatter parser~
- **[done]** ~YAML parser~
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
