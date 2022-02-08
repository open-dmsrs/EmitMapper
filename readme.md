## Project Description
Powerful customizable tool for mapping entities to each other. Entities can be plain objects, DataReaders, SQL commands and anything you need. The tool uses run-time code generation via the Emit library. It is useful for dealing with DTO objects, data access layers an so on.

## Changelog
* Feb 8, 2022 The newest benchmark test is shown that the performance problem has been resolved. to see the method ToInt32(decimal?) of class NullableConverter
## Road Map for migrating to netcore

you can unload the project EmitMapper if you have a plan to use this library

*  Step one
  Initial import all of the codes from older emitmapper - Done
  Compile success  - Done

*  Step two
  Initial import all of the unit test code - Done
  Running success - Done

*  Step three
  Import all of components - Work in Process
  Running success


## Supported platforms:

* NETStandard 2.1
* .Net Framework 4.8
* net 6.0
* netcore app 3.1
## About Emit Mapper

* Overview
* Benefits of EmitMapper
* Getting started
* Type conversion
* Customization

# Customization overview

Customization using default configurator
* Default configurator overview
* Custom converters
* Custom converters_for_generics
* Null substitution
* Ignoring members
* Custom constructors
* Shallow and_deep_mapping
* Names matching
* Post processing

Low-level customization using custom configuratorors
# Emit Mapper in practice.

* Benchmark: EmitMapper vs Handwritten code vs AutoMapper
* Objects change tracking
* Mapping DbDatareader to objects
* Mapping objects to DbCommand (UPDATE and INSERT) Last edited Jan 11, 2010 at 3:01 PM by romankovs