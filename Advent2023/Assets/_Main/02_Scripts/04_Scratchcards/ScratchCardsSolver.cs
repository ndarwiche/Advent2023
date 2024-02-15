using UnityEngine;
using System;
using System.Diagnostics;
using Advent2023.Helpers;
using Advent2023.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace Advent2023.ScratchCards
{
    [BurstCompile]
    public struct ScratchCardsJob : IJobParallelFor
    {
        public NativeArray<IntPtr> Lines;
        public NativeArray<int> LineSizes;
        public NativeArray<int> MatchingNumberCountArray;
        public NativeArray<int> LinePoints;
        public int WinningNumbersCount;
        public int ElfNumbersCount;

        public unsafe void Execute(int index)
        {
            UnsafeParallelHashSet<int> winningNumbers = new(WinningNumbersCount, Allocator.Temp);
            NativeArray<int> elfNumbers = new(ElfNumbersCount, Allocator.Temp);
            FillNumbers(index, ref elfNumbers, ref winningNumbers);
            int similarNumberCount = 0;
            for (int i = 0; i < elfNumbers.Length; i++)
            {
                if (winningNumbers.Contains(elfNumbers[i]))
                {
                    similarNumberCount++;
                }
            }

            MatchingNumberCountArray[index] = similarNumberCount;
            LinePoints[index] = similarNumberCount == 0 ? 0 : (int) math.pow(2, similarNumberCount - 1);
        }

        private unsafe void FillNumbers(int index, ref NativeArray<int> elfNumbers,
            ref UnsafeParallelHashSet<int> winningNumbers)
        {
            var line = Lines[index];
            byte* ptr = (byte*) line.ToPointer();
            int lineSize = LineSizes[index];
            //skip unnecessary words
            int startIndex = 10;//8//10
            //skip 8th character if it's a space
            if ((char) *(ptr + startIndex) == ' ')
                startIndex++;

            bool hasPassedNumberSeparator = false;
            int currentParsedNumber = 0;
            int elfNumberIndex = 0;
            for (int i = startIndex; i < lineSize; i++)
            {
                char currentChar = (char) *(ptr + i);
                if (currentChar == '|')
                {
                    hasPassedNumberSeparator = true;
                    continue;
                }

                if (currentChar == ' ')
                {
                    //2 white spaces in a row
                    if (currentParsedNumber == 0)
                        continue;
                    if (hasPassedNumberSeparator)
                    {
                        elfNumbers[elfNumberIndex] = currentParsedNumber;
                        elfNumberIndex++;
                    }
                    else
                    {
                        winningNumbers.Add(currentParsedNumber);
                    }

                    currentParsedNumber = 0;
                    continue;
                }

                currentParsedNumber = (currentChar - 48) + 10 * currentParsedNumber;
            }
            elfNumbers[elfNumberIndex] = currentParsedNumber;
        }
    }

    public class ScratchCardsSolver : MonoBehaviour
    {
        public void SolvePart1()
        {
            byte[] buffer = FileHelper.ReadAndGetLineIndices("Data/4/Input.txt", out var lineIndices);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            NativeArray<IntPtr> lines = new(lineIndices.Count, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lineIndices.Count; i++)
                {
                    fixed (byte* ptr = buffer)
                    {
                        lines[i] = (IntPtr) (ptr + lineIndices[i]);
                    }
                }
            }

            NativeArray<int> lineLengths = new(lineIndices.Count, Allocator.TempJob);
            for (int i = 0; i < lineIndices.Count - 1; i++)
            {
                lineLengths[i] = lineIndices[i + 1] - lineIndices[i];
            }

            //add final line length
            lineLengths[lineIndices.Count - 1] = buffer.Length - lineIndices[^1];


            var job = new ScratchCardsJob
            {
                Lines = lines,
                LineSizes = lineLengths,
                LinePoints = new(lines.Length, Allocator.TempJob),
                MatchingNumberCountArray = new(lines.Length, Allocator.TempJob),
                ElfNumbersCount = 25,//8,
                WinningNumbersCount = 10//5
            };
            job.Schedule(lines.Length, 32).Complete();
            //job.Run(lines.Length);

            //Sum the indices of the valid games
            int result = 0;
            for (int i = 0; i < job.Lines.Length; i++)
            {
                result += job.LinePoints[i];
            }

            job.Lines.Dispose();
            job.LinePoints.Dispose();
            job.LineSizes.Dispose();
            job.MatchingNumberCountArray.Dispose();
            stopwatch.Stop();
            ResultUi.Instance.ShowResult(stopwatch.ElapsedTicks, result);
        }


        public void SolvePart2()
        {
            byte[] buffer = FileHelper.ReadAndGetLineIndices("Data/4/Input.txt", out var lineIndices);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            NativeArray<IntPtr> lines = new(lineIndices.Count, Allocator.TempJob);
            //Get a pointer for each line, Jobs will accept IntPtr as a native array type  
            unsafe
            {
                for (var i = 0; i < lineIndices.Count; i++)
                {
                    fixed (byte* ptr = buffer)
                    {
                        lines[i] = (IntPtr) (ptr + lineIndices[i]);
                    }
                }
            }

            NativeArray<int> lineLengths = new(lineIndices.Count, Allocator.TempJob);
            for (int i = 0; i < lineIndices.Count - 1; i++)
            {
                lineLengths[i] = lineIndices[i + 1] - lineIndices[i];
            }

            //add final line length
            lineLengths[lineIndices.Count - 1] = buffer.Length - lineIndices[^1];

            var job = new ScratchCardsJob
            {
                Lines = lines,
                LineSizes = lineLengths,
                LinePoints = new(lines.Length, Allocator.TempJob),
                MatchingNumberCountArray = new(lines.Length, Allocator.TempJob),
                ElfNumbersCount = 25,//8,25
                WinningNumbersCount = 10//5,10
            };
            job.Schedule(lines.Length, 32).Complete();
            //job.Run(lines.Length);

            //Sum the indices of the valid games
            int result = GetTotalCardCount(job.MatchingNumberCountArray);

            job.Lines.Dispose();
            job.LinePoints.Dispose();
            job.LineSizes.Dispose();
            job.MatchingNumberCountArray.Dispose();
            stopwatch.Stop();
            ResultUi.Instance.ShowResult(stopwatch.ElapsedTicks, result);
        }

        private int GetTotalCardCount(NativeArray<int> matchingNumbers)
        {
            NativeArray<int> cards = new(matchingNumbers.Length, Allocator.Temp);
            int totalCards = matchingNumbers.Length;
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = 1;
            }
            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = 1; j <= matchingNumbers[i]; j++)
                {
                    cards[i + j] += cards[i];
                    totalCards += cards[i];
                }
            }

            return totalCards;
        }
    }
}