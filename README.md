# jodot
An experimental framework for making games in Godot!

It abstracted from stuff that I use when I make games. It features:
- an entity-component-system model
- dependency injection
- some handy utilities

I plan to add more to it as I use it for my projects.

## Pronounciation

jaw-deaux, yaw-doe, joe-dot, j'doot, judo, whatever

Version - 0.0.naN.infinity+1

## Setup
- add as a git submodule in your project
- set your project to target framework net8.0 in your .csproj file.
- create new classes that inherit from Events, ModelRendererContainer and ServiceContext
- add your ModelRendererContainer to autoload with name "ModelRendererContainer"
- add your ServiceContext subclass to the END of autolooad
