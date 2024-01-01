# Advent2023
Advent of code 2023, Solutions are in unsafe c# using unity's jobs and burst compiler.

## Benchmarks
Benchmark for Day1 trebuchet part 1.
Results are measured in ticks (100 nano seconds) using Stopwatch in System.Diagnostics.

|ConsoleApplication .net 7.0  | Unity jobs  | Unity jobs single threaded |
|--|--|--|
|~ 6200 ticks  | ~530 ticks   | ~1000 ticks  |

The code for the console application can be found [here](https://github.com/ndarwiche/Advent2023/blob/main/Advent2023/Assets/_Main/02_Scripts/01_Trebuchet/Advent1.cs)
