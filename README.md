# Pacman 

A Windows Forms desktop game featuring player movement, AI-driven
enemies, collision handling, multiple difficulty modes, and a persistent
scoring system saved in a SQLite database.

## Table of Contents

[About](#about)\
[Getting Started](#getting-started)\
[Prerequisites](#prerequisites)\
[Build and Run](#build-and-run)\
[Features](#features)\
[Technologies Used](#technologies-used)\
[License](#license)

## About

The project demonstrates skills in animation, collision detection, game
physics, real-time event handling, AI pathing, multi-form UI
interaction, and persistent storage using SQLite.

## Getting Started

You can run the project directly in Visual Studio without additional
tools beyond the included dependencies.

## Prerequisites

Before opening the project, ensure you have:

Visual Studio 2022 (recommended)\
.NET Desktop Development workload\
System.Data.SQLite installed via NuGet\
A Windows environment capable of running WinForms applications

The SQLite file database (scoress.db) is included and requires no
external setup.

## Build and Run

Clone or download the repository\
git clone https://github.com/yourusername/pacman-chase-game.git

Or download the ZIP file and extract it.

Open the solution in Visual Studio\
Go to File → Open → Project/Solution and select the .sln file.

Restore NuGet packages if needed\
Visual Studio usually restores them automatically.

Run the game\
Press F5 to start.\
The game begins at Form2, where the player selects a difficulty mode.

## Features

Player Movement and Animation\
Directional movement using the arrow keys\
Animated mouth opening and closing\
Collision detection with maze walls\
Screen wraparound at the left and right edges

Target Behavior\
Random movement when far from the player\
Avoidance behavior when the player approaches\
Multi-phase progression after each collision with the target

Enemy AI (Hard Mode)\
Random directional movement\
Wall detection\
Instant loss upon contact with the player

Maze System\
Fully designed maze using rectangle-based walls\
Includes rows, columns, T-shapes, and a center cage

Timer and End Conditions\
A 60-second countdown\
Game ends when time expires or when the final phase is completed

Scoring System\
Saves completion scores into a SQLite database\
Form2 displays the top 5 lowest completion times

Dual-Form Navigation\
Form2 handles difficulty selection and score viewing\
Form1 manages all game logic, animation, and rendering

## Technologies Used

C# Windows Forms\
.NET Framework / .NET Desktop Runtime\
SQLite (System.Data.SQLite)\
GDI+ graphics\
Git and GitHub for version control

## License

This project was developed for educational and demonstration purposes.\
It is not licensed for commercial use without permission from the
author.
