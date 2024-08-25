# Tobey

A static site generator written in C++. I've created this as a means to learn C++, and thus I've written as much as I
reasonably wanted from scratch myself, such as the YAML, Markdown and FrontMatter parsers. The only external libraries
used here are [inja](https://github.com/pantor/inja) for templating and [json](https://github.com/nlohmann/json) for
creating data for the inja templates.

## Features

- Basic Markdown support (see [Markdown support](#markdown-support))
- DSL for creating data from content files to be used in templates (see [Data composition](#data-composition)]
- Templating using [inja](#templating)

## Installation

To install Tobey, simply download the latest executable:

```bash
wget https://github.com/askonomm/tobey/releases/download/v0.2.5/tobey
```

Then, make the file executable:

```bash
chmod +x tobey
```

And finally, move the file to a directory in your PATH:

```bash
sudo mv tobey /usr/local/bin
```

## Usage

First, create a folder for your Tobey site files. Inside this folder, create a folder called `layouts` for your templates, and a folder called `content` for your content files (.md files).

Then, to use Tobey, simply run the executable inside the folder where your Tobey site files are located:

```bash
tobey
```

This will generate the site into a folder called `output`, which is what you can then upload to a server somewhere. 

If you're developing the site and want the compiler to watch for changes and automatically recompile the site, you can use the `--watch` flag:

```bash
tobey --watch
```

## Markdown support

Tobey uses a homegrown Markdown parser that supports the following features:

- Headers (`#`, `##`, `###`, `####`, `#####`, `######`)
- Bold (`**bold**`, `__bold__`)
- Italics (`*italics*`, `_italics_`)
- Links (`[link](url)`)
- Images (`![alt text](url)`)
- Strikethrough (`~strikethrough~`)
- Inline code (``` `code` ```)
- Fenced code block (```` ```code block``` ````)
- Quote block (`> quote`)
- Line break block (`---`, `***`, `___`)

## Data composition

Tobey uses a DSL to create data from content files to be used in templates. The DSL is written in YAML and is placed at the top of the content file, in the FrontMatter section that contains the YAML metadata. The DSL is written in the following format:

```yaml
data:
  var-name:
    where:
      key: val
    sort:
      by: key
      order: asc
```

So with the above, it will create a templating variable `{{var-name}}` that contains the data where `key` equals `val`, and is sorted by `key` in ascending order.

### DSL documentation

#### where

The `where` key is used to filter the data. It is written in the following format:

```yaml
where:
  key: val
```

This will filter the data where `key` equals `val`.

#### sort

The `sort` key is used to sort the data. It is written in the following format:

```yaml
sort:
  by: key
  order: asc
```

This will sort the data by `key` in ascending order. The `order` key can be either `asc` or `desc`.

## Templating

Tobey uses [inja]() for templating. Templates are placed in the `layouts` folder, and can be in any file format, whether that is an XML file or HTML file, it doesn't matter. 

Then, the template can be used in the content files by using the `layout` key in the FrontMatter section. The value of the `layout` key should be the name of the template file **with** the file extension. For example, if the template file is `post.html`, then the `layout` key should be `post.html`.

### Template variables

All variables in the template are the same as the keys in YAML section of the FrontMatter files. For example, if the YAML section is:

```yaml
title: Hello, world!
```

Then the variable in the template would be `{{title}}`. 

To get the Markdown content rendered as HTML, use the `{{content}}` variable.