# GrasshopperDigestor

This repository contains the backend files for GraphHop, a version control manager for grasshopper scripts. This solution contains files based off of Shapediver's best-practice templates for Rhino and Grasshopper Plugins. We build on top of this with a Rhino Plugin that scrapes (digests) attributes from grasshopper scripts and stores this in a Apache Tinkerpop graph database via our own custom graph operations code. We then query this database using a `Gremlin.Net` client. 

The main files of note are:

* `DigestGHFile.cs` is the main script for digesting grasshopper scripts.
* `Gremlin.cs` contains the `Gremlin` client.
* Within `Shared` -> `Data`, we have our main graph operations and data definitions for graphs.

## Data model of the graph database

* [Textual description of the data model](data_model_description.pdf)
* [Graphical representation of the data model](data_model_graphical_overview.pdf)
