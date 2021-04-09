# Newsticker - Open Source

This Program is a WPF Newsticker written in C# using RSS feeds from news organizations.

![Alt text](Screenshots/Newsticker.PNG?raw=true "Start Window")

## Features
* 4 newspapers + 2 newschannels
* Hyperlinking to the full articles
* Adjustable text size
* Weather forecast for selected regions

## Extensibility
To add a new newspaper simply inherit from `RssTransformBase` and create new `ArticleModel`s with the specific information from the newspaper specific RSS feed. 
To add a new location insert the link for the RSS feed inside `InitializeLocationRssLookup`.

If you like my work, check out  other projects on my [Github](https://github.com/lukashecke).

## Disclaimer
All product and company names are trademarks or registered trademarks of their respective holders. Use of them does not imply any affiliation with or endorsement by them.

## License

Copyright (c) Lukas Hecke. All rights reserved.

Licensed under the [MIT](LICENSE.txt) license.