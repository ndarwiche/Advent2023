using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;


public class Advent1
{
    public int Solve()
    {
        var lines = File.ReadLines("Data/1/Input.txt");
        Stopwatch stopwatch = new();
        stopwatch.Start();
        int sum = 0;
        foreach (var line in lines)
        {
            // unsafe
            // {
            //     fixed (char* ptr = line)
            //     {
            //         sum += SolveForLine(new Span<char>(ptr, line.Length));
            //     }
            // }
            sum += SolveForLine(line);
        }

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Elapsed ticks : {stopwatch.ElapsedTicks}");

        return sum;
    }


    private int SolveForLine(Span<char> s)
    {
        bool isFirstFound = false;
        bool isLastFound = false;
        int digit1 = 0;
        int digit2 = 0;
        int startInc = 1;
        int endInc = 1;
        for (int start = 0, end = s.Length - 1; start <= end; start += startInc, end -= endInc)
        {
            if (isFirstFound && isLastFound)
                break;
            if (!isFirstFound && char.IsNumber(s[start]))
            {
                isFirstFound = true;
                startInc = 0;
                digit1 = s[start] - 48;
            }

            if (!isLastFound && char.IsNumber(s[end]))
            {
                isLastFound = true;
                endInc = 0;
                digit2 = s[end] - 48;
            }
        }

        return digit1 * 10 + digit2;
    }

    private int SolveForLine(string s)
    {
        bool isFirstFound = false;
        bool isLastFound = false;
        int digit1 = 0;
        int digit2 = 0;
        int startInc = 1;
        int endInc = 1;
        for (int start = 0, end = s.Length - 1; start <= end; start += startInc, end -= endInc)
        {
            if (isFirstFound && isLastFound)
                break;
            if (!isFirstFound && char.IsNumber(s[start]))
            {
                isFirstFound = true;
                startInc = 0;
                digit1 = s[start] - 48;
            }

            if (!isLastFound && char.IsNumber(s[end]))
            {
                isLastFound = true;
                endInc = 0;
                digit2 = s[end] - 48;
            }
        }

        return digit1 * 10 + digit2;
    }
}