# Roozie.AutoInterface

# What is it?

Roozie.AutoInterface is a C# source generator that generates an interface for a class. The generated interface contains
the XML-doc comments and public properties and methods.

# Why?

Interfaces are great for keeping your code loosely coupled and unit testable. But, they add some maintenance overhead.
This source generator can help you to keep your interfaces up to date with your classes.

# How to use it?

1. Add the NuGet package to your project.
2. Add the `[AutoInterface]` attribute to the class you want to generate an interface.
3. Build your project.
4. That's it! The interface will be automatically generated, and you set the class to implement it.
    - If you set your class as partial, the generator will automatically implement the interface.

Check out the tests ([1](/Roozie.AutoInterface.Tests) [2](/Roozie.AutoInterface.Tests.Integration)) if you need some
examples.

## Configuration

You can configure the generator in the `[AutoInterface]` attribute. The following options are available:

| Option             | Default Value    | Description                                                                                                                                     |
|--------------------|------------------|-------------------------------------------------------------------------------------------------------------------------------------------------|
| Name               | "I" + Class name | Set the interface to whatever name you want.                                                                                                    |
| IncludeMethods     | `true`           | Set to `false` to not automatically include methods in the interface. You can mark a method as included by adding the `[AddToInterface]` to it. |
| IncludeProperties  | `true`           | Same as IncludeMethods                                                                                                                          |
| ImplementOnPartial | `true`           | When true, the interface will be automatically implemented if the class is marked as partial.                                                   |

# Contributing

Please open an issue if you find a bug or have a feature request. If you'd like to contribute, please open a pull
request.

# Kudos

Shout out to Andrew Lock's [Source Generator series](https://andrewlock.net/series/creating-a-source-generator/). It was
an excellent resource for learning all aspects of source generators.
