# jodot
An experimental framework for making games in Godot!

It abstracted from stuff that I use when I make games. It features:
- an entity-component-system model
- dependency injection
- some handy utilities

I plan to add more to it as I use it for my projects.

## Pronounciation

jaw-deaux, yaw-doe, joe-dot, j'doot, judo, whatever

Version - 0.0.naN.infinity

## Setup
- add as a git submodule in your project
- create new classes that inherit from Events and ServiceDirectory
- add your ServiceDirectory subclass to the END of autolooad
