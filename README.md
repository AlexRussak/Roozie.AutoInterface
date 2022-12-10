# Roozie.AutoInterface

# What is it?

Roozie.AutoInterface is a C# source generator that generates an interface for a given class. The generated interface contains public members of the class, including properties, and methods.

# Why?

Interfaces are great for keeping your code loosely coupled and unit testable. But, they add some maintenance overhead. This source generator can help you to keep your interfaces up to date with your classes.

# How to use it?

1. Add the NuGet package to your project.
2. Add the `[AutoInterface]` attribute to the class you want to generate an interface for.
3. Build your project.
4. That's it! The interface will be generated and you set the class to implement it.

If you'd like the generator to set the class as implementing the interface, set the class to partial.

## Configuration

You can configure the generator in the `[AutoInterface]` attribute. The following options are available:

| Option            | Default Value    | Description                                                                                                                                                   |
| ----------------- | ---------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Name              | "I" + Class name | Set the interface to whatever name you want.                                                                                                                  |
| IncludeMethods    | `true`           | Set to `false` to not automatically include methods in the interface. You can mark a method as being included by adding the `[AddToInterface]` to the member. |
| IncludeProperties | `true`           | Same as IncludeMethods                                                                                                                                        |

# Contributing

If you find a bug or have a feature request, please open an issue. If you'd like to contribute, please open a pull request.

# Kudos

Shout out to Andrew Lock's [Source Generator series](https://andrewlock.net/series/creating-a-source-generator/). It was a great resource for learning how to write a source generator.

# Examples

TBD
