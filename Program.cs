using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine("Enter 'normal' for regular calculations or 'derivatives' for finding derivatives. Type 'exit' to quit.");
            string mode = Console.ReadLine().Trim().ToLower();

            if (mode == "exit")
                break;

            switch (mode)
            {
                case "normal":
                    Console.WriteLine("Enter your equation:");
                    string equation = Console.ReadLine();
                    var allTokens = Tokenize(equation);
                    var rpnInput = ShuntingYard(allTokens);
                    var result = Calculate(rpnInput);
                    Console.WriteLine("Result: " + result);
                    break;
                case "derivatives":
                    Console.WriteLine("Derivative feature not implemented yet.");
                    // Implementation for derivatives will go here.
                    break;
                default:
                    Console.WriteLine("Invalid mode selected. Please choose 'normal' or 'derivatives'.");
                    break;
            }
        }
    }

    static List<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        var token = new StringBuilder();
        foreach (var ch in input)
        {
            if (char.IsDigit(ch) || ch == '.')
            {
                token.Append(ch);
            }
            else if (ch.IsOperator())
            {
                if (token.Length > 0)
                {
                    tokens.Add(token.ToString());
                    token.Clear();
                }
                tokens.Add(ch.ToString());
            }
        }

        if (token.Length > 0)
        {
            tokens.Add(token.ToString());
        }

        return tokens;
    }

    static List<string> ShuntingYard(List<string> tokens)
    {
        var operators = new Stack<string>();
        var outputQueue = new Queue<string>();

        foreach (var token in tokens)
        {
            if (token.IsNumber())
            {
                outputQueue.Enqueue(token);
            }
            else if (token.IsOperator())
            {
                while (operators.Count > 0 && Helpers.Priority[operators.Peek()[0]] >= Helpers.Priority[token[0]])
                {
                    outputQueue.Enqueue(operators.Pop());
                }
                operators.Push(token);
            }
        }

        while (operators.Count > 0)
        {
            outputQueue.Enqueue(operators.Pop());
        }

        return new List<string>(outputQueue);
    }

    static double Calculate(List<string> rpnTokens)
    {
        var stack = new Stack<double>();

        foreach (var token in rpnTokens)
        {
            if (token.IsNumber())
            {
                stack.Push(double.Parse(token));
            }
            else if (token.IsOperator())
            {
                double num2 = stack.Pop();
                double num1 = stack.Pop();

                double localResult = token switch
                {
                    "+" => num1 + num2,
                    "-" => num1 - num2,
                    "*" => num1 * num2,
                    "/" => num1 / num2,
                    _ => throw new ArgumentOutOfRangeException()
                };
                stack.Push(localResult);
            }
        }

        return stack.Pop();
    }
}

internal static class Helpers
{
    public static readonly Dictionary<char, int> Priority = new()
    {
        { '+', 1 },
        { '-', 1 },
        { '*', 2 },
        { '/', 2 },
    };

    public static bool IsOperator(this char @char) => Priority.ContainsKey(@char);
    public static bool IsOperator(this string token) => token.Length == 1 && Priority.ContainsKey(token[0]);
    public static bool IsNumber(this string input) => double.TryParse(input, out _);
}
